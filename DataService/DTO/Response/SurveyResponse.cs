using System;

namespace TradeMap.Service.DTO.Response
{
    public class SurveyResponse
    {
        public int Id { get; set; }
        public DateTime? CreateDate { get; set; }
        public string Description { get; set; }
        public string Geom { get; set; }
        public string Note { get; set; }
        public int? Status { get; set; }
        public int? BrandId { get; set; }
        public string BrandName { get; set; }
    }
}
