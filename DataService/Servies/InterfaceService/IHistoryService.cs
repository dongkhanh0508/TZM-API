using System;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.Servies.InterfaceService
{
    public interface IHistoryService
    {
        Task<PagedResults<HistoryResponse>> GetHistory(PagingRequestHistory model, int role, Guid accountId);
        void ArchiveHistory();
    }
}