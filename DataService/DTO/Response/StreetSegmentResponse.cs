using NetTopologySuite.Geometries;
using System;

namespace TradeMap.Service.DTO.Response
{
    public class StreetSegmentResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Geometry Geom { get; set; }
#nullable enable
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }

        //  public int? StreetId { get; set; }
        public int? WardId { get; set; }

        public int? StreetId { get; set; }
    }
}