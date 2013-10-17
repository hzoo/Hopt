using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using SimioAPI;

namespace WCFSimioNamespace
{
    [ServiceContract]
    public interface ISimioService
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        SimioConnectionParametersClass RunSimioNow(SimioConnectionParametersClass composite);

        [OperationContract]
        string GetMessage();

        // TODO: Add your service operations here
        [OperationContract]
        void SetProject(string project, string model);
        
        [OperationContract]
        ISimioProject GetCurrentProject();

        [OperationContract]
        void SetModel(string model);

        [OperationContract]
        IModel GetCurrentModel();

        [OperationContract]
        void setCurrentModelProperty(string serverName, string propertyName, string value);

        [OperationContract]
        string getCurrentModelProperty(string serverName, string propertyName);
    }
}
