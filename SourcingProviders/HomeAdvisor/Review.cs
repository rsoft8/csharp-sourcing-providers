using System;

namespace FcSoftware.SourcingProviders.HomeAdvisor
{
    public class Review
    {
        public string Id { get; set; }
        public double Rating { get; set; }

        public DateTime DatePublished { get; set; }

        public string Author { get; set; }

        public string Body { get; set; }

        public string Project { get; set;}

        public string Location { get; set; }
    }
}