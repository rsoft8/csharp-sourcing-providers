using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcSoftware.SourcingProviders.YellowPages
{
    public class MetaProperties
    {
        public string didYouMean { get; set; }
        public string errorCode { get; set; }
        public int listingCount { get; set; }
        public string message { get; set; }
        public RelatedCategories relatedCategories { get; set; }
        public InputParams inputParams { get; set; }
        public string requestId { get; set; }
        public string resultCode { get; set; }
        public string searchCity { get; set; }
        public double searchLat { get; set; }
        public double searchLon { get; set; }
        public string searchState { get; set; }
        public string searchType { get; set; }
        public int searchZip { get; set; }
        public int totalAvailable { get; set; }
        public string trackingRequestURL { get; set; }
        public string ypcAttribution { get; set; }
    }
}
