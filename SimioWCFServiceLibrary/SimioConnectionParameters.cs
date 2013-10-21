using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace WCFSimioNamespace
{
    [DataContract]
    public class SimioConnectionParametersClass
    {
        string _results = "";
        string _start = "";
        string _end = "";

        //string _modelpathandname = ""; //"SimioWCFServiceLibrary3\\SimioWCFServiceLibrary\\ED-v1.spfx";

        [DataMember]
        public string Results
        {
            get { return _results; }
            set { _results = value; }
        }
        [DataMember]
        public string Start
        {
            get { return _start; }
            set { _start = value; }
        }
        [DataMember]
        public string The_End
        {
            get { return _end; }
            set { _end = value; }
        }
        //[DataMember]
        //public string ModelPathAndName
        //{
        //    get { return _modelpathandname; }
        //    set { _modelpathandname = value; }
        //}
    }
}
