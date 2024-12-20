﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.ServiceModel.Web;
using System.Text.Json;
using RoutingServer;
using RoutingServer.ProxyCacheReference;
using Newtonsoft.Json.Linq;

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
        // exemple qui fonctionne : Rue de la Sainte Famille 31200 Toulouse -> Rue Alphonse Daudet 31200 Toulouse
        public string GetPath(string start, string end)
        {
            List<Position> response = GetKeyPoints(start, end);
           
            if (response.Count == 0)
            {
                return "Aucun chemin trouvé.";
            }
            string steps = "";
            for(int i = 0; i < response.Count; i+=2)
            {
                steps += GetSteps(response[i], response[i + 1]);
            }
            return steps;
        }

        public List<Position> GetKeyPoints(string start, string end)
        {
            Utils utils = new Utils();
            List<Position> positions = new List<Position>();

            // Ville - Contract - Adresse - Station Proche - Return
            string cityA = utils.GetCity(start);
            string cityB = utils.GetCity(end);

            List<Place> stationsCityA;
            //List<Place> stationsCityB;

            Position startPosition = CallAddressToPosition(start);
            Position endPosition = CallAddressToPosition(end);

            // 1er cas : même ville
            if (cityA==cityB)
            {
                stationsCityA = CallContract(cityA);
                // si on a pas de contrat pour la ville on fait un trajet direct
                if(stationsCityA == null || stationsCityA.Count == 0)
                {
                    positions.Add(startPosition);
                    positions.Add(endPosition);
                    return positions;
                }
                // sinon on fait un trajet avec 2 stations intermédiaires 
                else
                {
                    Place firstStation = utils.GetNearestPlace(stationsCityA, startPosition,endPosition);
                    Place lastStation = utils.GetNearestPlace(stationsCityA, endPosition,startPosition);

                    positions.Add(startPosition);
                    // si on a deux stations différentes 
                    if(firstStation != null && lastStation != null && firstStation != lastStation)
                    {
                        positions.Add(firstStation.position);
                        positions.Add(lastStation.position);
                    }
                    positions.Add(endPosition);
                    return positions;
                }
            }
            else
            { // 2ème cas : villes différentes
              // on doit pour chaque ville entre A et B, voir s'il y a un contrat et si c'est plus rapide de passer par une station
              //FindAllCities(start, end);

                // ne marche pas encore car on n'arrive pas à trouver les villes entre les deux points
                // donc on affiche uniquement le trajet direct
                Console.WriteLine("Villes différentes");
                stationsCityA = CallContract(cityA);
                List<Place> stationsCityB = CallContract(cityB);
                
                if(stationsCityA != null && stationsCityA.Count != 0 && stationsCityB != null && stationsCityB.Count != 0)
                {
                    Place firstStation = utils.GetNearestPlace(stationsCityA, startPosition, null);
                    Place lastStation = utils.GetNearestPlace(stationsCityB, endPosition, null);

                    positions.Add(startPosition);
                    if (firstStation != null && lastStation != null && firstStation != lastStation)
                    {
                        positions.Add(firstStation.position);
                        positions.Add(lastStation.position);
                    }
                    positions.Add(endPosition);
                    return positions;
                }
                else
                {
                    positions.Add(startPosition);
                    positions.Add(endPosition);
                }
            }
            return positions;
        }

        public string GetSteps(Position start, Position end)
        {
            try
            {
                string response = CallSteps(start, end);

                // Analyser la réponse JSON
                JObject jsonResponse = JObject.Parse(response);

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
            catch (Exception ex)
            {
                // Capture des erreurs potentielles
                Console.WriteLine("Erreur dans GetSteps: " + ex.Message);
                return "Une erreur s'est produite lors du traitement.";
            }
        }

        // --------------------------------------------------- Fonctions de Call au Proxy ---------------------------------------------------
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
            catch (Exception ex)
            {
                Console.WriteLine("Erreur lors de la création du client ProxyCache: " + ex.Message);
                if (ex.InnerException != null)
                {
                    Console.WriteLine("Détails de l'exception interne: " + ex.InnerException.Message);
                }
            }
            return res;
        }
        public List<Place> CallContract(string city)
        {
            string url = "https://api.jcdecaux.com/vls/v1/stations?contract=" + city + "&apiKey=da2a1717115e9e7a90149fd7d0c4afcff086e014";
            string response = CallProxy(url);

            if(response == "{ \"error\" : \"Specified contract does not exist\" }")
            {
                return null; // pas de contrat trouvé
            }
            List<Place> places = JsonSerializer.Deserialize<List<Place>>(response);
            //Console.WriteLine("places: " + places);
            return places;
        }
        public Position CallAddressToPosition(string address)
        {
            
            string url = "https://api-adresse.data.gouv.fr/search/?q=" + address.Replace(" ", "+");
            string response = CallProxy(url);
            var position = new Position();
            var jsonResponse = JsonDocument.Parse(response);
            
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
            return position;
        }
        public string CallSteps(Position start, Position end)
        {
            string url = "http://router.project-osrm.org/route/v1/driving/"
                    + start.lng.ToString(CultureInfo.InvariantCulture) + ","
                    + start.lat.ToString(CultureInfo.InvariantCulture) + ";"
                    + end.lng.ToString(CultureInfo.InvariantCulture) + ","
                    + end.lat.ToString(CultureInfo.InvariantCulture)
                    + "?overview=full&geometries=polyline&steps=true";

            string response = CallProxy(url);
            return response;
        }

        public string CallGPStoAddress(Position position)
        {
            string url = "https://api-adresse.data.gouv.fr/reverse/?lon=" + position.lng + "&lat=" + position.lat;
            string response = CallProxy(url);
            return response;
        }

        // --------------------------------------------------- -------------------------- ---------------------------------------------------

        // tentative de trouver toutes les villes entre deux points -> pour les trajets entre villes différentes
        // on regarde le contrat de chaque ville pour voir si on peut passer par une station

        // ne fonctionne pas encore 
        // la liste des villes entre les deux points ne semblent pas fonctionner
        public List<string> FindAllCities(string start, string end)
        {
            Position startPosition = CallAddressToPosition(start);
            Position endPosition = CallAddressToPosition(end);
            string steps = CallSteps(startPosition, endPosition);

            JObject jsonObject = JObject.Parse(steps); 
            // Liste pour stocker les positions
            List<Position> positionList = new List<Position>(); 
            // Parcourir chaque étape pour extraire les coordonnées et les ajouter à la liste de positions
            foreach (var route in jsonObject["routes"]) { 
                foreach (var leg in route["legs"]) { 
                    foreach (var step in leg["steps"]) { 
                        var location = step["maneuver"]["location"]; 
                        Position pos = new Position { 
                            lng = (double)location[0], // Longitude
                            lat = (double)location[1] // Latitude
                        }; 
                        positionList.Add(pos); 
                    } 
                } 
            } // Afficher les positions extraites
            Console.WriteLine("Positions: ");
            for(int i = 0; i < positionList.Count; i++)
            {
                Console.WriteLine(positionList[i].ToString());
            }

            List<string> cities = new List<string>();
            foreach (var pos in positionList) {
                jsonObject = JObject.Parse(CallGPStoAddress(pos));
                Console.WriteLine(jsonObject);
                if (jsonObject["features"].HasValues) {
                    string city = (string)jsonObject["features"]["properties"]["city"];
                    if (!cities.Contains(city))
                    {
                        cities.Add(city);
                        Console.WriteLine(city);
                    }
                }
            }
            Console.WriteLine("Cities: ");
            foreach (var city in cities)
            {
                Console.WriteLine(city);
            }
            return null;
        }
    }
}
