using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.Servies.InterfaceService
{
    public interface ISegmentService
    {
        Task<List<SegmentResponse>> GetSegment();
        Task<SegmentResponse> PostSegment(SegmentRequest model);
        Task<SegmentResponse> PutSegment(int id, SegmentRequest model);
        Task<SegmentResponse> DeleteSegment(int id);
        Task<SegmentResponse> GetSegmentByID(int id);
    }
}