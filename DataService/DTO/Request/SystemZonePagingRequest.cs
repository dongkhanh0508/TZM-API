namespace TradeMap.Service.DTO.Request
{
    public class SystemZonePagingRequest : PagingRequest
    {
        public int DistrictId { get; set; } = 0;
        public bool IsMe { get; set; } = false;
    }
}
