using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.Servies.ImplService
{
    public interface IGroupZoneServices
    {
        Task<CustomFeatureCollection> GetGroupZone(int brandId);

        Task<GroupZoneResponse> PostGroupZone(GroupZoneRequest model, int brandId);

        Task<GroupZoneResponse> PutGroupZone(int id, PutGroupZoneRequest model, int brandId);

        Task<GroupZoneResponse> DeleteGroupZone(int id, int brandId);

        Task<GroupZoneResponse> GetGroupZoneByID(int id, int brandId);

        Task<CustomFeatureCollection> GetFreeWard(int brandId);

        Task<CustomFeatureCollection> GetFreeDistrict(int brandId);

        Task<CustomFeatureCollection> GetFreeSystemzone(int brandId);

        Task<StoreTradeZoneForMapResponse> GetStoresByGroupZoneId(int groupzoneId, int brandId, int tradezoneVersionId);

        Task<List<ZoneWeightResponse>> GetTradeZoneByGroupZoneId(int groupzoneId, int tradezoneVersionId);
    }
}