using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RoutingServer
{
    class Program
    {
        static void Main(string[] args)
        {
            //ServiceCalculator.MathsOperationsClient client = new ServiceCalculator.MathsOperationsClient();
            //Console.WriteLine(client.Multiply(1, 2));
            Console.ReadLine();
        }


        // Partie Client
        /*
        public class Position
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
        }
        public class Place
        {
            public String name { get; set; }
            public String address { get; set; }
            public Position position { get; set; }
        }


        static readonly HttpClient client = new HttpClient();

        static async Task Main()
        {
            try
            {
                HttpResponseMessage response = await client.GetAsync("https://api.jcdecaux.com/vls/v3/stations?apiKey=da2a1717115e9e7a90149fd7d0c4afcff086e014");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody);

                String ville = Console.ReadLine();
                HttpResponseMessage response2 = await client.GetAsync("https://api.jcdecaux.com/vls/v1/stations?contract=" + ville + "&apiKey=da2a1717115e9e7a90149fd7d0c4afcff086e014");
                response2.EnsureSuccessStatusCode();
                string responseBody2 = await response2.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody2);

                String num = Console.ReadLine();
                HttpResponseMessage response3 = await client.GetAsync("https://api.jcdecaux.com/vls/v3/stations/" + num + "?contract=" + ville + "&apiKey=da2a1717115e9e7a90149fd7d0c4afcff086e014");
                response3.EnsureSuccessStatusCode();
                string responseBody3 = await response3.Content.ReadAsStringAsync();
                Console.WriteLine(responseBody3);

                Place place = JsonSerializer.Deserialize<Place>(responseBody3);
                GeoCoordinate geoCoordinate = new GeoCoordinate(place.position.latitude, place.position.longitude);
                List<Place> places = JsonSerializer.Deserialize<List<Place>>(responseBody2);
                Place nearest = null;
                foreach (var station in places)
                {
                    if (nearest == null && !place.Equals(station))
                    {
                        nearest = station;
                    }
                    if (nearest != null && !station.Equals(place))
                    {
                        GeoCoordinate geoCoordinateOther = new GeoCoordinate(place.position.latitude, place.position.longitude);
                        GeoCoordinate geoCoordinateNearest = new GeoCoordinate(nearest.position.latitude, nearest.position.longitude);

                        double distance = geoCoordinate.GetDistanceTo(geoCoordinateOther);
                        double distanceNear = geoCoordinate.GetDistanceTo(geoCoordinateNearest);

                        if (distance < distanceNear)
                        {
                            nearest = station;
                        }
                    }

                }
                Console.WriteLine(nearest.name);
                Console.WriteLine(nearest.address);
                Console.WriteLine(nearest.position.latitude);
                Console.WriteLine(nearest.position.longitude);



                Console.ReadLine();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("\nException Caught!");
                Console.WriteLine("Message :{0} ", e.Message);
            }

        }*/
    }
}
