using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.Servies.InterfaceService
{
    public interface IStoresService
    {
        Task<StoreResponse> GetStoreById(int id, Guid accountId, int role);

        Task<StoreResponse> PostStore(Guid accountId, PostStoresRequest model);

        Task<List<StoreBrandResponse>> GetStoreByBrandId(int brandId);

        Task<StoreResponse> PutStore(int id, Guid accountId, PostStoresRequest model);

        Task<StoreResponse> PutStoreForBrand(int id, Guid accountId, int brand, PutStoresForBrandRequest model);

        Task<StoreResponse> DeleteStore(int id, Guid accountId, int brandId, int role);

        Task<PagedResults<StoreResponse>> GetStoreNeedSurveyByAccountId(Guid accountId, PagingRequestGetSurvey model);

        Task<StoreResponse> ChangeFlagNeedSurvey(int id);

        Task<ApproveStoreResponse> ApproveStore(int id);

        Task<ApproveStoreResponse> RejectStore(int id);

        Task<List<string>> GetStoreTypes();

        Task<List<BuildingDetailResponse>> GetBuildingByStoreLocation(string coor, float radius);

        Task<StoreTimeSlotResponse> GetStoreTimeslot(int brandId);

        Task<StoreResponse> GetStoreOrder(int brandId, string coordinateString, DateTime timeOrder, int dateOrder);

       
    }
}