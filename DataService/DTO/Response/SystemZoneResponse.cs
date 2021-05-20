using NetTopologySuite.Geometries;
using System;

namespace TradeMap.Service.DTO.Response
{
    public class SystemZoneResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Geometry Geom { get; set; }
        public int? WardId { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public double? Weight { get; set; }
        public bool IsMySystemZone { get; set; }
    }
}