using static TradeMap.Service.Helpers.SortType;

namespace TradeMap.Service.DTO.Request
{
    public class PagingRequest
    {
        public int? Page { get; set; } = null;
        public int? PageSize { get; set; } = null;
        public string KeySearch { get; set; } = "";
        public SortOrder SortType { get; set; } = 0;
        public string ColName { get; set; } = "name";
    }
}