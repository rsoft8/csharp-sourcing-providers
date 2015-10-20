using HtmlAgilityPack;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;

namespace FcSoftware.SourcingProviders.HomeAdvisor
{
    public class HomeAdvisor
    {
        const string _rootUrl = @"http://www.homeadvisor.com/";
        const string _proReviewsUrl = @"http://www.homeadvisor.com/c.html";
        const string _prospectReviewsUrl = @"http://www.homeadvisor.com/sm/ratings/{0}?page={1}&filteredByTask=false&sort=newest";

        public async Task<List<Trade>> LoadTrades()
        {
            List<Trade> trades = null;

            // Grab the source of the page
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(_proReviewsUrl))
            using (var content = response.Content)
            {
                var xhtml = await content.ReadAsStringAsync();

                // Parse the XHTML result
                var doc = new HtmlDocument();
                doc.LoadHtml(xhtml);

                // Look for the <select> element with name catOid
                var select = doc.DocumentNode.SelectSingleNode("//select[@id='catOid']");

                // Grab all <option> children with a value not equal to zero
                // The <option> value attribute contains the ID
                // Inner node text of the element contains the trade name
                foreach (var child in select.SelectNodes("//option[not(@value='0')]"))
                {
                    var optValue = child.Attributes["value"].Value;
                    var optName = child.NextSibling.InnerText;

                    if (trades == null) trades = new List<Trade>();
                    trades.Add(new Trade()
                    {
                        Id = optValue,
                        Name = optName
                    });
                }
            }

            return trades;
        }

        public async Task<List<Prospect>> GetProspectsForTrade(Trade trade, string postalCode)
        {
            List<Prospect> prospects = null;

            // URL to POST to, requiring following form data:
            // - findContractor: searchByZip
            // - catOid:         [trade ID here]
            // - zip:            [zip code here]
            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string,string>("findContractor", "searchByZip"));
            values.Add(new KeyValuePair<string,string>("catOid", trade.Id));
            values.Add(new KeyValuePair<string,string>("zip", postalCode));

            using (var postContent = new FormUrlEncodedContent(values))
            using (var client = new HttpClient())
            using (var response = await client.PostAsync(_proReviewsUrl, postContent))
            using (var content = response.Content)
            {
                // Now have first page with the review content
                var xhtml = await  content.ReadAsStringAsync();

                // Parse the XHTML result
                var doc = new HtmlDocument();
                doc.LoadHtml(xhtml);
                
                // Look for the review pane containing prospects
                var reviewPane = doc.DocumentNode.SelectSingleNode("//div[@class='xmd-content-main']");

                var prospectDivClass = string.Format(".//div[@data-srzip='{0}']", postalCode);
                foreach (var child in reviewPane.SelectNodes(prospectDivClass))
                {
                    var prospect = new Prospect()
                    {
                        Service = trade,
                        RatingAvailable = false,
                        Id = child.Attributes["data-spid"].Value
                    };

                    // Parse the itemprops
                    foreach (var span in child.SelectNodes(".//span[@itemprop]"))
                    {
                        var attrValue = span.GetAttributeValue("itemprop", "").ToLower();

                        switch (attrValue)
                        {
                            case "name":
                                prospect.Name = span.InnerText;
                                break;
                            case "telephone":
                                prospect.Phone = span.InnerText;
                                break;
                            case "streetaddress":
                                prospect.StreetAddress = span.InnerText;
                                break;
                            case "addresslocality":
                                prospect.AddressLocality = span.InnerText;
                                break;
                            case "addressregion":
                                prospect.AddressRegion = span.InnerText;
                                break;
                            case "postalcode":
                                prospect.PostalCode = span.InnerText;
                                break;
                            default:
                                break;
                        }
                    }

                    // Get the URL for the detail page of the prospect
                    var urlNode = child.SelectNodes(".//a[@itemprop='url']").FirstOrDefault();
                    if (urlNode != null)
                    {
                        prospect.Url = new Uri(UrlCombine(_rootUrl, urlNode.Attributes["href"].Value));
                    }

                    var ratingRefNodes = child.SelectNodes(".//div[@class='l-column ratings-reference']/div");
                    if (ratingRefNodes != null && ratingRefNodes.First().Attributes["class"].Value.Equals("t-stars-small t-stars-rating"))
                    {
                        prospect.RatingAvailable = true;

                        // Parse star percentage
                        var reviewRatingNode = ratingRefNodes[0].ChildNodes.Where(x => x.Name.Equals("div")).FirstOrDefault();
                        if (reviewRatingNode != null)
                        {
                            // TODO: Move cleaning double to utility class
                            // Replace non digit and non decimal characters with nothing
                            var ratingCleaned = Regex.Replace(reviewRatingNode.Attributes["style"].Value, "[^0-9.]", "");
                            var starRating = 0.0;
                            if (double.TryParse(ratingCleaned, out starRating))
                            {
                                prospect.StarRating = starRating;
                            }
                        }

                        // Parse review count
                        var reviewCountNode = ratingRefNodes.Where(x => x.Attributes["class"].Value.Contains("verified-reviews")).FirstOrDefault();
                        if (reviewCountNode != null)
                        {
                            // TODO: Move cleaning integer to utility class
                            // Replace non digit characters with nothing
                            var reviewCountCleaned = Regex.Replace(reviewCountNode.InnerText, "[^0-9]", "");
                            var reviewCount = 0;
                            if (int.TryParse(reviewCountCleaned, out reviewCount))
                            {
                                prospect.ReviewCount = reviewCount;
                            }
                        }
                    }

                    // Finished parsing itemprops, add the object to the list
                    if (prospects == null) prospects = new List<Prospect>();
                    prospects.Add(prospect);
                }
            }

