using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.Servies.InterfaceService
{
    public interface IAssetsService
    {
        Task<PagedResults<AssetResponse>> GetAsset(PagingAssetRequest request, int brandId);

        Task<PagedResults<LogViolationResponse>> GetAssetLogViolation(PagingRequestLogViolation request, int brandId);
        Task<LogViolationResponse> GetAssetLogViolationById(int id);

        Task<List<AssetReportResponse>> GetAssetReport(int brandId);

        Task<AssetResponse> PostAsset(AssetRequest model);

        Task<AssetResponse> PutAsset(Guid id, AssetRequest model);

        Task<AssetResponse> DeleteAsset(Guid id);

        Task<AssetResponse> GetAssetByID(Guid id);
        Task<string> AssetAuthen(AssetAuthenRequest request);
    }
}