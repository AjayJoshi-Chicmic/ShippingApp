namespace ShippingApp.Models
{
    public class ResponseModelContainer
    {
        public int statusCode { get; set; } = 200;
        public string message { get; set; } = "Success";
        public List<ContainerTypeModel>? data { get; set; }
        public bool isSuccess { get; set; } = true;
        
    }
}
