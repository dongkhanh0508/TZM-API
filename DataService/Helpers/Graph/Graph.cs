using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;

namespace TradeMap.Service.Graph
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

        public Vertex GetVertex(Geometry id)
        {
            foreach (var item in this)
            {
                if (item.Geom == id)
                {
                    return item;
                }
            }
            return null;
        }

        public bool AddEdge(Geometry From, Geometry To)
        {
            Vertex u = this.GetVertex(From);
            Vertex v = this.GetVertex(To);
            if (u == null || v == null)
            {
                return false;
            }

            Edge e = new Edge(u, v);
            u.AdjList.Add(e);
            return true;
        }

        public void DFS(Geometry vertex, int label)
        {
            GetVertex(vertex).IsVisit = true;
            GetVertex(vertex).Label = label;
            // GetVertex(vertex).IsCollect = true;

            foreach (var item in GetVertex(vertex).AdjList)
            {
                if (!item.V.IsVisit)
                {
                    DFS(item.V.Geom, label);
                }
            }
        }

        public void DeepFirstSearch()
        {
            var vertices = this.ToList();
            int label = 0;
            foreach (var item in vertices)
            {
                label++;
                foreach (var vertex in this)
                {
                    vertex.IsVisit = false;
                }

                if (GetVertex(item.Geom).IsVisit == false)
                {
                    DFS(item.Geom, label);
                }
            }
        }

    }
}