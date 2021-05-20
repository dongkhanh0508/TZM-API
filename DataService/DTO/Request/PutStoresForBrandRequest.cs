namespace TradeMap.Service.DTO.Request
{
    public class PutStoresForBrandRequest
    {
        public string Name { get; set; }
        public string TimeSlot { get; set; }
        public string Address { get; set; }
        public string ImageUrl { get; set; }
        public int? AbilityToServe { get; set; }
    }
}
