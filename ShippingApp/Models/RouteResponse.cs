using System.Security.Cryptography;

namespace ShippingApp.Models
{
    public class RouteResponse
    {
        public string code { get; set; }
        public List<object> waypoints { get; set; }
        public List<trip> trips { get; set; }
    }
    public class trip
    {
        public string geometry { get; set; }
        public List<object> legs { get; set; }
        public string weight_name { get; set; }
        public double weight { get; set; }
        public double duration { get; set; }
        public double distance { get; set; }
    }
}
