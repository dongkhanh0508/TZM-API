using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.Servies.InterfaceService
{
    public interface IWardsService
    {
        Task<List<ProvinceResponse>> GetWard();
    }
}