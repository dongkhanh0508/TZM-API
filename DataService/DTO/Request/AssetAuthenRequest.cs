using System;

namespace TradeMap.Service.DTO.Request
{
    public class AssetAuthenRequest
    {
        public int BrandId { get; set; }
        public int StoreId { get; set; }
        public Guid AssetId { get; set; }
    }
}