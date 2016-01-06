using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xunit;
using FcSoftware.SourcingProviders.YellowPages;

namespace FcSoftware.SourcingProviderTests
{
    public class YellowPagesTests
    {
        [Fact]
        public async Task SearchPizza()
        {
            var results = await YellowPages.Search("10707", "pizza", "distance");

            Assert.NotNull(results);
        }
    }
}
