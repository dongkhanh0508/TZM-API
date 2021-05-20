using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TradeMap.Data.Context;
using TradeMap.Data.Entity;
using TradeMap.Data.UnitOfWork;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Exceptions;
using TradeMap.Service.Helpers;
using TradeMap.Service.InterfaceService;
using static TradeMap.Service.Helpers.StatusEnum;

namespace TradeMap.Service.ImplService
{
    public class BuildingService : IBuildingService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BuildingService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public static void TestCronJobAsync()
        {
            TestCronJob test = new TestCronJob()
            {
                Message = "Test Cronjon " + DateTime.UtcNow.AddHours(7).ToString()
            };
            TradeZoneMapContext context = new TradeZoneMapContext();
            context.TestCronJobs.Add(test);
            context.SaveChanges();
        }

        public async Task<BuildingResponse> ChangeFlagNeedSurvey(int id)
        {
            var rs = await _unitOfWork.Repository<Building>().GetAll().Where(x => x.Id == id).SingleOrDefaultAsync();
            if (rs == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Id Not Found!!!", null);
            }

            try
            {
                var accountId = _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.Geom.Intersects(rs.Geom)).SingleOrDefault();
                if (!(accountId.Account is null))
                {
                    await FirebaseHelper.SendNotificationAsync(accountId.Account.FcmToken, "You'll be surveying new building");
                }
                else
                {
                    throw new CrudException(HttpStatusCode.MethodNotAllowed, "You must assign Systemzone before!!!", null);
                }
                if (rs.Status == null)
                {
                    rs.Status = (int)Status.NeedSurvey;
                }
                else if (rs.Status == 1)
                {
                    rs.Status = (int)Status.NeedSurvey;
                }
                else if (rs.Status == 2)
                {
                    rs.Status = (int)Status.Surveyed;
                }
                else
                {
                    throw new CrudException(HttpStatusCode.MethodNotAllowed, "You must Approved to continue !!!", null);
                }

                await _unitOfWork.Repository<Building>().Update(rs, rs.Id);
                await _unitOfWork.CommitAsync();
                return new BuildingResponse
                {
                    Id = rs.Id,
                    Name = rs.Name,
                    Geom = rs.Geom,
                    NumberOfFloor = rs.NumberOfFloor,
                    Active = rs.Active,
                    CampusId = rs.CampusId,
                    CreateDate = rs.CreateDate,
                    ImageUrl = rs.ImageUrl,
                    Type = rs.Type?.Name,
                    Address = rs.Address,
                    Status = rs.Status,
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, e.Message, e.InnerException?.Message);
            }
        }

        public async Task<ApproveBuildingResponse> ApproveBuilding(int id)
        {
            var rs = await _unitOfWork.Repository<Building>().GetAll().Where(x => x.Id == id && x.Status == (int?)Status.NeedApproval).SingleOrDefaultAsync();
            if (rs == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Id Not Found!!!", null);
            }

            try
            {
                int? action = rs.Histories.OrderByDescending(o => o.CreateDate).FirstOrDefault().Action;
                if (action == (int)ActionEnum.ActionSurvey.DeleteBuilding)
                {
                    rs.Status = (int)Status.Deleted;
                }
                else
                {
                    rs.Status = (int)Status.Surveyed;
                }

                if (rs.ReferenceId != null)
                {
                    var floors = rs.Floors.Where(x => x.ReferenceId != null).ToList();
                    foreach (var item in floors)
                    {
                        foreach (var e in item.FloorAreas)
                        {
                            var stores = await _unitOfWork.Repository<Store>().GetAll().Where(x => x.FloorAreaId == e.ReferenceId && x.FloorAreaId != null).ToListAsync();
                            stores.ForEach(t => t.FloorAreaId = e.Id);
                            _unitOfWork.Repository<Store>().UpdateRange(stores.AsQueryable());
                        }
                    }
                    rs.Reference.Status = (int)Status.Deleted;
                }

                await _unitOfWork.Repository<Building>().Update(rs, rs.Id);
                await _unitOfWork.CommitAsync();
                return new ApproveBuildingResponse
                {
                    Id = rs.Id,
                    Name = rs.Name,
                    Geom = rs.Geom,
                    NumberOfFloor = rs.NumberOfFloor,
                    Active = rs.Active,
                    CampusId = rs.CampusId,
                    CreateDate = rs.CreateDate,
                    ImageUrl = rs.ImageUrl,
                    Type = rs.Type.Name,
                    Address = rs.Address,
                    Status = rs.Status,
                    Action = action
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Approve Building Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<BuildingResponse> DeleteBuilding(int id, Guid accountId)
        {
            var building = await _unitOfWork.Repository<Building>().GetById(id);
            if (building == null)
            {
                return null;
            }

            try
            {
                await _unitOfWork.Repository<History>().InsertAsync(new History
                {
                    AccountId = accountId,
                    BuildingId = building.Id,
                    Action = (int)ActionEnum.ActionSurvey.DeleteBuilding,
                    CreateDate = DateTime.UtcNow.AddHours(7),
                });
                building.Status = (int?)Status.NeedApproval;
                await _unitOfWork.CommitAsync();
                return new BuildingResponse
                {
                    Id = building.Id,
                    Name = building.Name,
                    Status = building.Status,
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Delete Building Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<BuildingDetailResponse> GetBuidingById(int id, Guid accountId)
        {
            var history = await _unitOfWork.Repository<History>().GetAll().Where(x => x.BuildingId == id).OrderByDescending(o => o.CreateDate).FirstOrDefaultAsync();
            HistoryResponse historyResponse = null;
            if (history != null)
            {
                historyResponse = new HistoryResponse()
                {
                    Id = history.Id,
                    AccountId = history.AccountId,
                    Action = history.Action,
                    CreateDate = history.CreateDate,
                    BuildingId = history.BuildingId,
                    StoreId = history.StoreId,
                    AccountName = history.Account.Fullname,
                    ReferenceName = history.Building != null ? history.Building.Name : history.Store.Name,
                    Role = history.Account.Role,
                    Geom = history.Building.Geom?.AsText()
                };
            }

            var rs = _unitOfWork.Repository<Building>().GetAll().Where(f => f.Id == id).Select(x => new BuildingDetailResponse
            {
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
                ReferenceId = x.ReferenceId,
                BuildingStreetSegments = new List<BuidingStreetSegmentResponse>(x.BuildingStreetSegments.Select(b => new BuidingStreetSegmentResponse { BuildingId = b.BuidingId, StreetSegmentId = b.StreetSegmentId })),
                History = x.Status == 3 ? historyResponse : null,
                ReferencrName = x.ReferenceId != null ? x.Reference.Name : null,
                Floors = new List<FloorResopnse>(x.Floors.OrderBy(r => r.FloorNumber).Select(f => new FloorResopnse
                {
                    Id = f.Id,
                    Name = f.Name,
                    BuildingId = f.BuildingId,
                    FloorNumber = f.FloorNumber,
                    ReferenceId = f.ReferenceId
                   ,
                    FloorAreas = new List<FloorAreaResopnse>(f.FloorAreas.Select(a => new FloorAreaResopnse
                    {
                        Id = a.Id,
                        FloorId = a.FloorId,
                        Name = a.Name,
                        ReferenceId = a.ReferenceId,
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
                            ReferenceId = s.ReferenceId,
                            TimeSlot = s.TimeSlot,
                            AbilityToServe = s.AbilityToServe,
                        }))
                    }))
                }))
            }).SingleOrDefault();
            var systemzones = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.AccountId == accountId && x.Geom.Intersects(rs.Geom)).Select(x => x.Id).ToListAsync();
            rs.IsEditable = systemzones.Any() && (rs.Status == (int?)Status.Surveyed || rs.Status == (int?)Status.NeedSurvey);
            if (rs.Geom != null)
            {
                rs.SystemzoneId = await GetSystemzoneIdByBuildingIntersect(rs.Geom);
            }
            var CustomerSegment = await _unitOfWork.Repository<CustomerSegment>().GetAll().Where(x => x.BuildingId == id)
                .Select(a => new CustomerSegmentResponse
                {
                    BuildingId = a.BuildingId,
                    BuildingName = a.Building.Name,
                    SegmentId = a.SegmentId,
                    SegmentName = a.Segment.Name,
                    PotentialCustomers = a.PotentialCustomers,
                    PrimaryAge = a.PrimaryAge,
                    TimeSlot = a.TimeSlot
                }).ToListAsync();
            rs.CustomerSegmentResponses = CustomerSegment;
            if (history != null)
            {
                if (history.Action == 2 && rs.Status == (int)Status.NeedApproval)
                {
                    foreach (var item in rs.Floors)
                    {
                        foreach (var e in item.FloorAreas)
                        {
                            var stores = await _unitOfWork.Repository<Store>().GetAll().Where(x => x.FloorAreaId == e.ReferenceId && x.FloorAreaId != null).Select(s => new StoreResponse
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
                                ReferenceId = s.ReferenceId,
                                TimeSlot = s.TimeSlot,
                                AbilityToServe = s.AbilityToServe,
                            }).ToListAsync();
                            rs.Floors.Where(p => p.Id == item.Id).FirstOrDefault().FloorAreas.Where(t => t.Id == e.Id).FirstOrDefault().Stores.AddRange(stores);
                        }
                    }
                }
            }
            return rs;
        }

        public async Task<BuildingDetailResponse> GetBuidingByStoreId(int storeId)
        {
            try
            {
                var store = await _unitOfWork.Repository<Store>().GetById(storeId);
                if (store == null)
                {
                    return null;
                }
                else
                {
                    var floorArea = await _unitOfWork.Repository<FloorArea>().GetById((int)store.FloorAreaId);
                    var floor = await _unitOfWork.Repository<Floor>().GetById((int)floorArea.FloorId);
                    return await _unitOfWork.Repository<Building>().GetAll().Where(f => f.Id == floor.BuildingId).Select(x => new BuildingDetailResponse
                    {
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
                        ReferenceId = x.ReferenceId,
                    }).SingleOrDefaultAsync();
                }
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Building by store id Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<PagedResults<BuildingResponse>> GetBuidingNeedSurveyByAccountId(Guid accountId, PagingRequestGetSurvey model)
        {
            List<BuildingResponse> list = null;
            try
            {
                var systemZone = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.AccountId == accountId).ToListAsync();
                List<BuildingResponse> buildings = new List<BuildingResponse>();
                foreach (var item in systemZone)
                {
                    var searchResult = await _unitOfWork.Repository<Building>()
                          .GetAll()
                              .Where(x => x.Name.ToLower()
                          .Contains(model.KeySearch.ToLower())
                          && item.Geom.Intersects(x.Geom) && x.Status == (int)model.Status && x.Status != (int?)Status.Deleted)
                              .Select(x => new BuildingResponse
                              {
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
                                  SystemZoneId = item.Id,
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

                    if (searchResult.Count != 0)
                    {
                        buildings.AddRange(searchResult);
                    }
                }
                for (int i = buildings.Count - 1; i > -1; i--)
                {
                    var removeSystemzoneId = await GetSystemzoneIdByBuildingIntersect(buildings.ElementAt(i).Geom);
                    var buildingToRemove = systemZone.Where(x => x.Id == removeSystemzoneId).SingleOrDefault();
                    if (buildingToRemove == null)
                    {
                        buildings.RemoveAt(i);
                    }
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

        public async Task<BuildingResponse> InsertBuilding(PostBuildingRequest model, Guid accountId)
        {
            var myGeom = GeoJsonHelper.ParseStringToGeoMetry(model.CoordinateString);
            Building building = new Building
            {
                Name = model.Name,
                Geom = myGeom,
                Active = model.Active,
                CreateDate = DateTime.UtcNow.AddHours(7),
                ImageUrl = model.ImageUrl,
                NumberOfFloor = model.NumberOfFloor,
                TypeId = model.Type,
                Status = (int?)Status.NeedApproval,
                Address = model.Address,
            };
            if (model.CampusId > 0)
            {
                building.CampusId = model.CampusId;
            }

            try
            {
                await _unitOfWork.Repository<Building>().InsertAsync(building);
                await _unitOfWork.CommitAsync();
                List<BuidingStreetSegmentResponse> buildingStreetSegments = new List<BuidingStreetSegmentResponse>();
                if (model.StreetSegmentIds != null)
                {
                    foreach (var item in model.StreetSegmentIds)
                    {
                        BuildingStreetSegment buildingStreetSegment = new BuildingStreetSegment { BuidingId = building.Id, StreetSegmentId = item };
                        await _unitOfWork.Repository<BuildingStreetSegment>().InsertAsync(buildingStreetSegment);
                        await _unitOfWork.CommitAsync();
                        buildingStreetSegments.Add(new BuidingStreetSegmentResponse
                        {
                            BuildingId = buildingStreetSegment.BuidingId,
                            StreetSegmentId = buildingStreetSegment.StreetSegmentId
                        });
                    }
                }
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Building Error!!!", e.InnerException?.Message);
            }
            List<FloorResopnse> floors = new List<FloorResopnse>();
            foreach (var floor in model.Floors)
            {
                Floor floorModel = new Floor
                {
                    FloorNumber = floor.FloorNumber,
                    Name = floor.Name,
                    BuildingId = building.Id,
                };
                try
                {
                    await _unitOfWork.Repository<Floor>().InsertAsync(floorModel);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception e)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Insert Floor Error!!!", e.InnerException?.Message);
                }
                List<FloorAreaResopnse> areas = new List<FloorAreaResopnse>();
                foreach (var areaItem in floor.FloorAreas)
                {
                    FloorArea area = new FloorArea
                    {
                        Name = areaItem.Name,
                        FloorId = floorModel.Id,
                    };
                    try
                    {
                        await _unitOfWork.Repository<FloorArea>().InsertAsync(area);
                        await _unitOfWork.CommitAsync();
                        areas.Add(new FloorAreaResopnse
                        {
                            Id = area.Id,
                            FloorId = area.FloorId,
                            Name = area.Name,
                        });
                    }
                    catch (Exception e)
                    {
                        throw new CrudException(HttpStatusCode.BadRequest, "Insert Area Error!!!", e.InnerException?.Message);
                    }
                }
                floors.Add(new FloorResopnse
                {
                    Id = floorModel.Id,
                    Name = floorModel.Name,
                    BuildingId = floorModel.BuildingId,
                    FloorNumber = floorModel.FloorNumber,
                    FloorAreas = areas
                });
            }
            try
            {
                await _unitOfWork.Repository<History>().InsertAsync(new History
                {
                    AccountId = accountId,
                    Action = (int)ActionEnum.ActionSurvey.InsertBuilding,
                    CreateDate = DateTime.UtcNow.AddHours(7),
                    BuildingId = building.Id,
                });
                await _unitOfWork.CommitAsync();
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Building Error!!!", e.InnerException?.Message);
            }

            return new BuildingResponse
            {
                Id = building.Id,
                Active = building.Active,
                CampusId = building.CampusId,
                CreateDate = building.CreateDate,
                Geom = building.Geom,
                ImageUrl = building.ImageUrl,
                Name = building.Name,
                NumberOfFloor = building.NumberOfFloor,
                Type = building.Type?.Name,
                Status = building.Status,
                Floors = floors,
                Address = building.Address
            };
        }

        public async Task<BuildingResponse> PutBuilding(int id, PostBuildingRequest model, Guid accountId)
        {
            var myGeom = GeoJsonHelper.ParseStringToGeoMetry(model.CoordinateString);
            var oldBuilding = await _unitOfWork.Repository<Building>().GetAll().Where(x => x.Id == id && (x.Status == (int?)Status.NeedSurvey || x.Status == (int?)Status.Surveyed)).FirstOrDefaultAsync();
            if (oldBuilding == null)
            {
                return null;
            }

            Building building = new Building
            {
                Name = model.Name,
                Geom = myGeom,
                Active = model.Active,
                CreateDate = DateTime.UtcNow.AddHours(7),
                ImageUrl = model.ImageUrl,
                NumberOfFloor = model.NumberOfFloor,
                TypeId = model.Type,
                Status = (int?)Status.NeedApproval,
                Address = model.Address,
                ReferenceId = oldBuilding.Id,
            };
            if (model.CampusId > 0)
            {
                building.CampusId = model.CampusId;
            }

            try
            {
                await _unitOfWork.Repository<Building>().InsertAsync(building);
                await _unitOfWork.CommitAsync();
                List<BuidingStreetSegmentResponse> buildingStreetSegments = new List<BuidingStreetSegmentResponse>();
                if (model.StreetSegmentIds != null)
                {
                    foreach (var item in model.StreetSegmentIds)
                    {
                        BuildingStreetSegment buildingStreetSegment = new BuildingStreetSegment { BuidingId = building.Id, StreetSegmentId = item };
                        await _unitOfWork.Repository<BuildingStreetSegment>().InsertAsync(buildingStreetSegment);
                        await _unitOfWork.CommitAsync();
                        buildingStreetSegments.Add(new BuidingStreetSegmentResponse
                        {
                            BuildingId = buildingStreetSegment.BuidingId,
                            StreetSegmentId = buildingStreetSegment.StreetSegmentId
                        });
                    }
                }
                List<FloorResopnse> floors = new List<FloorResopnse>();
                foreach (var floor in model.Floors)
                {
                    Floor floorModel = new Floor
                    {
                        FloorNumber = floor.FloorNumber,
                        BuildingId = building.Id,
                        Name = floor.Name,
                        ReferenceId = floor.Id != 0 ? floor.Id : null,
                    };
                    try
                    {
                        await _unitOfWork.Repository<Floor>().InsertAsync(floorModel);
                        await _unitOfWork.CommitAsync();
                    }
                    catch (Exception e)
                    {
                        throw new CrudException(HttpStatusCode.BadRequest, "Insert Floor Error!!!", e.InnerException?.Message);
                    }
                    List<FloorAreaResopnse> areas = new List<FloorAreaResopnse>();
                    foreach (var areaItem in floor.FloorAreas)
                    {
                        FloorArea area = new FloorArea
                        {
                            Name = areaItem.Name,
                            ReferenceId = areaItem.Id != 0 ? areaItem.Id : null,
                            FloorId = floorModel.Id,
                        };
                        try
                        {
                            await _unitOfWork.Repository<FloorArea>().InsertAsync(area);
                            await _unitOfWork.CommitAsync();
                            areas.Add(new FloorAreaResopnse
                            {
                                Id = area.Id,
                                FloorId = area.FloorId,
                                Name = area.Name,
                                ReferenceId = area.ReferenceId
                            });
                        }
                        catch (Exception e)
                        {
                            throw new CrudException(HttpStatusCode.BadRequest, "Insert Area Error!!!", e.InnerException?.Message);
                        }
                    }
                    floors.Add(new FloorResopnse
                    {
                        Id = floorModel.Id,
                        Name = floorModel.Name,
                        ReferenceId = floorModel.ReferenceId,
                        BuildingId = floorModel.BuildingId,
                        FloorNumber = floorModel.FloorNumber,
                        FloorAreas = areas
                    });
                }
                await _unitOfWork.Repository<History>().InsertAsync(new History
                {
                    AccountId = accountId,
                    Action = (int)ActionEnum.ActionSurvey.EditBuilding,
                    CreateDate = DateTime.UtcNow.AddHours(7),
                    BuildingId = building.Id,
                });
                oldBuilding.Status = (int?)Status.WaitingUpdate;
                await _unitOfWork.CommitAsync();
                return new BuildingResponse
                {
                    Id = building.Id,
                    Active = building.Active,
                    CampusId = building.CampusId,
                    CreateDate = building.CreateDate,
                    Geom = building.Geom,
                    ImageUrl = building.ImageUrl,
                    Name = building.Name,
                    NumberOfFloor = building.NumberOfFloor,
                    Type = building.Type?.Name,
                    Status = building.Status,
                    Floors = floors,
                    Address = building.Address
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update Building Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<List<TypeResponse>> GetListTypeBuilding()
        {
            try
            {
                return await _unitOfWork.Repository<TypeBuilding>().GetAll().Select(x => new TypeResponse { Id = x.Id, Name = x.Name }).ToListAsync();
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Type Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<ApproveBuildingResponse> RejectBuilding(int id)
        {
            var rs = await _unitOfWork.Repository<Building>().GetAll().Where(x => x.Id == id && x.Status == (int?)Status.NeedApproval).SingleOrDefaultAsync();
            if (rs == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Id Not Found!!!", null);
            }

            try
            {
                int? action = rs.Histories.OrderByDescending(o => o.CreateDate).FirstOrDefault().Action;
                if (action == (int)ActionEnum.ActionSurvey.InsertBuilding)
                {
                    rs.Status = (int)Status.Reject;
                }
                else if (action == (int)ActionEnum.ActionSurvey.EditBuilding)
                {
                    rs.Status = (int)Status.Reject;
                    if (rs.ReferenceId != null)
                    {
                        rs.Status = (int)Status.Reject;
                        rs.Reference.Status = (int)Status.NeedSurvey;
                    }
                }
                else
                {
                    rs.Status = (int)Status.NeedSurvey;
                }

                await _unitOfWork.Repository<Building>().Update(rs, rs.Id);
                await _unitOfWork.CommitAsync();
                return new ApproveBuildingResponse
                {
                    Id = rs.Id,
                    Name = rs.Name,
                    Geom = rs.Geom,
                    NumberOfFloor = rs.NumberOfFloor,
                    Active = rs.Active,
                    CampusId = rs.CampusId,
                    CreateDate = rs.CreateDate,
                    ImageUrl = rs.ImageUrl,
                    Type = rs.Type.Name,
                    Address = rs.Address,
                    Status = rs.Status,
                    Action = action
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Reject Building Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<int?> GetSystemzoneIdByBuildingIntersect(Geometry geom)
        {
            int? result = null;
            var listSystemzoneId = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.Geom.Intersects(geom)).ToListAsync();
            if ((listSystemzoneId?.Any()) == true)
            {
                foreach (var item in listSystemzoneId)
                {
                    item.Geom = item.Geom.Intersection(geom);
                }

                result = listSystemzoneId.OrderByDescending(item => item.Geom.Area).First().Id;
            }
            return result;
        }

        public void HardDeleteBuilding()
        {
            // var currentDate = DateTime.UtcNow.AddHours(7).AddDays(-30); // khoong the truyen thang vao linq nen phai AddDays(-30)
            var currentDate = DateTime.UtcNow.AddHours(7).AddDays(-1); // khoong the truyen thang vao linq nen phai AddDays(-30)
            var buildingToRemove = _unitOfWork.Repository<Building>().GetAll().Where(x => currentDate >= x.CreateDate && (x.Status == (int)Status.Deleted || x.Status == (int)Status.Reject)).ToList();
            List<History> listHistory = new List<History>();
            List<Building> listRemoveReferenceId = new List<Building>();
            if (buildingToRemove == null)
            {
                return;
            }

            try
            {
                foreach (var item in buildingToRemove)
                {
                    var buildingRemoveReferenceId = _unitOfWork.Repository<Building>().GetAll().Where(x => x.ReferenceId == item.Id).ToList().Select(c => { c.ReferenceId = null; return c; });
                    listRemoveReferenceId.AddRange(buildingRemoveReferenceId);
                    listHistory.AddRange(item.Histories);
                }
                _unitOfWork.Repository<Building>().UpdateRange(listRemoveReferenceId.AsQueryable<Building>());
                _unitOfWork.Repository<History>().DeleteRange(listHistory.AsQueryable<History>());
                _unitOfWork.Repository<Building>().DeleteRange(buildingToRemove.AsQueryable<Building>());
                _unitOfWork.Commit();
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Hard delete building Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<CustomerSegmentResponse> PostCustomerSegment(PostCustomerSegmentRequest model)
        {
            CustomerSegment customerSegment = new CustomerSegment
            {
                BuildingId = model.BuildingId,
                SegmentId = model.SegmentId,
                PotentialCustomers = model.PotentialCustomers,
                PrimaryAge = model.PrimaryAge,
                TimeSlot = model.TimeSlot
            };
            try
            {
                await _unitOfWork.Repository<CustomerSegment>().InsertAsync(customerSegment);
                await _unitOfWork.CommitAsync();
                customerSegment = _unitOfWork.Repository<CustomerSegment>().GetAll().Where(x => x.BuildingId == customerSegment.BuildingId && x.SegmentId == customerSegment.SegmentId).Include(a => a.Building).Include(b => b.Segment).SingleOrDefault();
                return new CustomerSegmentResponse
                {
                    BuildingId = customerSegment.BuildingId,
                    BuildingName = customerSegment.Building?.Name,
                    SegmentId = customerSegment.SegmentId,
                    SegmentName = customerSegment.Segment?.Name,
                    PotentialCustomers = customerSegment.PotentialCustomers,
                    PrimaryAge = customerSegment.PrimaryAge,
                    TimeSlot = customerSegment.TimeSlot
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert CustomerSegment Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<CustomerSegmentResponse> PutCustomerSegment(int buildingId, int segmentId, PutCustomerSegmentRequest model)
        {
            var buildingCategoey = _unitOfWork.Repository<CustomerSegment>().GetAll().Where(x => x.BuildingId == buildingId && x.SegmentId == segmentId).FirstOrDefault();
            if (buildingCategoey != null)
            {
                try
                {
                    buildingCategoey.PotentialCustomers = model.PotentialCustomers;
                    buildingCategoey.PrimaryAge = model.PrimaryAge;
                    buildingCategoey.TimeSlot = model.TimeSlot;

                    //_unitOfWork.Repository<CustomerSegment>().Update(buildingCategoey, id);
                    await _unitOfWork.CommitAsync();
                    return new CustomerSegmentResponse
                    {
                        BuildingId = buildingCategoey.BuildingId,
                        BuildingName = buildingCategoey.Building?.Name,
                        SegmentId = buildingCategoey.SegmentId,
                        SegmentName = buildingCategoey.Segment?.Name,
                        PotentialCustomers = buildingCategoey.PotentialCustomers,
                        PrimaryAge = buildingCategoey.PrimaryAge,
                        TimeSlot = buildingCategoey.TimeSlot
                    };
                }
                catch (Exception e)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Update CustomerSegment Error!!!", e.InnerException?.Message);
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<CustomerSegmentResponse> DeleteCustomerSegment(int buildingId, int segmentId)
        {
            var buildingCategoey = _unitOfWork.Repository<CustomerSegment>().GetAll().Where(x => x.BuildingId == buildingId && x.SegmentId == segmentId).FirstOrDefault();
            if (buildingCategoey != null)
            {
                try
                {
                    _unitOfWork.Repository<CustomerSegment>().Delete(buildingCategoey);
                    await _unitOfWork.CommitAsync();
                    return new CustomerSegmentResponse
                    {
                        BuildingId = buildingCategoey.BuildingId,
                        BuildingName = buildingCategoey.Building?.Name,
                        SegmentId = buildingCategoey.SegmentId,
                        SegmentName = buildingCategoey.Segment?.Name,
                        PotentialCustomers = buildingCategoey.PotentialCustomers,
                        PrimaryAge = buildingCategoey.PrimaryAge,
                        TimeSlot = buildingCategoey.TimeSlot
                    };
                }
                catch (Exception e)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Delete CustomerSegment Error!!!", e.InnerException?.Message);
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<List<CustomerSegmentResponse>> GetCustomerSegmentByID(int buildingId)
        {
            return await _unitOfWork.Repository<CustomerSegment>().GetAll().Where(a => a.BuildingId == buildingId).Select(x => new CustomerSegmentResponse
            {
                BuildingId = x.BuildingId,
                BuildingName = x.Building != null ? x.Building.Name : null,
                SegmentId = x.SegmentId,
                SegmentName = x.Segment != null ? x.Segment.Name : null,
                PotentialCustomers = x.PotentialCustomers,
                PrimaryAge = x.PrimaryAge,
                TimeSlot = x.TimeSlot
            }).ToListAsync();
        }
    }
}