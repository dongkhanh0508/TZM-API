using NetTopologySuite.Geometries;

namespace TradeMap.Service.DTO.Request
{
    public class PostStreetSegmentRequest
    {
        public string Name { get; set; }

        [Newtonsoft.Json.JsonConverter(typeof(NetTopologySuite.IO.Converters.GeometryConverter))]
        public Geometry Geom { get; set; }

        public int StreetId { get; set; }
        public int WardId { get; set; }
    }
}