using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SignalRSelfHost
{
    public class Response
    {
        public String name  { get; set; }
        public double value { get; set; }

        public Response(string name, double value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
