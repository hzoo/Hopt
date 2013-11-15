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
            SetProject("ED.spfx", "Model", "Experiment1"); //v13
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
            sql += "TimeInSystem real, AvgWaitingTime real, AvgNumberinWaitingRoom real, TraumaPeopleInSystem real, FastTrackPeopleInSystem real, ExamRoomPeopleInSystem real, ";
            sql += "TraumaUtilization real, ExamRoomUtilization real, FastTrackUtilization real)";
            command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();

            conn.Close();
        }

        public void insertResults(Configuration c, List<Response> responses)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source = configs.db");
            conn.Open();
            //SQLiteCommand cmd = new SQLiteCommand("select * from test2", conn);
            //var dr = cmd.ExecuteReader();
            //for (var i = 0; i < dr.FieldCount; i++)
            //{
            //    Console.WriteLine(dr.GetName(i));
            //}
            String sql = "Insert into Results (MainED, Trauma, FastTrack, RapidAdmission, Behavioral, Observation, ";
            sql += "TimeInSystem, AvgWaitingTime, AvgNumberinWaitingRoom, TraumaPeopleInSystem, FastTrackPeopleInSystem, ExamRoomPeopleInSystem, ";
            sql += "TraumaUtilization, ExamRoomUtilization, FastTrackUtilization) Values ";
            sql += "(@MainED, @Trauma, @FastTrack, @RapidAdmission, @Behavorial, @Observation, ";
            sql += "@TimeinSystem, @AvgWaitingTime, @AvgNumberinWaitingRoom, @TraumaPeopleInSystem, @FastTrackPeopleInSystem, @ExamRoomPeopleInSystem, ";
            sql += "@TraumaUtilization, @ExamRoomUtilization, @FastTrackUtilization)";
            SQLiteCommand command = new SQLiteCommand(sql,conn);
            foreach (RoomType room in c.rooms)
            {
                String value = "@" + room.name;
                if(room.included)
                    command.Parameters.AddWithValue(value,room.num);
                else
                    command.Parameters.AddWithValue(value,DBNull.Value);
            }
            foreach (Response r in responses)
            {
                String value = "@" + r.name;
                command.Parameters.AddWithValue(value,r.value);
            }
            command.ExecuteNonQuery();
            conn.Close();
            printAllConfigs();
        }


        public void insertConfiguration(Configuration c)
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source = configs.db");
            conn.Open();
            //SQLiteCommand cmd = new SQLiteCommand("select * from test2", conn);
            //var dr = cmd.ExecuteReader();
            //for (var i = 0; i < dr.FieldCount; i++)
            //{
            //    Console.WriteLine(dr.GetName(i));
            //}
            String sql = "Insert into Test2 (MainED, Trauma, FastTrack, RapidAdmission, Behavioral, Observation) Values ";
            sql += "(@MainED, @Trauma, @FastTrack, @RapidAdmission, @Behavioral, @Observation)";
            SQLiteCommand command = new SQLiteCommand(sql,conn);
            foreach (RoomType room in c.rooms)
            {
                String value = "@" + room.name;
                if(room.included)
                    command.Parameters.AddWithValue(value,room.num);
                else
                    command.Parameters.AddWithValue(value, 0);//DBNull.Value);
            }
            foreach (SQLiteParameter param in command.Parameters)
            {
                Console.WriteLine(param.ParameterName + param.Value);
            }
            command.ExecuteNonQuery();
            conn.Close();
            //printAllConfigs();
        }

        public void printAllConfigs()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source = configs.db");
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("select * from Results", conn);
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

        public void resetResults()
        {
            
        }   




        public List<Response> StartExperiment(Configuration c)
        {
            if (currentExperiment.IsBusy)
                return null;
            currentExperiment.Reset();

            // Specify run times.
            IRunSetup setup = currentExperiment.RunSetup;
            setup.StartingTime = new DateTime(2013, 10, 01); //not important?
            setup.WarmupPeriod = TimeSpan.FromHours(c.startupTime);
            setup.EndingTime = setup.StartingTime + TimeSpan.FromDays(c.daysToRun);
            System.Diagnostics.Debug.WriteLine("Starting time: " + setup.StartingTime);
            System.Diagnostics.Debug.WriteLine("Warmup time: " + setup.WarmupPeriod); 
            System.Diagnostics.Debug.WriteLine("Ending time: " + setup.EndingTime);

            //set number of replications
            //foreach (IScenario scenario in currentExperiment.Scenarios)
            //    scenario.ReplicationsRequired = c.NumberOfReps;
            currentExperiment.Scenarios[0].ReplicationsRequired = c.numberOfReps;
            System.Diagnostics.Debug.WriteLine("Number of scenarios: " + currentExperiment.Scenarios.Count);
            //use only 1 scenario
            while (currentExperiment.Scenarios.Count > 1)
            {
                currentExperiment.Scenarios.Remove(currentExperiment.Scenarios[1]);
            }
            System.Diagnostics.Debug.WriteLine("Number of scenarios: " + currentExperiment.Scenarios.Count);
            System.Diagnostics.Debug.WriteLine("Number of reps: " + currentExperiment.Scenarios[0].ReplicationsRequired);

            //change hospital values
            //arrivals

            //change RateTable.RateScaleFactor
            int annualArrivals;
            if (c.arrivals[0].value >= 0)
                annualArrivals = Convert.ToInt32(c.arrivals[0].value);
            else
                annualArrivals = 100000; // default
            //1.0019 is the sum of the base rate table (the base rate table is all in percents, and it adds up to a little more than 100%)
            currentModel.Facility.IntelligentObjects[0].Properties[29].Value = (annualArrivals/(365*1.0019)).ToString();
            System.Diagnostics.Debug.WriteLine("Rate Scale Factor: " + currentModel.Facility.IntelligentObjects[0].Properties[29].Value);
            
            //change RateTable.RateScaleFactor for peak day
            //double peakFactor;
            //if (c.arrivals[1].value >= 0 && c.arrivals[1].value <= 1)
            //    peakFactor = 12.0/c.arrivals[1].value;
            //else
            //    peakFactor = 1.2; //default
            //currentModel.Facility.IntelligentObjects[0].Properties[29].Value = (peakFactor*c.arrivals[0].value / (365 * 1.0019)).ToString();

            //service
            //for (int i = 0; i < c.serviceTimes.Length; i++)
            //{
            //    if (c.serviceTimes[i].name == "Exam Room" && c.serviceTimes[i].included == true)
            //    {
            //        currentModel.Facility.IntelligentObjects["ExamRoom"].Properties["ProcessingTime"].Value = c.serviceTimes[i].averageRoomTime.ToString();
            //        System.Diagnostics.Debug.WriteLine(c.serviceTimes[i].name + " " + currentModel.Facility.IntelligentObjects["ExamRoom"].Properties["ProcessingTime"].Value);
            //    }
            //    else if (c.serviceTimes[i].name == "Wait Room" && c.serviceTimes[i].included == true)
            //    {
            //        currentModel.Facility.IntelligentObjects["WaitRoom"].Properties["ProcessingTime"].Value = c.serviceTimes[i].averageRoomTime.ToString();
            //        System.Diagnostics.Debug.WriteLine(c.serviceTimes[i].name + " " + currentModel.Facility.IntelligentObjects["WaitRoom"].Properties["ProcessingTime"].Value);
            //    }
            //    else if (c.serviceTimes[i].name == "Trauma" && c.serviceTimes[i].included == true)
            //    {
            //        currentModel.Facility.IntelligentObjects["Trauma"].Properties["ProcessingTime"].Value = c.serviceTimes[i].averageRoomTime.ToString();
            //        System.Diagnostics.Debug.WriteLine(c.serviceTimes[i].name + " " + currentModel.Facility.IntelligentObjects["Trauma"].Properties["ProcessingTime"].Value);
            //    }
            //    else if (c.serviceTimes[i].name == "Fast Track" && c.serviceTimes[i].included == true)
            //    {
            //        currentModel.Facility.IntelligentObjects["FastTrack"].Properties["ProcessingTime"].Value = c.serviceTimes[i].averageRoomTime.ToString();
            //        System.Diagnostics.Debug.WriteLine(c.serviceTimes[i].name + " " + currentModel.Facility.IntelligentObjects["FastTrack"].Properties["ProcessingTime"].Value);
            //    }
            //    else if (c.serviceTimes[i].name == "Rapid Admission" && c.serviceTimes[i].included == true)
            //    {
            //        currentModel.Facility.IntelligentObjects["RapidAdmission"].Properties["ProcessingTime"].Value = c.serviceTimes[i].averageRoomTime.ToString();
            //        System.Diagnostics.Debug.WriteLine(c.serviceTimes[i].name + " " + currentModel.Facility.IntelligentObjects["RapidAdmission"].Properties["ProcessingTime"].Value);
            //    }
            //    else if (c.serviceTimes[i].name == "Behavioral" && c.serviceTimes[i].included == true)
            //    {
            //        currentModel.Facility.IntelligentObjects["Behavioral"].Properties["ProcessingTime"].Value = c.serviceTimes[i].averageRoomTime.ToString();
            //        System.Diagnostics.Debug.WriteLine(c.serviceTimes[i].name + " " + currentModel.Facility.IntelligentObjects["Behavioral"].Properties["ProcessingTime"].Value);
            //    }
            //    else if (c.serviceTimes[i].name == "Observational" && c.serviceTimes[i].included == true)
            //    {
            //        currentModel.Facility.IntelligentObjects["Observational"].Properties["ProcessingTime"].Value = c.serviceTimes[i].averageRoomTime.ToString();
            //        System.Diagnostics.Debug.WriteLine(c.serviceTimes[i].name + " " + currentModel.Facility.IntelligentObjects["Observational"].Properties["ProcessingTime"].Value);
            //    }
            //}

            //acuities
            currentModel.Tables[0].Rows[0].Properties["Probability"].Value = c.acuityInfo[0].value.ToString();
            currentModel.Tables[0].Rows[1].Properties["Probability"].Value = c.acuityInfo[1].value.ToString();
            currentModel.Tables[0].Rows[2].Properties["Probability"].Value = c.acuityInfo[2].value.ToString();
            currentModel.Tables[0].Rows[3].Properties["Probability"].Value = c.acuityInfo[3].value.ToString();
            currentModel.Tables[0].Rows[4].Properties["Probability"].Value = c.acuityInfo[4].value.ToString();
            System.Diagnostics.Debug.WriteLine("Acuity 1 = " + (c.acuityInfo[0].value * 100) + "%");
            System.Diagnostics.Debug.WriteLine("Acuity 2 = " + (c.acuityInfo[1].value * 100) + "%");
            System.Diagnostics.Debug.WriteLine("Acuity 3 = " + (c.acuityInfo[2].value * 100) + "%");
            System.Diagnostics.Debug.WriteLine("Acuity 4 = " + (c.acuityInfo[3].value * 100) + "%");
            System.Diagnostics.Debug.WriteLine("Acuity 5 = " + (c.acuityInfo[4].value * 100) + "%");

            //change control values (the actual configuration)
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
            //insertResults(c, currentResponses);
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
