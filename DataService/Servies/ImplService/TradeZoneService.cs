using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TradeMap.Algorithm.Algorithm;
using TradeMap.Algorithm.Model;
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
    public class TradeZoneService : ITradeZoneService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TradeZoneService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StoreTradeZoneResponse> GroupSystemzoneForStoreByDistance(int brandId, double distance, string timeSlot, List<int> storesId)
        {
            List<Geometry> remainZones = new List<Geometry>();
            List<Helpers.TradeZoneModel> tradezoneModels = new List<Helpers.TradeZoneModel>();
            try
            {
                List<List<StoreTradezone>> lists = new List<List<StoreTradezone>>();
                var model = await GetListSystemzoneByGroupZoneId(brandId, storesId, timeSlot);

                Parallel.ForEach(model, currentModel =>
                 {
                     var totalGeom = NetTopologySuite.Operation.Union.UnaryUnionOp.Union(currentModel.Systemzones.Select(x => x.Geom).ToList());
                     var data = GroupSystemzone.GroupSystemzoneForStoresByDistance(distance, currentModel.Systemzones, currentModel.Stores);
                     foreach (var item in data)
                     {
                         item.TradeZoneGeom = NetTopologySuite.Operation.Union.UnaryUnionOp.Union(item.ListSystemzone.Select(x => x.Geom).ToList());
                         item.GroupZoneId = currentModel.Id;
                     }
                     var selectedGeom = NetTopologySuite.Operation.Union.UnaryUnionOp.Union(data.Select(x => x.TradeZoneGeom).ToList());
                     var remainGeom = totalGeom.Difference(selectedGeom);
                     lock (remainZones)
                     {
                         if (!remainGeom.IsEmpty)
                         {
                             remainZones.Add(remainGeom);
                         }
                     }
                     lock (lists)
                     {
                         lists.Add(data);
                     }
                 });
                var storeTradeZones = new List<DTO.Response.StoreTradeZone>();
                foreach (var storeTradezones in lists)
                {
                    foreach (var item in storeTradezones)
                    {
                        CustomFeatureCollection cftStore = new CustomFeatureCollection();
                        CustomFeatureCollection cftStores = new CustomFeatureCollection();
                        var tempStore = new DTO.Response.StoreTradeZone()
                        {
                            Id = item.Store.Id,
                            Name = item.Store.Name
                        };

                        Feature ftStore = new Feature
                        {
                            Geometry = item.Store.Geom,
                        };
                        ftStore.Properties.F1 = item.Store.Name;
                        ftStore.Properties.F2 = item.Store.Id.ToString();
                        cftStores.Features.Add(ftStore);

                        Feature ft = new Feature
                        {
                            Geometry = item.TradeZoneGeom
                        };
                        ft.Properties.F1 = item.Store.Name;
                        ft.Properties.F2 = item.Store.Id.ToString();
                        ft.Properties.F5 = item.TotalWeight.ToString();
                        ft.Properties.F4 = item.GroupZoneId;

                        cftStore.Features.Add(ft);
                        tempStore.TradeZoneGeom = cftStore;
                        tempStore.StoresGeom = cftStores;
                        storeTradeZones.Add(tempStore);
                    }
                }
                CustomFeatureCollection cftZone = new CustomFeatureCollection();
                if (remainZones.Any())
                {
                    foreach (var item in remainZones)
                    {
                        Feature ft = new Feature
                        {
                            Geometry = item
                        };
                        cftZone.Features.Add(ft);
                    }
                }
                else
                {
                    cftZone = null;
                }

                StoreTradeZoneResponse result = new StoreTradeZoneResponse()
                {
                    RemainZones = cftZone,
                    StoreTradeZone = storeTradeZones
                };

                return result;
            }
            catch (System.Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Group Systemzone Error!!!", e.InnerException?.Message);
            }
        }

        private async Task<List<TradeZoneExcuteModel>> GetListSystemzoneByGroupZoneId(int brandId, List<int> storesId, string timeSlot)
        {
            List<TradeZoneExcuteModel> models = new List<TradeZoneExcuteModel>();
            var indexInput = timeSlot.Select((b, i) => b == '1' ? i : -1).Where(x => x != -1).ToList();

            var weightPotential = _unitOfWork.Repository<Config>().GetAll().Where(x => x.Name.Equals("WeightPotential") && x.Active == true).Select(x => x.Value).FirstOrDefault();
            var stores = await _unitOfWork.Repository<Store>().GetAll().Where(x => storesId.Contains(x.Id)).ToListAsync();
            var groupzones = await _unitOfWork.Repository<GroupZone>().GetAll().Where(x => x.BrandId == brandId).ToListAsync();
            groupzones = groupzones.Where(x => stores.Any(c => x.Geom.Contains(c.Geom))).ToList();
            var groupzonesGeom = groupzones.Select(x => x.Geom).ToList();
            var systemzonesDb = await _unitOfWork.Repository<SystemZone>().GetAll().ToListAsync();
            var listSystemzones = systemzonesDb.Where(x => groupzonesGeom.Any(c => c.Intersects(x.Geom))).ToList();
            var brandSegment = _unitOfWork.Repository<Segment>().GetAll().Where(x => x.Id == brandId).FirstOrDefault();
            if (!(brandSegment is null)){
                foreach (var item in brandSegment.CustomerSegments)
                {
                    foreach (var index in indexInput)
                    {
                        if (item.TimeSlot.ElementAt(index).Equals('1'))
                        {
                            var systemzone = listSystemzones.Where(x => x.Geom.Intersects(item.Building.Geom)).FirstOrDefault();
                            if (!(systemzone is null) && (item.Building.Status != (int)Status.Deleted && item.Building.Status != (int)Status.Reject))
                            {
                                var weight = (int)((item.PotentialCustomers / weightPotential) + 1);
                                systemzone.WeightNumber += weight;
                                listSystemzones.Where(x => x.Id == systemzone.Id).FirstOrDefault().WeightNumber = systemzone.WeightNumber;
                            }
                            break;
                        }
                    }
                }
            }
         
            foreach (var item in groupzones)
            {
                TradeZoneExcuteModel model = new TradeZoneExcuteModel()
                {
                    Id = item.Id,
                };
                model.Stores = stores.Where(x => item.Geom.Contains(x.Geom)).ToList();

                var systemzones = listSystemzones.Where(x => item.Geom.Contains(x.Geom.Buffer(-0.0000261))).ToList();
                model.Systemzones.AddRange(systemzones);
                models.Add(model);
            }

            return models;
        }

        public async Task<List<TradezoneResponse>> InsertTradezone(List<TradezoneRequest> list, int brandId)
        {
            List<TradeZone> insertList = new List<TradeZone>();
            foreach (var item in list)
            {
                TradeZone insertItem = new TradeZone()
                {
                    Name = item.Name,
                    GroupZoneId = item.GroupzoneId,
                    Geom = item.Geom,
                    WeightNumber = item.WeightNumber,
                };
                insertList.Add(insertItem);
            }
            await _unitOfWork.Repository<TradeZone>().InsertRangeAsync(insertList.AsQueryable());
            await _unitOfWork.CommitAsync();
            var result = insertList.Select(x => new TradezoneResponse { Id = x.Id, Geom = x.Geom, WeightNumber = x.WeightNumber }).ToList();
            return result;
        }
    }
}