using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace TradeMap.Service.Graph
{
    public class Vertex
    {
        public List<Edge> AdjList { get; set; } = new List<Edge>();

        public bool IsCollect { get; set; } = false;

        public Geometry Geom { get; set; } = null;

        public Vertex(Geometry geometry)
        {
            Geom = geometry;
        }

        public int Label { get; set; } = 0;

        public bool IsVisit { get; set; } = false;
    }
}