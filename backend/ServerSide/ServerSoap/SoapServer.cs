using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace ServerSoap
{
    // REMARQUE : vous pouvez utiliser la commande Renommer du menu Refactoriser pour changer le nom de classe "Service1" à la fois dans le code et le fichier de configuration.
    class SoapServer : ISoapServer
    {

        // faire les méthodes 
        public int Add(int num1, int num2)
        {
            return num1 + num2;
        }

    }
}
