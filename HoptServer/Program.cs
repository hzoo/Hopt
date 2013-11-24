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
            int port = 8001;
            Boolean openPortNotFound = true;
            while (openPortNotFound)
            {
                try
                {
                    string url = "http://*:" + port.ToString();
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
        public void RunOpt(Configuration c)
        {
            System.Diagnostics.Debug.WriteLine("Run Opt");
            s.chooseModel(c);
            s.LoadHospitalData(c);
            Constraint[] cs = new Constraint[6];
            for(int i = 0; i < 6; i++)
            {
                cs[i] = new Constraint();
            }
            
            // Run given config to begin with
            ConfigResult cr = s.RunOpt(c);
            // Check Utilization
            cs[0].responseName = "ExamRoom";
            // If less than 50%, don't even both trying to add
            if (cr.examroomu < 50)
            {
                cs[0].lowerBound = c.rooms[0].num;
                cs[0].upperBound = c.rooms[0].num;
            }
            else
            {
                cs[0].lowerBound = c.rooms[0].num;
                // looping until util 50%
                for (int i = c.rooms[0].num + 4; i < 1000; i += 4)
                {
                    Configuration c2 = c;
                    c2.rooms[0].num = i;
                    ConfigResult cr2 = s.RunOpt(c2);
                    if (cr2.examroomu < 50)
                    {
                        cs[0].upperBound = i;
                        break;
                    }
                }
            }
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