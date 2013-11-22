using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HoptServer.Models
{
    public class RoomConstraint
    {
        public string name { get; set; }
        public double averageWaitTime { get; set; }
        public int maximumNumberOfRooms { get; set; }
    }
}
