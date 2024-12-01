using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Web;
using System.Text.Json;
using RoutingServer;
using RoutingServer.ProxyCacheReference;

namespace ServerRouting
{
    public class RoutingServer : IRoutingServer
    {
        public void HandleHttpOptions()
        {
            var headers = WebOperationContext.Current.OutgoingResponse.Headers;
            headers.Remove("Access-Control-Allow-Origin"); // Remove any existing values
            headers.Add("Access-Control-Allow-Origin", "*");
            headers.Add("Access-Control-Allow-Methods", "GET, OPTIONS");
            headers.Add("Access-Control-Allow-Headers", "Content-Type, Accept, Authorization");
        }

        public string GetPath(string start, string end)
        {
            string res = CallKeyPoints(start, end);
            return res;
        }

        public string CallProxy(string url)
        {
            string res = "";
            try
            { // Créez une instance du client pour le service
                var headers = WebOperationContext.Current.OutgoingResponse.Headers;
                headers.Remove("Access-Control-Allow-Origin"); // Remove any existing values
                headers.Add("Access-Control-Allow-Origin", "*");
                ProxyCacheClient proxy = new ProxyCacheClient(); 
                res = proxy.CallApi(url);
            } 
            catch (Exception ex) { 
                Console.WriteLine("Erreur lors de la création du client ProxyCache: " + ex.Message); 
                if (ex.InnerException != null) { 
                    Console.WriteLine("Détails de l'exception interne: " + ex.InnerException.Message); 
                } 
            }             
            return res;
        }

        public string CallKeyPoints(string start, string end)
        {
            Utils utils = new Utils();
            List<Position> positions1 = new List<Position>();

            // Ville - Contract - Adresse - Station Proche - Return
            string cityA = utils.GetCity(start);
            string cityB = utils.GetCity(end);

            List<Place> placesA = RetreveContract(cityA);
            Position startPosition = AddressToPosition(start);

            List<Place> placesB = RetreveContract(cityB);
            Position endPosition = AddressToPosition(end);


            positions1.Add(startPosition);
            positions1.Add(endPosition);

            if (placesA != null && placesA.Count > 0 && placesB != null && placesB.Count > 0)
            {
                Place firstStation = utils.GetNearestPlace(placesA, startPosition);
                positions1.Add(firstStation.position);

                Place lastStation = utils.GetNearestPlace(placesB, endPosition);
                positions1.Add(lastStation.position);
            }

            string response = "";
            foreach (var position in positions1) { response += position.ToString() + "\n"; }
            return response;
        }

        public List<Place> RetreveContract(string city)
        {
            string url = "https://api.jcdecaux.com/vls/v1/stations?contract=" + city + "&apiKey=da2a1717115e9e7a90149fd7d0c4afcff086e014";
            string response = CallProxy(url);
            List<Place> places = JsonSerializer.Deserialize<List<Place>>(response);
            return places;
        }

        public Position AddressToPosition(string address)
        {
            string url = "https://api-adresse.data.gouv.fr/search/?q=" + address;
            string response = CallProxy(url);
            var position = new Position();
            var jsonResponse = JsonDocument.Parse(response);
            foreach (var feature in jsonResponse.RootElement.GetProperty("features").EnumerateArray())
            {
                var coordinates = feature.GetProperty("geometry").GetProperty("coordinates").EnumerateArray();
                position = new Position { lng = coordinates.First().GetDouble(), lat = coordinates.Last().GetDouble() };
            }
            return position;
        }
    }
}
