using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoptServer.Models
{
    public class Constraint
    {
        public String responseName { get; set; }
        public double upperBound { get; set; } // Maybe set to -1 if it doesn't have one
        public double lowerBound { get; set; } // Maybe set to -1 if it doesn't have one


        public String createSQL()
        {
            return "Hi!";
        }

    }
}
