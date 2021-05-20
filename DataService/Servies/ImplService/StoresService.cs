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
    public class StoresService : IStoresService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StoresService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StoreResponse> GetStoreById(int id, Guid accountId, int role)
        {
            var history = await _unitOfWork.Repository<History>().GetAll().Where(x => x.StoreId == id).OrderByDescending(o => o.CreateDate).FirstOrDefaultAsync();
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
                    Geom = history.Store.Geom?.AsText()
                };
            }

            var rs = await _unitOfWork.Repository<Store>().GetAll().Where(x => x.Id == id).Select(s => new StoreDetailResponse
            {
                Name = s.Name,
                Address = s.Address,
                FloorAreaName = s.FloorArea == null ? null : s.FloorArea.Name,
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
                AbilityToServe = s.AbilityToServe,
                TimeSlot = s.TimeSlot,
                History = historyResponse,
                StoreStreetSegments = new List<StoreStreetSegmentResponse>(s.StoreStreetSegments.Select(m => new StoreStreetSegmentResponse { StoreId = m.StoreId, StreetSegmentId = m.StreetSegmentId }))
            }).FirstOrDefaultAsync();
            if (rs.Geom != null)
            {
                rs.SystemzoneId = await GetSystemzoneIdByStoreIntersect(rs.Geom);
            }
            if (role == 2)
            {
                var systemZone = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.AccountId == accountId).ToListAsync();
                rs.IsEditable = systemZone.Any() && systemZone.Any(c => c.Geom.Intersects(rs.Geom)) && (rs.Status == (int?)Status.Surveyed || rs.Status == (int?)Status.NeedSurvey);
            }
            return rs;
        }

        public async Task<StoreResponse> PostStore(Guid accountId, PostStoresRequest model)
        {
            var myGeom = GeoJsonHelper.ParseStringToPoint(model.CoordinateString);

            Store insert = new Store
            {
                Name = model.Name,
                TimeSlot = model.TimeSlot,
                CreateDate = DateTime.UtcNow.AddHours(7),
                BrandId = model.BrandId,
                Geom = myGeom,
                Address = model.Address,
                Status = (int?)Status.NeedApproval,
                AbilityToServe = model.AbilityToServe,
                ImageUrl = model.ImageUrl
            };
            if (model.FloorAreaId > 0)
            {
                insert.FloorAreaId = model.FloorAreaId;
            }

            try
            {
                await _unitOfWork.Repository<Store>().InsertAsync(insert);
                await _unitOfWork.CommitAsync();
                List<StoreStreetSegmentResponse> storeStreetSegmentResponses = new List<StoreStreetSegmentResponse>();
                if (model.StreetSegmentIds != null)
                {
                    foreach (var item in model.StreetSegmentIds)
                    {
                        StoreStreetSegment storeStreetSegment = new StoreStreetSegment { StoreId = insert.Id, StreetSegmentId = item };
                        await _unitOfWork.Repository<StoreStreetSegment>().InsertAsync(storeStreetSegment);
                        await _unitOfWork.CommitAsync();
                        storeStreetSegmentResponses.Add(new StoreStreetSegmentResponse
                        {
                            StoreId = storeStreetSegment.StoreId,
                            StreetSegmentId = storeStreetSegment.StreetSegmentId
                        });
                    }
                }
                await _unitOfWork.Repository<History>().InsertAsync(new History
                {
                    AccountId = accountId,
                    Action = (int)ActionEnum.ActionSurvey.InsertStore,
                    CreateDate = DateTime.UtcNow.AddHours(7),
                    StoreId = insert.Id,
                });
                await _unitOfWork.CommitAsync();
                return new StoreResponse
                {
                    Name = insert.Name,
                    Address = insert.Address,
                    FloorAreaName = insert.FloorArea?.Name,
                    BrandName = insert.Brand?.Name,
                    CreateDate = insert.CreateDate,
                    Geom = insert.Geom,
                    Id = insert.Id,
                    FloorAreaId = insert.FloorAreaId,
                    StoreStreetSegments = storeStreetSegmentResponses,
                    BrandId = insert.BrandId,
                    ImageUrl = insert.ImageUrl,
                    Status = insert.Status,
                    TimeSlot = insert.TimeSlot,
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Store Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<List<StoreBrandResponse>> GetStoreByBrandId(int brandId)
        {
            return await _unitOfWork.Repository<Store>().GetAll().Include(x => x.Brand).Where(x => x.BrandId == brandId && (x.Status != (int?)Status.Deleted && x.Status! != (int?)Status.Reject)).Select(s => new StoreBrandResponse
            {
                Name = s.Name,
                Address = s.Address,
                FloorAreaName = s.FloorArea == null ? null : s.FloorArea.Name,
                BrandName = s.Brand == null ? null : s.Brand.Name,
                CreateDate = s.CreateDate,
                WKT = s.Geom.AsText(),
                Id = s.Id,
                FloorAreaId = s.FloorAreaId,
                ImageUrl = s.ImageUrl,
                BrandId = s.BrandId,
                AbilityToServe = s.AbilityToServe,
                Status = s.Status,
                Geom = s.Geom,
                TimeSlot = s.TimeSlot,
                Type = s.Brand.Segment.Name,
                StoreStreetSegments = new List<StoreStreetSegmentResponse>(s.StoreStreetSegments.Select(m => new StoreStreetSegmentResponse { StoreId = m.StoreId, StreetSegmentId = m.StreetSegmentId }))
            }).ToListAsync();
        }

        public async Task<StoreResponse> PutStore(int id, Guid accountId, PostStoresRequest model)
        {
            var myGeom = GeoJsonHelper.ParseStringToPoint(model.CoordinateString);
            var store = await _unitOfWork.Repository<Store>().GetById(id);
            if (store == null)
            {
                return null;
            }

            Store insert = new Store
            {
                Name = model.Name,
                CreateDate = DateTime.UtcNow.AddHours(7),
                BrandId = model.BrandId,
                Geom = myGeom,
                Address = model.Address,
                Status = (int?)Status.NeedApproval,
                AbilityToServe = model.AbilityToServe,
                ImageUrl = model.ImageUrl,
                ReferenceId = store.Id,
                TimeSlot = model.TimeSlot,
            };
            if (model.FloorAreaId > 0)
            {
                insert.FloorAreaId = model.FloorAreaId;
            }

            try
            {
                await _unitOfWork.Repository<Store>().InsertAsync(insert);
                await _unitOfWork.CommitAsync();
                List<StoreStreetSegmentResponse> storeStreetSegmentResponses = new List<StoreStreetSegmentResponse>();
                if (model.StreetSegmentIds != null)
                {
                    foreach (var item in model.StreetSegmentIds)
                    {
                        StoreStreetSegment storeStreetSegment = new StoreStreetSegment { StoreId = insert.Id, StreetSegmentId = item };
                        await _unitOfWork.Repository<StoreStreetSegment>().InsertAsync(storeStreetSegment);
                        await _unitOfWork.CommitAsync();
                        storeStreetSegmentResponses.Add(new StoreStreetSegmentResponse
                        {
                            StoreId = storeStreetSegment.StoreId,
                            StreetSegmentId = storeStreetSegment.StreetSegmentId
                        });
                    }
                }
                await _unitOfWork.Repository<History>().InsertAsync(new History
                {
                    AccountId = accountId,
                    Action = (int)ActionEnum.ActionSurvey.EditStore,
                    CreateDate = DateTime.UtcNow.AddHours(7),
                    StoreId = insert.Id,
                });
                store.Status = (int?)Status.WaitingUpdate;
                await _unitOfWork.CommitAsync();
                return new StoreResponse
                {
                    Name = insert.Name,
                    Address = insert.Address,
                    FloorAreaName = insert.FloorArea?.Name,
                    BrandName = insert.Brand?.Name,
                    CreateDate = insert.CreateDate,
                    Geom = insert.Geom,
                    Id = insert.Id,
                    FloorAreaId = insert.FloorAreaId,
                    StoreStreetSegments = storeStreetSegmentResponses,
                    BrandId = insert.BrandId,
                    ImageUrl = insert.ImageUrl,
                    Type = insert.Brand?.Segment?.Name,
                    Status = insert.Status,
                    AbilityToServe = insert.AbilityToServe,
                    TimeSlot = insert.TimeSlot
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update Store Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<StoreResponse> DeleteStore(int id, Guid accountId, int brandId, int role)
        {
            var store = await _unitOfWork.Repository<Store>().GetById(id);
            if (store == null)
            {
                return new StoreResponse();
            }

            if (role == 1)
            {
                if (store.BrandId != brandId)
                {
                    throw new CrudException(HttpStatusCode.Unauthorized, "You do not have permission to delete this store!!!", "");
                }
            }

            try
            {
                await _unitOfWork.Repository<History>().InsertAsync(new History
                {
                    AccountId = accountId,
                    Action = (int)ActionEnum.ActionSurvey.DeleteStore,
                    CreateDate = DateTime.UtcNow.AddHours(7),
                    StoreId = store.Id,
                });
                store.Status = (int?)Status.NeedApproval;
                await _unitOfWork.CommitAsync();
                return new StoreResponse
                {
                    Name = store.Name,
                    Address = store.Address,
                    FloorAreaName = store.FloorArea?.Name,
                    BrandName = store.Brand?.Name,
                    CreateDate = store.CreateDate,
                    Geom = store.Geom,
                    Id = store.Id,
                    FloorAreaId = store.FloorAreaId,
                    BrandId = store.BrandId,
                    ImageUrl = store.ImageUrl,
                    Type = store.Brand?.Segment?.Name,
                    Status = store.Status,
                    AbilityToServe = store.AbilityToServe,
                    TimeSlot = store.TimeSlot,
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Delete Store Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<PagedResults<StoreResponse>> GetStoreNeedSurveyByAccountId(Guid accountId, PagingRequestGetSurvey model)
        {
            List<StoreResponse> list = null;
            try
            {
                var systemZone = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.AccountId == accountId).ToListAsync();
                List<StoreResponse> stores = new List<StoreResponse>();
                foreach (var item in systemZone)
                {
                    var searchResult = await _unitOfWork.Repository<Store>()
                          .GetAll()
                              .Where(x => x.Name.ToLower()
                          .Contains(model.KeySearch.ToLower())
                          && item.Geom.Intersects(x.Geom) && x.Status == (int)model.Status && x.Status != (int?)Status.Deleted)
                              .Select(x => new StoreResponse
                              {
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
                                  SystemZoneId = item.Id,
                                  TimeSlot = x.TimeSlot
                              })
                              .ToListAsync();

                    if (searchResult.Count != 0)
                    {
                        stores.AddRange(searchResult);
                    }
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

        public async Task<StoreResponse> ChangeFlagNeedSurvey(int id)
        {
            var rs = await _unitOfWork.Repository<Store>().GetAll().Where(x => x.Id == id).SingleOrDefaultAsync();
            if (rs == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Id Not Found!!!", null);
            }

            try
            {
                if (rs.Status == null || rs.Status == 1)
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

                var accountId = _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.Geom.Intersects(rs.Geom)).SingleOrDefault();

                if (accountId != null)
                {
                    await FirebaseHelper.SendNotificationAsync(accountId.Account.FcmToken, "You'll be surveying new building");
                }
                await _unitOfWork.CommitAsync();
                return new StoreResponse
                {
                    Name = rs.Name,
                    Address = rs.Address,
                    FloorAreaName = rs.FloorArea?.Name,
                    BrandName = rs.Brand?.Name,
                    CreateDate = rs.CreateDate,
                    Geom = rs.Geom,
                    Id = rs.Id,
                    FloorAreaId = rs.FloorAreaId,
                    ImageUrl = rs.ImageUrl,
                    BrandId = rs.BrandId,
                    Type = rs.Brand?.Segment?.Name,
                    Status = rs.Status,
                    AbilityToServe = rs.AbilityToServe,
                    TimeSlot = rs.TimeSlot
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Change Flag Need Survey Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<ApproveStoreResponse> ApproveStore(int id)
        {
            var rs = await _unitOfWork.Repository<Store>().GetAll().Where(x => x.Id == id).SingleOrDefaultAsync();
            if (rs == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Id Not Found!!!", null);
            }

            try
            {
                int? action = rs.Histories.OrderByDescending(o => o.CreateDate).FirstOrDefault().Action;
                if (action == (int)ActionEnum.ActionSurvey.DeleteStore)
                {
                    rs.Status = (int)Status.Deleted;
                }
                else
                {
                    rs.Status = (int)Status.Surveyed;
                }

                if (rs.ReferenceId != null)
                {
                    rs.Reference.Status = (int)Status.Deleted;
                }
                await _unitOfWork.Repository<Store>().Update(rs, rs.Id);
                await _unitOfWork.CommitAsync();
                return new ApproveStoreResponse
                {
                    Name = rs.Name,
                    Address = rs.Address,
                    FloorAreaName = rs.FloorArea?.Name,
                    BrandName = rs.Brand?.Name,
                    CreateDate = rs.CreateDate,
                    Geom = rs.Geom,
                    Id = rs.Id,
                    FloorAreaId = rs.FloorAreaId,
                    ImageUrl = rs.ImageUrl,
                    BrandId = rs.BrandId,
                    Type = rs.Brand?.Segment?.Name,
                    Status = rs.Status,
                    Action = action,
                    AbilityToServe = rs.AbilityToServe,
                    TimeSlot = rs.TimeSlot
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Approve Store Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<ApproveStoreResponse> RejectStore(int id)
        {
            var rs = await _unitOfWork.Repository<Store>().GetAll().Where(x => x.Id == id).SingleOrDefaultAsync();
            if (rs == null)
            {
                throw new CrudException(HttpStatusCode.NotFound, "Id Not Found!!!", null);
            }

            try
            {
                int? action = rs.Histories.OrderByDescending(o => o.CreateDate).FirstOrDefault().Action;
                if (action == (int)ActionEnum.ActionSurvey.InsertStore)
                {
                    rs.Status = (int)Status.Reject;
                }
                else if (action == (int)ActionEnum.ActionSurvey.EditStore)
                {
                    rs.Status = (int)Status.Reject;
                    if (rs.ReferenceId != null)
                    {
                        rs.Reference.Status = (int)Status.NeedSurvey;
                    }
                }
                else
                {
                    rs.Status = (int)Status.NeedSurvey;
                }

                await _unitOfWork.Repository<Store>().Update(rs, rs.Id);
                await _unitOfWork.CommitAsync();
                return new ApproveStoreResponse
                {
                    Name = rs.Name,
                    Address = rs.Address,
                    FloorAreaName = rs.FloorArea?.Name,
                    BrandName = rs.Brand?.Name,
                    CreateDate = rs.CreateDate,
                    Geom = rs.Geom,
                    Id = rs.Id,
                    FloorAreaId = rs.FloorAreaId,
                    ImageUrl = rs.ImageUrl,
                    BrandId = rs.BrandId,
                    Type = rs.Brand?.Segment?.Name,
                    Status = rs.Status,
                    Action = action,
                    AbilityToServe = rs.AbilityToServe,
                    TimeSlot = rs.TimeSlot,
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Reject Store Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<int?> GetSystemzoneIdByStoreIntersect(Geometry geom)
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

        public async Task<List<string>> GetStoreTypes()
        {
            List<string> list = new List<string>();
            try
            {
                list = await _unitOfWork.Repository<Segment>().GetAll().Select(x => x.Name).Distinct().ToListAsync();
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Store Type Error!!!", e.InnerException?.Message);
            }
            return list;
        }

        public async Task<List<BuildingDetailResponse>> GetBuildingByStoreLocation(string coor, float radius)
        {
            Geometry geom = null;
            // unit: meter
            try
            {
                geom = GeoJsonHelper.ParseStringToPoint(coor);
                // s = c0/360; c0 = 40075.017
                return await _unitOfWork.Repository<Building>().GetAll().Where(x => (x.Geom.Distance(geom) * 40075.017 * 1000 / 360) <= radius && x.Status == (int)Status.Surveyed).Select(x => new BuildingDetailResponse
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
                    ReferencrName = x.ReferenceId != null ? x.Reference.Name : null,
                    Floors = new List<FloorResopnse>(x.Floors.Select(f => new FloorResopnse
                    {
                        Id = f.Id,
                        BuildingId = f.BuildingId,
                        FloorNumber = f.FloorNumber,
                        Name = f.Name,
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
                                ReferenceId = s.ReferenceId,
                                AbilityToServe = s.AbilityToServe,
                                TimeSlot = s.TimeSlot
                            }))
                        }))
                    }))
                }).ToListAsync();
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Building by Store Location Error!!!", e.InnerException?.Message);
            }
        }

        public void ArchiveStore()
        {
            var currenetDate = DateTime.UtcNow.AddHours(7).AddDays(-30);
            var storetoRemove = _unitOfWork.Repository<Store>().GetAll().
                Where(x => currenetDate >= x.CreateDate && (x.Status == (int)Status.Deleted || x.Status == (int)Status.Reject))
                .ToList();
            if (storetoRemove.Any())
            {
                List<Store> listToRemoveReferenceID = new List<Store>();
                foreach (var item in storetoRemove)
                {
                    var removeReferenceId = _unitOfWork.Repository<Store>().GetAll().Where(x => x.ReferenceId == item.Id).ToList();
                    removeReferenceId.ForEach(x => x.ReferenceId = null);
                    listToRemoveReferenceID.AddRange(removeReferenceId);
                }
                _unitOfWork.Repository<Store>().UpdateRange(listToRemoveReferenceID.AsQueryable<Store>());
                _unitOfWork.Repository<Store>().DeleteRange(storetoRemove.AsQueryable<Store>());
                _unitOfWork.Commit();
            }
        }

        public async Task<StoreResponse> PutStoreForBrand(int id, Guid accountId, int brandId, PutStoresForBrandRequest model)
        {
            var store = await _unitOfWork.Repository<Store>().GetById(id);
            if (store == null)
            {
                return null;
            }

            if (store.BrandId != brandId)
            {
                throw new CrudException(HttpStatusCode.MethodNotAllowed, "Update Store Error!!!", "");
            }

            store.Name = model.Name;
            store.Address = model.Address;
            if (!string.IsNullOrEmpty(model.ImageUrl))
            {
                store.ImageUrl = model.ImageUrl;
            }
            store.Address = model.Address;
            store.TimeSlot = model.TimeSlot;
            store.AbilityToServe = model.AbilityToServe;
            try
            {
                await _unitOfWork.Repository<Store>().Update(store, id);
                await _unitOfWork.Repository<History>().InsertAsync(new History
                {
                    AccountId = accountId,
                    Action = (int)ActionEnum.ActionSurvey.EditStore,
                    CreateDate = DateTime.UtcNow.AddHours(7),
                    StoreId = store.Id,
                });
                await _unitOfWork.CommitAsync();
                return new StoreResponse
                {
                    Name = store.Name,
                    Address = store.Address,
                    BrandName = store.Brand?.Name,
                    CreateDate = store.CreateDate,
                    Geom = store.Geom,
                    Id = store.Id,
                    BrandId = store.BrandId,
                    ImageUrl = store.ImageUrl,
                    Type = store.Brand?.Segment?.Name,
                    Status = store.Status,
                    AbilityToServe = store.AbilityToServe,
                    TimeSlot = store.TimeSlot
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update Store Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<StoreTimeSlotResponse> GetStoreTimeslot(int brandId)
        {
            StoreTimeSlotResponse result = new StoreTimeSlotResponse();
            try
            {
                var storeOfBrand = await _unitOfWork.Repository<Store>().GetAll().Where(x => x.BrandId == brandId && (x.Status == (int)Status.Surveyed || x.Status == (int)Status.NeedSurvey || x.Status == (int)Status.WaitingUpdate)).Select(x => new { x.Name, x.Id, x.Geom, x.TimeSlot }).AsNoTracking().ToListAsync();

                foreach (var item in result.ListStore)
                {
                    var index = item.TimeSlot.IndexOf('1');
                    var storeInTimeSlot = storeOfBrand.Where(x => x.TimeSlot.ElementAt(index).Equals('1')).ToList();
                    foreach (var store in storeInTimeSlot)
                    {
                        Feature ft = new Feature()
                        {
                            Geometry = store.Geom,
                            Properties = new Properties
                            {
                                F1 = store.Name,
                                F3 = store.Id,
                            }
                        };
                        item.Stores.Features.Add(ft);
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Store Time Slot Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<StoreResponse> GetStoreOrder(int brandId, string coordinateString, DateTime timeOrder, int dateOrder)
        {
            var timeInMinute = (timeOrder.Hour * 60) + timeOrder.Minute;
            var timeSlot = (int)timeInMinute / (60 * 6); // 6 is 1 timeslot
            var tradezoneVer = await _unitOfWork.Repository<TradeZoneVersion>().GetAll().Where(x => x.IsActive == true && x.BrandId == brandId).FirstOrDefaultAsync();
            if (!tradezoneVer.TimeSlot.ElementAt(timeSlot).Equals('1'))
            {
                return null;
            }
            var dateInVersion = Convert.ToString(tradezoneVer.DateFilter, 2).PadLeft(7, '0');
            if (!dateInVersion.ElementAt(dateOrder).Equals('1'))
            {
                return null;
            }
            if (!(tradezoneVer is null))
            {
                var location = GeoJsonHelper.ParseStringToPoint(coordinateString);
                var tradezone = tradezoneVer.TradeZones;
                var result = tradezone.Where(x => x.Geom.Intersects(location)).FirstOrDefault();
                if (result != null)
                {
                    return new StoreResponse
                    {
                        Name = result.StoreTradeZones.ElementAt(0).Store.Name,
                        Address = result.StoreTradeZones.ElementAt(0).Store.Address,
                        BrandName = result.StoreTradeZones.ElementAt(0).Store.Brand?.Name,
                        CreateDate = result.StoreTradeZones.ElementAt(0).Store.CreateDate,
                        Geom = result.StoreTradeZones.ElementAt(0).Store.Geom,
                        Id = result.StoreTradeZones.ElementAt(0).Store.Id,
                        BrandId = result.StoreTradeZones.ElementAt(0).Store.BrandId,
                        ImageUrl = result.StoreTradeZones.ElementAt(0).Store.ImageUrl,
                        Type = result.StoreTradeZones.ElementAt(0).Store.Brand?.Segment?.Name,
                        Status = result.StoreTradeZones.ElementAt(0).Store.Status,
                        AbilityToServe = result.StoreTradeZones.ElementAt(0).Store.AbilityToServe,
                        TimeSlot = result.StoreTradeZones.ElementAt(0).Store.TimeSlot
                    };
                }
                else return null;
            }
            else return null;
        }
    }
}