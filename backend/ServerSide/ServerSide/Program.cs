using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using System.Threading.Tasks;

namespace ServerSide
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Uri baseAddress = new Uri("http://localhost:8734/Design_Time_Addresses/ServerSide/Service1/");
            ServiceHost host = new ServiceHost(typeof(RoutingServer), baseAddress);

            host.AddServiceEndpoint(typeof(IRoutingServer), new WebHttpBinding(), "").Behaviors.Add(new WebHttpBehavior());

            host.Open();
            Console.WriteLine("RoutingServer is host at " + baseAddress);
            Console.WriteLine("Host is running... Press <Enter> key to stop");
            Console.ReadLine();
            host.Close();
        }
    }
}
