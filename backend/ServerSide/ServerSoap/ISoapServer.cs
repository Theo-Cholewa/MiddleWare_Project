using System.ServiceModel;

namespace Proxy
{
    [ServiceContract]
    public interface ISoapServer
    {
        [OperationContract]
        int Add(int num1, int num2);

        [OperationContract]
        string GetContracts(string city);
    }
}
