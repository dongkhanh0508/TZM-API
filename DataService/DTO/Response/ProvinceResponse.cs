using System.Collections.Generic;

namespace TradeMap.Service.DTO.Response
{
    public class ProvinceResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<DistrictResponse> Districts { get; set; }
    }

    public class DistrictResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public List<WardResponse> Wards { get; set; }
    }

    public class WardResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }

    public class WardWithDistrict : WardResponse
    {
        public string DistrictName { get; set; }
    }
}