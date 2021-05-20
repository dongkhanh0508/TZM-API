using System.Collections.Generic;
using TradeMap.Data.Entity;

namespace TradeMap.Algorithm.Graph
{
    public class Vertex
    {
        public SystemZone SystemZone { get; set; }
        public List<Edge> AdjList { get; set; } = new List<Edge>();

        public bool IsCollect { get; set; } = false;

        public Vertex(SystemZone SystemZone)
        {
            this.SystemZone = SystemZone;
        }
        public int Level { get; set; } = -1;

        public int Label { get; set; } = 0;

        public bool IsVisit { get; set; } = false;

        public int SelectedId { get; set; } = 0;
    }
}