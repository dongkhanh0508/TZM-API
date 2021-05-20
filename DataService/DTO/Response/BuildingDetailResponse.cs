using System.Collections.Generic;

namespace TradeMap.Service.DTO.Response
{
    public class BuildingDetailResponse : BuildingResponse
    {
        public int? SystemzoneId { get; set; }
        public List<CustomerSegmentResponse> CustomerSegmentResponses { get; set; }
    }
}
