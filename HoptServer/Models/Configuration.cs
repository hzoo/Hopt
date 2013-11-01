using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoptServer
{
    public class Configuration
    {
        public int daysToRun { get; set; }
        public int numberOfReps { get; set; }
        public RoomType[] rooms { get; set; }

        public override string ToString()
        {
            string s = "";
            foreach (RoomType room in rooms)
            {
                if (room.included == true)
                {
                    s += room.num + ",";
                }
            }
            return s;
        }
    }
}
