using System;
using System.Net.Http;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace FcSoftware.SourcingProviders.YellowPages
{
    public class YellowPages
    {
        const string _ypSearchUrl = @"http://pubapi.yp.com/search-api/search/devapi/search?searchloc={0}&term={1}&format=json&sort={2}&pagenum={3}&radius={4}&listingcount=25&key={5}";
        const string _ypUserAgent = @"Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident / 6.0)";
        const string _ypApiKey = @"";
        

        public async static Task<RootObject> Search(string location, string term, string sort, int radius = 5, int pageNum = 1)
        {
            RootObject result = null;

            var searchApiUrl = string.Format(_ypSearchUrl, location, term, sort, pageNum, radius, _ypApiKey);

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Add("User-Agent", _ypUserAgent);
                using (var response = await client.GetAsync(searchApiUrl))
                using (var content = response.Content)
                {

                    var json = await content.ReadAsStringAsync();
                    result = JsonConvert.DeserializeObject<RootObject>(json);
                }
            }
            

            return result;               
        }
    }
}
