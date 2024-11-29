using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    public class Place
    {
        public String name { get; set; }
        public String address { get; set; }
        public Position position { get; set; }

        public override string ToString() { return $"Name: {name}, Address: {address}, Position: {position}"; }
    }
}
