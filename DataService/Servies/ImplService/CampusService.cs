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
using TradeMap.Service.Servies.InterfaceService;

namespace TradeMap.Service.Servies.ImplService
{
    public class CampusService : ICampusService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CampusService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<CampusResponse> CreateCampus(PostCampusRequest model)
        {
            try
            {
                var buildings = await _unitOfWork.Repository<Building>().GetAll().Where(x => model.Geom.Contains(x.Geom)).ToListAsync();
                foreach (var item in buildings)
                {
                    item.CampusId = null;
                    if (item.BuildingStreetSegments.Count != 0)
                    {
                        _unitOfWork.Repository<BuildingStreetSegment>().DeleteRange((IQueryable<BuildingStreetSegment>)item.BuildingStreetSegments);
                    }
                }
                var campus = new Campus
                {
                    Name = model.Name,
                    Buildings = buildings,
                    CreateDate = DateTime.UtcNow.AddHours(7),
                    Geom = model.Geom
                };
                await _unitOfWork.Repository<Campus>().InsertAsync(campus);
                await _unitOfWork.CommitAsync();
                List<CampusStreetSegment> list = new List<CampusStreetSegment>();
                foreach (var item in model.StreetSegmentId)
                {
                    CampusStreetSegment campusStreet = new CampusStreetSegment
                    {
                        StreetSegmentId = item,
                        CampusId = campus.Id
                    };
                    list.Add(campusStreet);
                }
                await _unitOfWork.Repository<CampusStreetSegment>().InsertRangeAsync(list.AsQueryable());
                await _unitOfWork.CommitAsync();
                return new CampusResponse
                {
                    Id = campus.Id,
                    CreateDate = campus.CreateDate,
                    Geom = campus.Geom,
                    ModifyDate = campus.ModifyDate,
                    Name = campus.Name
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Create Campus Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<CampusResponse> DeleteCampus(int id)
        {
            var rs = await _unitOfWork.Repository<Campus>().GetAll().Where(x => x.Id == id).SingleOrDefaultAsync();
            var buildings = await _unitOfWork.Repository<Building>().GetAll().Where(x => rs.Geom.Contains(x.Geom)).ToListAsync();
            var campusStreetSegment = await _unitOfWork.Repository<CampusStreetSegment>().GetAll().Where(x => x.CampusId == rs.Id).ToListAsync();
            if (rs == null)
            {
                return null;
            }

            try
            {
                _unitOfWork.Repository<CampusStreetSegment>().DeleteRange(campusStreetSegment.AsQueryable());
                _unitOfWork.Repository<Campus>().Delete(rs);
                foreach (var item in buildings)
                {
                    item.CampusId = null;
                    if (item.BuildingStreetSegments.Count != 0)
                    {
                        var listSSM = _unitOfWork.Repository<BuildingStreetSegment>().GetAll().Where(x => x.BuidingId == item.Id);
                        _unitOfWork.Repository<BuildingStreetSegment>().DeleteRange(listSSM);
                    }
                    foreach (var x in campusStreetSegment)
                    {
                        await _unitOfWork.Repository<BuildingStreetSegment>().InsertAsync(new BuildingStreetSegment { BuidingId = item.Id, StreetSegmentId = x.StreetSegmentId });
                    }
                }
                await _unitOfWork.CommitAsync();
                return new CampusResponse
                {
                    Id = rs.Id,
                    CreateDate = rs.CreateDate,
                    Geom = rs.Geom,
                    ModifyDate = rs.ModifyDate,
                    Name = rs.Name
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Delete Campus Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<CampusResponse> PutCampus(int id, PutCampusRequest model)
        {
            var campus = await _unitOfWork.Repository<Campus>().GetById(id);
            if (campus != null)
            {
                try
                {
                    campus.Name = model.Name;
                    campus.ModifyDate = DateTime.UtcNow.AddHours(7);
                    var listStreetSegment = _unitOfWork.Repository<CampusStreetSegment>().GetAll().Where(x => x.CampusId == id);
                    _unitOfWork.Repository<CampusStreetSegment>().DeleteRange(listStreetSegment);
                    await _unitOfWork.Repository<Campus>().Update(campus, id);
                    List<CampusStreetSegment> list = new List<CampusStreetSegment>();
                    foreach (var item in model.StreetSegmentId)
                    {
                        CampusStreetSegment campusStreet = new CampusStreetSegment
                        {
                            StreetSegmentId = item,
                            CampusId = campus.Id
                        };
                        list.Add(campusStreet);
                    }
                    await _unitOfWork.Repository<CampusStreetSegment>().InsertRangeAsync(list.AsQueryable());
                    await _unitOfWork.CommitAsync();
                    return new CampusResponse
                    {
                        Id = campus.Id,
                        CreateDate = campus.CreateDate,
                        Geom = campus.Geom,
                        ModifyDate = campus.ModifyDate,
                        Name = campus.Name
                    };
                }
                catch (Exception e)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Update Campus Error!!!", e.InnerException?.Message);
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<CampusResponse> GetCampusById(int id)
        {
            try
            {
                return await _unitOfWork.Repository<Campus>().GetAll().Where(x => x.Id == id).Include(a => a.CampusStreetSegments).Select(rs => new CampusResponse
                {
                    Id = rs.Id,
                    CreateDate = rs.CreateDate,
                    Geom = rs.Geom,
                    ModifyDate = rs.ModifyDate,
                    Name = rs.Name,
                    ListStreetSegment = new List<CampusStreetSegmentRespone>(rs.CampusStreetSegments.Select(f => new CampusStreetSegmentRespone
                    {
                        Id = f.StreetSegmentId,
                        Name = f.StreetSegment.Name
                    }))
                }).SingleOrDefaultAsync();
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Campust by id Error!!!", e.InnerException?.Message);
            }
        }
    }
}