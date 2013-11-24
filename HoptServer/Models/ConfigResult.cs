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
        public double fasttracku;
        public double rapidadmissionu;
        public double behavioru;
        public double observationu;
        public double LWBS;
        public double initialCost;
        public double annualCost;
        public double totalCost;
        public int totalVisits;
        public double examroom_wt;
        public double trauma_wt;
        public double fasttrack_wt;

        public ConfigResult()
        {

        }

        public ConfigResult(int e, int t, int f, int r, int b, int o, double ti, double aw, double an, double tr, double ex, double fa, double ra, double be, double ob, double l, double ic, double ac, double tc, int tv, double erwt, double twt, double ftwt)
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
            fasttracku = fa;
            rapidadmissionu = ra;
            behavioru = be;
            observationu = ob;
            LWBS = l;
            initialCost = ic;
            annualCost = ac;
            totalCost = tc;
            totalVisits = tv;
            examroom_wt = erwt;
            trauma_wt = twt;
            fasttrack_wt = ftwt;
        }
    
    }
}
