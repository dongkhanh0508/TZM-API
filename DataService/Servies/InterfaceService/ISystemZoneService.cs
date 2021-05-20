using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.Servies.InterfaceService
{
    public interface ISystemZoneService
    {
        Task<SystemZoneResponse> PostSystemZone(PostSystemZoneRequset model);

        Task<List<StoreBuildingResponse>> GetStoreBuildingBySystemZoneId(GetStoreBuildingBySystemZoneIdRequest request);

        Task<PagedResults<SystemZoneResponse>> GetSystemZone(SystemZonePagingRequest request, Guid accountId);

        Task<SystemZoneResponse> PutSystemZone(int id, PutSystemZone model);

        Task<AssignSystemZoneRespones> PostAssignSystemZone(Guid accountId, int systemZoneId);

        Task<AssignSystemZoneRespones> DeleteAssignSystemZone(Guid accountId, int systemZoneId);

        Task<SystemZoneResponse> GetSystemZoneById(int id);

        Task<SystemZoneResponse> DeleteSystemZone(int id);

        Task<Geometry> CheckSystemZoneClose(int wardId);

        Task<bool> CheckSystemzoneFillWard(int wardId);

        Task<PagedResults<BuildingResponse>> GetBuidingBySystemzoneId(PagingRequestGetSurvey model, int Id, Guid curentAccountId);

        Task<PagedResults<StoreResponse>> GeStoreBySystemzoneId(PagingRequestGetSurvey model, int Id, Guid curentAccountId);
    }
}