            return prospects;
        }

        public async Task<List<Review>> GetReviewsForProspect(Prospect p)
        {
            List<Review> reviews = null;

            // 10 Reviews per page
            var maxReviewPages = Math.Ceiling(p.ReviewCount / 10.0);

            // Loop through all of the reivew pages
            for (var curPage = 0; curPage < maxReviewPages; curPage++)
            {
                // Get the page of reviews
                var url = string.Format(_prospectReviewsUrl, p.Id, (curPage + 1));

                using (var client = new HttpClient())
                using (var response = await client.GetAsync(url))
                using (var content = response.Content)
                {
                    var xhtml = await content.ReadAsStringAsync();

                    // Parse the XHTML result
                    var doc = new HtmlDocument();
                    doc.LoadHtml(xhtml);

                    foreach (var reviewNode in doc.DocumentNode.SelectNodes(".//div[@itemprop='review']"))
                    {

                        var review = new Review();

                        // Parse the itemprops
                        foreach (var span in reviewNode.SelectNodes(".//span[@itemprop]"))
                        {
                            var attrValue = span.GetAttributeValue("itemprop", "").ToLower();

                            switch (attrValue)
                            {
                                case "datepublished":
                                    review.DatePublished = DateTime.Parse(span.InnerText);
                                    break;
                                case "reviewrating":
                                    review.Rating = double.Parse(span.InnerText);
                                    break;
                                case "reviewbody":
                                    review.Body = span.InnerText;
                                    break;
                                case "author":
                                    review.Author = span.InnerText;
                                    break;
                            }
                        } // End parsing itemprops

                        // Parse review ID
                        review.Id = reviewNode.SelectSingleNode(".//div/@data-rating-id").GetAttributeValue("data-rating-id", string.Empty);

                        // TODO: Parse location (not always linked in a href)

                        // TODO: Parse project (not always linked in a href)


                        // Add the review to the collection
                        if (reviews == null) reviews = new List<Review>();
                        reviews.Add(review);
                    }
                }
            }

            return reviews;
        }

        // TODO: Add to Utility library
        //       Add null check
        private string UrlCombine(string url1, string url2)
        {
            if (url1.Length == 0) return url2;
            if (url2.Length == 0) return url1;

            url1 = url1.TrimEnd('/', '\\');
            url2 = url2.TrimStart('/', '\\');

            return string.Format("{0}/{1}", url1, url2);
        }
    }
}
