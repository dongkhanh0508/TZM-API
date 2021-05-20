using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.Servies.InterfaceService
{
    public interface ITradeZoneVersionServices
    {
        Task<TradeZoneVersionResponse> CreateTradeZoneVersion(PostTradeZoneVerison model, int brandId);

        Task<List<TradeZoneVersionResponse>> GetTradeZoneVersionByBrandId(int brandId, string dateFilter, string timeSlot);

        Task<TradeZoneVersionDetailResponse> GetTradeZoneVersionById(int Id);

        Task<TradeZoneVersionDetailResponse> GetTradeZoneVersionActive(int brandId);

        Task<TradeZoneVersionResponse> ChangeFlagActiveVersion(int brandId, int Id);

        Task<TradeZoneVersionResponse> DeleteTradeZoneVersionById(int brandId, int id);

        Task<List<ZoneCoverageResponse>> GetGroupZoneByTradeZoneVersionId(int tradezoneId, int brandId);

        Task<List<GroupStoreTradeZoneResponse>> GetListGroupStoreTradeZoneByTradeZoneVersionId(int brandId);
    }
}