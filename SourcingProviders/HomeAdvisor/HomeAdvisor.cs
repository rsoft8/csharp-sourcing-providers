using HtmlAgilityPack;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FcSoftware.SourcingProviders.HomeAdvisor
{
    public class HomeAdvisor
    {
        public async Task<List<Service>> LoadServices()
        {
            List<Service> services = null;

            // URL with dropdown box of categories
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
                // Inner node text of the element contains the category name
                foreach (var child in select.SelectNodes("//option[not(@value='0')]"))
                {
                    var optValue = child.Attributes["value"].Value;
                    var optName = child.NextSibling.InnerText;

                    if (services == null) services = new List<Service>();
                    services.Add(new Service()
                    {
                        Id = optValue,
                        Name = optName
                    });
                }
            }

            return services;
        }

        public async Task<List<ProspectReview>> GetReviewsForService(Service service, string postalCode)
        {
            List<ProspectReview> reviews = null;

            // URL to POST to, requiring following form data:
            // - findContractor: searchByZip
            // - catOid:         [category ID here]
            // - zip:            [zip code here]
            var url = @"http://www.homeadvisor.com/c.html";
            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string,string>("findContractor", "searchByZip"));
            values.Add(new KeyValuePair<string,string>("catOid", service.Id));
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
                    var prospectReview = new ProspectReview();
                    prospectReview.Service = service;

                    // Parse the itemprops
                    foreach (var span in child.SelectNodes(".//span[@itemprop]"))
                    {
                        var attrValue = span.GetAttributeValue("itemprop", "");

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
                            case "addressLocality":
                                prospectReview.AddressLocality = span.InnerText;
                                break;
                            case "addressRegion":
                                prospectReview.AddressRegion = span.InnerText;
                                break;
                            case "postalCode":
                                prospectReview.PostalCode = span.InnerText;
                                break;
                            default:
                                break;
                        }
                    }

                    // TODO: Update the rating parsing
                    // Parse the review count and star rating
                    // These fall under divs rather than the span @itemprop
                    //var ratingRef = child.SelectNodes(".//div[@class='l-column ratings-reference']").Descendants();
                    //var starsReviewsNode = ratingRef != null ? ratingRef.Where(x => x.Attributes["class"].Value == "t-stars-small").FirstOrDefault() : null;
                    //if (starsReviewsNode != null)
                    //{
                    //    // Rating found, get review count and star info
                    //    var starStyleAttr = starsReviewsNode.Attributes["style"].Value;
                    //    var starRatingStr = starStyleAttr.Substring(7, starStyleAttr.Length - 1);
                    //    var verifiedReviewsNode = ratingRef.Where(x => x.Attributes["class"].Value.Contains("verified-reviews")).FirstOrDefault();
                    //    var ratingStr = verifiedReviewsNode != null ? verifiedReviewsNode.InnerText : string.Empty;
                    //}

                    // Finished parsing itemprops, add the object to the list
                    if (reviews == null) reviews = new List<ProspectReview>();
                    reviews.Add(prospectReview);
                }
            }

            return reviews;
        }
    }
}
