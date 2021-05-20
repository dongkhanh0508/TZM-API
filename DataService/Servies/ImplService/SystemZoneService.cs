using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TradeMap.Data.Entity;
using TradeMap.Data.UnitOfWork;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Exceptions;
using TradeMap.Service.Helpers;
using TradeMap.Service.Servies.InterfaceService;
using static TradeMap.Service.Helpers.StatusEnum;

namespace TradeMap.Service.Servies.ImplService
{
    public class SystemZoneService : ISystemZoneService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SystemZoneService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SystemZoneResponse> PostSystemZone(PostSystemZoneRequset model)
        {
            SystemZone systemZone = new SystemZone
            {
                Name = model.Name,
                Geom = model.Geom,
                CreateDate = DateTime.UtcNow.AddHours(7),
                WardId = model.WardId,
                WeightNumber = CalculateWeightNumberForNewSystemzone(model.Geom, model.WardId)
            };
            try
            {
                await _unitOfWork.Repository<SystemZone>().InsertAsync(systemZone);
                await _unitOfWork.CommitAsync();
                SystemZoneResponse systemZoneResponse = new SystemZoneResponse
                {
                    Id = systemZone.Id,
                    Name = systemZone.Name,
                    Geom = systemZone.Geom,
                    CreateDate = systemZone.CreateDate,
                    WardId = systemZone.WardId,
                    Weight = systemZone.WeightNumber
                };
                return systemZoneResponse;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<AssignSystemZoneRespones> PostAssignSystemZone(Guid accountId, int systemZoneId)
        {
            var assignSystemzone = await _unitOfWork.Repository<SystemZone>().GetById(systemZoneId);
            assignSystemzone.AccountId = accountId;
            var fcm = await _unitOfWork.Repository<Account>().GetByIdGuid(accountId);
            string FcmToken = fcm.FcmToken;
            try
            {
                await _unitOfWork.Repository<SystemZone>().Update(assignSystemzone, assignSystemzone.Id);
                await _unitOfWork.CommitAsync();
                await FirebaseHelper.SendNotificationAsync(FcmToken, "You'll be surveying new system zone");

                return new AssignSystemZoneRespones
                {
                    AccountId = accountId,
                    SystemZoneId = systemZoneId
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Assign Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<SystemZoneResponse> PutSystemZone(int id, PutSystemZone model)
        {
            var systemZone = await _unitOfWork.Repository<SystemZone>().GetById(id);
            if (systemZone != null)
            {
                systemZone.Name = model.Name;
                systemZone.ModifyDate = DateTime.UtcNow.AddHours(7);
                try
                {
                    await _unitOfWork.Repository<SystemZone>().Update(systemZone, id);
                    await _unitOfWork.CommitAsync();
                    return new SystemZoneResponse
                    {
                        Id = systemZone.Id,
                        Name = systemZone.Name,
                        Geom = systemZone.Geom,
                        CreateDate = systemZone.CreateDate,
                        WardId = systemZone.WardId,
                    };
                }
                catch (Exception)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Update Error!!!", null);
                }
            }
            else
            {
                throw new CrudException(HttpStatusCode.NotFound, "Id not founded!!!", "");
            }
        }

        public async Task<SystemZoneResponse> DeleteSystemZone(int id)
        {
            var systemZone = await _unitOfWork.Repository<SystemZone>().GetById(id);
            if (systemZone != null)
            {
                try
                {
                    _unitOfWork.Repository<SystemZone>().Delete(systemZone);
                    await _unitOfWork.CommitAsync();
                    return new SystemZoneResponse
                    {
                        Id = systemZone.Id,
                        Name = systemZone.Name,
                        Geom = systemZone.Geom,
                        CreateDate = systemZone.CreateDate,
                        WardId = systemZone.WardId,
                    };
                }
                catch (Exception)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Delete Error!!!", null);
                }
            }
            else
            {
                throw new CrudException(HttpStatusCode.NotFound, "Id not founded!!!", "");
            }
        }

        public async Task<AssignSystemZoneRespones> DeleteAssignSystemZone(Guid accountId, int systemZoneId)
        {
            var rs = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.Id == systemZoneId).SingleOrDefaultAsync();
            if (rs != null)
            {
                try
                {
                    rs.AccountId = null;
                    await _unitOfWork.Repository<SystemZone>().Update(rs, rs.Id);
                    await _unitOfWork.CommitAsync();
                    return new AssignSystemZoneRespones
                    {
                        AccountId = rs.AccountId,
                        SystemZoneId = rs.Id,
                    };
                }
                catch (Exception e)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Delete Error!!!", e.InnerException?.Message);
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<SystemZoneResponse> GetSystemZoneById(int id)
        {
            try
            {
                var systemzone = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.Id == id).Select(x => new SystemZoneResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Geom = x.Geom,
                    CreateDate = x.CreateDate,
                    WardId = x.WardId,
                    Weight = x.WeightNumber
                }).SingleOrDefaultAsync();
                return systemzone;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Systemzone by id Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<Geometry> CheckSystemZoneClose(int wardId)
        {
            var systemzones = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.WardId == wardId).Select(x => x.Geom).ToListAsync();
            var reuslt = GeoJsonHelper.CombineGeoCollection(systemzones);
            return reuslt;
        }

        public async Task<bool> CheckSystemzoneFillWard(int wardId)
        {
            var systemzones = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.WardId == wardId).Select(x => x.Geom).ToListAsync();
            var wardGeom = await _unitOfWork.Repository<Ward>().GetAll().Where(x => x.Id == wardId).Select(x => x.Geom).SingleOrDefaultAsync();
            var systemzonesGeom = GeoJsonHelper.CombineGeoCollection(systemzones);
            double percent = systemzonesGeom.Area / wardGeom.Area;
            if (percent > 0.98 && percent < 1.02)
            {
                return true;
            }
            return false;
        }

        private double CalculateWeightNumberForNewSystemzone(Geometry geometry, int wardId)
        {
            var weightBoundary = _unitOfWork.Repository<Config>().GetAll().Where(x => x.Name.Equals("WeightBoundary") && x.Active == true).Select(x => x.Value).SingleOrDefault();

            double weightNumber = 0;
            var districtPopulationDensity = _unitOfWork.Repository<Ward>().GetAll().Where(x => x.Id == wardId).SingleOrDefault().District.PopulationDensity;
            var systemzonePopoulation = (geometry.Area * 12096.719247500967) * districtPopulationDensity;
            int temp = (int)((systemzonePopoulation / weightBoundary) + 1);
            weightNumber += temp;
            var buildingsInSystemzone = _unitOfWork.Repository<Building>().GetAll().Where(x => geometry.Intersects(x.Geom) &&
            x.Status != (int?)Status.Deleted &&
            x.Status != (int?)Status.WaitingUpdate &&
            x.Status != (int?)Status.Reject).ToList();
            weightNumber += buildingsInSystemzone.Where(x => (x.NumberOfFloor >= 0 && x.NumberOfFloor <= 5) || x.NumberOfFloor == null).Count() * 1
                + buildingsInSystemzone.Where(x => x.NumberOfFloor > 5 && x.NumberOfFloor <= 10).Count() * 2
                + buildingsInSystemzone.Where(x => x.NumberOfFloor > 10).Count() * 3;
            return weightNumber;
        }

        public async Task<PagedResults<SystemZoneResponse>> GetSystemZone(SystemZonePagingRequest request, Guid accountId)
        {
            List<SystemZoneResponse> list = null;
            var systemZoneAcc = await _unitOfWork.Repository<Account>().GetAll().Where(x => x.Id == accountId).Include(x => x.SystemZones).SingleOrDefaultAsync();

            try
            {
                List<SystemZoneResponse> systemZone = null;
                if (request.IsMe)
                {
                    systemZone = systemZoneAcc.SystemZones.Select(x => new SystemZoneResponse
                    {
                        Id = x.Id,
                        CreateDate = x.CreateDate,
                        Geom = x.Geom,
                        ModifyDate = x.ModifyDate,
                        Name = x.Name,
                        IsMySystemZone = true,
                        WardId = x.WardId,
                    }).ToList();
                }
                else
                {
                    if (request.DistrictId != 0)
                    {
                        var district = await _unitOfWork.Repository<District>().GetAll().Where(x => x.Id == request.DistrictId).FirstOrDefaultAsync();
                        systemZone = await _unitOfWork.Repository<SystemZone>()
                                .GetAll()
                                    .Where(x => x.Name.ToLower()
                                .Contains(request.KeySearch.ToLower())
                                && district.Geom.Intersects(x.Geom)).Select(x => new SystemZoneResponse
                                {
                                    Id = x.Id,
                                    CreateDate = x.CreateDate,
                                    Geom = x.Geom,
                                    ModifyDate = x.ModifyDate,
                                    Name = x.Name,
                                    IsMySystemZone = IsMySystemZone(systemZoneAcc, x.Id)
                                  ,
                                    WardId = x.WardId
                                }).ToListAsync();
                    }
                    else
                    {
                        systemZone = await _unitOfWork.Repository<SystemZone>()
                                .GetAll()
                                    .Where(x => x.Name.ToLower()
                                .Contains(request.KeySearch.ToLower())
                                ).Select(x => new SystemZoneResponse
                                {
                                    Id = x.Id,
                                    CreateDate = x.CreateDate,
                                    Geom = x.Geom,
                                    ModifyDate = x.ModifyDate,
                                    Name = x.Name,
                                    IsMySystemZone = IsMySystemZone(systemZoneAcc, x.Id),
                                    WardId = x.WardId
                                }).ToListAsync();
                    }
                }

                list = PageHelper<SystemZoneResponse>.Sorting(request.SortType, systemZone.AsEnumerable(), request.ColName);
                var result = PageHelper<SystemZoneResponse>.Paging(list, request.Page, request.PageSize);
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get System-zone Error!!!", e.InnerException?.Message);
            }
        }

        private static bool IsMySystemZone(Account sysZoneAcc, int id)
        {
            return sysZoneAcc.SystemZones.AsQueryable().Any(h => h.Id == id);
        }

        public async Task<List<StoreBuildingResponse>> GetStoreBuildingBySystemZoneId(GetStoreBuildingBySystemZoneIdRequest request)
        {
            List<StoreBuildingResponse> results = new List<StoreBuildingResponse>();
            List<Building> buildings = new List<Building>();
            List<Store> stores = new List<Store>();
            var systemZone = await _unitOfWork.Repository<SystemZone>().GetById(request.SystemZoneId);
            if (systemZone == null)
            {
                return null;
            }

            if (request.Status == 0)
            {
                buildings = await _unitOfWork.Repository<Building>().GetAll().Where(x => systemZone.Geom.Intersects(x.Geom) && x.Status != (int?)Status.Deleted && x.Status != (int?)Status.WaitingUpdate && x.Status != (int?)Status.Reject).ToListAsync();
                stores = await _unitOfWork.Repository<Store>().GetAll().Where(x => systemZone.Geom.Intersects(x.Geom) && x.Status != (int?)Status.Deleted && x.Status != (int?)Status.WaitingUpdate && x.Status != (int?)Status.Reject).ToListAsync();
            }
            else
            {
                buildings = await _unitOfWork.Repository<Building>().GetAll().Where(x => systemZone.Geom.Intersects(x.Geom) && x.Status == request.Status).ToListAsync();
                stores = await _unitOfWork.Repository<Store>().GetAll().Where(x => systemZone.Geom.Intersects(x.Geom) && x.Status == request.Status).ToListAsync();
            }

            if (request.Type == 1)
            {
                results.AddRange(buildings.Select(x => new StoreBuildingResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = "Building",
                    Geom = x.Geom,
                    Status = x.Status
                }).ToList());
            }
            else if (request.Type == 2)
            {
                results.AddRange(stores.Select(x => new StoreBuildingResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = "Store",
                    Geom = x.Geom,
                    Status = x.Status
                }).ToList());
            }
            else
            {
                results.AddRange(buildings.Select(x => new StoreBuildingResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = "Building",
                    Geom = x.Geom,
                    Status = x.Status
                }).ToList());
                results.AddRange(stores.Select(x => new StoreBuildingResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Type = "Store",
                    Geom = x.Geom,
                    Status = x.Status
                }).ToList());
            }
            return results;
        }

        public async Task<PagedResults<BuildingResponse>> GetBuidingBySystemzoneId(PagingRequestGetSurvey model, int Id, Guid curentAccountId)
        {
            List<BuildingResponse> list = null;
            try
            {
                List<BuildingResponse> buildings = new List<BuildingResponse>();
                List<BuildingResponse> searchResult = new List<BuildingResponse>();
                bool isEdit = false;
                var systemzoneGeom = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.Id == Id).Select(x => new { x.Geom, x.Id }).AsNoTracking().SingleOrDefaultAsync();
                var systemzones = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.AccountId == curentAccountId).AsNoTracking().ToListAsync();
                isEdit = systemzones.Any(x => x.Id == systemzoneGeom.Id);
                if (model.Status == Status.All)
                {
                    searchResult = await _unitOfWork.Repository<Building>()
                  .GetAll()
                      .Where(x => x.Name.ToLower()
                  .Contains(model.KeySearch.ToLower())
                  && systemzoneGeom.Geom.Intersects(x.Geom) && (x.Status != (int?)Status.Deleted && x.Status != (int?)Status.Reject && x.Status != (int?)Status.WaitingUpdate))
                      .Select(x => new BuildingResponse
                      {
                          Id = x.Id,
                          IsEditable = isEdit && (x.Status == (int?)Status.Surveyed || x.Status == (int?)Status.NeedSurvey),
                          Name = x.Name,
                          Geom = x.Geom,
                          NumberOfFloor = x.NumberOfFloor,
                          Active = x.Active,
                          CampusId = x.CampusId,
                          CreateDate = x.CreateDate,
                          ImageUrl = x.ImageUrl,
                          Type = x.Type.Name,
                          Address = x.Address,
                          Status = x.Status,
                          SystemZoneId = Id,
                          Floors = new List<FloorResopnse>(x.Floors.Select(f => new FloorResopnse
                          {
                              Id = f.Id,
                              Name = f.Name,
                              BuildingId = f.BuildingId,
                              FloorNumber = f.FloorNumber,
                              FloorAreas = new List<FloorAreaResopnse>(f.FloorAreas.Select(a => new FloorAreaResopnse
                              {
                                  Id = a.Id,
                                  FloorId = a.FloorId,
                                  Name = a.Name,
                                  Stores = new List<StoreResponse>(a.Stores.Select(s => new StoreResponse
                                  {
                                      Name = s.Name,
                                      Address = s.Address,
                                      FloorAreaName = s.FloorArea != null ? s.FloorArea.Name : null,
                                      BrandName = s.Brand != null ? s.Brand.Name : null,
                                      CreateDate = s.CreateDate,
                                      Geom = s.Geom,
                                      Id = s.Id,
                                      FloorAreaId = s.FloorAreaId,
                                      BrandId = s.BrandId,
                                      ImageUrl = s.ImageUrl,
                                      Type = s.Brand.Segment.Name,
                                      Status = s.Status,
                                      ReferenceId = s.ReferenceId
                                  }))
                              }))
                          }))
                      })
                      .ToListAsync();
                }
                else
                {
                    searchResult = await _unitOfWork.Repository<Building>()
                    .GetAll()
                        .Where(x => x.Name.ToLower()
                    .Contains(model.KeySearch.ToLower())
                    && systemzoneGeom.Geom.Intersects(x.Geom) && x.Status == (int)model.Status && (x.Status != (int?)Status.Deleted && x.Status != (int?)Status.Reject && x.Status != (int?)Status.WaitingUpdate))
                        .Select(x => new BuildingResponse
                        {
                            IsEditable = isEdit && (x.Status == (int?)Status.Surveyed || x.Status == (int?)Status.NeedSurvey),
                            Id = x.Id,
                            Name = x.Name,
                            Geom = x.Geom,
                            NumberOfFloor = x.NumberOfFloor,
                            Active = x.Active,
                            CampusId = x.CampusId,
                            CreateDate = x.CreateDate,
                            ImageUrl = x.ImageUrl,
                            Type = x.Type.Name,
                            Address = x.Address,
                            Status = x.Status,
                            SystemZoneId = Id,

                            Floors = new List<FloorResopnse>(x.Floors.Select(f => new FloorResopnse
                            {
                                Id = f.Id,
                                Name = f.Name,
                                BuildingId = f.BuildingId,
                                FloorNumber = f.FloorNumber,
                                FloorAreas = new List<FloorAreaResopnse>(f.FloorAreas.Select(a => new FloorAreaResopnse
                                {
                                    Id = a.Id,
                                    FloorId = a.FloorId,
                                    Name = a.Name,
                                    Stores = new List<StoreResponse>(a.Stores.Select(s => new StoreResponse
                                    {
                                        Name = s.Name,
                                        Address = s.Address,
                                        FloorAreaName = s.FloorArea != null ? s.FloorArea.Name : null,
                                        BrandName = s.Brand != null ? s.Brand.Name : null,
                                        CreateDate = s.CreateDate,
                                        Geom = s.Geom,
                                        Id = s.Id,
                                        FloorAreaId = s.FloorAreaId,
                                        BrandId = s.BrandId,
                                        ImageUrl = s.ImageUrl,
                                        Type = s.Brand.Segment.Name,
                                        Status = s.Status,
                                        ReferenceId = s.ReferenceId
                                    }))
                                }))
                            }))
                        })
                        .ToListAsync();
                }
                if (searchResult.Any())
                {
                    buildings.AddRange(searchResult);
                }

                list = PageHelper<BuildingResponse>.Sorting(model.SortType, buildings.AsEnumerable(), model.ColName);
                var result = PageHelper<BuildingResponse>.Paging(list, model.Page, model.PageSize);
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Building Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<PagedResults<StoreResponse>> GeStoreBySystemzoneId(PagingRequestGetSurvey model, int Id, Guid curentAccountId)
        {
            List<StoreResponse> list = null;
            try
            {
                bool isEdit = false;
                var systemzoneGeom = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.Id == Id).Select(x => new { x.Geom, x.Id }).AsNoTracking().SingleOrDefaultAsync();
                var systemzones = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.AccountId == curentAccountId).AsNoTracking().ToListAsync();
                isEdit = systemzones.Any(x => x.Id == systemzoneGeom.Id);
                List<StoreResponse> stores = new List<StoreResponse>();
                List<StoreResponse> searchResult = new List<StoreResponse>();
                if (model.Status == Status.All)
                {
                    searchResult = await _unitOfWork.Repository<Store>()
                   .GetAll()
                       .Where(x => x.Name.ToLower()
                   .Contains(model.KeySearch.ToLower())
                   && systemzoneGeom.Geom.Intersects(x.Geom) && (x.Status != (int?)Status.Deleted && x.Status != (int?)Status.Reject && x.Status != (int?)Status.WaitingUpdate))
                       .Select(x => new StoreResponse
                       {
                           IsEditable = isEdit && (x.Status == (int?)Status.Surveyed || x.Status == (int?)Status.NeedSurvey),
                           Id = x.Id,
                           Address = x.Address,
                           FloorAreaId = x.FloorAreaId,
                           FloorAreaName = x.FloorArea != null ? x.FloorArea.Name : null,
                           Name = x.Name,
                           BrandId = x.BrandId,
                           BrandName = x.Brand != null ? x.Brand.Name : null,
                           CreateDate = x.CreateDate,
                           Geom = x.Geom,
                           ImageUrl = x.ImageUrl,
                           Status = x.Status,
                           Type = x.Brand.Segment.Name,
                           AbilityToServe = x.AbilityToServe,
                           SystemZoneId = Id,
                           TimeSlot = x.TimeSlot
                       })
                       .ToListAsync();
                }
                else
                {
                    searchResult = await _unitOfWork.Repository<Store>()
                    .GetAll()
                        .Where(x => x.Name.ToLower()
                    .Contains(model.KeySearch.ToLower())
                    && systemzoneGeom.Geom.Intersects(x.Geom) && x.Status == (int)model.Status && (x.Status != (int?)Status.Deleted && x.Status != (int?)Status.Reject && x.Status != (int?)Status.WaitingUpdate))
                        .Select(x => new StoreResponse
                        {
                            IsEditable = isEdit && (x.Status == (int?)Status.Surveyed || x.Status == (int?)Status.NeedSurvey),
                            Id = x.Id,
                            Address = x.Address,
                            FloorAreaId = x.FloorAreaId,
                            FloorAreaName = x.FloorArea != null ? x.FloorArea.Name : null,
                            Name = x.Name,
                            BrandId = x.BrandId,
                            BrandName = x.Brand != null ? x.Brand.Name : null,
                            CreateDate = x.CreateDate,
                            Geom = x.Geom,
                            ImageUrl = x.ImageUrl,
                            Status = x.Status,
                            Type = x.Brand.Segment.Name,
                            AbilityToServe = x.AbilityToServe,
                            SystemZoneId = Id,
                            TimeSlot = x.TimeSlot
                        })
                        .ToListAsync();
                }

                if (searchResult.Any())
                {
                    stores.AddRange(searchResult);
                }

                list = PageHelper<StoreResponse>.Sorting(model.SortType, stores.AsEnumerable(), model.ColName);
                var result = PageHelper<StoreResponse>.Paging(list, model.Page, model.PageSize);
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Store Error!!!", e.InnerException?.Message);
            }
        }
    }
}