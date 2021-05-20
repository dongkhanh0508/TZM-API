using NetTopologySuite.Geometries;

namespace TradeMap.Service.DTO.Request
{
    public class StoreBuildingResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public int? Status { get; set; }
        public Geometry Geom { get; set; }
    }
}
