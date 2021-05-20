using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Servies.ImplService;

namespace TradeMap.Service.Servies.InterfaceService
{
    public interface IMapService
    {
        Task<CustomFeatureCollection> GetSystemZoneForMap(string coordinateString);

        Task<CustomFeatureCollection> GetSystemZoneForMapBySurveyorId(Guid surveyorId);

        Task<CustomFeatureCollection> GetBuildingForMap(string coordinateString, int role);

        Task<CustomFeatureCollection> GetCampusForMap(string coordinateString);

        Task<CustomFeatureCollection> GetDistrictForMap(string coordinateString);

        Task<CustomFeatureCollection> GetStoreForMap(string coordinateString, int role);

        Task<CustomFeatureCollection> GetStoreByBrandIdForMap(int brandId);

        Task<CustomFeatureCollection> GetWardForMap(string coordinateString);

        Task<WardWithDistrict> GetWardIdByLocation(string coordinateString);

        bool CheckCampus(string coordinateString);

        Task<CheckBuidlingResponse> CheckBuildingInCampus(string coordinateString, Guid id);

        Task<CustomFeatureCollection> GetBuildingOfSurveyorForMap(Guid surveyorId);

        Task<CustomFeatureCollection> GetStoreOfSurveyorForMap(Guid surveyorId);

        //  bool IsIntersectWard(List<T> listWards);

        //   bool IsDuplicateWardInGroupZone(List<Geometry> groupZones, List<T> listWards);

        bool IsValidGroupZone(List<int> listWardId, int brandId, int type);

        Task<CustomFeatureCollection> GetWardBoundaryForMap(string coordinateString);

        Task<bool> CheckStoreInSystemzoneAsync(Guid accountId, string coordinateString);
    }
}