using static TradeMap.Service.Helpers.StatusEnum;

namespace TradeMap.Service.DTO.Request
{
    public class PagingRequestGetSurvey : PagingRequest
    {
        public Status Status { get; set; } = 0;
    }
}