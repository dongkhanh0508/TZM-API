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

namespace TradeMap.Service.Servies.ImplService
{
    public class GroupZoneServices : IGroupZoneServices
    {
        private readonly IUnitOfWork _unitOfWork;

        public GroupZoneServices(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<GroupZoneResponse> DeleteGroupZone(int id, int brandId)
        {
            GroupZoneResponse groupZone = null;
            var result = await _unitOfWork.Repository<GroupZone>().GetAll().Where(x => x.Id == id && x.BrandId == brandId).Select(x => new { x.Id, x.BrandId, x.Geom, x.Name, x.TradeZones }).AsNoTracking().SingleOrDefaultAsync();
            var tradezoneVersionsId = result.TradeZones.DistinctBy(c => c.TradeZoneVersionId).Select(x => x.TradeZoneVersionId);
            var tradezoneVersion = await _unitOfWork.Repository<TradeZoneVersion>().GetAll().Where(x => tradezoneVersionsId.Any(c => c == x.Id)).ToListAsync();
            var isDeleteActive = tradezoneVersion.Any(x => x.IsActive);
            if (result != null)
            {
                groupZone = new GroupZoneResponse()
                {
                    Id = result.Id,
                    BrandId = result.BrandId,
                    Geom = result.Geom,
                    Name = result.Name,
                    IsDeleteActive = isDeleteActive
                };
                await _unitOfWork.Repository<GroupZone>().HardDelete(result.Id);
                _unitOfWork.Repository<TradeZoneVersion>().DeleteRange(tradezoneVersion.AsQueryable());
                await _unitOfWork.CommitAsync();
            }
            return groupZone;
        }

        public async Task<CustomFeatureCollection> GetGroupZone(int brandId)
        {
            var result = await _unitOfWork.Repository<GroupZone>().GetAll().Where(x => x.BrandId == brandId).Select(x => new GroupZoneResponse() { BrandId = brandId, Geom = x.Geom, Id = x.Id, Name = x.Name }).ToListAsync();
            CustomFeatureCollection cft = new CustomFeatureCollection();
            foreach (var item in result)
            {
                Feature ft = new Feature()
                {
                    Geometry = item.Geom,
                    Properties = new Properties()
                    {
                        F3 = brandId,
                        F1 = item.Name,
                        F4 = item.Id
                    }
                };
                cft.Features.Add(ft);
            }
            return cft;
        }

        public async Task<GroupZoneResponse> GetGroupZoneByID(int id, int brandId)
        {
            return await _unitOfWork.Repository<GroupZone>().GetAll().Where(x => x.BrandId == brandId && x.Id == id).Select(x => new GroupZoneResponse() { BrandId = brandId, Geom = x.Geom, Id = x.Id, Name = x.Name }).FirstOrDefaultAsync();
        }

        public async Task<GroupZoneResponse> PostGroupZone(GroupZoneRequest model, int brandId)
        {
            try
            {
                List<Geometry> geoms;
                if (model.Type == (int)GroupZoneType.Ward)
                {
                    geoms = await _unitOfWork.Repository<Ward>().GetAll().Where(x => model.ListZoneId.Any(c => c == x.Id)).Select(x => x.Geom).ToListAsync();
                }
                else if (model.Type == (int)GroupZoneType.District)
                {
                    geoms = await _unitOfWork.Repository<District>().GetAll().Where(x => model.ListZoneId.Any(c => c == x.Id)).Select(x => x.Geom).ToListAsync();
                }
                else
                {
                    geoms = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => model.ListZoneId.Any(c => c == x.Id)).Select(x => x.Geom).ToListAsync();
                }
                Geometry groupZoneGeom = (geoms.Any()) ? GeoJsonHelper.CombineGeoCollection(geoms) : null;
                var insertItem = new GroupZone()
                {
                    BrandId = brandId,
                    Geom = groupZoneGeom,
                    Name = model.Name,
                };
                await _unitOfWork.Repository<GroupZone>().InsertAsync(insertItem);
                await _unitOfWork.CommitAsync();
                return new GroupZoneResponse() { Id = insertItem.Id, Geom = insertItem.Geom, BrandId = insertItem.BrandId, Name = insertItem.Name };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Groupzone Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<GroupZoneResponse> PutGroupZone(int id, PutGroupZoneRequest model, int brandId)
        {
            var result = await _unitOfWork.Repository<GroupZone>().GetAll().Where(x => x.Id == id && x.BrandId == brandId).SingleOrDefaultAsync();
            if (result != null)
            {
                result.Name = model.Name;
                await _unitOfWork.Repository<GroupZone>().Update(result, result.Id);
                await _unitOfWork.CommitAsync();
                GroupZoneResponse groupZone = new GroupZoneResponse()
                {
                    Id = result.Id,
                    BrandId = result.BrandId,
                    Geom = result.Geom,
                    Name = result.Name
                };
                return groupZone;
            }
            return null;
        }

        public async Task<CustomFeatureCollection> GetFreeWard(int brandId)
        {
            var groupZones = await _unitOfWork.Repository<GroupZone>().GetAll().Where(x => x.BrandId == brandId).Select(x => x.Geom).ToListAsync();
            var freeWards = _unitOfWork.Repository<Ward>().GetAll().ToList().Distinct();
            freeWards = freeWards.Where(x => !groupZones.Any(c => c.Contains(x.Geom))).ToList();
            CustomFeatureCollection result = new CustomFeatureCollection();
            foreach (var item in freeWards)
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

        public async Task<CustomFeatureCollection> GetFreeDistrict(int brandId)
        {
            var groupZones = await _unitOfWork.Repository<GroupZone>().GetAll().Where(x => x.BrandId == brandId).ToListAsync();
            foreach (var item in groupZones)
            {
                item.Geom = item.Geom.Buffer(-0.0000261);
            }

            var freeDistrict = _unitOfWork.Repository<District>().GetAll()
                .Select(x => new { x.Geom, x.Id, x.Name }).AsEnumerable();
            freeDistrict = freeDistrict.Where(x => !groupZones.Any(c => x.Geom.Contains(c.Geom))).ToList();
            CustomFeatureCollection result = new CustomFeatureCollection();
            foreach (var item in freeDistrict)
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

        public async Task<CustomFeatureCollection> GetFreeSystemzone(int brandId)
        {
            var groupZones = await _unitOfWork.Repository<GroupZone>().GetAll().Where(x => x.BrandId == brandId).Select(x => x.Geom).ToListAsync();
            var freeDistrict = _unitOfWork.Repository<SystemZone>().GetAll().Select(x => new { x.Geom, x.Id, x.Name }).AsNoTracking().ToList();
            freeDistrict = freeDistrict.Where(x => !groupZones.Any(c => c.Contains(x.Geom.Buffer(-0.0000261)))).ToList();
            CustomFeatureCollection result = new CustomFeatureCollection();
            foreach (var item in freeDistrict)
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

        public async Task<StoreTradeZoneForMapResponse> GetStoresByGroupZoneId(int groupzoneId, int brandId, int tradezoneVersionId)
        {
            var tradeZone = await _unitOfWork.Repository<TradeZone>().GetAll().Where(x => x.GroupZoneId == groupzoneId && x.TradeZoneVersionId == tradezoneVersionId).Select(x => new { x.Geom, x.Id, x.Name, x.WeightNumber, x.StoreTradeZones }).ToListAsync();
            var groupZone = await _unitOfWork.Repository<GroupZone>().GetAll().Where(x => x.Id == groupzoneId).Select(x => x.Geom.Boundary).SingleOrDefaultAsync();
            StoreTradeZoneForMapResponse result = new StoreTradeZoneForMapResponse()
            {
                GroupZoneBoundary = groupZone
            };

            foreach (var item in tradeZone)
            {
                Store store = item.StoreTradeZones.FirstOrDefault().Store;

                StoreTradeZoneForMap storeTradeZoneForMap = new StoreTradeZoneForMap()
                {
                    Store = new ZoneResponse
                    {
                        Geom = store.Geom,
                        Id = store.Id,
                        Name = store.Name,
                    },
                    TradeZone = new ZoneWeightResponse
                    {
                        Name = item.Name,
                        Id = item.Id,
                        Geom = item.Geom,
                        Weight = item.WeightNumber,
                    }
                };
                result.ListStoreTradeZone.Add(storeTradeZoneForMap);
            }
            return result;
        }




        public async Task<List<ZoneWeightResponse>> GetTradeZoneByGroupZoneId(int groupzoneId, int tradezoneVersionId)
        {
            var result = await _unitOfWork.Repository<TradeZone>().GetAll().Where(x => x.TradeZoneVersionId == tradezoneVersionId && x.GroupZoneId == groupzoneId).Select(x => new ZoneWeightResponse { Geom = x.Geom, Id = x.Id, Name = x.Name, Weight = x.WeightNumber }).AsNoTracking().ToListAsync();
            return result;
        }
    }
}