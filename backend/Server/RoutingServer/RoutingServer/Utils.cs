using System.Collections.Generic;
using System.Device.Location;

namespace RoutingServer
{
    internal class Utils
    {
        public Place GetNearestPlace(List<Place> places, Position coord)
        {
            GeoCoordinate geoCoordinate = new GeoCoordinate(coord.lat, coord.lng);
            Place best = null;
            foreach (var place in places)
            {
                if (best == null)
                {
                    best = place;
                }
                GeoCoordinate geoCoordinateOther = new GeoCoordinate(place.position.lat, place.position.lng);
                GeoCoordinate geoCoordinateBest = new GeoCoordinate(best.position.lat, best.position.lng);
                double distance = geoCoordinate.GetDistanceTo(geoCoordinateOther);
                double distanceBest = geoCoordinate.GetDistanceTo(geoCoordinateBest);
                if (distance < distanceBest)
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
    }
}
