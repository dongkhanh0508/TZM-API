using NetTopologySuite.Geometries;
using System.Collections.Generic;

namespace TradeMap.Service.DTO.Response
{
    public class CustomFeatureCollection
    {
        public CustomFeatureCollection()
        {
        }

        public string Type { get; set; } = "FeatureCollection";
        public List<Feature> Features { get; set; } = new List<Feature>();
    }

    public class Feature
    {
        public Feature()
        {
            Geometry = null;
            Properties = new Properties();
        }

        public string Type { get; set; } = "Feature";
        public Geometry Geometry { get; set; }
        public Properties Properties { get; set; }
    }

    public class Properties
    {
        public Properties()
        {
        }

        public string F1 { get; set; }
        public string F2 { get; set; }
        public int? F3 { get; set; }
        public int F4 { get; set; }
        public string F5 { get; set; }
    }
}