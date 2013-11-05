using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using SimioAPI;
using System.IO;
using HoptServer;
using System.Data.SQLite;

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
            SetProject("ED-v11.spfx", "Model", "Experiment1");
            createTables();
        }

        public void createTables()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source = configs.db");
            conn.Open();
            String sql = "Create table if not exists Test2 (MainED int, Trauma int, FastTrack int, RapidAdmission int, Behavioral int, Observation int)";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            sql = "Create table if not exists Results (MainED int, Trauma int, FastTrack int, RapidAdmission int, Behavioral int, Observation int, ";
            sql += "TimeInSystem real, AvgWaitingTime real, AvgNumberinWaitingRoom real, TruamaPeopleInSystem real, FastTrackPeopleInSystem real, MainEDPeopleInSystem real, ";
            sql += "TraumaUtilization real, MainEDUtilization real, FastTrackUtilization real)";
            command.ExecuteNonQuery();
            conn.Close();
        }


        public void insertConfiguration(Configuration c)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source = configs.db");
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("select * from test2", conn);
            var dr = cmd.ExecuteReader();
            for (var i = 0; i < dr.FieldCount; i++)
            {
                Console.WriteLine(dr.GetName(i));
            }
            String sql = "Insert into Test2 (MainED, Trauma, FastTrack, RapidAdmission, Behavioral, Observation) Values ";
            sql += "(@MainED, @Trauma, @FastTrack, @RapidAdmission, @Behavorial, @Observation)";
            SQLiteCommand command = new SQLiteCommand(sql,conn);
            foreach (RoomType room in c.rooms)
            {
                String value = "@" + room.name;
                if(room.included)
                    command.Parameters.AddWithValue(value,room.num);
                else
                    command.Parameters.AddWithValue(value,DBNull.Value);
            }
            foreach (SQLiteParameter param in command.Parameters)
            {
                Console.WriteLine(param.ParameterName + param.Value);
            }
            command.ExecuteNonQuery();
            conn.Close();
            printAllConfigs();
        }

        public void printAllConfigs()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source = configs.db");
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("select * from test2", conn);
            SQLiteDataReader dr = cmd.ExecuteReader();
            while(dr.Read())
            {
                Console.Write("MainEd:" + dr["MainED"].ToString());
                Console.Write(" Trauma:" + dr["Trauma"].ToString());
                Console.Write(" FastTrack:" + dr["FastTrack"].ToString());
                Console.Write(" RapidAdmission:" + dr["RapidAdmission"].ToString());
                Console.Write(" Behavioral:" + dr["Behavioral"].ToString());
                Console.Write(" Observation:" + dr["Observation"].ToString());
                Console.WriteLine();
                Console.WriteLine();

            }
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
            setup.EndingTime = setup.StartingTime + TimeSpan.FromDays(c.daysToRun);
            System.Diagnostics.Debug.WriteLine("Starting time: " + setup.StartingTime);
            System.Diagnostics.Debug.WriteLine("Warmup time: " + setup.WarmupPeriod); 
            System.Diagnostics.Debug.WriteLine("Ending time: " + setup.EndingTime);

            //set number of replications
            //foreach (IScenario scenario in currentExperiment.Scenarios)
            //    scenario.ReplicationsRequired = c.NumberOfReps;
            currentExperiment.Scenarios[0].ReplicationsRequired = c.numberOfReps;
            System.Diagnostics.Debug.WriteLine("Number of scenarios: " + currentExperiment.Scenarios.Count); 
            currentExperiment.Scenarios.Remove(currentExperiment.Scenarios[1]);
            currentExperiment.Scenarios.Remove(currentExperiment.Scenarios[1]);
            currentExperiment.Scenarios.Remove(currentExperiment.Scenarios[1]);
            currentExperiment.Scenarios.Remove(currentExperiment.Scenarios[1]);
            System.Diagnostics.Debug.WriteLine("Number of scenarios: " + currentExperiment.Scenarios.Count);
            System.Diagnostics.Debug.WriteLine("Number of reps: " + currentExperiment.Scenarios[0].ReplicationsRequired);

            for (int i = 0; i < c.rooms.Length; i++)
            {
                if (c.rooms[i].included == true)
                {
                string num = "";
                    currentExperiment.Scenarios[0].SetControlValue(currentExperiment.Controls[i], c.rooms[i].num.ToString());
                currentExperiment.Scenarios[0].GetControlValue(currentExperiment.Controls[i], ref num);
                System.Diagnostics.Debug.WriteLine("Control " + i + ": " + num);
            }
                //else
            }

            insertConfiguration(c);

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
