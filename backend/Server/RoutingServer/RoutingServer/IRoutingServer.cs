using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ServerRouting
{
    [ServiceContract]
    public interface IRoutingServer
    {
        [OperationContract]
        [WebInvoke(Method = "OPTIONS", UriTemplate = "*")]
        void HandleHttpOptions();

        [OperationContract]
        [WebInvoke(Method = "GET", UriTemplate = "path?start={start}&end={end}", ResponseFormat = WebMessageFormat.Json, BodyStyle = WebMessageBodyStyle.Wrapped)]
        string GetPath(string start, string end);
    }
    
}
