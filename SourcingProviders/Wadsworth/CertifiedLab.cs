using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcSoftware.SourcingProviders.Wadsworth
{
    public enum LabCategory
    {
        AirAndEmissions,
        MedicalMarihuana,
        NonPotableWater,
        PotableWater,
        SolidAndHazardousWaste
    }

    public class CertifiedLab
    {
        public CertifiedLab()
        {
            ApprovedCategories = new List<string>();
            ApprovedFieldsOfAnalysis = new List<string>();
        }

        public string Name { get; set; }

        public string Type { get; set; }

        public string ID { get; set; }

        public string Director { get; set; }

        public string Phone { get; set; }
        
        public string Address { get; set; }

        public string County { get; set;  }

        public string Country { get; set; }

        public string LastUpdated { get; set; }

        public List<string> ApprovedCategories { get; set; }

        public List<string> ApprovedFieldsOfAnalysis { get; set; }
    }
}
