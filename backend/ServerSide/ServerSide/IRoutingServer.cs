using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom d'interface "IService1" à la fois dans le code et le fichier de configuration.
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
