using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoptServer
{
    public class ResponseString
    {
        public String name { get; set; }
        public String value { get; set; }

        public ResponseString(string name, string value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
