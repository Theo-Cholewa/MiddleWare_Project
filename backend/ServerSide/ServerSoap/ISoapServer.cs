using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServerSoap
{
    [ServiceContract()]
    public interface ISoapServer
    {
        [OperationContract()]
        int Add(int num1, int num2);
    }
}
