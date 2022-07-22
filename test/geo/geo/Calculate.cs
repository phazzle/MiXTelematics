using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace geo
{
    class Calculate
    {

        //file path \test\geo\geo\bin\Debug\netcoreapp3.1\VehiclePositions.dat
        string fileName = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\VehiclePositions.dat";

        //StopWatch to keep track of time
        System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();


        //String builder to create a string from char 
        StringBuilder sb = new StringBuilder();

        //default date
        DateTime dt = new DateTime(1970, 1, 1, 0, 0, 0, 0);

        //create a vehicle list. The list comes from the binary file.
        List<Vehicle> vlist = new List<Vehicle>();
        int startIndex = 0;

        //create a location list that will store all locations -> (10 Items)
        readonly List<Location> locations = new List<Location>();
        List<Vehicle> near_by = new List<Vehicle>();


        /// <summary>
        /// 1.Read from the file and store the data to vlist (Vehicle list)
        /// 2.Call Calculate distance between 2 coordinates
        /// 3.Sort the coordinates using index. Vehicle.Index.
        /// </summary>

        public Calculate(List<Location> locations)
        {
            this.locations = locations;
        }

        public void doCalculations()
        {

            if (!File.Exists(fileName))
            {
                Console.WriteLine("Unable to find the file");
            }
            else
            {
                //step 1 load data from binary file
                stopwatch.Start();
                byte[] fileBytes = File.ReadAllBytes(fileName);
                runAddDataToList(fileBytes);
                stopwatch.Stop();
                Console.WriteLine(string.Format("Data file read execution time : {0} ms", (object)stopwatch.ElapsedMilliseconds));
                long readDataElapsedMilliseconds = stopwatch.ElapsedMilliseconds;
                //end step 1

                //step 2 calculate distance and sort by nearest vehicle location.
                stopwatch.Start();

                //start calculations
                //Split the data in Partitions
                var rangePartitioner = Partitioner.Create(0, locations.Count);

                System.Threading.Tasks.Parallel.ForEach(rangePartitioner, (range, loopState) =>
                {
                    for (int j = range.Item1; j < range.Item2; j++)
                    {
                        //maximun and minmum latitude and longitude +-180
                        double maxValue = 180.1;
                        int index = 0;
                        for (int i = 0; i < vlist.Count; i++)
                        {
                            Location vLocation = new Location() { Longitude = vlist[i].Longitude, Latitude = vlist[i].Latitude };
                            
                            double result = haversine_distance(locations[j], vLocation);
                            if (result < maxValue)
                            {
                                index = i;
                                maxValue = result;
                            }

                        }
                    //add the smallest value with an index since we are using Partitions.
                    //the list will be unorderd
                    vlist[index].Index = j;
                        near_by.Add(vlist[index]);
                    }
                });

                //order the list//
                near_by = near_by.OrderBy(z => z.Index).ToList();

                //end calculations


                //Display the stopwatch ElapsedMilliseconds
                stopwatch.Stop();
                Console.WriteLine(string.Format("Closest position calculation execution time {0} ms", (object)stopwatch.ElapsedMilliseconds));
                Console.WriteLine(string.Format("Total execution time : {0} ms", (object)(readDataElapsedMilliseconds + stopwatch.ElapsedMilliseconds)));
                Console.WriteLine();

                near_by.ForEach(delegate (Vehicle v)
                {
                    Console.WriteLine("ID : {0}\n Lat: {1}\n Long: {2}\n Time: {3} \n Registartaion : {4} \n Index : {5} \n\n",
                            v.PositionId, v.Latitude, v.Longitude, v.RecordedTimeUTC, v.VehicleRegistration, v.Index + 1);
                });
                //end step 2///
            }

        }


        //Add data to the list. The list lenght/30 is to 2 million vehicles
        void runAddDataToList(byte[] fileBytes)
        {
            for (int x = 0; x < fileBytes.Length / 30; x++)
            {
                addDataToList(fileBytes);

            }
        }

        ///<summary>
        ///Convert byte to a list. In order to read a specific vehicle it would take 30 indexes
        ///To read PostionId, Latitude, Longitude it takes 4 indexes (12 in total)
        ///VehicleRegistration takes 10 indexes
        ///To read RecordedTimeUTC takes 8 indexes
        ///</summary>
        void addDataToList(byte[] fileBytes)
        {
            Vehicle vehicle = new Vehicle();
            vehicle.PositionId = BitConverter.ToInt32(fileBytes, startIndex);
            startIndex = startIndex + 4;
            StringBuilder stringBuilder = new StringBuilder();
            while (fileBytes[startIndex] != (byte)0)
            {
                stringBuilder.Append((char)fileBytes[startIndex]);
                startIndex++;
            }
            vehicle.VehicleRegistration = stringBuilder.ToString();
            startIndex++;
            vehicle.Latitude = BitConverter.ToSingle(fileBytes, startIndex);
            startIndex = startIndex + 4;
            vehicle.Longitude = (BitConverter.ToSingle(fileBytes, startIndex));
            startIndex = startIndex + 4;

            vehicle.RecordedTimeUTC = dt.AddSeconds((double)(BitConverter.ToUInt64(fileBytes, startIndex)));
            startIndex = startIndex + 8;

            vlist.Add(vehicle);
        }

        //Haversine distance is a method to calculate distance between 2 points it will return a double as a distance.
        static double haversine_distance(Location mk1, Location mk2)
        {
            double R = 3958.8; // Radius of the Earth in miles
            double rlat1 = mk1.Latitude * (Math.PI / 180); // Convert degrees to radians
            double rlat2 = mk2.Latitude * (Math.PI / 180); // Convert degrees to radians
            double difflat = rlat2 - rlat1; // Radian difference (latitudes)
            double difflon = (mk2.Longitude - mk1.Longitude) * (Math.PI / 180); // Radian difference (longitudes)

            return 2 * R * Math.Asin(Math.Sqrt(Math.Sin(difflat / 2) * Math.Sin(difflat / 2) + Math.Cos(rlat1) * Math.Cos(rlat2) * Math.Sin(difflon / 2) * Math.Sin(difflon / 2)));

        }



    }
}
