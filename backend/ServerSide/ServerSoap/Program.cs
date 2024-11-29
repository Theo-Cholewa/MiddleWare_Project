using System.ServiceModel.Description;
using System.ServiceModel;
using System;

namespace Proxy
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Uri httpUrl = new Uri("http://localhost:8733/Design_Time_Addresses/ServerSoap/Service1");
            Uri tcpUrl = new Uri("net.tcp://localhost:8733/Design_Time_Addresses/ServerSoap/Service1");

            ServiceHost host = new ServiceHost(typeof(SoapServer), httpUrl, tcpUrl);

            host.AddServiceEndpoint(typeof(ISoapServer), new WSHttpBinding(), "");

            ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            smb.HttpGetEnabled = true;
            host.Description.Behaviors.Add(smb);

            host.Open();

            Console.WriteLine("Proxy is running");
            Console.ReadLine();
        }
    }
}
