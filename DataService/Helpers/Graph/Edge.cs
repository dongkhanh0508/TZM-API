namespace TradeMap.Service.Graph
{
    public class Edge
    {
        public Edge(Vertex u, Vertex v)
        {
            U = u;
            V = v;
        }

        public Vertex U { get; set; }

        public Vertex V { get; set; }
    }
}