using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    public class Position
    {
        public double lat { get; set; }
        public double lng { get; set; }

        public override string ToString() { return $"Latitude: {lat}, Longitude: {lng}"; }
    }
}
