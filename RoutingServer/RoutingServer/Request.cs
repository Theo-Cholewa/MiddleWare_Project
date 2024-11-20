using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServerSoap
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
    public class ServerRequest: IServerRequest
    {
        public int Add(int x, int y)
        {
            return x + y;
        }

        public double Divide(int x, int y)
        {
            if (y != 0) return x / y;
            return 0;
        }

        public int Multiply(int x, int y)
        {
            return x * y;
        }

        public int Subtract(int x, int y)
        {
            return x - y;
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }
    }
}
