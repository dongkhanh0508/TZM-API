using static TradeMap.Service.Helpers.TypeAssetEnum;

namespace TradeMap.Service.DTO.Request
{
    public class PagingAssetRequest : PagingRequest
    {
        public TypeAsset TypeAsset { get; set; } = 0;
        public int StoreId { get; set; }
    }
}
