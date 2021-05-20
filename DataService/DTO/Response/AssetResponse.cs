using System;

namespace TradeMap.Service.DTO.Response
{
    public class AssetResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int? Type { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public bool? IsDeleted { get; set; }
    }
}
