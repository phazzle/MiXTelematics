
using System.Collections.Generic;

namespace geo
{

    class Program
    {
        static void Main(string[] args)
        {


            List<Location> locations = new List<Location>();
            Location loc1 = new Location { Latitude = 34.544909f, Longitude = -102.100843f };
            locations.Add(loc1);

            Location loc2 = new Location { Latitude = 32.34554f, Longitude = -99.12312f };
            locations.Add(loc2);

            Location loc3 = new Location { Latitude = 33.23423f, Longitude = -100.2141f};
            locations.Add(loc3);

             Location loc4 = new Location { Latitude = 35.19574f, Longitude = -95.3489f };
            locations.Add(loc4);

            Location loc5 = new Location { Latitude = 31.89584f, Longitude = -97.78957f };
            locations.Add(loc5);

            Location loc6 = new Location { Latitude = 32.89584f, Longitude = -101.7896f };
            locations.Add(loc6);

            Location loc7 = new Location { Latitude = 34.11584f, Longitude = -100.2257f };
            locations.Add(loc7);

            Location loc8 = new Location { Latitude = 32.33584f, Longitude = -99.99223f };
            locations.Add(loc8);

            Location loc9 = new Location { Latitude = 33.53534f, Longitude = -94.79223f };
            locations.Add(loc9);

            Location loc10 = new Location { Latitude = 32.23423f, Longitude = -100.2222f };
            locations.Add(loc10);


            Calculate c = new Calculate(locations);
            c.doCalculations();



        }




    }

    
}

