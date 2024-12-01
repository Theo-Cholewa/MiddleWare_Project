using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ServerRouting
{
    public class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:8734/RoutingServer/");
            ServiceHost host = new ServiceHost(typeof(RoutingServer), baseAddress);

            var endpoint = host.AddServiceEndpoint(typeof(IRoutingServer), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());

            //ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            //smb.HttpGetEnabled = true;
            //host.Description.Behaviors.Add(smb);

            host.Open();
            Console.WriteLine("RoutingServer is hosted at " + baseAddress);
            Console.WriteLine("Host is running... Press <Enter> key to stop");
            Console.ReadLine();
            host.Close();
        }
    }
}
