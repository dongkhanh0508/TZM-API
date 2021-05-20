using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace TradeMap.Service.DTO.Response
{
    public class StoreResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Geometry Geom { get; set; }
        public DateTime? CreateDate { get; set; }
        public bool IsEditable { get; set; }
        public int? BrandId { get; set; }
        public string Address { get; set; }
        public int? FloorAreaId { get; set; }
        public string FloorAreaName { get; set; }
        public string BrandName { get; set; }
        public string Type { get; set; }
        public int? Status { get; set; }
        public List<StoreStreetSegmentResponse> StoreStreetSegments { get; set; }
        public string ImageUrl { get; set; }
        public int? ReferenceId { get; set; }
        public int? AbilityToServe { get; set; }
        public int SystemZoneId { get; set; }
        public string TimeSlot { get; set; }

        public HistoryResponse History { get; set; }
    }

    public class StoreBrandResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string WKT { get; set; }
        public DateTime? CreateDate { get; set; }
        public TimeSpan? CloseTime { get; set; }
        public TimeSpan? OpenTime { get; set; }
        public int? BrandId { get; set; }
        public string Address { get; set; }
        public int? FloorAreaId { get; set; }
        public string FloorAreaName { get; set; }
        public string BrandName { get; set; }
        public string Type { get; set; }
        public int? Status { get; set; }
        public List<StoreStreetSegmentResponse> StoreStreetSegments { get; set; }
        public string ImageUrl { get; set; }
        public int? ReferenceId { get; set; }
        public int? AbilityToServe { get; set; }
        public HistoryResponse History { get; set; }

        public string TimeSlot { get; set; }

        public Geometry Geom { get; set; }
    }

    public class StoreStreetSegmentResponse
    {
        public int StoreId { get; set; }
        public int StreetSegmentId { get; set; }
    }
}