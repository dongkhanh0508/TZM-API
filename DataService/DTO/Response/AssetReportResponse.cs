using static TradeMap.Service.Helpers.TypeAssetEnum;

namespace TradeMap.Service.DTO.Response
{
    public class AssetReportResponse
    {
        public TypeAsset TypeAsset { get; set; }
        public int Total { get; set; }
    }
}
