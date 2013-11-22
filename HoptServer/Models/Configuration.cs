using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HoptServer.Models;

namespace HoptServer
{
    public class Configuration
    {
        public ResponseInt daysToRun { get; set; }
        public ResponseInt numberOfReps { get; set; }
        public Response startupTime { get; set; }
        public ResponseString rateTable { get; set; }
        public Response[] arrivalInfo { get; set; }
        public Response[] acuityInfo { get; set; }
        public ServiceInfo[] serviceInfo { get; set; }
        public ConstraintInfo constraintInfo { get; set; }
        public CostInfo costInfo { get; set; }
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
