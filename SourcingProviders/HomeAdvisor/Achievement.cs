using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FcSoftware.SourcingProviders.HomeAdvisor
{
    public class Achievement
    {
        private static List<Achievement> _possibleAchievements;

        public static List<Achievement> PossibleAchievements
        {
            get
            {
                if (_possibleAchievements == null)
                    LoadAchievements();

                return _possibleAchievements;
            }
        }

        private Achievement() { }

        private Achievement(string image, string name)
        {
            Image = image;
            Name = name;
        }


        public string Image { get; set; }

        public string Name { get; set; }

        private static void LoadAchievements()
        {
            _possibleAchievements = new List<Achievement>();

            _possibleAchievements.Add(new Achievement("toprated.png", "Top Rated Professional"));
            _possibleAchievements.Add(new Achievement("elite.png", "Elite Service Professional"));
            _possibleAchievements.Add(new Achievement("soap.png", "Seal of Approval"));
            _possibleAchievements.Add(new Achievement("1year.png", "One Year With Home Advisor"));
            _possibleAchievements.Add(new Achievement("3year.png", "Three Years With Home Advisor"));
            _possibleAchievements.Add(new Achievement("5year.png", "Five Years With Home Advisor"));
            _possibleAchievements.Add(new Achievement("10year.png", "Ten Years With Home Advisor"));
            _possibleAchievements.Add(new Achievement("20reviews.png", "20 Homeowner Reviews"));
            _possibleAchievements.Add(new Achievement("50reviews.png", "50 Homeowner Reviews"));
            _possibleAchievements.Add(new Achievement("100reviews.png", "100 Homeowner Reviews"));
        }
    }
}
