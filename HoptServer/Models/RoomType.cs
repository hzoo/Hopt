using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoptServer
{
    public class RoomType
    {
        public int id { get; set; }
        public String name { get; set; }
        public int num { get; set; }
        public int originalNum { get; set; }
        public int optNum { get; set; }
        public int maxNum { get; set; }
        public Boolean included { get; set; }
    }
}
