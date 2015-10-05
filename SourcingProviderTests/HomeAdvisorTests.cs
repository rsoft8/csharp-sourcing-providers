using Xunit;
using System.Linq;  
using System.Threading.Tasks;
using FcSoftware.SourcingProviders.HomeAdvisor;

namespace FcSoftware.SourcingProviderTests
{
    public class HomeAdvisorTests
    {
        [Fact]
        public async Task GetServiceTypes()
        {
            var ha = new HomeAdvisor();
            var services = await ha.LoadServices();

            Assert.NotNull(services);
            Assert.Equal("Handyman Services", services.Single(x => x.Id == "-12039").Name);
        }
    }
}
