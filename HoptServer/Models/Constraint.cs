using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoptServer.Models
{
    public class Constraint
    {
        public String responseName { get; set; }
        public double upperBound { get; set; } // Maybe set to -1 if it doesn't have one
        public double lowerBound { get; set; } // Maybe set to -1 if it doesn't have one


        public String createSQL()
        {
            String sql = responseName;
            if(lowerBound >= 0 && upperBound >= 0)
                sql += " >= " + lowerBound + " and " + responseName  + " <= " + upperBound;
            else if(lowerBound >= 0)
                sql += " >= " + lowerBound;
            else if(upperBound >= 0)
                sql += " <= " + upperBound;
            else
                sql = null;

            return sql;
                
        }

    }
}
