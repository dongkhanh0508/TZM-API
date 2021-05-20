using NetTopologySuite.Geometries;
using System;

namespace TradeMap.Service.Helpers
{
    public class AssetRedis
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int StoreId { get; set; }

        public int Type { get; set; }

        public string StoreName { get; set; }

        public int BrandId { get; set; }

        public string BrandName { get; set; }

        public string Location { get; set; }

        public string CurrentLocation { get; set; }

        public DateTime StartTime { get; set; }
    }

    public class AssetRedisResponse : AssetRedis
    {
        public Geometry Geometry { get; set; }

        public Geometry CurrentLocationGeometry { get; set; }

    }

    public class TimePointAsset
    {
        public DateTime LocationTime { get; set; }

        public Geometry LocationPoint { get; set; }
    }
}