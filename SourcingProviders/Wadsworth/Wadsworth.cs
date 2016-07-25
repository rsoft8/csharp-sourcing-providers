using HtmlAgilityPack;
using System.Linq;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System;
using Newtonsoft.Json;

namespace FcSoftware.SourcingProviders.Wadsworth
{
    public class Wadsworth
    {
        const string _rootUrl = @"http://wadsworth.org/views/ajax";

        public async static Task<List<CertifiedLab>> GetPagedSearchResults(int pageNum)
        {
            List<CertifiedLab> results = null;

            // URL to POST to, requiring following form data:
            // - page:             [page number]
            // - view_name:         elap
            // - view_display_id:   elap
            var values = new List<KeyValuePair<string, string>>();
            values.Add(new KeyValuePair<string, string>("page", pageNum.ToString()));
            values.Add(new KeyValuePair<string, string>("view_name", "elap"));
            values.Add(new KeyValuePair<string, string>("view_display_id", "elap"));

            using (var postContent = new FormUrlEncodedContent(values))
            using (var client = new HttpClient())
            using (var response = await client.PostAsync(_rootUrl, postContent))
            using (var content = response.Content)
            {
                // Now have JSON content
                var payload = await content.ReadAsStringAsync();
                var json = JsonConvert.DeserializeObject<List<RootObject>>(payload);

                if (results == null) results = new List<CertifiedLab>();
                foreach (var obj in json)
                {
                    if (obj.data != null)
                    {
                        //results.Add(obj.data);

                        // Extra processing
                        var doc = new HtmlDocument();
                        doc.LoadHtml(obj.data);
                        //doc.LoadHtml(doc.DocumentNode);

                        try
                        {
                            var rootDiv = doc.DocumentNode.SelectSingleNode("/div[1]");
                            var resultsTable = rootDiv.SelectSingleNode("//div[1]/div[2]/table[contains(@class, 'views-table sticky-enabled')]");
                            //if (resultsTable == null)
                            //    resultsTable = rootDiv.SelectSingleNode("//div[1]/div[2]/table[@class='views-table sticky-enabled cols-3 tableheader-processed sticky-table']");

                            if (resultsTable != null)
                            {
                                //var resultsTable = doc.DocumentNode.SelectSingleNode("//table[@class='views-table sticky-enabled cols-3 tableheader-processed sticky-table']");
                                var trNodes = resultsTable.SelectNodes("//tbody/tr");
                                if (trNodes != null)
                                {
                                    foreach (var trChild in trNodes)
                                    {
                                        var tdCounter = 0;
                                        var certifiedLab = new CertifiedLab();
                                        foreach (var tdChild in trChild.SelectNodes("./td"))
                                        {
                                            if (tdCounter == 0)
                                            {
                                                // name and address info
                                                certifiedLab.Name = tdChild.ChildNodes.Single(x => x.Name.Equals("h2")).InnerText.Trim();
                                                var strongCounter = 0;
                                                foreach (var strong in tdChild.ChildNodes.Where(x => x.Name.Equals("strong")))
                                                {
                                                    var textValue = System.Net.WebUtility.HtmlDecode(strong.NextSibling.InnerText.Trim());
                                                    if (strongCounter == 0) certifiedLab.ID = textValue;
                                                    else if (strongCounter == 1) certifiedLab.Type = textValue;
                                                    else if (strongCounter == 2) certifiedLab.Director = textValue;
                                                    else if (strongCounter == 3) certifiedLab.Phone = textValue;
                                                    else if (strongCounter == 4) certifiedLab.Address = textValue;
                                                    else if (strongCounter == 5) certifiedLab.County = textValue;
                                                    else if (strongCounter == 6) certifiedLab.Country = textValue;

                                                    strongCounter++;
                                                }
                                                certifiedLab.LastUpdated = tdChild.ChildNodes.Single(x => x.Name.Equals("i")).InnerText.Trim();
                                            }
                                            else if (tdCounter == 1)
                                            {
                                                // approved categories
                                                var nodes = tdChild.SelectNodes("./ul/li");
                                                if (nodes != null)
                                                {
                                                    foreach (var li in nodes)
                                                    {
                                                        certifiedLab.ApprovedCategories.Add(System.Net.WebUtility.HtmlDecode(li.InnerText.Trim()));
                                                    }
                                                }
                                            }
                                            else if (tdCounter == 2)
                                            {
                                                // approved fields of analysis
                                                var nodes = tdChild.SelectNodes("./ul/li");
                                                if (nodes != null)
                                                {
                                                    foreach (var li in nodes)
                                                    {
                                                        certifiedLab.ApprovedFieldsOfAnalysis.Add(System.Net.WebUtility.HtmlDecode(li.InnerText.Trim()));
                                                    }
                                                }
                                            }

                                            tdCounter++;
                                        }

                                        results.Add(certifiedLab);
                                    }
                                }
                                else
                                {
                                    var page = pageNum;
                                }
                            }

                        }
                        catch (Exception ex)
                        {
                            var msg = ex.Message;
                        }
                       
                    }
                        
                }


            }

            return results;
        }
    }
}
