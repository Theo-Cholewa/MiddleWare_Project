using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Description;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ProxyCache
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Uri httpUrl = new Uri("http://localhost:8733/Design_Time_Addresses/ProxyCache/Service");
            Uri tcpUrl = new Uri("net.tcp://localhost:8733/Design_Time_Addresses/ProxyCache/Service");

            ServiceHost host = new ServiceHost(typeof(ProxyCache), httpUrl, tcpUrl);

            host.AddServiceEndpoint(typeof(IProxyCache), new WSHttpBinding(), "");

            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);

            host.Open();

            Console.WriteLine("ProxyCache is running");
            Console.ReadLine();
        }
    }
}
