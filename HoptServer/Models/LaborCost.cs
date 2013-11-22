using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoptServer.Models
{
    public class LaborCost
    {
        public string name { get; set; }
        public double wage { get; set; }
        public ResponseInt[] rooms { get; set; }
    }
}
