using HtmlAgilityPack;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FcSoftware.SourcingProviders.HomeAdvisor
{
    public class HomeAdvisor
    {
        public async Task<List<Trade>> LoadTrades()
        {
            List<Trade> trades = null;

            // URL with dropdown box of trades
            var url = @"http://www.homeadvisor.com/c.html";

            // Grab the source of the page
            using (var client = new HttpClient())
            using (var response = await client.GetAsync(url))
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

        public async Task<List<ProspectReview>> GetProspectsForTrade(Trade trade, string postalCode)
        {
            List<ProspectReview> reviews = null;

            // URL to POST to, requiring following form data:
            // - findContractor: searchByZip
            // - catOid:         [trade ID here]
            // - zip:            [zip code here]
            var url = @"http://www.homeadvisor.com/c.html";
            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string,string>("findContractor", "searchByZip"));
            values.Add(new KeyValuePair<string,string>("catOid", trade.Id));
            values.Add(new KeyValuePair<string,string>("zip", postalCode));

            using (var postContent = new FormUrlEncodedContent(values))
            using (var client = new HttpClient())
            using (var response = await client.PostAsync(url, postContent))
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
                    var prospectReview = new ProspectReview()
                    {
                        Service = trade,
                        RatingAvailable = false
                    };

                    // Parse the itemprops
                    foreach (var span in child.SelectNodes(".//span[@itemprop]"))
                    {
                        var attrValue = span.GetAttributeValue("itemprop", "").ToLower();

                        switch (attrValue)
                        {
                            case "name":
                                prospectReview.Name = span.InnerText;
                                break;
                            case "telephone":
                                prospectReview.Phone = span.InnerText;
                                break;
                            case "streetaddress":
                                prospectReview.StreetAddress = span.InnerText;
                                break;
                            case "addresslocality":
                                prospectReview.AddressLocality = span.InnerText;
                                break;
                            case "addressregion":
                                prospectReview.AddressRegion = span.InnerText;
                                break;
                            case "postalcode":
                                prospectReview.PostalCode = span.InnerText;
                                break;
                            default:
                                break;
                        }
                    }

                    var ratingRefNodes = child.SelectNodes(".//div[@class='l-column ratings-reference']/div");
                    if (ratingRefNodes != null && ratingRefNodes.First().Attributes["class"].Value.Equals("t-stars-small t-stars-rating"))
                    {
                        prospectReview.RatingAvailable = true;

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
                                prospectReview.StarRating = starRating;
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
                                prospectReview.ReviewCount = reviewCount;
                            }
                        }
                    }

                    // Finished parsing itemprops, add the object to the list
                    if (reviews == null) reviews = new List<ProspectReview>();
                    reviews.Add(prospectReview);
                }
            }

            return reviews;
        }
    }
}
