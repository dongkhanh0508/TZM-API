using NetTopologySuite.Geometries;

namespace TradeMap.Service.DTO.Request
{
    public class TradezoneRequest
    {
        public int StoreId { get; set; }
        public Geometry Geom { get; set; }

        public int GroupzoneId { get; set; }

        public float WeightNumber { get; set; }

        public string TimeSlot { get; set; }

        public string Name { get; set; }
    }
}