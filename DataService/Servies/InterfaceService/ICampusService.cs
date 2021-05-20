using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.Servies.InterfaceService
{
    public interface ICampusService
    {
        Task<CampusResponse> PutCampus(int id, PutCampusRequest model);

        Task<CampusResponse> DeleteCampus(int id);

        Task<CampusResponse> CreateCampus(PostCampusRequest model);

        Task<CampusResponse> GetCampusById(int id);
    }
}