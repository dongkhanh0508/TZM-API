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
    public class HistoryService : IHistoryService
    {
        private readonly IUnitOfWork _unitOfWork;

        public HistoryService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<PagedResults<HistoryResponse>> GetHistory(PagingRequestHistory model, int role, Guid accountId)
        {
            List<HistoryResponse> list = null;
            try
            {
                List<History> history = null;
                if (model.Action == 0)
                {
                    history = await _unitOfWork.Repository<History>()
                           .GetAll()
                               .Where(x => x.Building != null ? x.Building.Name.ToLower()
                           .Contains(model.KeySearch.ToLower()) : x.Store.Name.ToLower()
                           .Contains(model.KeySearch.ToLower())).ToListAsync();
                }
                else if (model.Action != 0)
                {
                    history = await _unitOfWork.Repository<History>()
                            .GetAll()
                                .Where(x => (x.Building != null ? x.Building.Name.ToLower()
                            .Contains(model.KeySearch.ToLower()) : x.Store.Name.ToLower()
                            .Contains(model.KeySearch.ToLower())) && x.Action == (int?)model.Action).ToListAsync();
                }
                if (model.Status != 0)
                {
                    history = history.Where(x => x.Building != null ? x.Building.Status == (int?)model.Status : x.Store.Status == (int?)model.Status).ToList();
                }
                IEnumerable<HistoryResponse> rs = null;
                if (model.Type == 1)
                {
                    if (role == 2)
                    {
                        history = history.Where(x => x.AccountId == accountId).ToList();
                    }

                    rs = history.Where(a => a.Action == 1 || a.Action == 2 || a.Action == 3).Select(x => new HistoryResponse
                    {
                        Id = x.Id,
                        AccountId = x.AccountId,
                        Action = x.Action,
                        CreateDate = x.CreateDate,
                        BuildingId = x.BuildingId,
                        StoreId = x.StoreId,
                        AccountName = x.Account.Fullname,
                        ReferenceName = x.Building != null ? x.Building.Name : x.Store.Name,
                        Role = x.Account.Role,
                        Geom = x.Building.Geom.AsText(),
                        Status = x.Building.Status
                    }).AsEnumerable();
                }
                else if (model.Type == 2)
                {
                    if (role == 2)
                    {
                        history = history.Where(x => x.AccountId == accountId).ToList();
                    }

                    rs = history.Where(a => a.Action == 4 || a.Action == 5 || a.Action == 6).Select(x => new HistoryResponse
                    {
                        Id = x.Id,
                        AccountId = x.AccountId,
                        Action = x.Action,
                        CreateDate = x.CreateDate,
                        BuildingId = x.BuildingId,
                        StoreId = x.StoreId,
                        AccountName = x.Account.Fullname,
                        ReferenceName = x.Building != null ? x.Building.Name : x.Store.Name,
                        Role = x.Account.Role,
                        Geom = x.Store.Geom.AsText(),
                        Status = x.Store.Status
                    }).AsEnumerable();
                }
                else
                {
                    if (role == 2)
                    {
                        history = history.Where(x => x.AccountId == accountId).ToList();
                    }
                    rs = history.Select(x => new HistoryResponse
                    {
                        Id = x.Id,
                        AccountId = x.AccountId,
                        Action = x.Action,
                        CreateDate = x.CreateDate,
                        BuildingId = x.BuildingId,
                        StoreId = x.StoreId,
                        AccountName = x.Account.Fullname,
                        ReferenceName = x.Building != null ? x.Building.Name : x.Store?.Name,
                        Role = x.Account.Role,
                        Geom = x.Building != null ? x.Building.Geom.AsText() : x.Store.Geom.AsText(),
                        Status = x.Building != null ? x.Building.Status : x.Store.Status
                    }).AsEnumerable();
                }

                list = PageHelper<HistoryResponse>.Sorting(model.SortType, rs, model.ColName);
                var result = PageHelper<HistoryResponse>.Paging(list, model.Page, model.PageSize);
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get History Error!!!", e.InnerException?.Message);
            }
        }

#pragma warning disable CA1041 // Provide ObsoleteAttribute message

        [Obsolete]
#pragma warning restore CA1041 // Provide ObsoleteAttribute message
        public void ArchiveHistory()
        {
            var currentDate = DateTime.UtcNow.AddHours(7).AddDays(-30);
            var historyToRemove = _unitOfWork.Repository<History>().GetAll().Where(x => currentDate >= x.CreateDate).ToList();
            if (historyToRemove.Any())
            {
                _unitOfWork.Repository<History>().DeleteRange(historyToRemove.AsQueryable<History>());
                _unitOfWork.Commit();
            }
        }
    }
}