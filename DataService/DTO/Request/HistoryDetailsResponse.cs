using System;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.DTO.Request
{
    public class HistoryDetailsResponse
    {
        public int Id { get; set; }
        public int? StoreId { get; set; }
        public Guid? AccountId { get; set; }
        public string AccountName { get; set; }
        public int? Action { get; set; }
        public int? StatusApproval { get; set; }
        public DateTime? CreateDate { get; set; }
        public DateTime? ModifyDate { get; set; }
        public string Note { get; set; }
        public int? BuildingId { get; set; }

        public BuildingResponse Building { get; set; }
        public StoreResponse Store { get; set; }
        public BuildingResponse BuildingUpdated { get; set; }
        public StoreResponse StoreUpdated { get; set; }
    }
}