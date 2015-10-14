using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcSoftware.SourcingProviders.HomeAdvisor
{
    public class Prospect
    {
        public const int StarsRatingMax = 5;

        public Trade Service { get; set; }

        public string Name { get; set; }

        public string Phone { get; set; }

        public string StreetAddress { get; set; }

        public string AddressLocality { get; set; }

        public string AddressRegion { get; set; }

        public string PostalCode { get; set; }

        public int ReviewCount { get; set; }

        public double StarRating { get; set; }

        public bool RatingAvailable { get; set; }

        public Uri Url { get; set; }

        public string Id { get; set; }

        public List<Review> Reviews { get; set; }

        public List<Achievement> Achievements { get; set; }
    }
}
