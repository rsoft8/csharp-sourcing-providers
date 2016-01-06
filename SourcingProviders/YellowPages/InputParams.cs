using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcSoftware.SourcingProviders.YellowPages
{
    public class InputParams
    {
        public string appId { get; set; }
        public string dnt { get; set; }
        public string format { get; set; }
        public string userIpAddress { get; set; }
        public string userReferrer { get; set; }
        public string requestId { get; set; }
        public bool shortUrl { get; set; }
        public string test { get; set; }
        public string userAgent { get; set; }
        public string visitorId { get; set; }
        public int listingCount { get; set; }
        public bool phoneSearch { get; set; }
        public int radius { get; set; }
        public int searchLocation { get; set; }
        public string sort { get; set; }
        public string term { get; set; }
        public string termType { get; set; }
    }
}
