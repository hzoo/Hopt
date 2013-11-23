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
        Boolean _completed = false;
        Configuration _c;
        List<Response> currentResponses = new List<Response>();

        public Simio()
        {
            //initialize
            SetProject("ED-v19-opt.spfx", "Model", "Experiment1"); //v19
            createTables();
        }

        public void createTables()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source = configs.db");
            conn.Open();

            String sql = "Create table if not exists Test2 (ExamRoom int, Trauma int, FastTrack int, RapidAdmission int, Behavioral int, Observation int)";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();

            sql = "Create table if not exists Results (ExamRoom int, Trauma int, FastTrack int, RapidAdmission int, Behavioral int, Observation int, ";
            sql += "TimeInSystem real, AvgWaitingTime real, AvgNumberinWaitingRoom real, ";
            sql += "TraumaUtilization real, ExamRoomUtilization real, FastTrackUtilization real, RapidAdmissionUnitUtilization real, BehavioralUtilization real, ObservationUtilization real, ";
            sql += "LWBS real, InitialCost real, AnnualCost real,  TotalCost real)";
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
            String sql = "Insert into Results (ExamRoom, Trauma, FastTrack, RapidAdmission, Behavioral, Observation, ";
            sql += "TimeInSystem, AvgWaitingTime, AvgNumberinWaitingRoom, ";
            sql += "TraumaUtilization, ExamRoomUtilization, FastTrackUtilization, RapidAdmissionUnitUtilization, BehavioralUtilization, ObservationUtilization, ";
            sql += "LWBS, Cost) Values ";
            sql += "(@ExamRoom, @Trauma, @FastTrack, @RapidAdmission, @Behavioral, @Observation, ";
            sql += "@TimeinSystem, @AvgWaitingTime, @AvgNumberinWaitingRoom, ";
            sql += "@TraumaUtilization, @ExamRoomUtilization, @FastTrackUtilization, @RapidAdmissionUnitUtilization, @BehavioralUtilization, @ObservationUtilization, ";
            sql += "@LWBS, @InitialCost, @AnnualCost, @TotalCost)";
            SQLiteCommand command = new SQLiteCommand(sql,conn);
            foreach (RoomType room in c.rooms)
            { 
                String value = "@" + room.name.Replace(" ","");
                Console.WriteLine(value);
                if(room.included)
                    command.Parameters.AddWithValue(value,room.num);
                else
                    command.Parameters.AddWithValue(value,DBNull.Value);
            }
            foreach (Response r in responses)
            {
                String value = "@" + r.name.Replace(" ", "");
                Console.WriteLine(value);
                command.Parameters.AddWithValue(value,r.value);
            }
            command.Parameters.AddWithValue("@InitialCost", -1.0);
            command.Parameters.AddWithValue("@AnnualCost", -1.0);
            command.Parameters.AddWithValue("@TotalCost", -1.0);
            Console.WriteLine(command.Parameters.Count);
            command.ExecuteNonQuery();
            conn.Close();
            printAllResults();
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
            String sql = "Insert into Test2 (ExamRoom, Trauma, FastTrack, RapidAdmission, Behavioral, Observation) Values ";
            sql += "(@ExamRoom, @Trauma, @FastTrack, @RapidAdmission, @Behavioral, @Observation)";
            SQLiteCommand command = new SQLiteCommand(sql,conn);
            foreach (RoomType room in c.rooms)
            {
                String value = "@" + room.name.Replace(" ","");
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

        public void printAllResults()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source = configs.db");
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("select * from Results order by cost", conn);
            SQLiteDataReader dr = cmd.ExecuteReader();
            while(dr.Read())
            {
                Console.Write("ExamRoom:" + dr["ExamRoom"].ToString());
                Console.Write(" Trauma:" + dr["Trauma"].ToString());
                Console.Write(" FastTrack:" + dr["FastTrack"].ToString());
                Console.Write(" RapidAdmission:" + dr["RapidAdmission"].ToString());
                Console.Write(" Behavioral:" + dr["Behavioral"].ToString());
                Console.Write(" Observation:" + dr["Observation"].ToString());
                Console.Write(" LWBS:" + dr["LWBS"].ToString());
                Console.Write(" Cost:" + dr["TotalCost"].ToString());
                Console.WriteLine();
                Console.WriteLine();

            }
        }

        public List<ConfigResult> queryResults()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source = configs.db");
            conn.Open();
            SQLiteCommand cmd = new SQLiteCommand("select * from Results order by cost", conn);
            SQLiteDataReader dr = cmd.ExecuteReader();
            List<ConfigResult> list = new List<ConfigResult>();
            while(dr.Read())
            {
                int examRoom = Convert.ToInt32(dr["ExamRoom"]);
                int trauma = Convert.ToInt32(dr["Trauma"]);
                int fastTrack = Convert.ToInt32(dr["FastTrack"]);
                int rapidAdmission = Convert.ToInt32(dr["RapidAdmission"]);
                int behavioral = Convert.ToInt32(dr["Behavioral"]);
                int observation = Convert.ToInt32(dr["Observation"]);
                double timeinsystem = Convert.ToDouble(dr["TimeInSystem"]);
                double avgwaitingtime = Convert.ToDouble(dr["AvgWaitingTime"]);
                double avgnumberinwaitingroom = Convert.ToDouble(dr["AvgNumberinWaitingRoom"]);
                double traumau = Convert.ToDouble(dr["TraumaUtilization"]);
                double examroomu = Convert.ToDouble(dr["ExamRoomUtilization"]);
                double fastttracku = Convert.ToDouble(dr["FastTrackUtilization"]);
                double rapidadmissionu = Convert.ToDouble(dr["RapidAdmissionUnitUtilization"]);
                double behavioru = Convert.ToDouble(dr["BehaviorUtilization"]);
                double observationu = Convert.ToDouble(dr["ObservationUtilization"]);
                double LWBS = Convert.ToDouble(dr["LWBS"]);
                double cost = Convert.ToDouble(dr["Cost"]);
                ConfigResult c = new ConfigResult(examRoom, trauma, fastTrack, rapidAdmission, behavioral, observation, timeinsystem, avgwaitingtime, avgnumberinwaitingroom, traumau, examroomu, fastttracku, rapidadmissionu, behavioru, observationu, LWBS, cost);
                list.Add(c);
            }
            return list;
            

        }

        public void resetResults()
        {
            SQLiteConnection conn = new SQLiteConnection("Data Source = configs.db");
            conn.Open();
            String sql = "drop table if exists Results";
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
            conn.Close();
        }

        public void setWarmUpTimeInHours(double startupTime)
        {
            IRunSetup setup = currentExperiment.RunSetup;
            setup.StartingTime = new DateTime(2013, 10, 01); //not important?
            setup.WarmupPeriod = TimeSpan.FromHours(startupTime);
            System.Diagnostics.Debug.WriteLine("Starting time: {0,-10}",setup.StartingTime);
            System.Diagnostics.Debug.WriteLine("Warmup time  : {0,-10} days",setup.WarmupPeriod);
        }

        public void setRunLengthInDays(double daysToRun)
        {
            IRunSetup setup = currentExperiment.RunSetup;
            setup.EndingTime = setup.StartingTime + TimeSpan.FromDays(daysToRun);
            System.Diagnostics.Debug.WriteLine("Ending time  : {0,-10}",setup.EndingTime);
        }

        public void setServiceTimes(Configuration c)
        {
            for (int i = 0; i < c.serviceInfo.Length; i++)
            {
                if (c.serviceInfo[i].name == "Exam Room" && c.serviceInfo[i].included == true)
                {
                    currentModel.Facility.IntelligentObjects["ExamRoom"].Properties["ProcessingTime"].Value = c.serviceInfo[i].averageRoomTime.ToString();
                    System.Diagnostics.Debug.WriteLine(c.serviceInfo[i].name + " " + currentModel.Facility.IntelligentObjects["ExamRoom"].Properties["ProcessingTime"].Value);
                }
                //else if (c.serviceTimes[i].name == "Wait Room" && c.serviceTimes[i].included == true)
                //{
                //    currentModel.Facility.IntelligentObjects["WaitRoom"].Properties["ProcessingTime"].Value = c.serviceTimes[i].averageRoomTime.ToString();
                //    System.Diagnostics.Debug.WriteLine(c.serviceTimes[i].name + " " + currentModel.Facility.IntelligentObjects["WaitRoom"].Properties["ProcessingTime"].Value);
                //}
                else if (c.serviceInfo[i].name == "Trauma" && c.serviceInfo[i].included == true)
                {
                    currentModel.Facility.IntelligentObjects["Trauma"].Properties["ProcessingTime"].Value = c.serviceInfo[i].averageRoomTime.ToString();
                    System.Diagnostics.Debug.WriteLine(c.serviceInfo[i].name + " " + currentModel.Facility.IntelligentObjects["Trauma"].Properties["ProcessingTime"].Value + " hours");
                }
                else if (c.serviceInfo[i].name == "Fast Track" && c.serviceInfo[i].included == true)
                {
                    currentModel.Facility.IntelligentObjects["FastTrack"].Properties["ProcessingTime"].Value = c.serviceInfo[i].averageRoomTime.ToString();
                    System.Diagnostics.Debug.WriteLine(c.serviceInfo[i].name + " " + currentModel.Facility.IntelligentObjects["FastTrack"].Properties["ProcessingTime"].Value + " hours");
                }
                else if (c.serviceInfo[i].name == "Rapid Admission" && c.serviceInfo[i].included == true)
                {
                    currentModel.Facility.IntelligentObjects["RapidAdmissionUnit"].Properties["ProcessingTime"].Value = c.serviceInfo[i].averageRoomTime.ToString();
                    System.Diagnostics.Debug.WriteLine(c.serviceInfo[i].name + " " + currentModel.Facility.IntelligentObjects["RapidAdmissionUnit"].Properties["ProcessingTime"].Value + " hours");
                }
                else if (c.serviceInfo[i].name == "Behavioral" && c.serviceInfo[i].included == true)
                {
                    currentModel.Facility.IntelligentObjects["Behavioral"].Properties["ProcessingTime"].Value = c.serviceInfo[i].averageRoomTime.ToString();
                    System.Diagnostics.Debug.WriteLine(c.serviceInfo[i].name + " " + currentModel.Facility.IntelligentObjects["Behavioral"].Properties["ProcessingTime"].Value + " hours");
                }
                else if (c.serviceInfo[i].name == "Observation" && c.serviceInfo[i].included == true)
                {
                    currentModel.Facility.IntelligentObjects["Observation"].Properties["ProcessingTime"].Value = c.serviceInfo[i].averageRoomTime.ToString();
                    System.Diagnostics.Debug.WriteLine(c.serviceInfo[i].name + " " + currentModel.Facility.IntelligentObjects["Observation"].Properties["ProcessingTime"].Value + " hours");
                }
            }
        }
        
        public void setAcuityPercentages(int acuity, double value)
        {
            currentModel.Tables[0].Rows[acuity-1].Properties["Probability"].Value = (value / 100).ToString();
            System.Diagnostics.Debug.WriteLine("Acuity " + acuity + " = " + (value) + "%");
            //currentModel.Tables[0].Rows[0].Properties["Probability"].Value = (c.acuityInfo[0].value / 100).ToString();
            //currentModel.Tables[0].Rows[1].Properties["Probability"].Value = (c.acuityInfo[1].value / 100).ToString();
            //currentModel.Tables[0].Rows[2].Properties["Probability"].Value = (c.acuityInfo[2].value / 100).ToString();
            //currentModel.Tables[0].Rows[3].Properties["Probability"].Value = (c.acuityInfo[3].value / 100).ToString();
            //currentModel.Tables[0].Rows[4].Properties["Probability"].Value = (c.acuityInfo[4].value / 100).ToString();
            //System.Diagnostics.Debug.WriteLine("Acuity 1 = " + (c.acuityInfo[0].value) + "%");
            //System.Diagnostics.Debug.WriteLine("Acuity 2 = " + (c.acuityInfo[1].value) + "%");
            //System.Diagnostics.Debug.WriteLine("Acuity 3 = " + (c.acuityInfo[2].value) + "%");
            //System.Diagnostics.Debug.WriteLine("Acuity 4 = " + (c.acuityInfo[3].value) + "%");
            //System.Diagnostics.Debug.WriteLine("Acuity 5 = " + (c.acuityInfo[4].value) + "%");
        }

        public void setNumberOfReplicationsforScenario(int scenario, int numberOfReps) {
            currentExperiment.Scenarios[scenario].ReplicationsRequired = numberOfReps;
            System.Diagnostics.Debug.WriteLine("Scen {0} reps  : {1,-10}", scenario,currentExperiment.Scenarios[scenario].ReplicationsRequired);
        }
        private void removeAllButOneScenario()
        {
            System.Diagnostics.Debug.WriteLine("Scenarios    : {0,-10}", currentExperiment.Scenarios.Count);
            //use only 1 scenario
            while (currentExperiment.Scenarios.Count > 1)
            {
                currentExperiment.Scenarios.Remove(currentExperiment.Scenarios[1]);
            }
            System.Diagnostics.Debug.WriteLine("Scenarios    : {0,-10}", currentExperiment.Scenarios.Count);
        }

        public void setArrivals(string type, int annualArrivals, double peakPercentageofYear)
        {
            System.Diagnostics.Debug.WriteLine("Ann. arrivals: {0,-10}", annualArrivals);

            //change RateTable.RateScaleFactor
            //1.0019 is the sum of the base rate table (the base rate table is all in percents, and it adds up to a little more than 100%)
            if (type == "average")
            {
                currentModel.Facility.IntelligentObjects[0].Properties[29].Value = (annualArrivals / (365 * 1.0019)).ToString();
                System.Diagnostics.Debug.WriteLine("RSF - average: {0,-20}", currentModel.Facility.IntelligentObjects[0].Properties[29].Value);
            }
            //change RateTable.RateScaleFactor for peak day
            else if (type== "peak")
            {
                double peakFactor;
                if (peakPercentageofYear >= 0 && peakPercentageofYear <= 1)
                    peakFactor = 12.0 * peakPercentageofYear / 100;
                else
                    peakFactor = 1.2; //default
                //rate scale factor
                currentModel.Facility.IntelligentObjects[0].Properties[29].Value = (peakFactor * annualArrivals / (365 * 1.0019)).ToString();
                System.Diagnostics.Debug.WriteLine("Peak % of yr : {0,-10}",peakPercentageofYear);
                System.Diagnostics.Debug.WriteLine("RSF - peak   : {0,-20}",currentModel.Facility.IntelligentObjects[0].Properties[29].Value);
            }
        }

        public void setNumberOfReplicationsforAllScenarios(int reps)
        {
            foreach (IScenario scenario in currentExperiment.Scenarios)
            {
                scenario.ReplicationsRequired = reps;
            }
        }

        public List<Response> StartExperiment(Configuration c)
        {
            if (currentExperiment.IsBusy)
                return null;
            currentExperiment.Reset();

            //save config as private
            _c = c;

            //Specify run times
            setWarmUpTimeInHours(c.startupTime.value);
            setRunLengthInDays(c.daysToRun.value);

            //set number of replications for the one scenario
            setNumberOfReplicationsforScenario(0, c.numberOfReps.value);
            removeAllButOneScenario();
            //if multiple scenarios
            //setNumberOfReplicationsforAllScenarios(c.numberOfReps.value);
            
            //change hospital values (type, annualArrivals, %ofyear)
            setArrivals(c.rateTable.value, Convert.ToInt32(c.arrivalInfo[0].value), c.arrivalInfo[1].value);

            setServiceTimes(c);

            setAcuityPercentages(1, c.acuityInfo[0].value);
            setAcuityPercentages(2, c.acuityInfo[1].value);
            setAcuityPercentages(3, c.acuityInfo[2].value);
            setAcuityPercentages(4, c.acuityInfo[3].value);
            setAcuityPercentages(5, c.acuityInfo[4].value);

            //change control values (the actual configuration)
            for (int i = 0; i < c.rooms.Length; i++)
            {
                if (c.rooms[i].included == true)
                {
                    string num = "";
                    currentExperiment.Scenarios[0].SetControlValue(currentExperiment.Controls[i], c.rooms[i].num.ToString());
                    currentExperiment.Scenarios[0].GetControlValue(currentExperiment.Controls[i], ref num);
                    System.Diagnostics.Debug.WriteLine("{0,-20}: {1,-2} rooms", c.rooms[i].name, num);
                }
                else if (c.rooms[i].included == false && (c.rooms[i].name == "Rapid Admission" || c.rooms[i].name == "Behavioral" || c.rooms[i].name == "Observation"))
                {
                    string num = "";
                    currentExperiment.Scenarios[0].SetControlValue(currentExperiment.Controls[i], "0");
                    currentExperiment.Scenarios[0].GetControlValue(currentExperiment.Controls[i], ref num);
                    System.Diagnostics.Debug.WriteLine("{0,-20}: {1,-2} rooms", c.rooms[i].name, num);
                }
            }

            //listeners (run completed, scenario, completed, etc)
            addSimioEventListeners();

            //run simulation
            runSimulationAsync();

            //insertResults(c, currentResponses);
            return currentResponses;
        }

        public void runSimulationAsync()
        {
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
        }

        public void addSimioEventListeners()
        {
            currentExperiment.ScenarioEnded += new EventHandler<ScenarioEndedEventArgs>(experiment_ScenarioEnded);
            currentExperiment.RunCompleted += new EventHandler<RunCompletedEventArgs>(experiment_RunCompleted);
            //currentExperiment.RunProgressChanged += new EventHandler<RunProgressChangedEventArgs>(experiment_RunProgressChanged);
            currentExperiment.ReplicationEnded += new EventHandler<ReplicationEndedEventArgs>(experiment_ReplicationEnded);
        }

        void experiment_RunCompleted(object sender, RunCompletedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Run Completed: " + e.TotalRunTime + "s");
        }

        void experiment_RunProgressChanged(object sender, RunProgressChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("Run Progress Changed: " + e.ProgressPercentage);
        }

        void experiment_ReplicationEnded(object sender, ReplicationEndedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("#" + e.ReplicationNumber + " Replication Ended: " + e.ActualRuntimeInSeconds + "s");
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
                    System.Diagnostics.Debug.WriteLine("{0} {1,-40} {2,-4}",e.Scenario.Name,response.Name,responseValue.ToString());

                    //only if we want to send back individual responses
                    Response r = new Response(response.Name, responseValue);
                    currentResponses.Add(r);
                }
            }

            calculateCosts(_c);

            //foreach (IScenarioResult result in e.Results)
            //{
            //    System.Diagnostics.Debug.WriteLine(result.DataItem + " " + result.DataSource + " " + result.ToString());

            //    //only if we want to send back individual responses
            //    //Response r = new Response(response.Name, responseValue);
            //    //currentResponses.Add(r);
            //}
            _completed = true;
            System.Diagnostics.Debug.WriteLine("Scenario Ended");
        }

        private void calculateCosts(Configuration c)
        {
            double interestRate = 0.05;
            double growthRate = 0.04;
            int yearsToCompletion = 5;
            int yearsAhead = 10;

            HoptServer.Models.CalculateCosts calc = new HoptServer.Models.CalculateCosts();
            double initial = calc.initialCost(c.costInfo, c.rooms);
            double annual = calc.annualCost(c.costInfo, c.rooms, c.acuityInfo, c.arrivalInfo);
            double total = calc.costAtConstructionStart(c.costInfo, c.rooms, c.acuityInfo, c.arrivalInfo, interestRate, growthRate, yearsToCompletion, yearsAhead);
            System.Diagnostics.Debug.WriteLine("Fixed Cost: " + initial);
            System.Diagnostics.Debug.WriteLine("Variable Cost: " + annual);
            System.Diagnostics.Debug.WriteLine("Total Cost in 10 yrs: " + total);

            SQLiteConnection conn = new SQLiteConnection("Data Source = configs.db");
            conn.Open();
            string sql = "Update Results set InitialCost = " + initial + ", AnnualCost= " + annual + ", Total= " + total;
            sql += " where ";
            for (int i = 0; i < c.rooms.Length; i++)
            {
                if(i < 5)
                    sql += c.rooms[i].name + " = " + c.rooms[i].num + " and ";
                else
                    sql += c.rooms[i].name + " = " + c.rooms[i].num;
            }
            SQLiteCommand command = new SQLiteCommand(sql, conn);
            command.ExecuteNonQuery();
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
