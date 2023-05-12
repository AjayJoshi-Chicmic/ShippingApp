using ShippingApp.Models;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace ShippingApp.Services
{
    /// <summary>
    /// 
    /// A service to call third party Apis 
    /// 
    /// </summary>
    public class ApiCallingService : IApiCallingService
    {
        //variables
        public readonly IConfiguration _configuration;
        public string baseUrlS3 = "";
		public string MapUrl = "";

        //constructor with DI
		public ApiCallingService(IConfiguration configuration)
        {
            this._configuration = configuration;
            baseUrlS3 = _configuration.GetSection("ApiUrl:Url1").Value!;
            MapUrl = _configuration.GetSection("ApiUrl:Url2").Value!;
		}

        //-------A function for Calling Api to get product type------------->
		public ProductType GetProductType(Guid? productTypeId)
        {
            // creating a http client instance
            using (var client = new HttpClient())
            {
                //creating api url for request 
                client.BaseAddress = new Uri(baseUrlS3);//WebApi 1 project URL
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                StringBuilder appendUrl = new StringBuilder("api/ProductType/Search?");
                if (productTypeId != null)
                {
                    //adding product id to the url
                    appendUrl.Append("productTypeId=" + productTypeId + "");
                }
                //getting response
                var res = client.GetAsync(appendUrl.ToString()).Result;
                //response data in string
                var data = res.Content.ReadAsStringAsync().Result;
                //converting response into object
                ResponseModelProduct response = JsonSerializer.Deserialize<ResponseModelProduct>(data)!;
                ProductType product = response.data!.FirstOrDefault()!;
                return product;
            }
        }
		//-------A function for Calling Api to get container type------------->
		public ContainerTypeModel GetContainerType(Guid? containerTypeId)
        {
			// creating a http client instance
			using (var client = new HttpClient())
            {
				//creating api url for request 
				client.BaseAddress = new Uri(baseUrlS3);//WebApi 1 project URL
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                StringBuilder appendUrl = new StringBuilder("api/ContainerType/Search?");
                if (containerTypeId != null)
                {
					//adding cotainer id to the url
					appendUrl.Append("containerTypeId=" + containerTypeId + "");
                }
				//getting response
				var res = client.GetAsync(appendUrl.ToString()).Result;
				//response data in string
				var data = res.Content.ReadAsStringAsync().Result;
				//converting response into object
				ResponseModelContainer response = JsonSerializer.Deserialize<ResponseModelContainer>(data)!;
                ContainerTypeModel container = response.data!.FirstOrDefault()!;
                return container;
            }
        }
		//-------A function for Calling Api to distance b/w two checkpoints------------->
		public double GetDistance(GetCheckpointModel cp1, GetCheckpointModel cp2)
        {
			// creating a http client instance
			using (var client = new HttpClient())
            {
				//creating api url for request 
				var res = client.GetAsync($"{MapUrl}{cp1.longitude},{cp1.latitude};{cp2.longitude},{cp2.latitude}?access_token=pk.eyJ1Ijoiam9vc2hpIiwiYSI6ImNsaDRjeTBiazBqeG0zZ281enNzOXR4cjcifQ.eLxwYoRL5rhHhQxjv9mZkg").Result;
                //detting data in json and converting it into object
                var data = res.Content.ReadFromJsonAsync<RouteResponse>();

                return (data.Result!.trips[0].distance/1000);
            }
        }
    }
}
