namespace TradeMap.Service.DTO.Request
{
    public class PutCustomerSegmentRequest
    {
        public string TimeSlot { get; set; }
        public int? PrimaryAge { get; set; }
        public int PotentialCustomers { get; set; }
    }
}
