using NetTopologySuite.Geometries;
using ServiceStack.Redis;
using System.Linq;
using TradeMap.Data.Entity;
using TradeMap.Data.UnitOfWork;
using TradeMap.Service.Helpers;
using TradeMap.Service.Servies.InterfaceService;

namespace TradeMap.Service.Servies.ImplService
{
    public class ViolationLogService : IViolationLogService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IRedisClientsManager _redisClientsManager;

        public ViolationLogService(IUnitOfWork unitOfWork, IRedisClientsManager redisClientsManager)
        {
            _unitOfWork = unitOfWork;
            _redisClientsManager = redisClientsManager;
        }

        public void CheckViolation()
        {
            var redis = _redisClientsManager.GetClient();
            var assets = redis.GetAllKeys();
            foreach (var item in assets)
            {
                bool isEnd = false;
                bool isStart = false;
                var currentAsset = redis.Get<AssetRedis>(item);
                ViolationLog log = new ViolationLog()
                {
                    AssetId = currentAsset.Id,
                    Description = "Test",
                    Severity = 1,
                    TypeViolation = 1
                };
                Geometry locationGeometry = null;
                if (currentAsset.Location.Contains(","))
                {
                    locationGeometry = GeoJsonHelper.ParseStringToLineString(currentAsset.Location);
                }
                else
                {
                    locationGeometry = GeoJsonHelper.ParseStringToLineString(currentAsset.Location);
                }

                var tradeZones = _unitOfWork.Repository<TradeZoneVersion>().GetAll().Where(x => x.BrandId == currentAsset.BrandId && x.IsActive == true).Select(x => x.TradeZones).SingleOrDefault();

                var tradezone = tradeZones.Where(x => x.StoreTradeZones.Any(x => x.StoreId == currentAsset.StoreId)).FirstOrDefault();
                if (!(tradezone is null))
                {
                    log.Geom = locationGeometry.Difference(tradezone.Geom);
                    string[] points = currentAsset.Location.Split(", ");
                    for (int i = 0; i < points.Length; i++)
                    {
                        var point = GeoJsonHelper.ParseStringToPoint(points.ElementAt(i));
                        if (!tradezone.Geom.Intersects(point))
                        {
                            if (!isStart)
                            {
                                log.StartTime = currentAsset.StartTime.AddSeconds(3 * i);
                                isStart = true;
                            }
                            else
                            {
                                isEnd = true;
                                log.EndTime = currentAsset.StartTime.AddSeconds(3 * i);
                            }
                        }
                    }
                    if (isStart)
                    {
                        if (!isEnd)
                        {
                            log.EndTime = log.StartTime;
                        }
                        _unitOfWork.Repository<ViolationLog>().Insert(log);
                        _unitOfWork.Commit();
                    }
                }
            }
        }
    }
}