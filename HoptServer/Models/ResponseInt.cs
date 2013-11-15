using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoptServer
{
    public class ResponseInt
    {
        public string name { get; set; }
        public int value { get; set; }

        public ResponseInt(string name, int value)
        {
            this.name = name;
            this.value = value;
        }
    }
}
