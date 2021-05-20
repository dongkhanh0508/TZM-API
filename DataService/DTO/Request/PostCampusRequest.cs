using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace TradeMap.Service.DTO.Request
{
    public class PostCampusRequest
    {
        public string Name { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(NetTopologySuite.IO.Converters.GeometryConverter))]
        public Geometry Geom { get; set; }

        public List<int> StreetSegmentId { get; set; }
    }
}