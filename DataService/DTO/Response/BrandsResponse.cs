namespace TradeMap.Service.DTO.Response
{
    public class BrandsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool? Active { get; set; }
        public string IconUrl { get; set; }
        public string ImageUrl { get; set; }
        public int? SegmentId { get; set; }
        public string SegmentName { get; set; }
    }
}