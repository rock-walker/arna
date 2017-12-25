using System;

namespace AP.EntityModel.Common
{
    public class GeoMarker
    {
        public Guid ID { get; set; }
        public double Lat { get; set; }
        public double Lng { get; set; }

        public override string ToString()
        {
            return Lat + "; " + Lng;
        }
    } 
}
