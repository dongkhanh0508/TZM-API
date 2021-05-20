using System;

namespace TradeMap.Service.DTO.Response
{
    public class SurveyorResponse
    {
        public Guid Id { get; set; }
        public bool Active { get; set; }
        public string Fullname { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }
}