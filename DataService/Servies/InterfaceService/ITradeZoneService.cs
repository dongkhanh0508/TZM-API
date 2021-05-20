using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.Servies.InterfaceService
{
    public interface ITradeZoneService
    {
        Task<StoreTradeZoneResponse> GroupSystemzoneForStoreByDistance(int brandId, double distance, string timeSlot, List<int> storesId);

        Task<List<TradezoneResponse>> InsertTradezone(List<TradezoneRequest> list, int brandId);
    }
}