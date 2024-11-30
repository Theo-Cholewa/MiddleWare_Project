using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using System.Device.Location;
using System.Xml.Linq;

using System.Text.Json;
using System.Collections.Generic;
using System.Data.SqlClient;
using static System.Net.WebRequestMethods;
using System.Linq;

namespace ClientSide
{
    internal class Program
    {
        

        public async Task<List<Place>> RetreveContract(string city)
        {
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync("https://api.jcdecaux.com/vls/v1/stations?contract=" + city + "&apiKey=da2a1717115e9e7a90149fd7d0c4afcff086e014");
            response.EnsureSuccessStatusCode();
            string responseBody = await response.Content.ReadAsStringAsync();
            List<Place> places = JsonSerializer.Deserialize<List<Place>>(responseBody);
            return places;
        }

        public Place GetNearestPlace(List<Place> places, Position coord)
        {
            GeoCoordinate geoCoordinate = new GeoCoordinate(coord.lat, coord.lng);
            Place best = null;
            foreach(var place in places)
            {
                if(best == null)
                {
                    best = place;
                }
                GeoCoordinate geoCoordinateOther = new GeoCoordinate(place.position.lat, place.position.lng);
                GeoCoordinate geoCoordinateBest = new GeoCoordinate(best.position.lat, best.position.lng);
                double distance = geoCoordinate.GetDistanceTo(geoCoordinateOther);
                double distanceBest = geoCoordinate.GetDistanceTo(geoCoordinateBest);
                if(distance < distanceBest)
                {
                    best = place;
                }
            }
            return best;
        }

        public string GetCity(string address)
        {
            string res = "";
            string[] strings = address.Split(' ');
            for (int i = 0; i < strings.Length; i++)
            {
                if (strings[i].Length == 5 && int.TryParse(strings[i], out _))
                {
                    for (int j = i + 1; j < strings.Length; j++)
                    {
                        res += strings[j];
                        if (j != strings.Length - 1)
                        {
                            res += " ";
                        }
                    }
                    break;
                }
            }
            return res;
        }

        public async Task<Position> AddressToPosition(string address)
        {
            HttpClient client = new HttpClient(); 
            HttpResponseMessage response = await client.GetAsync("https://api-adresse.data.gouv.fr/search/?q=" + address.Replace(" ", "+")); 
            response.EnsureSuccessStatusCode(); 
            string responseBody = await response.Content.ReadAsStringAsync(); 
            var position = new Position();
            var jsonResponse = JsonDocument.Parse(responseBody); 
            foreach (var feature in jsonResponse.RootElement.GetProperty("features").EnumerateArray()) { 
                var coordinates = feature.GetProperty("geometry").GetProperty("coordinates").EnumerateArray(); 
                position = new Position { lng = coordinates.First().GetDouble(), lat = coordinates.Last().GetDouble() };
            }
            return position;
        }
        public async Task<List<Position>> GetKeyPoints(string start, string end)
        {
            List<Position> points = new List<Position>();

            string cityA = GetCity(start);
            string cityB = GetCity(end);

            List<Place> placesA = RetreveContract(cityA).Result;
            List<Place> placesB = RetreveContract(cityB).Result;

            Position startPosition = await AddressToPosition(start);
            Position endPosition = await AddressToPosition(end);

            points.Add(startPosition);
            points.Add(endPosition);

            if (placesA.Count != 0 && placesB.Count != 0)
            {
                Place startStation = GetNearestPlace(placesA, startPosition);
                Place endStation = GetNearestPlace(placesB, endPosition);
                points.Add(startStation.position);
                points.Add(endStation.position);
            }
            return points;
        }

        static async Task Main()
        {
            /*
            string start = "2 Rue Maurice Caunes, 31200 Toulouse";
            string end = "Centre Commercial, Mirail FR, MIRAIL, 31100 Toulouse";

            ClientSide.Program program = new ClientSide.Program();

            Task<List<Position>> positionTask = program.GetKeyPoints(start, end); 
            List<Position> positions = await positionTask; 
            foreach (var position in positions) { Console.WriteLine(position); }*/
            Console.ReadLine();
        }
    }

    public class Position
    {
        public double lat { get; set; }
        public double lng { get; set; }

        public override string ToString() { return $"Latitude: {lat}, Longitude: {lng}"; }
    }
    public class Place
    {
        public String name { get; set; }
        public String address { get; set; }
        public Position position { get; set; }

        public override string ToString() { return $"Name: {name}, Address: {address}, Position: {position}"; }
    }
}
