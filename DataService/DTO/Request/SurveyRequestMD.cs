using NetTopologySuite.Geometries;

namespace TradeMap.Service.DTO.Request
{
    public class SurveyRequestMD
    {
        public string Description { get; set; }
        public Geometry Geom { get; set; }
        public string Note { get; set; }
    }
}
