using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using System;
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
    public class StreetSegmentsService : IStreetSegmentsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public StreetSegmentsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<StreetSegmentResponse> PostStreetSegment(PostStreetSegmentRequest model)
        {
            StreetSegment insert = new StreetSegment
            {
                Name = model.Name,
                Geom = model.Geom,
                CreateDate = DateTime.UtcNow.AddHours(7).Date,
                WardId = model.WardId,
                StreetId = model.StreetId
            };
            try
            {
                _unitOfWork.Repository<StreetSegment>().Insert(insert);
                await _unitOfWork.CommitAsync();
                return new StreetSegmentResponse
                {
                    Name = insert.Name,
                    Geom = insert.Geom,
                    CreateDate = insert.CreateDate,
                    WardId = insert.WardId,
                    Id = insert.Id,
                    StreetId = insert.StreetId
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<ListStreetSegmentResponse> GetStreetSegmentInRadius(string coor)
        {
            Geometry geom = null;
            var radius = _unitOfWork.Repository<Config>().GetAll().Where(x => x.Active && x.Name.Equals("BuildingRadius")).Select(x => x.Value).FirstOrDefault();// unit: meter StreetSegmentRadius
            try
            {
                geom = GeoJsonHelper.ParseStringToGeoMetry(coor);
                // s = c0/360; c0 = 40075.017
                var list = await _unitOfWork.Repository<StreetSegment>().GetAll().Where(x => (x.Geom.Distance(geom) * 40075.017 * 1000 / 360) <= radius).Select(x => new StreetSegmentResponse
                {
                    Name = x.Name,
                    Geom = x.Geom,
                    CreateDate = x.CreateDate,
                    ModifyDate = x.ModifyDate,
                    WardId = x.WardId,
                    Id = x.Id
                }).ToListAsync();
                return new ListStreetSegmentResponse
                {
                    ListStreetSegment = list
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Street Segment by Building Location Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<ListStreetSegmentResponse> GetStreetSegmentsByBuildingId(int id)
        {
            try
            {
                var list = await _unitOfWork.Repository<BuildingStreetSegment>().GetAll().Where(x => x.BuidingId == id).Select(x => new StreetSegmentResponse
                {
                    Name = x.StreetSegment.Name,
                    CreateDate = x.StreetSegment.CreateDate,
                    ModifyDate = x.StreetSegment.ModifyDate,
                    WardId = x.StreetSegment.WardId,
                    Id = x.StreetSegment.Id
                }).ToListAsync();
                return new ListStreetSegmentResponse
                {
                    ListStreetSegment = list
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Street Segment by Building Id Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<ListStreetSegmentResponse> GetStreetSegmentsByStoreId(int id)
        {
            try
            {
                var rs = await _unitOfWork.Repository<StoreStreetSegment>().GetAll().Where(x => x.StoreId == id).Select(x => new StreetSegmentResponse
                {
                    Name = x.StreetSegment.Name,
                    CreateDate = x.StreetSegment.CreateDate,
                    ModifyDate = x.StreetSegment.ModifyDate,
                    WardId = x.StreetSegment.WardId,
                    Id = x.StreetSegment.Id
                }).ToListAsync();
                return new ListStreetSegmentResponse
                {
                    ListStreetSegment = rs
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Street Segment by Building Id Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<ListStreetSegmentResponse> GetStreetSegmentInRadiusPoint(string coor)
        {
            Geometry geom = null;
            var radius = _unitOfWork.Repository<Config>().GetAll().Where(x => x.Active && x.Name.Equals("StoreRadius")).Select(x => x.Value).FirstOrDefault(); ;// unit: meter
            try
            {
                geom = GeoJsonHelper.ParseStringToPoint(coor);
                // s = c0/360; c0 = 40075.017
                var rs = await _unitOfWork.Repository<StreetSegment>().GetAll().Where(x => (x.Geom.Distance(geom) * 40075.017 * 1000 / 360) <= radius).Select(x => new StreetSegmentResponse
                {
                    Name = x.Name,
                    Geom = x.Geom,
                    CreateDate = x.CreateDate,
                    ModifyDate = x.ModifyDate,
                    WardId = x.WardId,
                    Id = x.Id
                }).ToListAsync();
                return new ListStreetSegmentResponse
                {
                    ListStreetSegment = rs
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Street Segment by Store Location Error!!!", e.InnerException?.Message);
            }
        }
    }
}