using System.Collections.Generic;
using System.Linq;
using TradeMap.Data.Entity;

namespace TradeMap.Algorithm.Graph
{
    public class Graph : List<Vertex>
    {
        public Graph() : base()
        {
        }

        public void AddVertext(Vertex v)
        {
            Add(v);
        }

        public void AddRangeVertext(List<Vertex> list)
        {
            AddRange(list);
        }

        public Vertex GetVertex(int id)
        {
            foreach (var item in this)
            {
                if (item.SystemZone.Id == id) return item;
            }
            return null;
        }

        public bool AddEdge(SystemZone From, SystemZone To)
        {
            Vertex u = this.GetVertex(From.Id);
            Vertex v = this.GetVertex(To.Id);
            if (u == null || v == null) return false;
            Edge e = new Edge(u, v);
            u.AdjList.Add(e);
            return true;
        }

        public void DFS(int vertex)
        {
            GetVertex(vertex).IsVisit = true;
            GetVertex(vertex).Label = vertex;

            foreach (var item in GetVertex(vertex).AdjList)
            {
                if (!item.V.IsVisit)
                {
                    DFS(item.V.SystemZone.Id);
                }
            }
        }

        public void DFS1(int vertex, int label)
        {
            GetVertex(vertex).IsVisit = true;
            GetVertex(vertex).Label = label;
            // GetVertex(vertex).IsCollect = true;

            foreach (var item in GetVertex(vertex).AdjList)
            {
                if (!item.V.IsVisit)
                {
                    DFS1(item.V.SystemZone.Id, label);
                }
            }
        }

        public void DeepFirstSearch()
        {
            var vertices = this.ToList();

            foreach (var item in vertices)
            {
                foreach (var vertex in this)
                {
                    vertex.IsVisit = false;
                }

                if (GetVertex(item.SystemZone.Id).IsVisit == false)
                    DFS1(item.SystemZone.Id, item.SystemZone.Id);
            }
        }

        public List<int> InvalidVertex()
        {
            return this.Where(x => !x.IsCollect).Select(x => x.SystemZone.Id).ToList();
        }

        public void BFS(int vertex)
        {
            List<Vertex> rs = new List<Vertex>();

            GetVertex(vertex).Level = 0;
            int level = 1;
            rs.Add(GetVertex(vertex));
            while (rs.Count > 0)
            {
                List<Vertex> temp = new List<Vertex>();
                foreach (var item in rs)
                {
                    foreach (var edge in item.AdjList)
                    {
                        Vertex v = edge.V;
                        if (v.Level == -1)
                        {
                            edge.V.Level = level;
                            temp.Add(edge.V);
                        }
                    }
                }
                rs.Clear();
                rs.AddRange(temp);
                level++;
            }
        }
    }
}