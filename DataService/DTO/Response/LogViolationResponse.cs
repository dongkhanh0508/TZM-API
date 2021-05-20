using NetTopologySuite.Geometries;
using System;

namespace TradeMap.Service.DTO.Response
{
    public class LogViolationResponse
    {
        public int Id { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Description { get; set; }
        public Guid? AssetId { get; set; }
        public string AssetName { get; set; }
        public int? StoreId { get; set; }
        public string StoreName { get; set; }
        public int TypeViolation { get; set; }
        public int Severity { get; set; }
        public string Geom { get; set; }

        public Geometry Geometry { get; set; }
    }
}