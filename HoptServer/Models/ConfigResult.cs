using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HoptServer
{
    public class ConfigResult
    {
        public int examRoom;
        public int trauma;
        public int fastTrack;
        public int rapidAdmission;
        public int behavioral;
        public int observation;
        public double timeinsystem;
        public double avgwaitingtime;
        public double avgnumberinwaitingroom;
        public double traumau;
        public double examroomu;
        public double fastttracku;
        public double rapidadmissionu;
        public double behavioru;
        public double observationu;
        public double LWBS;
        public double cost;

        public ConfigResult(int e, int t, int f, int r, int b, int o, double ti, double aw, double an, double tr, double ex, double fa, double ra, double be, double ob, double l, double c)
        {
            examRoom = e;
            trauma = t;
            fastTrack = f;
            rapidAdmission = r;
            behavioral = b;
            observation = o;
            timeinsystem = ti;
            avgwaitingtime = aw;
            avgnumberinwaitingroom = an;
            traumau = tr;
            examroomu = ex;
            fastttracku = fa;
            rapidadmissionu = ra;
            behavioru = be;
            observationu = ob;
            LWBS = l;
            cost = c;
        }
    
    }
}
