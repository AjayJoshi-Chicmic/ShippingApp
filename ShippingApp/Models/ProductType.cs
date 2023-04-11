namespace ShippingApp.Models
{
    public class ProductType
    {
        public Guid typeId { get; set; }
        public string Type { get; set; } = string.Empty;
        public bool isFragile { get; set; } = false;
    }
}
