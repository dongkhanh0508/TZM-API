using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;

namespace TradeMap.Service.DTO.Response
{
    public class CampusResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Geometry Geom { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public List<CampusStreetSegmentRespone> ListStreetSegment { get; set; }
    }

    public class CampusStreetSegmentRespone
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}