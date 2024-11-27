using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ServiceHost host = new ServiceHost(typeof(RoutingServer));
            var binding = new WebHttpBinding();
            var endpoint = host.AddServiceEndpoint(typeof(IRoutingServer), binding, "http://localhost:8733/Design_Time_Addresses/ServerSide/Service1/");
            endpoint.EndpointBehaviors.Add(new CorsBehavior());

            host.Open();
            Console.WriteLine("Service is running");
            Console.ReadLine();
        }
    }
}
