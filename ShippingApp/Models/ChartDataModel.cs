namespace ShippingApp.Models
{
    public class ChartDataModel
    {
        public List<int>? monthShipment{ get; set; }
        public List<float>? monthRevenue { get; set; }
        public ChartDataModel(List<int>? monthShipment, List<float>? monthRevenue)
        {
            this.monthShipment = monthShipment;
            this.monthRevenue = monthRevenue;
        }
    }

}
