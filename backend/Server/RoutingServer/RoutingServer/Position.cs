namespace RoutingServer
{
    public class Position
    {
        public double lat { get; set; }
        public double lng { get; set; }

        public override string ToString() { return $"Latitude: {lat}, Longitude: {lng}"; }
    }
}
