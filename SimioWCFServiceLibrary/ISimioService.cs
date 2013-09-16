using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

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
    }
}
