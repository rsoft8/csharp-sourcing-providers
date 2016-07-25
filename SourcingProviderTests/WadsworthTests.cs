using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using FcSoftware.SourcingProviders.Wadsworth;
using Newtonsoft.Json;

namespace FcSoftware.SourcingProviderTests
{
    public class WadsworthTests
    {
        [Fact]
        public async Task GetCertifiedLabs()
        {
            var overallList = new List<CertifiedLab>();
            for (var i = 0; i < 47; i++)
            {
                var results = await Wadsworth.GetPagedSearchResults(i);
                overallList.AddRange(results);
            }

            var json = JsonConvert.SerializeObject(overallList);
            System.IO.File.WriteAllText(@"C:\temp\wadsworth.json", json);
            Assert.NotNull(overallList);
        }
    }
}
