using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.Diagnostics;
using SimioAPI;
using System.Web;

namespace WCFSimioNamespace
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Single)]
    public class SimioService : ISimioService
    {
        private string _testResutls = "";
        private static double _expReturnValue = 0.0;
        int _currentRun = 1;
        int _numberOfRuns = 1;
        bool _completed = false;

        public SimioConnectionParametersClass RunSimioNow(SimioConnectionParametersClass SimioClass)
        {

            ISimioProject project;

            // Set extension folder path
            SimioProjectFactory.SetExtensionsPath(Directory.GetCurrentDirectory().ToString());

            // Open project
            string[] warnings;
            string strfilepathname = "ED-v2.spfx";

            project = SimioProjectFactory.LoadProject(strfilepathname, out warnings);

            IModel model = project.Models["Model"];


            IExperiment experiment = model.Experiments["Experiment1"];


            // Add event handler for ScenarioEnded
            experiment.ScenarioEnded += new EventHandler<ScenarioEndedEventArgs>(experiment_ScenarioEnded);
            experiment.RunCompleted += new EventHandler<RunCompletedEventArgs>(experiment_RunCompleted);
            experiment.RunProgressChanged += new EventHandler<RunProgressChangedEventArgs>(experiment_RunProgressChanged);
            experiment.ReplicationEnded += new EventHandler<ReplicationEndedEventArgs>(experiment_ReplicationEnded);

            _testResutls += "Start!!!" + "\r\n<br>";

            _testResutls += "Start run: " + DateTime.Now.ToString() + " \r\n <br>";

            // Reset Experiment
            experiment.Reset();
            _currentRun = 1;
            _numberOfRuns = 2;

            try
            {

                // Run Experiment
                experiment.RunAsync();
                int i = 0;
                do
                {

                    i++;
                }
                while (!(_completed));

                SimioClass.Results = "Success || " + _testResutls + "<br>";

            }
            catch (Exception ex)
            {
                SimioClass.Results = "Error || " + ex.Message+ "<br>";
            }

            return SimioClass;
        }

        void experiment_ReplicationEnded(object sender, ReplicationEndedEventArgs e)
        {
            if (e.ReplicationEndedState == ExperimentationStatus.Failed)
            {
                _testResutls += e.ErrorMessage;
            }
        }

        void experiment_RunProgressChanged(object sender, RunProgressChangedEventArgs e)
        {
            IExperiment experiment = (IExperiment)sender;
        }

        void experiment_RunCompleted(object sender, RunCompletedEventArgs e)
        {
            _testResutls += "End run: " + DateTime.Now.ToString() + " return value: " + _expReturnValue.ToString() + "\r\n" + "<br>";
            IExperiment experiment = (IExperiment)sender;
            _completed = true;


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
                    _expReturnValue += responseValue;
                    _testResutls += "End scenario(" + e.Scenario.Name + ") Response(" + response.Name + " = " + responseValue.ToString() + ")\r\n" + "<br>";
                }
            }
        }

        void setModelPropertyValue(IModel model, string serverName, string propertyName, string value)
        {
            IProperty prop = model.Facility.IntelligentObjects[serverName].Properties[propertyName];
            prop.Value = value;
            
            //how to change unit?
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public string GetMessage()
        {
            return "Hello From WCF Service ";
        }
    }
}
