using System;

namespace TradeMap.Service.Servies.ImplService
{
    public class CheckBuidlingResponse
    {
        public int? Id { get; set; }

        public String Name { get; set; }

        public bool IsValid { get; set; }
    }
}