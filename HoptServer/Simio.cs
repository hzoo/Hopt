using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimioAPI;
using System.IO;
using HoptServer;

namespace HoptServer
{
    public class Simio
    {
        ISimioProject currentProject;
        IModel currentModel;
        IExperiment currentExperiment;
        Double _runTime;
        Boolean _completed = false;
        List<Response> currentResponses = new List<Response>();

        public Simio()
        {
            //initialize
            SetProject("ED-v10-henry.spfx", "Model", "Experiment1");
        }

        public List<Response> StartExperiment(Configuration c)
        {
            if (currentExperiment.IsBusy)
                return null;
            currentExperiment.Reset();

            // Specify run times.
            IRunSetup setup = currentExperiment.RunSetup;
            setup.StartingTime = new DateTime(2013, 10, 01); //not important?
            //setup.WarmupPeriod = TimeSpan.FromHours(8.0);
            setup.EndingTime = setup.StartingTime + TimeSpan.FromDays(c.DaysToRun);
            System.Diagnostics.Debug.WriteLine("Starting time: " + setup.StartingTime);
            System.Diagnostics.Debug.WriteLine("Warmup time: " + setup.WarmupPeriod); 
            System.Diagnostics.Debug.WriteLine("Ending time: " + setup.EndingTime);

            //set number of replications
            //foreach (IScenario scenario in currentExperiment.Scenarios)
            //    scenario.ReplicationsRequired = c.NumberOfReps;
            currentExperiment.Scenarios[0].ReplicationsRequired = c.NumberOfReps;
            System.Diagnostics.Debug.WriteLine("Number of scenarios: " + currentExperiment.Scenarios.Count); 
            currentExperiment.Scenarios.Remove(currentExperiment.Scenarios[1]);
            currentExperiment.Scenarios.Remove(currentExperiment.Scenarios[1]);
            currentExperiment.Scenarios.Remove(currentExperiment.Scenarios[1]);
            currentExperiment.Scenarios.Remove(currentExperiment.Scenarios[1]);
            System.Diagnostics.Debug.WriteLine("Number of scenarios: " + currentExperiment.Scenarios.Count);
            System.Diagnostics.Debug.WriteLine("Number of reps: " + currentExperiment.Scenarios[0].ReplicationsRequired);

            for (int i = 0; i < c.Rooms.Length; i++) {
                string num = "";
                currentExperiment.Scenarios[0].SetControlValue(currentExperiment.Controls[i], c.Rooms[i].num.ToString());
                currentExperiment.Scenarios[0].GetControlValue(currentExperiment.Controls[i], ref num);
                System.Diagnostics.Debug.WriteLine("Control " + i + ": " + num);
            }

            //listeners
            currentExperiment.ScenarioEnded += new EventHandler<ScenarioEndedEventArgs>(experiment_ScenarioEnded);
            currentExperiment.RunCompleted += new EventHandler<RunCompletedEventArgs>(experiment_RunCompleted);
            currentExperiment.RunProgressChanged += new EventHandler<RunProgressChangedEventArgs>(experiment_RunProgressChanged);
            currentExperiment.ReplicationEnded += new EventHandler<ReplicationEndedEventArgs>(experiment_ReplicationEnded);

            //run
            //currentExperiment.RunAsync();
            System.Diagnostics.Debug.WriteLine("Start Run");
            try
            {
                currentExperiment.RunAsync();
                int i = 0;
                do
                {
                    i++;
                }
                while (!(_completed));
                System.Diagnostics.Debug.WriteLine("End Run");

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Error || " + ex.Message);
            }
            return currentResponses;
        }

        void experiment_RunCompleted(object sender, RunCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Run Completed");
        }

        void experiment_RunProgressChanged(object sender, RunProgressChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Run Progress Changed");
        }

        void experiment_ReplicationEnded(object sender, ReplicationEndedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Replication Ended");
        }

        void experiment_ScenarioEnded(object sender, ScenarioEndedEventArgs e)
        {
            IExperiment experiment = (IExperiment)sender;

            // get response value
            foreach (IExperimentResponse response in experiment.Responses)
            {
                double responseValue = 0.0;
                if (e.Scenario.GetResponseValue(response, ref responseValue))
                {
                    //e.Scenario.Name - 001
                    //response.Name - AvgTimeInSystem
                    //responseValue.ToString() - Value
                    System.Diagnostics.Debug.WriteLine(e.Scenario.Name + " " + response.Name + " " + responseValue.ToString());
                    
                    //only if we want to send back individual responses
                    Response r = new Response(response.Name, responseValue);
                    currentResponses.Add(r);
                }
            }
            _completed = true;
            System.Diagnostics.Debug.WriteLine("Scenario Ended");
        }

        public void SetProject(string project, string model, string experiment)
        {
            // Set extension folder path
            SimioProjectFactory.SetExtensionsPath(Directory.GetCurrentDirectory().ToString());

            // Open project
            string[] warnings;
            currentProject = SimioProjectFactory.LoadProject(project, out warnings);
            if (model != null || model != "")
            {
                SetModel(model);
                SetExperiment(experiment);
                //return currentProject;
            }
            //return null;
        }

        public ISimioProject GetCurrentProject()
        {
            return currentProject;
        }

        public void SetModel(string model)
        {
            if (currentProject != null)
            {
                currentModel = currentProject.Models[model];
                //return currentModel;
            }
            //return null;
        }

        public IModel GetCurrentModel()
        {
            return currentModel;
        }

        public void SetExperiment(string experiment)
        {
            currentExperiment = currentModel.Experiments[experiment];
        }
    }
}
