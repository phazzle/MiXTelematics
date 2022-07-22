using System;
using System.Collections.Generic;
using System.Text;

namespace geo
{
    class Vehicle
    {
        public int PositionId { get; set; }
        public string VehicleRegistration { get; set; }
        public float Latitude { get; set; }
        public float Longitude { get; set; }
        public DateTime RecordedTimeUTC { get; set; }
        public int Index { get; set; }
    }
}
