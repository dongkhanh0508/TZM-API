using NetTopologySuite.Geometries;

namespace TradeMap.Service.DTO.Response
{
    public class TradezoneResponse
    {
        public int Id { get; set; }
        public Geometry Geom { get; set; }

        public string TimeSlot { get; set; }

        public double? WeightNumber { get; set; }
    }
}
