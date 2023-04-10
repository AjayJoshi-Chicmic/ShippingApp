namespace ShippingApp.Models
{
    public class ProductModel
    {
        public Guid productId { get; set; } = Guid.NewGuid();
        public string productName { get; set; } = string.Empty;
        public Guid typeId { get; set; }
        public bool isFragile { get; set; } = false;

    }
}
