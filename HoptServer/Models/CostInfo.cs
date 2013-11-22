using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoptServer.Models
{
    public class CostInfo
    {
        public CapitalCost[] capital { get; set; }
        public LaborCost[] labor { get; set; }
        public Response utility { get; set; }
    }
}
