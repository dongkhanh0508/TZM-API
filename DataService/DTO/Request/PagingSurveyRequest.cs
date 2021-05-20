using static TradeMap.Service.Helpers.StatusSurveyRequestEnum;

namespace TradeMap.Service.DTO.Request
{
    public class PagingSurveyRequest : PagingRequest
    {
        public StatusRequest Status { get; set; } = 0;
        public int BrandId { get; set; } = 0;
    }
}
