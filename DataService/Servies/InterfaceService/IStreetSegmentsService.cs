using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.Servies.InterfaceService
{
    public interface IStreetSegmentsService
    {
        Task<StreetSegmentResponse> PostStreetSegment(PostStreetSegmentRequest model);

        Task<ListStreetSegmentResponse> GetStreetSegmentInRadius(string coor);

        Task<ListStreetSegmentResponse> GetStreetSegmentsByBuildingId(int id);

        Task<ListStreetSegmentResponse> GetStreetSegmentInRadiusPoint(string coor);

        Task<ListStreetSegmentResponse> GetStreetSegmentsByStoreId(int id);
    }
}