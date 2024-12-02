using Newtonsoft.Json.Linq;
using ServerSide.ClientProxy;
using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.Security.Policy;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using System.Xml.Linq;

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

        /**
         * @param start: adresse de départ
         * @param end: adresse d'arrivée
         */
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
            Console.WriteLine("start: " + start);
            Console.WriteLine("end: " +  end);

            List<Position> response = CallKeyPoints(start, end).Result;
            Console.WriteLine("response: \n" + response);
            
            string steps = GetSteps(response[0], response[2]).Result;
            steps += GetSteps(response[2], response[3]).Result;
            steps += GetSteps(response[3], response[1]).Result;
            Console.WriteLine("STEPS: " + steps);

            return steps;
        }


        // exemple qui fonctionne : Rue de la Sainte Famille 31200 Toulouse -> Rue Alphonse Daudet 31200 Toulouse

        public async Task<string> GetSteps(Position start, Position end)
        {
            try
            {
                Console.WriteLine("start: " + start + ", end: " + end);

                string apiKey = "AIzaSyC5flSpbKIIXEsMApGjy1OHjS3XIvmPd10";
                string url = "http://router.project-osrm.org/route/v1/driving/"
                    + start.lng.ToString(CultureInfo.InvariantCulture) + ","
                    + start.lat.ToString(CultureInfo.InvariantCulture) + ";"
                    + end.lng.ToString(CultureInfo.InvariantCulture) + ","
                    + end.lat.ToString(CultureInfo.InvariantCulture)
                    + "?overview=full&geometries=polyline&steps=true";

                Console.WriteLine("URL: "+url);

                using (HttpClient client = new HttpClient())
                {
                    HttpResponseMessage response = await client.GetAsync(url);

                    if (response.IsSuccessStatusCode)
                    {
                        string responseBody = await response.Content.ReadAsStringAsync();

                        // Analyser la réponse JSON
                        JObject jsonResponse = JObject.Parse(responseBody);

                        // Vérifier si des étapes sont disponibles
                        if (jsonResponse["routes"] != null && jsonResponse["routes"].HasValues)
                        {
                            var firstRoute = jsonResponse["routes"].FirstOrDefault();
                            var firstLeg = firstRoute?["legs"]?.FirstOrDefault();
                            var steps = firstLeg?["steps"];

                            if (steps != null && steps.HasValues)
                            {
                                string path = "";
                                foreach (var step in steps)
                                {
                                    // Extraire l'instruction basée sur le type de manœuvre et le nom de la rue
                                    string maneuverType = step["maneuver"]?["type"]?.ToString();
                                    string maneuverModifier = step["maneuver"]?["modifier"]?.ToString();
                                    string maneuverLongitude = step["maneuver"]?["location"][0].ToString();
                                    string maneuverLatitude = step["maneuver"]?["location"][1].ToString();
                                    string duration = step["duration"].ToString();
                                    string distance = step["distance"].ToString();
                                    string exit = " ";

                                    if (maneuverType == "roundabout")
                                    {
                                        exit = step["maneuver"]["exit"].ToString();
                                    }


                                    // Construire l'instruction
                                    string instruction = string.IsNullOrEmpty(maneuverType)
                                        ? "Étape inconnue"
                                        : $"({maneuverType} {exit} {maneuverModifier}), [({distance}m)-({duration}s)]";

                                    // Ajouter l'instruction à la chaîne path
                                    string instruction_unitaire = "";
                                    instruction_unitaire += maneuverModifier + "+";
                                    instruction_unitaire += maneuverType + "+";
                                    instruction_unitaire += exit + "+";
                                    instruction_unitaire += duration + "+";
                                    instruction_unitaire += distance + "+";
                                    instruction_unitaire += maneuverLongitude + "+";
                                    instruction_unitaire += maneuverLatitude + "|";
                                    path += instruction_unitaire;
                                }
                                return path;
                            }
                            else
                            {
                                return "Aucune étape trouvée.";
                            }
                        }
                        else
                        {
                            return "Aucune direction trouvée.";
                        }
                    }
                    else
                    {
                        return "Erreur lors de l'appel à l'API OSRM.";
                    }
                }
            }
            catch (Exception ex)
            {
                // Capture des erreurs potentielles
                Console.WriteLine("Erreur dans GetSteps: " + ex.Message);
                return "Une erreur s'est produite lors du traitement.";
            }
        }

        /**
         * @return [position depart, position arrivee, position stationProcheDepart, position stationProcheArrivee]
         */
        public async Task<List<Position>> CallKeyPoints(string start, string end)
        {
            Utils utils = new Utils();
            List<Position> positions1 = new List<Position>();

            // Ville - Contract - Adresse - Station Proche - Return
            string cityA = utils.GetCity(start);
            Console.WriteLine("cityA: " + cityA);
            string cityB = utils.GetCity(end);
            Console.WriteLine("cityB: " + cityB);


            List<Place> placesA = RetreveContract(cityA);
            Position startPosition = AddressToPosition(start).Result;
            Console.WriteLine("startPosition: " + startPosition);


            List<Place> placesB = RetreveContract(cityB);
            Position endPosition =  AddressToPosition(end).Result;
            Console.WriteLine("endPosition: " + endPosition);


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

            return positions1;
            /*
            string response = "";
            foreach (var position in positions1) { response += position.ToString() + "\n"; }
            return response;*/

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
            //Console.WriteLine("result : " + response);
            List<Place> places = JsonSerializer.Deserialize<List<Place>>(response);   // remettre le call au Proxy plus tard
            Console.WriteLine(places);
            return places;
        }
        
        public async Task<string> CallTest(string city)
        {
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
            try
            {
                Console.WriteLine("Start address to position call: " + address);

                // Remplacement de l'espace par "+" pour un encodage correct
                HttpResponseMessage response = await client.GetAsync("https://api-adresse.data.gouv.fr/search/?q=" + address.Replace(" ", "+"));

                // Vérifier que la réponse est réussie
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                var position = new Position();
                var jsonResponse = JsonDocument.Parse(responseBody);

                // Assurez-vous qu'il y a des "features" dans la réponse
                if (jsonResponse.RootElement.GetProperty("features").EnumerateArray().Any())
                {
                    foreach (var feature in jsonResponse.RootElement.GetProperty("features").EnumerateArray())
                    {
                        var coordinates = feature.GetProperty("geometry").GetProperty("coordinates").EnumerateArray();
                        position = new Position { lng = coordinates.First().GetDouble(), lat = coordinates.Last().GetDouble() };
                    }
                }
                else
                {
                    Console.WriteLine("Aucune feature trouvée pour l'adresse : " + address);
                }

                Console.WriteLine("End address to position call: " + address);
                return position;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erreur dans AddressToPosition pour l'adresse '" + address + "': " + ex.Message);
                return null;
            }
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
