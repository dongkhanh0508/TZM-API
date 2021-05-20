using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TradeMap.Data.Entity;
using TradeMap.Data.UnitOfWork;
using static TradeMap.Service.Helpers.StatusEnum;

namespace TradeMap.Service.CronJob
{
    public class MyCronJob : CronJobService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MyCronJob(IScheduleConfig<MyCronJob> config, IUnitOfWork unitOfWork)
            : base(config.CronExpression, config.TimeZoneInfo)
        {
            _unitOfWork = unitOfWork;
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            HardDeleteBuilding();
            return base.StartAsync(cancellationToken);
        }

        public override Task DoWork(CancellationToken cancellationToken)
        {
            CalculateWeightNumber();
            return Task.CompletedTask;
        }

        private void CalculateWeightNumber()
        {
            var weightBoundary = _unitOfWork.Repository<Config>().GetAll().Where(x => x.Name.Equals("WeightBoundary") && x.Active == true).Select(x => x.Value).FirstOrDefault();
            var systemzones = _unitOfWork.Repository<SystemZone>().GetAll().Include(x => x.Ward).ToList();
            foreach (var item in systemzones)
            {
                item.WeightNumber = 0;
                var districtPopulationDensity = item.Ward.District.PopulationDensity;
                var systemzonePopoulation = (item.Geom.Area * 12096.719247500967) * districtPopulationDensity;
                int temp = (int)((systemzonePopoulation / weightBoundary) + 1);
                item.WeightNumber += temp;
                var buildingsInSystemzone = _unitOfWork.Repository<Building>().GetAll().Where(x => item.Geom.Intersects(x.Geom) &&
                 x.Status != (int?)Status.Deleted &&
                 x.Status != (int?)Status.WaitingUpdate &&
                 x.Status != (int?)Status.Reject).ToList();
                item.WeightNumber += buildingsInSystemzone.Where(x => (x.NumberOfFloor >= 0 && x.NumberOfFloor <= 5) || x.NumberOfFloor == null).Count() * 1
                    + buildingsInSystemzone.Where(x => x.NumberOfFloor > 5 && x.NumberOfFloor <= 10).Count() * 2
                    + buildingsInSystemzone.Where(x => x.NumberOfFloor > 10).Count() * 3;
            }

            var log = new TestCronJob()
            {
                Message = "Cronjob working at " + TimeZoneInfo.ConvertTime(DateTime.Now,
                 TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time"))
            };
            _unitOfWork.Repository<TestCronJob>().Insert(log);
            _unitOfWork.Repository<SystemZone>().UpdateRange(systemzones.AsQueryable());
            _unitOfWork.CommitAsync();
        }

        private void HardDeleteBuilding()
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
            catch (Exception )
            {
            }
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            return base.StopAsync(cancellationToken);
        }
    }
}