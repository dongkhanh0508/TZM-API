using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.InterfaceService
{
    public interface IBuildingService
    {
        Task<BuildingDetailResponse> GetBuidingById(int id, Guid accountId);

        Task<BuildingResponse> InsertBuilding(PostBuildingRequest model, Guid accountId);

        Task<BuildingResponse> PutBuilding(int id, PostBuildingRequest model, Guid accountId);

        Task<PagedResults<BuildingResponse>> GetBuidingNeedSurveyByAccountId(Guid accountId, PagingRequestGetSurvey model);

        Task<BuildingResponse> ChangeFlagNeedSurvey(int id);

        Task<BuildingResponse> DeleteBuilding(int id, Guid accountId);

        Task<List<TypeResponse>> GetListTypeBuilding();

        //PagedResults<BuildingResponse> GetBuidingSurveyBySurveyorId(Guid accountId, PagingRequest model);

        Task<ApproveBuildingResponse> ApproveBuilding(int id);

        Task<ApproveBuildingResponse> RejectBuilding(int id);

        void HardDeleteBuilding();

        Task<CustomerSegmentResponse> PostCustomerSegment(PostCustomerSegmentRequest model);

        Task<CustomerSegmentResponse> PutCustomerSegment(int buildingId, int SegmentId, PutCustomerSegmentRequest model);

        Task<CustomerSegmentResponse> DeleteCustomerSegment(int buildingId, int SegmentId);

        Task<List<CustomerSegmentResponse>> GetCustomerSegmentByID(int buildingId);

        Task<BuildingDetailResponse> GetBuidingByStoreId(int storeId);
    }
}