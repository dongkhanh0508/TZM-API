using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TradeMap.Data.Entity;
using TradeMap.Data.UnitOfWork;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Exceptions;
using TradeMap.Service.Graph;
using TradeMap.Service.Helpers;
using TradeMap.Service.Servies.InterfaceService;
using static TradeMap.Service.Helpers.StatusEnum;

namespace TradeMap.Service.Servies.ImplService
{
    public class MapService : IMapService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MapService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CustomFeatureCollection> GetCampusForMap(string coordinateString)
        {
            try
            {
                Geometry geom = GeoJsonHelper.ParseStringToGeoMetry(coordinateString);
                geom.SRID = 4326;
                CustomFeatureCollection result = new CustomFeatureCollection();
                var campuses = await _unitOfWork.Repository<Campus>().GetAll().Where(x => geom.Contains(x.Geom)).ToListAsync();
                foreach (var item in campuses)
                {
                    Feature ft = new Feature
                    {
                        Geometry = item.Geom
                    };
                    ft.Properties.F1 = item.Id.ToString();
                    ft.Properties.F2 = item.Name;
                    result.Features.Add(ft);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get system zone Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<CustomFeatureCollection> GetBuildingForMap(string coordinateString, int role)
        {
            try
            {
                Geometry geom = GeoJsonHelper.ParseStringToGeoMetry(coordinateString);
                CustomFeatureCollection result = new CustomFeatureCollection();
                geom.SRID = 4326;
                dynamic buildings;
                // List<Building> buildings = null;
                if (role == 0 || role == 2)
                {
                    buildings = await _unitOfWork.Repository<Building>().GetAll().Where(x => geom.Contains(x.Geom) && x.Status != (int?)Status.Deleted && x.Status != (int?)Status.WaitingUpdate && x.Status != (int?)Status.Reject).Select(x => new { x.Name, Type = x.Type.Name, x.Status, x.Id, x.Geom }).AsNoTracking().ToListAsync();
                }
                else
                {
                    buildings = await _unitOfWork.Repository<Building>().GetAll().Where(x => geom.Contains(x.Geom) && (x.Status == (int?)Status.Surveyed || x.Status == (int?)Status.WaitingUpdate)).Select(x => new { x.Name, Type = x.Type.Name, x.Status, x.Id, x.Geom }).AsNoTracking().ToListAsync();
                }

                foreach (var item in buildings)
                {
                    Feature ft = new Feature
                    {
                        Geometry = item.Geom,
                    };
                    ft.Properties.F1 = item.Type;
                    ft.Properties.F2 = item.Name;
                    ft.Properties.F3 = item.Status;
                    ft.Properties.F4 = item.Id;
                    ft.Properties.F5 = item.Geom.Centroid.AsText();
                    result.Features.Add(ft);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get building Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<CustomFeatureCollection> GetBuildingOfSurveyorForMap(Guid surveyorId)
        {
            try
            {
                var systemZoneAcc = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.AccountId == surveyorId).ToListAsync();
                CustomFeatureCollection result = new CustomFeatureCollection();
                List<Building> buildings = new List<Building>();
                foreach (var item in systemZoneAcc)
                {
                    var buildingsTemp = await _unitOfWork.Repository<Building>().GetAll().Where(x => item.Geom.Intersects(x.Geom) && x.Status != (int?)Status.Deleted && x.Status != (int?)Status.WaitingUpdate && x.Status != (int?)Status.Reject).ToListAsync();
                    if (buildingsTemp.Count != 0)
                    {
                        buildings.AddRange(buildingsTemp);
                    }
                }
                foreach (var item in buildings)
                {
                    Feature ft = new Feature
                    {
                        Geometry = item.Geom,
                    };
                    ft.Properties.F1 = item.Type?.Name;
                    ft.Properties.F2 = item.Name;
                    ft.Properties.F3 = item.Status;
                    ft.Properties.F4 = item.Id;
                    ft.Properties.F5 = item.Geom.Centroid.AsText();
                    result.Features.Add(ft);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get building Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<CustomFeatureCollection> GetStoreOfSurveyorForMap(Guid surveyorId)
        {
            try
            {
                var systemZoneAcc = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.AccountId == surveyorId).ToListAsync();
                CustomFeatureCollection result = new CustomFeatureCollection();
                List<Store> stores = new List<Store>();
                foreach (var item in systemZoneAcc)
                {
                    var temp = await _unitOfWork.Repository<Store>().GetAll().Where(x => item.Geom.Intersects(x.Geom) && x.Status != (int?)Status.Deleted && x.Status != (int?)Status.WaitingUpdate && x.Status != (int?)Status.Reject).ToListAsync();
                    if (temp.Count != 0)
                    {
                        stores.AddRange(temp);
                    }
                }
                foreach (var item in stores)
                {
                    Feature ft = new Feature
                    {
                        Geometry = item.Geom,
                    };
                    ft.Properties.F1 = item.Brand?.Segment?.Name;
                    ft.Properties.F2 = item.Name;
                    ft.Properties.F3 = item.Status;
                    ft.Properties.F4 = item.Id;
                    result.Features.Add(ft);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get store Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<CustomFeatureCollection> GetDistrictForMap(string coordinateString)
        {
            try
            {
                Geometry geom = GeoJsonHelper.ParseStringToGeoMetry(coordinateString);
                CustomFeatureCollection result = new CustomFeatureCollection();
                geom.SRID = 4326;
                var district = await _unitOfWork.Repository<District>().GetAll().Where(x => geom.Contains(x.Geom)).ToListAsync();
                foreach (var item in district)
                {
                    Feature ft = new Feature
                    {
                        Geometry = item.Geom.Boundary
                    };
                    ft.Properties.F2 = item.Name;
                    result.Features.Add(ft);
                }

                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get district boundary Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<CustomFeatureCollection> GetStoreForMap(string coordinateString, int role)
        {
            try
            {
                Geometry geom = GeoJsonHelper.ParseStringToGeoMetry(coordinateString);
                CustomFeatureCollection result = new CustomFeatureCollection();
                geom.SRID = 4326;
                dynamic stores;
                if (role == 0 || role == 2)
                {
                    stores = await _unitOfWork.Repository<Store>().GetAll().Where(x => geom.Contains(x.Geom) && x.Status != (int?)Status.Deleted
                     && x.Status != (int?)Status.Reject
                     && x.Status != (int?)Status.WaitingUpdate && x.FloorAreaId == null).
                     Select(x => new { x.Geom, SegmentName = x.Brand.Segment.Name, x.Id, x.Status, x.Name }).AsNoTracking()
                     .ToListAsync();
                }
                else
                {
                    stores = await _unitOfWork.Repository<Store>().GetAll().Where(x => geom.Contains(x.Geom) && x.Status == (int?)Status.Surveyed || x.Status == (int?)Status.WaitingUpdate)
                        .Select(x => new { x.Geom, SegmentName = x.Brand.Segment.Name, x.Id, x.Status, x.Name }).AsNoTracking()
                        .ToListAsync();
                }

                foreach (var item in stores)
                {
                    Feature ft = new Feature
                    {
                        Geometry = item.Geom
                    };
                    ft.Properties.F1 = item.SegmentName;
                    ft.Properties.F2 = item.Name;
                    ft.Properties.F4 = item.Id;
                    ft.Properties.F3 = item.Status;
                    result.Features.Add(ft);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Store Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<CustomFeatureCollection> GetSystemZoneForMap(string coordinateString)
        {
            try
            {
                Geometry geom = GeoJsonHelper.ParseStringToGeoMetry(coordinateString);
                geom.SRID = 4326;
                CustomFeatureCollection result = new CustomFeatureCollection();
                var systemZones = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => geom.Contains(x.Geom)).ToListAsync();
                foreach (var item in systemZones)
                {
                    Feature ft = new Feature
                    {
                        Geometry = item.Geom
                    };
                    ft.Properties.F1 = item.Id.ToString();
                    ft.Properties.F2 = item.Name;

                    result.Features.Add(ft);
                }

                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get system zone Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<CustomFeatureCollection> GetWardBoundaryForMap(string coordinateString)
        {
            try
            {
                var result = new CustomFeatureCollection();
                Geometry geom = GeoJsonHelper.ParseStringToGeoMetry(coordinateString);

                geom.SRID = 4326;
                var wards = await _unitOfWork.Repository<Ward>().GetAll().Where(x => geom.Contains(x.Geom)).ToListAsync();

                foreach (var item in wards)
                {
                    Feature ft = new Feature
                    {
                        Geometry = item.Geom.Boundary
                    };
                    ft.Properties.F2 = item.Name;
                    result.Features.Add(ft);
                }

                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get boundary wards Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<CustomFeatureCollection> GetWardForMap(string coordinateString)
        {
            try
            {
                var result = new CustomFeatureCollection();
                Geometry geom = GeoJsonHelper.ParseStringToGeoMetry(coordinateString);

                geom.SRID = 4326;
                var wards = await _unitOfWork.Repository<Ward>().GetAll().Where(x => geom.Contains(x.Geom)).ToListAsync();

                foreach (var item in wards)
                {
                    Feature ft = new Feature
                    {
                        Geometry = item.Geom
                    };
                    ft.Properties.F3 = item.Id;
                    ft.Properties.F2 = item.Name;
                    result.Features.Add(ft);
                }

                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get boundary wards Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<WardWithDistrict> GetWardIdByLocation(string coordinateString)
        {
            Geometry geom = GeoJsonHelper.ParseStringToGeoMetry(coordinateString);
            geom.SRID = 4326;
            geom = geom.Buffer(-0.0000261);
            WardWithDistrict result = null;
            try
            {
                var ward = await _unitOfWork.Repository<Ward>().GetAll().Where(x => x.Geom.Contains(geom)).FirstOrDefaultAsync();
                if (ward != null && CheckSystemzone(geom))
                {
                    result = new WardWithDistrict()
                    {
                        Id = ward.Id,
                        Name = ward.Name,
                        DistrictName = ward.District?.Name
                    };
                }
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Ward Id by Location error", e.InnerException?.Message);
            }
        }

        private bool CheckSystemzone(Geometry geom)
        {
            bool isBoundary = true;
            var intersects = _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.Geom.Touches(geom)).AsEnumerable();
            isBoundary = intersects.Any();
            if (!isBoundary)
            {
                var temp = _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.Geom.Intersects(geom)).AsEnumerable();
                return !temp.Any();
            }
            else
            {
                return true;
            }
        }

        public async Task<bool> CheckStoreInSystemzoneAsync(Guid accountId, string coordinateString)
        {
            var systemzones = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.AccountId == accountId).ToListAsync();
            Geometry geom = GeoJsonHelper.ParseStringToPoint(coordinateString);
            var intersects = systemzones.Any(x => x.Geom.Intersects(geom));
            return intersects;
        }

        public bool CheckCampus(string coordinateString)
        {
            Geometry geom = GeoJsonHelper.ParseStringToGeoMetry(coordinateString);
            bool isValid = true;
            try
            {
                isValid = !_unitOfWork.Repository<Campus>().GetAll().Any(x => x.Geom.Intersects(geom));
                if (isValid)
                {
                    isValid = !_unitOfWork.Repository<Building>().GetAll().Any(x => x.Geom.Intersects(geom.Boundary));
                }
                return isValid;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Check New Campus error", e.InnerException?.Message);
            }
        }

        public async Task<CheckBuidlingResponse> CheckBuildingInCampus(string coordinateString, Guid id)
        {
            Geometry geom = GeoJsonHelper.ParseStringToGeoMetry(coordinateString);
            CheckBuidlingResponse result = new CheckBuidlingResponse();
            bool isValid = true;
            try
            {
                var systemzones = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.AccountId == id).ToListAsync();
                var isInSystemzone = systemzones.Any(x => x.Geom.Contains(geom));
                var isValidBuilding = !_unitOfWork.Repository<Building>().GetAll().Any(x => x.Geom.Intersects(geom));
                isValid = isValidBuilding && isInSystemzone;

                if (isValid)
                {
                    result.IsValid = true;
                    var campus = await _unitOfWork.Repository<Campus>().GetAll()
                        .Where(x => x.Geom.Contains(geom))
                        .Select(x => new CheckBuidlingResponse { Id = x.Id, Name = x.Name })
                        .FirstOrDefaultAsync();
                    if (campus != null)
                    {
                        result.Name = campus.Name;
                        result.Id = campus.Id;
                    }
                    else
                    {
                        var isIntersect = _unitOfWork.Repository<Campus>().GetAll()
                        .Any(x => x.Geom.Intersects(geom));
                        if (isIntersect)
                            return null;
                        else
                            result.Id = -1;
                    }
                }
                else return null;
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Check Building in Campus error", e.InnerException?.Message);
            }
        }

        public async Task<CustomFeatureCollection> GetStoreByBrandIdForMap(int brandId)
        {
            try
            {
                CustomFeatureCollection result = new CustomFeatureCollection();
                List<Store> stores = null;
                stores = await _unitOfWork.Repository<Store>().GetAll().Where(x => x.BrandId == brandId && (x.Status == (int?)Status.Surveyed
                 || x.Status == (int?)Status.WaitingUpdate)).ToListAsync();

                foreach (var item in stores)
                {
                    Feature ft = new Feature
                    {
                        Geometry = item.Geom
                    };
                    ft.Properties.F1 = item.Brand?.Segment?.Name;
                    ft.Properties.F2 = item.Name;
                    ft.Properties.F4 = item.Id;
                    result.Features.Add(ft);
                }
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Store Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<CustomFeatureCollection> GetSystemZoneForMapBySurveyorId(Guid surveyorId)
        {
            try
            {
                CustomFeatureCollection result = new CustomFeatureCollection();
                var systemZoneAcc = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.AccountId == surveyorId).ToListAsync();

                foreach (var item in systemZoneAcc)
                {
                    Feature ft = new Feature
                    {
                        Geometry = item.Geom
                    };
                    ft.Properties.F1 = item.Id.ToString();
                    ft.Properties.F2 = item.Name;
                    result.Features.Add(ft);
                }

                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get system zone Error!!!", e.InnerException?.Message);
            }
        }

        public static bool IsIntersectWard(List<Geometry> listWards)
        {
            bool result;

            var g = InitGraph(listWards);
            g.DeepFirstSearch();
            var wardId = g.Select(x => x.Label).Distinct().ToList();
            result = (wardId.Count == 1);
            return result;
        }

        public static bool IsDuplicateWardInGroupZone(List<Geometry> groupZones, List<Geometry> listZones)
        {
            foreach (var item in listZones)
            {
                if (groupZones.Any(x => x.Contains(item)))
                {
                    return false;
                }
            }
            return true;
        }

        public bool IsValidGroupZone(List<int> listZoneId, int brandId, int type)
        {
            bool result = false;
            var groupZones = _unitOfWork.Repository<GroupZone>().GetAll().Where(x => x.BrandId == brandId).Select(x => x.Geom).ToList();

            List<Geometry> geoms;
            if (type == (int)GroupZoneType.Ward)
            {
                geoms = _unitOfWork.Repository<Ward>().GetAll().Where(x => listZoneId.Any(c => c == x.Id)).Select(x => x.Geom).ToList();
            }
            else if (type == (int)GroupZoneType.District)
            {
                geoms = _unitOfWork.Repository<District>().GetAll().Where(x => listZoneId.Any(c => c == x.Id)).Select(x => x.Geom).ToList();
            }
            else
            {
                geoms = _unitOfWork.Repository<SystemZone>().GetAll().Where(x => listZoneId.Any(c => c == x.Id)).Select(x => x.Geom).ToList();
            }
            Geometry groupZoneGeom = (geoms.Any()) ? GeoJsonHelper.CombineGeoCollection(geoms) : null;

            if (IsDuplicateWardInGroupZone(groupZones, geoms) && IsIntersectWard(geoms))
            {
                result = true;
            }

            return result;
        }

        private static Service.Graph.Graph InitGraph(List<Geometry> wards)
        {
            Service.Graph.Graph g = new Service.Graph.Graph();
            try
            {
                int id = 0;
                foreach (var item in wards)
                {
                    g.AddVertext(new Vertex(item));
                    id++;
                }
                foreach (var item in g)
                {
                    var listEdge = wards.Where(x => x != item.Geom).ToList();
                    foreach (var edge in listEdge)
                    {
                        if (item.Geom.Intersects(edge))
                        {
                            g.AddEdge(item.Geom, edge);
                        }
                    }
                }
            }
            catch (Exception)
            {
                throw;
            }
            return g;
        }
    }
}