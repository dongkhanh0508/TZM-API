using Microsoft.EntityFrameworkCore;
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

namespace TradeMap.Service.Servies.ImplService
{
    public class TradeZoneVersionServices : ITradeZoneVersionServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public TradeZoneVersionServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<TradeZoneVersionResponse> CreateTradeZoneVersion(PostTradeZoneVerison model, int brandId)
        {
            List<TradeZone> zones = new List<TradeZone>();
            try
            {
                var stores = await _unitOfWork.Repository<Store>().GetAll().Where(x => x.BrandId == brandId).Select(x => new { x.Id, x.Geom }).AsNoTracking().ToListAsync();
                stores = stores.Where(x => model.TradeZones.Any(c => x.Id == c.StoreId)).ToList();
                var insertVersion = new TradeZoneVersion()
                {
                    BrandId = brandId,
                    DateFilter = Convert.ToInt32(model.DateFilter, 2),
                    Description = model.Description,
                    Distance = model.Distance,
                    TimeSlot = model.TimeSlot,
                    Name = model.Name,
                    CreateDate = TimeZoneInfo.ConvertTime(DateTime.Now,
                         TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
                };
                await _unitOfWork.Repository<TradeZoneVersion>().InsertAsync(insertVersion);
                await _unitOfWork.CommitAsync();
                var groupzoneId = model.TradeZones.Select(x => x.GroupzoneId).Distinct();
                var groupzone = await _unitOfWork.Repository<GroupZone>().GetAll().Where(x => groupzoneId.Any(c => c == x.Id)).Select(x => new { x.Id, x.Geom }).ToListAsync();

                foreach (var item in model.TradeZones)
                {
                    var groupTradezoneArea = groupzone.Where(x => x.Id == item.GroupzoneId).Select(x => x.Geom.Area).SingleOrDefault();
                    var insertTradezone = new TradeZone()
                    {
                        TradeZoneVersionId = insertVersion.Id,
                        Geom = item.Geom,
                        Name = item.Name,
                        GroupZoneId = item.GroupzoneId,
                        WeightNumber = item.WeightNumber,
                        Coverage = item.Geom.Area / groupTradezoneArea,
                    };

                    zones.Add(insertTradezone);
                }

                await _unitOfWork.Repository<TradeZone>().InsertRangeAsync(zones.AsQueryable());
                await _unitOfWork.CommitAsync();
                List<Data.Entity.StoreTradeZone> insertList = new List<Data.Entity.StoreTradeZone>();
                foreach (var item in stores)
                {
                    var tradezoneId = zones.Where(x => x.Geom.Contains(item.Geom)).Select(x => x.Id).FirstOrDefault();
                    Data.Entity.StoreTradeZone insertItem = new Data.Entity.StoreTradeZone()
                    {
                        StoreId = item.Id,
                        TradeZoneId = tradezoneId
                    };
                    zones.Remove(zones.Where(x => x.Id == tradezoneId).SingleOrDefault());
                    insertList.Add(insertItem);
                }
                await _unitOfWork.Repository<Data.Entity.StoreTradeZone>().InsertRangeAsync(insertList.AsQueryable());
                await _unitOfWork.CommitAsync();
                return new TradeZoneVersionResponse()
                {
                    Id = insertVersion.Id,
                    Name = insertVersion.Name,
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert TradeZone Version Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<List<TradeZoneVersionResponse>> GetTradeZoneVersionByBrandId(int brandId, string dateFilter, string timeSlot)
        {
            var timeSlotIndexes = timeSlot.Select((b, i) => b == '1' ? i : -1).Where(x => x != -1).ToList();
            var dateIndexes = dateFilter.Select((b, i) => b == '1' ? i : -1).Where(x => x != -1).ToList();
            try
            {
                var listTradezoneVersion = await _unitOfWork.Repository<TradeZoneVersion>().GetAll().Where(x => x.BrandId == brandId)
               .Select(x => new TradeZoneVersionResponse { Id = x.Id, Name = x.Name, IsActive = x.IsActive, DateFilter = Convert.ToString(x.DateFilter, 2).PadLeft(7, '0'), TimeSlot = x.TimeSlot }
               ).AsNoTracking().ToListAsync();
                List<TradeZoneVersionResponse> result = new List<TradeZoneVersionResponse>();
                foreach (var item in listTradezoneVersion)
                {
                    bool timeSlotMatch = false;
                    bool dateMatch = false;
                    foreach (var timeIndex in timeSlotIndexes)
                    {
                        if (item.TimeSlot.ElementAt(timeIndex) == '1')
                        {
                            timeSlotMatch = true;
                            break;
                        }
                    }
                    foreach (var dateIndex in dateIndexes)
                    {
                        if (item.DateFilter.ElementAt(dateIndex) == '1')
                        {
                            dateMatch = true;
                            break;
                        }
                    }
                    if (timeSlotMatch && dateMatch)
                    {
                        result.Add(item);
                    }
                }
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get TradeZone Version Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<TradeZoneVersionResponse> DeleteTradeZoneVersionById(int brandId, int id)
        {
            try
            {
                var delItem = await _unitOfWork.Repository<TradeZoneVersion>().GetAll().Where(x => x.Id == id && x.BrandId == brandId).SingleOrDefaultAsync();
                var result = new TradeZoneVersionResponse { Id = delItem.Id, Name = delItem.Name, IsActive = delItem.IsActive };
                _unitOfWork.Repository<TradeZoneVersion>().Delete(delItem);
                await _unitOfWork.CommitAsync();
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Delete TradeZone Version Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<TradeZoneVersionResponse> ChangeFlagActiveVersion(int brandId, int Id)
        {
            try
            {
                var currentActive = await _unitOfWork.Repository<TradeZoneVersion>().GetAll().Where(x => x.BrandId == brandId && x.IsActive == true).FirstOrDefaultAsync();
                if (!(currentActive is null))
                {
                    currentActive.IsActive = false;

                    await _unitOfWork.Repository<TradeZoneVersion>().Update(currentActive, currentActive.Id);
                }
                var versionNeedActive = await _unitOfWork.Repository<TradeZoneVersion>().GetAll().Where(x => x.Id == Id).SingleOrDefaultAsync();
                versionNeedActive.IsActive = true;
                await _unitOfWork.Repository<TradeZoneVersion>().Update(versionNeedActive, versionNeedActive.Id);
                await _unitOfWork.CommitAsync();
                return new TradeZoneVersionResponse()
                {
                    Id = versionNeedActive.Id,
                    IsActive = versionNeedActive.IsActive,
                    Name = versionNeedActive.Name
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get TradeZone Version Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<TradeZoneVersionDetailResponse> GetTradeZoneVersionActive(int brandId)
        {
            try
            {
                var result = await _unitOfWork.Repository<TradeZoneVersion>().GetAll().Where(x => x.BrandId == brandId && x.IsActive == true).Select(x => new TradeZoneVersionDetailResponse() { Id = x.Id, BrandId = x.BrandId, CreateDate = x.CreateDate, DateFilter = Convert.ToString(x.DateFilter, 2).PadLeft(7, '0'), Description = x.Description, Distance = x.Distance, IsActive = x.IsActive, Name = x.Name, TimeSlot = x.TimeSlot }).AsNoTracking().FirstOrDefaultAsync();
                if (result is null)
                {
                    return null;
                }

                var tradeZone = await _unitOfWork.Repository<TradeZone>().GetAll().Where(x => x.TradeZoneVersionId == result.Id).Select(x => new { x.Geom, storeGeom = x.StoreTradeZones.FirstOrDefault().Store.Geom, x.WeightNumber, x.Name, x.Id }).ToListAsync();
                List<TradeZoneVersionForMap> tradeZones = new List<TradeZoneVersionForMap>();
                foreach (var item in tradeZone)
                {
                    var response = new TradeZoneVersionForMap
                    {
                        Name = item.Name,
                        TotalWeight = item.WeightNumber,
                        TradeZoneId = item.Id
                    };
                    Feature ft = new Feature()
                    {
                        Geometry = item.Geom,
                        Properties = new Properties
                        {
                        }
                    };
                    response.TradeZoneGeom.Features.Add(ft);
                    ft = new Feature()
                    {
                        Geometry = item.storeGeom,
                        Properties = new Properties
                        {
                        }
                    };
                    response.StoreGeom.Features.Add(ft);
                    tradeZones.Add(response);
                }
                result.TradeZones = tradeZones;
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get TradeZone Version By Id Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<TradeZoneVersionDetailResponse> GetTradeZoneVersionById(int Id)
        {
            try
            {
                var result = await _unitOfWork.Repository<TradeZoneVersion>().GetAll().Where(x => x.Id == Id).
                    Select(x => new TradeZoneVersionDetailResponse() { Id = x.Id, BrandId = x.BrandId, CreateDate = x.CreateDate, DateFilter = Convert.ToString(x.DateFilter, 2).PadLeft(7, '0'), Description = x.Description, Distance = x.Distance, IsActive = x.IsActive, Name = x.Name, TimeSlot = x.TimeSlot }).AsNoTracking().SingleOrDefaultAsync();
                var tradeZone = await _unitOfWork.Repository<TradeZone>().GetAll().Where(x => x.TradeZoneVersionId == Id).Select(x => new { x.Geom, storeGeom = x.StoreTradeZones.FirstOrDefault().Store.Geom, x.WeightNumber, x.Name, x.Id }).ToListAsync();
                List<TradeZoneVersionForMap> tradeZones = new List<TradeZoneVersionForMap>();
                foreach (var item in tradeZone)
                {
                    var response = new TradeZoneVersionForMap
                    {
                        Name = item.Name,
                        TotalWeight = item.WeightNumber,
                        TradeZoneId = item.Id
                    };
                    Feature ft = new Feature()
                    {
                        Geometry = item.Geom,
                        Properties = new Properties
                        {
                        }
                    };
                    response.TradeZoneGeom.Features.Add(ft);
                    ft = new Feature()
                    {
                        Geometry = item.storeGeom,
                        Properties = new Properties
                        {
                        }
                    };
                    response.StoreGeom.Features.Add(ft);
                    tradeZones.Add(response);
                }
                result.TradeZones = tradeZones;
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get TradeZone Version By Id Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<List<ZoneCoverageResponse>> GetGroupZoneByTradeZoneVersionId(int tradezoneId, int brandId)
        {
            try
            {
                var tradezone = await _unitOfWork.Repository<TradeZone>().GetAll().Where(x => x.TradeZoneVersionId == tradezoneId).Select(x => new { x.Geom, x.GroupZoneId, x.TradeZoneVersionId, x.Coverage }).ToListAsync();
                var groupzoneBrand = await _unitOfWork.Repository<GroupZone>().GetAll().Where(x => x.BrandId == brandId).ToListAsync();
                var result = groupzoneBrand.Where(x => tradezone.Any(c => x.Geom.Contains(c.Geom.Buffer(-0.0000261)))).Select(x => new ZoneCoverageResponse { Geom = x.Geom, Id = x.Id, Name = x.Name }).ToList();
                tradezone = tradezone.DistinctBy(x => x.Geom).ToList();
                foreach (var item in result)
                {
                    var tradezoneInGroupzone = tradezone.Where(x => x.GroupZoneId == item.Id && x.TradeZoneVersionId == tradezoneId).ToList();
                    tradezoneInGroupzone.ForEach(c => item.Coverage += c.Coverage * 100);
                    item.Coverage = Math.Round((double)item.Coverage, 2);
                }

                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Groupzone by Tradzone Version Id Error!!!", e.InnerException?.Message); ;
            }
        }

        public async Task<List<GroupStoreTradeZoneResponse>> GetListGroupStoreTradeZoneByTradeZoneVersionId(int brandId)
        {
            var tradezoneActive = await _unitOfWork.Repository<TradeZoneVersion>().GetAll().Where(x => x.IsActive && x.BrandId == brandId).Select(x => new { x.TradeZones, x.BrandId, x.Id }).FirstOrDefaultAsync();
            if (!(tradezoneActive is null))
            {
                var tradezone = await _unitOfWork.Repository<TradeZone>().GetAll().Where(x => x.TradeZoneVersionId == tradezoneActive.Id).Select(x => new { x.Geom, x.Id, x.Name, x.GroupZoneId, x.WeightNumber, x.StoreTradeZones }).ToListAsync();
                var store = tradezone.Select(x => x.StoreTradeZones.FirstOrDefault()).ToList();
                var groupzoneBrand = await _unitOfWork.Repository<GroupZone>().GetAll().Where(x => x.BrandId == brandId).ToListAsync();
                var result = groupzoneBrand.Where(x => tradezone.Any(c => x.Geom.Contains(c.Geom.Buffer(-0.0000261)))).Select(x => new GroupStoreTradeZoneResponse { Geom = x.Geom, Id = x.Id, Name = x.Name, BrandId = x.BrandId }).ToList();
                foreach (var item in result)
                {
                    item.TradeZones = tradezone.Where(x => x.GroupZoneId == item.Id).
                        Select(x => new TradeZoneGroupZone { TotalWeight = x.WeightNumber, Geom = x.Geom, Id = x.Id, Name = x.Name }).ToList();
                    item.Stores = store.Where(x => item.Geom.Intersects(x.Store.Geom)).Select(x => new StoreGroupZone { Geom = x.Store.Geom, Id = x.StoreId, Name = x.Store.Name }).ToList();
                    item.Geom = item.Geom.Boundary;
                }
                return result;
            }
            else return null;
        }
    }
}