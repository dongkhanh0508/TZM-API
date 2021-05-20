namespace TradeMap.Service.DTO.Response
{
    public class CustomerSegmentResponse
    {
        public int BuildingId { get; set; }
        public string BuildingName { get; set; }
        public int SegmentId { get; set; }
        public string SegmentName { get; set; }
        public string TimeSlot { get; set; }
        public int? PrimaryAge { get; set; }
        public int? PotentialCustomers { get; set; }
    }
}
