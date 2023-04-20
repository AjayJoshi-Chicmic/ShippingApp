namespace ShippingApp.Models
{
    public class ResponseModelProduct
    {
        public int statusCode { get; set; } = 200;
        public string message { get; set; } = "Success";
        public List<ProductType>? data { get; set; }
        public bool isSuccess { get; set; } = true;
    }
}
