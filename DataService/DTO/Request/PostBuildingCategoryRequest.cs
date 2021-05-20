namespace TradeMap.Service.DTO.Request
{
    public class PostCustomerSegmentRequest
    {
        public int BuildingId { get; set; }
        public int SegmentId { get; set; }
        public string TimeSlot { get; set; }
        public int? PrimaryAge { get; set; }
        public int PotentialCustomers { get; set; }
    }
}
