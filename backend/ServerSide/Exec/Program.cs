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
            var options = new JsonSerializerOptions { 
                PropertyNameCaseInsensitive = true, }; 
            List<Place> places = JsonSerializer.Deserialize<List<Place>>(responseBody, options);
            Console.WriteLine(places.Count);
            Console.WriteLine(places.Last().ToString());
            return places;
        }

        public Place GetNearestPlace(List<Place> places, Position coord)
        {
            GeoCoordinate geoCoordinate = new GeoCoordinate(coord.latitude, coord.longitude);
            Place best = null;
            foreach(var place in places)
            {
                if(best == null)
                {
                    best = place;
                }
                GeoCoordinate geoCoordinateOther = new GeoCoordinate(place.position.latitude, place.position.longitude);
                GeoCoordinate geoCoordinateBest = new GeoCoordinate(best.position.latitude, best.position.longitude);
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

        public async Task<List<Position>> GetKeyPoints(string start, string end)
        {
            List<Position> points = new List<Position>();

            string cityA = GetCity(start);
            string cityB = GetCity(end);

            List<Place> placesA = RetreveContract(cityA).Result;
            //Console.WriteLine(placesA[0].ToString());
            List<Place> placesB = RetreveContract(cityB).Result;
            //Console.WriteLine(placesB.Count);
            if (placesA.Count != 0 && placesB.Count != 0)
            {
                Place startStation = GetNearestPlace(placesA, placesA[0].position);
                Place endStation = GetNearestPlace(placesB, placesB[0].position);
                
                Console.WriteLine(startStation.name);
                Console.WriteLine(startStation.position);
                Console.WriteLine(endStation.name);
                Console.WriteLine(endStation.position);
                points.Add(startStation.position);
                points.Add(endStation.position);
            }

            HttpClient client = new HttpClient();
            string positionStart = start.Split(',')[0].Replace(' ', '+');
            HttpResponseMessage responsePositionStart = await client.GetAsync("https://api-adresse.data.gouv.fr/search/?q=" + positionStart);
            responsePositionStart.EnsureSuccessStatusCode();
            string responsePositionStartBody = await responsePositionStart.Content.ReadAsStringAsync();
            Position positionStartLeVrai = JsonSerializer.Deserialize<Position>(responsePositionStartBody);

            string positionEnd = start.Split(',')[0].Replace(' ', '+');
            HttpResponseMessage responsePositionEnd = await client.GetAsync("https://api-adresse.data.gouv.fr/search/?q=" + positionEnd);
            responsePositionEnd.EnsureSuccessStatusCode();
            string responsePositionEndBody = await responsePositionEnd.Content.ReadAsStringAsync();
            Position positionEndLeVrai = JsonSerializer.Deserialize<Position>(responsePositionEndBody);

            points.Add(positionStartLeVrai);
            points.Add(positionEndLeVrai);

            return points;
        }

        static async Task Main()
        {
            string start = "2 Rue Maurice Caunes, 31200 Toulouse";
            string end = "Centre Commercial, Mirail FR, MIRAIL, 31100 Toulouse";

            ClientSide.Program program = new ClientSide.Program();

            Task<List<Position>> positionTask = program.GetKeyPoints(start, end); 
            List<Position> positions = await positionTask; 
            foreach (var position in positions) { Console.WriteLine(position); }
            Console.ReadLine();
        }
    }

    public class Position
    {
        public double latitude { get; set; }
        public double longitude { get; set; }

        public override string ToString() { return $"Latitude: {latitude}, Longitude: {longitude}"; }
    }
    public class Place
    {
        public String name { get; set; }
        public String address { get; set; }
        public Position position { get; set; }

        public override string ToString() { return $"Name: {name}, Address: {address}, Position: {position}"; }
    }
}
