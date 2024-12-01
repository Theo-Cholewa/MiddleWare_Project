using ServerSide.ClientProxy;
using ServerSide.ProxyCache;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServerSide
{
    public class RoutingServer : IRoutingServer
    {
        HttpClient client = new HttpClient();
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
            /*var headers = WebOperationContext.Current.OutgoingResponse.Headers;
            headers.Remove("Access-Control-Allow-Origin"); // Remove any existing values
            headers.Add("Access-Control-Allow-Origin", "*");
            SoapServerClient soapServerClient = new SoapServerClient();
            string a = "" + soapServerClient.Add(1, 2);
            string b = CallContract(start);
            Console.WriteLine("b: " + b);
            return a + b ;*/

            /*
            string start = "2 Rue Maurice Caunes, 31200 Toulouse";
            string end = "Centre Commercial, Mirail FR, MIRAIL, 31100 Toulouse";

            ClientSide.Program program = new ClientSide.Program();

            Task<List<Position>> positionTask = program.GetKeyPoints(start, end);
            List<Position> positions = await positionTask;
            foreach (var position in positions) { Console.WriteLine(position); }*/
            Console.WriteLine("GetPath");
            string response = CallKeyPoints(start, end).Result;
            Console.WriteLine("response: " + response);
            return response;
        }

        public async Task<string> CallKeyPoints(string start, string end)
        {
            Utils utils = new Utils();
            List<Position> positions1 = new List<Position>();

            // Ville - Contract - Adresse - Station Proche - Return
            string cityA = utils.GetCity(start);
            string cityB = utils.GetCity(end);
            Console.WriteLine("cityA: " + cityA);

            List<Place> placesA = RetreveContract(cityA);
            Position startPosition = AddressToPosition(start).Result;
            Console.WriteLine("startPosition: " + startPosition);


            List<Place> placesB = RetreveContract(cityB);
            Position endPosition =  AddressToPosition(end).Result;


            positions1.Add(startPosition);
            positions1.Add(endPosition);

            if (placesA != null && placesA.Count > 0 && placesB != null && placesB.Count >0)
            {
                Place firstStation = utils.GetNearestPlace(placesA, startPosition);
                Console.WriteLine("firstStation: " + firstStation);
                positions1.Add(firstStation.position);

                Place lastStation = utils.GetNearestPlace(placesB, endPosition);
                positions1.Add(lastStation.position);
            }

            string response = "";
            foreach (var position in positions1) { response += position.ToString() + "\n"; }
            return response;

            /*
            Console.WriteLine("CallKeyPoints");
            var headers = WebOperationContext.Current.OutgoingResponse.Headers;
            headers.Remove("Access-Control-Allow-Origin");
            headers.Add("Access-Control-Allow-Origin", "*");
            Console.WriteLine("start call");
            Task<List<Position>> positionTask = GetKeyPoints(start, end);
            Console.WriteLine("end call");
            List<Position> positions = await positionTask;
            Console.WriteLine("positions: " + positions);
            string response = "";
            foreach (var position in positions) { response += position.ToString(); }
            return response;*/
        }

        public List<Place> RetreveContract(string city)
        {
            Console.WriteLine("CallContract : " + city);
            /*var headers = WebOperationContext.Current.OutgoingResponse.Headers;
            headers.Remove("Access-Control-Allow-Origin");
            headers.Add("Access-Control-Allow-Origin", "*");*/


            /*SoapServerClient soapServerClient = new SoapServerClient();
            Console.WriteLine("start call");
            string response = soapServerClient.GetContracts(city);
            Console.WriteLine("end call");
            List<Place> places = JsonSerializer.Deserialize<List<Place>>(response);
            Console.WriteLine("places: " + places);
            return places;*/

            String response = CallTest(city).Result;
            Console.WriteLine("result : " + response);
            List<Place> places = JsonSerializer.Deserialize<List<Place>>(response);   // remettre le call au Proxy plus tard
            Console.WriteLine(places);
            return places;
        }
        
        public async Task<string> CallTest(string city)
        {
            ProxyCacheClient proxyCacheClient = new ProxyCacheClient();
            Console.WriteLine("test proxy : " + proxyCacheClient.GetContracts(city));

            // c'est le proxy qui le fait normalement
            var headers = WebOperationContext.Current.OutgoingResponse.Headers;
            headers.Remove("Access-Control-Allow-Origin"); // Remove any existing values
            headers.Add("Access-Control-Allow-Origin", "*");
            
            HttpResponseMessage response = await client.GetAsync("https://api.jcdecaux.com/vls/v1/stations?contract=" + city + "&apiKey=da2a1717115e9e7a90149fd7d0c4afcff086e014");


            //Console.WriteLine("responseBody: " + responseBody);
            return await response.Content.ReadAsStringAsync();//.Result;
        }

        

        /*
        public List<Place> RetreveContract(string city)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://api.jcdecaux.com/vls/v1/stations?contract=" + city + "&apiKey=da2a1717115e9e7a90149fd7d0c4afcff086e014");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            List<Place> places = JsonSerializer.Deserialize<List<Place>>(responseBody);
            return places;
            string response = CallContract(city);

            // faire ici la vérification de la réponse -> si on ne trouve pas de contract
            
        }*/

        public async Task<Position> AddressToPosition(string address)
        {
            var headers = WebOperationContext.Current.OutgoingResponse.Headers;
            headers.Remove("Access-Control-Allow-Origin"); // Remove any existing values
            headers.Add("Access-Control-Allow-Origin", "*");
            Console.WriteLine("Start address to position call");
            HttpResponseMessage response = await client.GetAsync("https://api-adresse.data.gouv.fr/search/?q=" + address.Replace(" ", "+"));
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            var position = new Position();
            var jsonResponse = JsonDocument.Parse(responseBody);
            foreach (var feature in jsonResponse.RootElement.GetProperty("features").EnumerateArray())
            {
                var coordinates = feature.GetProperty("geometry").GetProperty("coordinates").EnumerateArray();
                position = new Position { lng = coordinates.First().GetDouble(), lat = coordinates.Last().GetDouble() };
            }
            Console.WriteLine("End address to position call");
            return position;
        }

        public async Task<List<Position>> GetKeyPoints(string start, string end)
        {
            Utils utils = new Utils();
            Console.WriteLine("0");
            List<Position> points = new List<Position>();

            string cityA = utils.GetCity(start);
            string cityB = utils.GetCity(end);
            Console.WriteLine("1");
            List<Place> placesA = RetreveContract(cityA);
            List<Place> placesB = RetreveContract(cityB);
            Console.WriteLine("2");
            Position startPosition = await AddressToPosition(start);
            Position endPosition = await AddressToPosition(end);
            Console.WriteLine("3");
            points.Add(startPosition);
            points.Add(endPosition);

            if (placesA.Count != 0 && placesB.Count != 0)
            {
                
                Place startStation = utils.GetNearestPlace(placesA, startPosition);
                Place endStation = utils.GetNearestPlace(placesB, endPosition);
                points.Add(startStation.position);
                points.Add(endStation.position);
            }
            Console.WriteLine("4");
            return points;
        }

    }
}
