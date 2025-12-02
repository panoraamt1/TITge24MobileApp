using MauiPlanets.Models;

namespace MauiPlanets.Services
{
    internal class PlanetsServices
    {
        private static List<Planet> planets = new()
        {
            new()
            {
                Name = "Mercury",
                Subtitle = "The smallest planet",
                HeroImage = "mercury.png",
                Description = "Mercury is the first planet from the Sun and the smallest in the Solar System. It is a rocky planet with a trace atmosphere and a surface gravity slightly higher than that of Mars. The surface of Mercury is similar to Earth's Moon, being heavily cratered, with an expansive rupes system generated from thrust faults, and bright ray systems, formed by ejecta. Its largest crater, Caloris Planitia, has a diameter of 1,550 km (960 mi), which is about one-third the diameter of the planet (4,880 km or 3,030 mi). Being the most inferior orbiting planet, it always appears close to the sun in Earth's sky, either as a \"morning star\" or an \"evening star\". It is the planet with the highest delta-v required for travel from Earth, as well as to and from the other planets in the Solar System.",
                AccentColorStart = Color.FromArgb("#353535"),
                AccentColorEnd = Color.FromArgb("#8d9098"),
                Images = new()
                {
                    "https://science.nasa.gov/wp-content/uploads/2023/11/mercury-messenger-globe-pia15162.jpg",
                    "https://science.nasa.gov/wp-content/uploads/2023/09/spectra-mercury.jpg?w=1024",
                    "https://cdn.mos.cms.futurecdn.net/v2/t:0,l:240,cw:1440,ch:1080,q:80,w:1440/w3kqDGBSTqVnNTpd5pajWm.jpg",

                }
            }
        };

        public static List<Planet> GetFeaturedPlanets()
        { 
            var random = new Random();
            var randomizePlanet = planets.OrderBy(item => random.Next());

            return randomizePlanet.Take(2).ToList();
        }

        public static List<Planet> GetAllPlanets() => planets;
    }
}
