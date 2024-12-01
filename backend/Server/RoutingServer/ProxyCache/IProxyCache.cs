using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ProxyCache
{
    [ServiceContract]
    public interface IProxyCache
    {
        [OperationContract]
        string GetData(int value);

        [OperationContract]
        string CallApi(string url);
    }
}
