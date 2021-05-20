using NetTopologySuite.Geometries;

namespace TradeMap.Algorithm.Model
{
    public class StoreInMap
    {
        public int Id { get; set; }

        public Geometry Geom { get; set; }

        public string Name { get; set; }
    }
}