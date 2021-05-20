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
    public class SegmentService : ISegmentService
    {
        private readonly IUnitOfWork _unitOfWork;

        public SegmentService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<SegmentResponse> DeleteSegment(int id)
        {
            var Segment = await _unitOfWork.Repository<Segment>().GetAll().Where(x => x.Id == id).FirstOrDefaultAsync();
            if (Segment != null)
            {
                try
                {
                    _unitOfWork.Repository<Segment>().Delete(Segment);
                    await _unitOfWork.CommitAsync();
                    return new SegmentResponse
                    {
                        Id = Segment.Id,
                        Name = Segment.Name
                    };
                }
                catch (Exception e)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Delete Segment Error!!!", e.InnerException?.Message);
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<List<SegmentResponse>> GetSegment()
        {
            return await _unitOfWork.Repository<Segment>().GetAll().Select(x => new SegmentResponse { Id = x.Id, Name = x.Name }).ToListAsync();
        }

        public async Task<SegmentResponse> GetSegmentByID(int id)
        {
            var Segment = await _unitOfWork.Repository<Segment>().GetAll().Where(x => x.Id == id).FirstOrDefaultAsync();
            if (Segment != null)
            {
                return new SegmentResponse
                {
                    Id = Segment.Id,
                    Name = Segment.Name
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<SegmentResponse> PostSegment(SegmentRequest model)
        {
            Segment Segment = new Segment
            {
                Name = model.Name,
            };
            try
            {
                await _unitOfWork.Repository<Segment>().InsertAsync(Segment);
                await _unitOfWork.CommitAsync();
                return new SegmentResponse
                {
                    Id = Segment.Id,
                    Name = Segment.Name
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Segment Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<SegmentResponse> PutSegment(int id, SegmentRequest model)
        {
            var Segment = await _unitOfWork.Repository<Segment>().GetAll().Where(x => x.Id == id).FirstOrDefaultAsync();
            if (Segment != null)
            {
                try
                {
                    Segment.Name = model.Name;
                    await _unitOfWork.Repository<Segment>().Update(Segment, id);
                    await _unitOfWork.CommitAsync();
                    return new SegmentResponse
                    {
                        Id = Segment.Id,
                        Name = Segment.Name
                    };
                }
                catch (Exception e)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Update Segment Error!!!", e.InnerException?.Message);
                }
            }
            else
            {
                return null;
            }
        }
    }
}