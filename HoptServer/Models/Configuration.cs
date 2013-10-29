using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoptServer
{
    public class Configuration
    {
        public int DaysToRun { get; set; }
        public int NumberOfReps { get; set; }
        public RoomType[] Rooms { get; set; }

        public override string ToString()
        {
            string s = "";
            foreach (RoomType room in Rooms)
            {
                s += room.num + ",";
            }
            return s;
        }
    }
}
