using System;
using System.Collections.Generic;
using System.Device.Location;
using System.Diagnostics;
using System.Linq;

namespace RoutingServer
{
    internal class Utils
    {
        // places = liste des lieux potentiels
        // coord = coordonnées de départ
        // other = coordonnées de destination
        // si coord - best - other < coord - other * 2 alors on fait coord - best - other
        public Place GetNearestPlace(List<Place> places, Position coord, Position other)
        {
            GeoCoordinate geoCoordinate = new GeoCoordinate(coord.lat, coord.lng);
            GeoCoordinate geoCoordinateOther = new GeoCoordinate(other.lat, other.lng);
            GeoCoordinate geoCoordinateBest;
            Place best = null;
            foreach (var place in places)
            {
                if (best == null)
                {
                    best = place;
                }
                GeoCoordinate geoCoordinatePlace = new GeoCoordinate(place.position.lat, place.position.lng);
                geoCoordinateBest = new GeoCoordinate(best.position.lat, best.position.lng);
                double distance = geoCoordinate.GetDistanceTo(geoCoordinatePlace);
                double distanceBest = geoCoordinate.GetDistanceTo(geoCoordinateBest);
                if (distance < distanceBest)
                {
                    best = place;
                }
            }

            geoCoordinateBest = new GeoCoordinate(best.position.lat, best.position.lng);
            double trajetCoordBestEndOther = geoCoordinate.GetDistanceTo(geoCoordinateBest) + geoCoordinate.GetDistanceTo(geoCoordinateOther); ;
            if(trajetCoordBestEndOther > geoCoordinate.GetDistanceTo(geoCoordinateOther)*2){
                return null;
            }
            return best;
        }


        public string GetCity(string address)
        {
            Console.WriteLine("ADDRESS: " + address);
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
            Console.WriteLine("CITY: " + res);
            // si on n'a pas de code postal
            if(res == "")
            {
                string[] a = address.Split(',');
                return a[a.Length - 2]; // -1 = pays
            }
            Console.WriteLine("CITY: " + res);
            return res;
        }
    }
}
