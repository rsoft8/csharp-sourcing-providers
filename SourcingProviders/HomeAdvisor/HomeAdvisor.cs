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
            var url = @"http://www.homeadvisor.com/c.html";

            using (var client = new HttpClient())
            using (var response = await client.GetAsync(url))
            using (var content = response.Content)
            {
                var xhtml = await content.ReadAsStringAsync();

                // parse the XHTML result
                var doc = new HtmlDocument();
                doc.LoadHtml(xhtml);

                var select = doc.DocumentNode.SelectSingleNode("//select[@id='catOid']");
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
