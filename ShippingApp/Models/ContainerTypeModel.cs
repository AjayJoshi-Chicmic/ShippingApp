namespace ShippingApp.Models
{
    public class ContainerTypeModel
    {
        public Guid containerTypeId { get; set; }
        public string containerName { get; set; }
        public float price { get; set; } = -1;
    }
}
