using ShippingApp.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ShippingApp.Services
{
    public class ApiCallingService : IApiCallingService
    {
        string baseUrlS3 = "http://192.180.0.127:4040/";
        public ProductType GetProductType(Guid? productTypeId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrlS3);//WebApi 1 project URL
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                StringBuilder appendUrl = new StringBuilder("api/ProductType/Search?");
                if (productTypeId != null)
                {
                    appendUrl.Append("productTypeId=" + productTypeId + "");

                }
                var res = client.GetAsync(appendUrl.ToString()).Result;
                var data = res.Content.ReadAsStringAsync().Result;

                ResponseModelProduct response = JsonSerializer.Deserialize<ResponseModelProduct>(data)!;
                ProductType product = response.data!.FirstOrDefault()!;
                return product;
            }
        }
        public ContainerTypeModel GetContainerType(Guid? containerTypeId)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(baseUrlS3);//WebApi 1 project URL
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                StringBuilder appendUrl = new StringBuilder("api/ContainerType/Search?");
                if (containerTypeId != null)
                {
                    appendUrl.Append("containerTypeId=" + containerTypeId + "");

                }
                var res = client.GetAsync(appendUrl.ToString()).Result;
                var data = res.Content.ReadAsStringAsync().Result;

                ResponseModelContainer response = JsonSerializer.Deserialize<ResponseModelContainer>(data)!;
                ContainerTypeModel container = response.data!.FirstOrDefault()!;
                return container;
            }
        }

        public double GetDistance(CheckpointModel cp1, CheckpointModel cp2)
        {

            using (var client = new HttpClient())
            {
                var res = client.GetAsync($"https://api.mapbox.com/optimized-trips/v1/mapbox/driving/{cp1.longitude},{cp1.latitude};{cp2.longitude},{cp2.latitude}?access_token=pk.eyJ1Ijoiam9vc2hpIiwiYSI6ImNsaDRjeTBiazBqeG0zZ281enNzOXR4cjcifQ.eLxwYoRL5rhHhQxjv9mZkg").Result;
                var d = res.Content.ReadAsStringAsync().Result;
                var data = res.Content.ReadFromJsonAsync<RouteResponse>();

                return (data.Result!.trips[0].distance/1000);
            }
        }
    }
}
