using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCapp.Models
{
    public class RegionCoordinates
    {
        public int RegionCoordinatesID { get; set; }

        public string RegionName { get; set; }

        public double Latitude { get; set; }

        public double Longitude { get; set; }

    }
}