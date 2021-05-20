namespace TradeMap.Service.DTO.Request
{
    public class GetStoreBuildingBySystemZoneIdRequest
    {
        public int Type { get; set; } = 0;
        public int Status { get; set; } = 0;
        public int SystemZoneId { get; set; }
    }
}