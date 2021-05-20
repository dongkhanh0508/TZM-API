using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace TradeMap.Service.DTO.Response
{
    public class BuildingResponse
    {
        public int Id { get; set; }

        public bool IsEditable { get; set; }
        public string Name { get; set; }
        public int? OsmId { get; set; }
        public DateTime? CreateDate { get; set; }
        public Geometry Geom { get; set; }
        public string ImageUrl { get; set; }
        public string Type { get; set; }
        public bool? Active { get; set; }
        public int? NumberOfFloor { get; set; }
        public int? CampusId { get; set; }
        public int? Status { get; set; }
        public string Address { get; set; }
        public int? SystemZoneId { get; set; }
        public List<FloorResopnse> Floors { get; set; }
        public List<BuidingStreetSegmentResponse> BuildingStreetSegments { get; set; }
        public int? ReferenceId { get; set; }
        public string ReferencrName { get; set; }
        public HistoryResponse History { get; set; }
    }

    public class FloorResopnse
    {
        public int Id { get; set; }
        public int? FloorNumber { get; set; }
        public string Name { get; set; }
        public int? BuildingId { get; set; }
        public int? ReferenceId { get; set; }

        public List<FloorAreaResopnse> FloorAreas { get; set; }
    }

    public class FloorAreaResopnse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int? FloorId { get; set; }
        public int? ReferenceId { get; set; }
        public List<StoreResponse> Stores { get; set; }
    }

    public class BuidingStreetSegmentResponse
    {
        public int BuildingId { get; set; }
        public int StreetSegmentId { get; set; }
    }
}