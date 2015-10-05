using HtmlAgilityPack;
using System;
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
    }
}
