using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SignalRSelfHost
{
    public class Responses
    {
        public String name { get; set; }

        public Responses(string name)
        {
            this.name = name;
        }
    }
}
