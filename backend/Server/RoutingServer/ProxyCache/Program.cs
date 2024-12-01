using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace ProxyCache
{
    public class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:8733/ProxyCache/");
            ServiceHost host = new ServiceHost(typeof(ProxyCache), baseAddress);

            var endpoint = host.AddServiceEndpoint(typeof(IProxyCache), new WebHttpBinding(), "");
            endpoint.Behaviors.Add(new WebHttpBehavior());

            //ServiceMetadataBehavior smb = new ServiceMetadataBehavior();
            //smb.HttpGetEnabled = true;
            //host.Description.Behaviors.Add(smb);

            host.Open();
            Console.WriteLine("ProxyCache is hosted at " + baseAddress);
            Console.WriteLine("Press <Enter> to stop the service.");
            Console.ReadLine();
            host.Close();
        }
    }
}
