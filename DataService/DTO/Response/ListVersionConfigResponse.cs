using System.Collections.Generic;

namespace TradeMap.Service.DTO.Response
{
    public class ListVersionConfigResponse
    {
        public int CurrentActive { get; set; }
        public List<int> VersionList { get; set; }
    }
}
