using System;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin.Hosting;
using Owin;
using Microsoft.Owin.Cors;
using SimioAPI;
using System.Collections.Generic;

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
            List<Response> r = s.StartExperiment(c);
            Clients.All.getResponses(r);
        }

        public void ReturnNextConfig(Configuration c, Double previousCost)
        {
            List<Response> r = s.StartExperiment(c);
            Clients.All.getResponses(r);
        }
    }
    public class OptHub : Hub
    {
        Simio s = new Simio();

        public void RunConfig(Configuration c)
        {
            System.Diagnostics.Debug.WriteLine("RunConfig");
            List<Response> r = s.StartExperiment(c);
            Clients.All.getResponses(r);
        }
        
        public void ReturnNextConfig(Configuration c, Double previousCost)
        {
            System.Diagnostics.Debug.WriteLine("ReturnNextConfig");
            List<Response> r = s.StartExperiment(c);
            Clients.All.getResponses(r);
        }
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