using Xunit;
using System.Linq;
using System.Threading.Tasks;
using FcSoftware.SourcingProviders.HomeAdvisor;
using System.Collections.Generic;

namespace FcSoftware.SourcingProviderTests
{
    public class HomeAdvisorTests
    {
        [Fact]
        public async Task GetTrades()
        {
            var ha = new HomeAdvisor();
            var trades = await ha.LoadTrades();

            Assert.NotNull(trades);
            Assert.Equal("Handyman Services", trades.Single(x => x.Id == "-12039").Name);
        }

        [Fact]
        public async Task GetProspectsForTrade()
        {
            var trade = new Trade()
            {
                Id = "-12039",
                Name = "Handyman Services"
            };
            var ha = new HomeAdvisor();
            var prospects = await ha.GetProspectsForTrade(trade, "10707");

            Assert.NotNull(prospects);
            Assert.NotEqual(0, prospects.Count);
        }

        [Fact]
        public async Task GetProspectReviews()
        {
            var p = new Prospect()
            {
                ReviewCount = 22,
                Id = "22403306"
            };

            var ha = new HomeAdvisor();
            var reviews = await ha.GetReviewsForProspect(p);

            Assert.NotNull(reviews);
            Assert.NotEqual(0, reviews.Count);
        }

        [Fact]
        public void GetProspectsForTradesWithZipCodes()
        {
            var tradeHandyman = new Trade()
            {
                Id = "-12039",
                Name = "Handyman Services"
            };

            var tradePlumbing = new Trade()
            {
                Id = "-12058",
                Name = "Plumbing"
            };

            var trades = new List<Trade>();
            trades.Add(tradeHandyman);
            trades.Add(tradePlumbing);

            var zipCodes = new List<string>();
            zipCodes.Add("10707");
            zipCodes.Add("11782");

            var ha = new HomeAdvisor();
            var results = ha.GetProspectsForTradesWithZipCodes(trades, zipCodes);

            using (var file = new System.IO.StreamWriter("prospects.txt"))
            {
                foreach (var result in results)
                {
                    file.WriteLine($"{result.Name} - {result.Service.Name} - {result.PostalCode} - {result.StreetAddress}");
                }
            }
                

            Assert.NotNull(results);
            Assert.NotEqual(0, results.Count);
        }
    }
}
