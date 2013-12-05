using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;
using Microsoft.Owin.Cors;
using SimioAPI;
using System.Collections.Generic;
using HoptServer.Models;

namespace HoptServer
{
    class Program
    {
        static void Main(string[] args)
        {
            // This will *ONLY* bind to localhost, if you want to bind to all addresses
            // use http://*:8080 to bind to all addresses.
            // See http://msdn.microsoft.com/en-us/library/system.net.httplistener.aspx
            // for more information.

            //find open port
            Boolean localhost = true;
            int port = 8001;
            Boolean openPortNotFound = true;
            string url;
            while (openPortNotFound)
            {
                try
                {
                    if (localhost)
                    {
                        url = "http://localhost:" + port.ToString();
                    }
                    else
                    {
                        url = "http://*:" + port.ToString();
                    }
                    using (WebApp.Start<Startup>(url))
                    {
                        Console.WriteLine("Server running on {0}", url);
                        Console.ReadLine();
                        openPortNotFound = false;
                    }
                }
                catch (Exception)
                {
                    Console.WriteLine("Port " + port + " Closed");
                    port++;
                }
            }
        }
    }
    class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.Map("/signalr", map =>
            {
                // Setup the cors middleware to run before SignalR.
                // By default this will allow all origins. You can
                // configure the set of origins and/or http verbs by
                // providing a cors options with a different policy.
                map.UseCors(CorsOptions.AllowAll);

                var hubConfiguration = new HubConfiguration
                {
                    // You can enable JSONP by uncommenting line below.
                    // JSONP requests are insecure but some older browsers (and some
                    // versions of IE) require JSONP to work cross domain
                    EnableJSONP = true
                };

                // Run the SignalR pipeline. We're not using MapSignalR
                // since this branch is already runs under the "/signalr"
                // path.
                map.RunSignalR(hubConfiguration);
            });
        }
    }
    public class ChatHub : Hub
    {
        Simio s = new Simio();

        public void RunConfig(Configuration c)
        {
            s.chooseModel(c);
            List<Response> r = s.StartExperiment(c);
            Clients.All.getResponses(r);
        }

        //public void ReturnNextConfig(Configuration c, Double previousCost)
        //{
        //    List<Response> r = s.StartExperiment(c);
        //    Clients.All.getResponses(r);
        //}
    }
    public class OptHub : Hub
    {
        Simio s = new Simio();

        public double getUtilizationForRoomType(string name, ConfigResult cr) {
            double utilization = 0.0;
            if (name == "examroomu")
            {
                utilization = cr.examroomu;
            }
            else if (name == "traumau")
            {
                utilization = cr.traumau;
            }
            else if (name == "fasttracku")
            {
                utilization = cr.fasttracku;
            }
            else if (name == "rapidadmissionu")
            {
                utilization = cr.rapidadmissionu;
            }
            else if (name == "observationu")
            {
                utilization = cr.observationu;
            }
            else if (name == "behavioru")
            {
                utilization = cr.behavioru;
            }
            return utilization;
        }

        //public void findRoomConstraintsForRoomType(Constraint[] cs, int num, string name, Configuration c, ConfigResult cr, Configuration c2, ConfigResult cr2)
        //{
        //    // Check Utilization
        //    cs[num].responseName = name;
        //    // If less than 50%, don't even both trying to add

        //    double utilization = getUtilizationForRoomType(name, cr);
        //    //System.Diagnostics.Debug.WriteLine(name + " " + utilization);
        //    if (utilization < 50)
        //    {
        //        cs[num].lowerBound = c.rooms[num].num;
        //        cs[num].upperBound = c.rooms[num].num;
        //    }
        //    else
        //    {
        //        cs[num].lowerBound = c.rooms[num].num;
        //        // looping until util 50%
        //        for (int i = c.rooms[num].num + 4; i < 1000; i += 4)
        //        {
        //            c2 = (Configuration) c.Clone();
        //            c2.rooms[num].num = i;
        //            cr2 = s.RunOpt(c2);
        //            double utilization2 = getUtilizationForRoomType(name, cr2);
        //            //System.Diagnostics.Debug.WriteLine(name + " " + utilization);
        //            if (utilization2 < 50)
        //            {
        //                cs[num].upperBound = i;
        //                break;
        //            }
        //        }
        //    }
        //}

        public void FindOpt(int num, ref Configuration c, ref ConfigResult cr, ref Boolean canIterate)
        {
            //System.Diagnostics.Debug.WriteLine("Find Opt");
            //double utilization = getUtilizationForRoomType(name, cr);
            Boolean costDecreases = true;
            while (costDecreases == true)
            {
                System.Diagnostics.Debug.WriteLine("New: " + c.rooms[num].num + " Old:" + c.rooms[num].originalNum);
                if (c.rooms[num].num <= c.rooms[num].originalNum)
                {
                    c.rooms[num].num = c.rooms[num].originalNum;
                    costDecreases = false;
                }
                else
                {
                    double interestRate = 0.04;
                    double growthRate = 0.03;
                    int yearsToCompletion = 5;
                    int yearsAhead = 10;
                    double[] utilResponses = new double[6];
                    utilResponses[0] = cr.examroomu;
                    utilResponses[1] = cr.traumau;
                    utilResponses[2] = cr.fasttracku;
                    utilResponses[3] = cr.rapidadmissionu;
                    utilResponses[4] = cr.behavioru;
                    utilResponses[5] = cr.observationu;
                    HoptServer.Models.CalculateCosts calc = new HoptServer.Models.CalculateCosts();
                    double oldTotalCost = calc.costAtConstructionStart(c.costInfo, c.rooms, c.acuityInfo, c.arrivalInfo, interestRate, growthRate, yearsToCompletion, yearsAhead, c.daysToRun, utilResponses, cr.LWBS);

                    c.rooms[num].num = c.rooms[num].num - 1;
                    System.Diagnostics.Debug.WriteLine("Num rooms: " + c.rooms[num].num);
                    cr = s.RunOptNew(c);

                    System.Diagnostics.Debug.WriteLine("Cost: " + s.getTotalCost() + " " + oldTotalCost);
                    Boolean waitingTime = false;
                    if (c.rooms[0].included == true) {
                        waitingTime = waitingTime || cr.examroom_wt > Convert.ToDouble(c.serviceInfo[0].averageRoomTime);
                    }
                    if (c.rooms[1].included == true)
                    {
                        waitingTime = waitingTime || cr.trauma_wt > Convert.ToDouble(c.serviceInfo[1].averageRoomTime);
                    }
                    if (c.rooms[2].included == true)
                    {
                        waitingTime = waitingTime || cr.fasttrack_wt > Convert.ToDouble(c.serviceInfo[2].averageRoomTime);
                    }

                    if (s.getTotalCost() > oldTotalCost || waitingTime == true)
                    {
                        costDecreases = false;
                        c.rooms[num].num = c.rooms[num].num + 1;
                        double totalCost = calc.costAtConstructionStart(c.costInfo, c.rooms, c.acuityInfo, c.arrivalInfo, interestRate, growthRate, yearsToCompletion, yearsAhead, c.daysToRun, utilResponses, cr.LWBS);
                        System.Diagnostics.Debug.WriteLine("Cost: " + totalCost);
                    }
                    else
                    {
                        canIterate = true;
                    }
                }
            }
            //cr = s.RunOptNew(c);
        }

        public void RunOpt(Configuration c)
        {
            System.Diagnostics.Debug.WriteLine("Run Opt");
            s.chooseModel(c);
            s.LoadHospitalData(c);
            ConfigResult cr = s.RunOptNew(c);
            Boolean canIterate = true;
            int counter = 0;
            //while (canIterate == true) {
                canIterate = false;
            for (int j = 0; j < 2; j++) {
                for (int i = 0; i < 6; i++)
                {
                    if (c.rooms[i].included == true)
                    {
                        FindOpt(i, ref c, ref cr, ref canIterate);
                    }
                }
                counter++;
            }
            System.Diagnostics.Debug.WriteLine("Number of iterations: " + counter);
            //cr = s.RunOptNew(c);

            for (int i = 0; i < 6; i++)
            {
                System.Diagnostics.Debug.WriteLine(c.rooms[i].num);
            }

            Clients.All.getResponses(c,cr);

            //Constraint[] cs = new Constraint[6];
            //for(int i = 0; i < 6; i++)
            //{
            //    cs[i] = new Constraint();
            //}

            //// Run given config to begin with
            //ConfigResult cr = s.RunOpt(c);
            //System.Diagnostics.Debug.WriteLine(cr.examroomu);
            //System.Diagnostics.Debug.WriteLine(cr.traumau);
            //System.Diagnostics.Debug.WriteLine(cr.fasttracku);
            //System.Diagnostics.Debug.WriteLine(cr.rapidadmissionu);
            //System.Diagnostics.Debug.WriteLine(cr.observationu);
            //System.Diagnostics.Debug.WriteLine(cr.behavioru);

            //// Check Utilization
            //Configuration c2 = new Configuration();
            //ConfigResult cr2 = new ConfigResult();

            //findRoomConstraintsForRoomType(cs,0, "examroomu", c, cr, c2, cr2);
            //findRoomConstraintsForRoomType(cs,1, "traumau", c, cr, c2, cr2);
            //findRoomConstraintsForRoomType(cs,2, "fasttracku", c, cr, c2, cr2);
            //findRoomConstraintsForRoomType(cs,3, "rapidadmissionu", c, cr, c2, cr2);
            //findRoomConstraintsForRoomType(cs,4, "behavioru", c, cr, c2, cr2);
            //findRoomConstraintsForRoomType(cs,5, "observationu", c, cr, c2, cr2);
            //System.Diagnostics.Debug.WriteLine("ExamRoom: " + cs[0].lowerBound + "," + cs[0].upperBound);
            //System.Diagnostics.Debug.WriteLine("Trauma: " + cs[1].lowerBound + "," + cs[1].upperBound);
            //System.Diagnostics.Debug.WriteLine("FastTrack: " + cs[2].lowerBound + "," + cs[2].upperBound);
            //System.Diagnostics.Debug.WriteLine("Rapid Admission: " + cs[3].lowerBound + "," + cs[3].upperBound);
            //System.Diagnostics.Debug.WriteLine("Behvioral: " + cs[4].lowerBound + "," + cs[4].upperBound);
            //System.Diagnostics.Debug.WriteLine("Observation: " + cs[5].lowerBound + "," + cs[5].upperBound);
        }

        //    int iterations = 0;
        //    while (iterations < 3)
        //    {
        //        ConfigResult cr = s.RunOpt(c);
        //        //check waiting time (primary (exam, fast track, trauma) - secondary (rapid admission, observation, behavioral)

        //        //check utilization
        //        if (cr.examroomu > 80) // need to find the right index by hardcode in simio or using a for loop
        //        {
        //            c.rooms[0].num = c.rooms[0].num + 1;
        //        }

        //        iterations++;
        //    }

        //    //Clients.All.getResponses(r);
        //}

        public void RunConfig(Configuration c)
        {
            s.chooseModel(c);
            System.Diagnostics.Debug.WriteLine("RunConfig");
            List<Response> r = s.StartExperiment(c);
            Clients.All.getResponses(r);
        }

        //public void ReturnNextConfig(Configuration c, Double previousCost)
        //{
        //    System.Diagnostics.Debug.WriteLine("ReturnNextConfig");
        //    List<Response> r = s.StartExperiment(c);
        //    Clients.All.getResponses(r);
        //}
    }
    //public class SimioOptHub : Hub
    //{
    //    private readonly SimioOpt _simio;

    //    //public SimioOptHub() : this(SimioOpt.Instance) { }

    //    public SimioOptHub(SimioOpt simio)
    //    {
    //        _simio = simio;
    //    }

    //    //public IEnumerable<Stock> GetAllStocks()
    //    //{
    //    //    return _simio.GetAllStocks();
    //    //}
    //}
}