namespace ShippingApp.Models
{
    public class ProductType
    {
        public Guid typeId { get; set; }
        public string type { get; set; } = string.Empty;
        public float price { get; set; } = -1;
        public bool isFragile { get; set; } = false;
    }
}
