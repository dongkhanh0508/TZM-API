using static TradeMap.Service.Helpers.ActionEnum;
using static TradeMap.Service.Helpers.StatusEnum;

namespace TradeMap.Service.DTO.Request
{
    public class PagingRequestHistory : PagingRequest
    {
        public ActionSurvey Action { get; set; } = 0;
        public int Type { get; set; } = 0;
        public Status Status { get; set; } = 0;
    }
}