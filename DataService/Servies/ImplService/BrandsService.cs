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
    public class BrandsService : IBrandsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public BrandsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<BrandsResponse> DeleteBrand(int id)
        {
            var account = _unitOfWork.Repository<Account>().GetAll().Where(x => x.BrandId == id && x.Active == true).ToList();
            if (account.Count != 0)
            {
                throw new CrudException(HttpStatusCode.Conflict, "Brand has account active!!!", "");
            }
            else
            {
                var brand = _unitOfWork.Repository<Brand>().GetAll().Where(x => x.Id == id && x.Active == true).FirstOrDefault();
                if (brand != null)
                {
                    brand.Active = false;
                    try
                    {
                        await _unitOfWork.Repository<Brand>().Update(brand, id);
                        await _unitOfWork.CommitAsync();
                        return new BrandsResponse
                        {
                            Id = brand.Id,
                            Name = brand.Name,
                            IconUrl = brand.IconUrl,
                            ImageUrl = brand.ImageUrl,
                            Active = brand.Active,
                            SegmentId = brand.SegmentId,
                            SegmentName = brand.Segment?.Name
                        };
                    }
                    catch (Exception e)
                    {
                        throw new CrudException(HttpStatusCode.BadRequest, "Delete Brand Error!!!", e.InnerException?.Message);
                    }
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<BrandsResponse> GetBrandByID(int id)
        {
            var brand = await _unitOfWork.Repository<Brand>().GetAll().Where(x => x.Id == id && x.Active == true).FirstOrDefaultAsync();
            if (brand != null)
            {
                return new BrandsResponse
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    IconUrl = brand.IconUrl,
                    ImageUrl = brand.ImageUrl,
                    Active = brand.Active,
                    SegmentId = brand.SegmentId,
                    SegmentName = brand.Segment?.Name
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<List<BrandsResponse>> GetBrands()
        {
            return await _unitOfWork.Repository<Brand>().GetAll().Where(b => b.Active == true)
                .Select(x => new BrandsResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    IconUrl = x.IconUrl,
                    ImageUrl = x.ImageUrl,
                    Active = x.Active,
                    SegmentId = x.SegmentId,
                    SegmentName = x.Segment == null ? null : x.Segment.Name
                }).ToListAsync();
        }

        public async Task<BrandsResponse> PostBrand(PostBrandRequest model, int role)
        {
            Brand brand = new Brand
            {
                Name = model.Name,
                Active = true,

                SegmentId = model.SegmentId
            };
            brand.ImageUrl = model.ImageUrl ?? "";
            brand.IconUrl = model.IconUrl ?? "";
            brand.SegmentId = model.SegmentId > 0 ? model.SegmentId : 0;
            try
            {
                await _unitOfWork.Repository<Brand>().InsertAsync(brand);
                await _unitOfWork.CommitAsync();
                brand = await _unitOfWork.Repository<Brand>().GetAll().Where(x => x.Id == brand.Id).Include(x => x.Segment).SingleOrDefaultAsync();
                return new BrandsResponse
                {
                    Id = brand.Id,
                    Name = brand.Name,
                    IconUrl = brand.IconUrl,
                    ImageUrl = brand.ImageUrl,
                    Active = brand.Active,
                    SegmentId = brand.SegmentId,
                    SegmentName = brand.Segment?.Name
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Brand Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<BrandsResponse> PutBrand(int id, PostBrandRequest model)
        {
            var brand = _unitOfWork.Repository<Brand>().GetAll().Where(x => x.Id == id && x.Active == true).FirstOrDefault();
            if (brand != null)
            {
                try
                {
                    brand.Name = model.Name;
                    brand.SegmentId = model.SegmentId;
                    if (String.IsNullOrEmpty(model.ImageUrl))
                    {
                        brand.ImageUrl = model.ImageUrl;
                    }

                    if (String.IsNullOrEmpty(model.IconUrl))
                    {
                        brand.IconUrl = model.IconUrl;
                    }

                    if (model.SegmentId > 0)
                    {
                        brand.SegmentId = model.SegmentId;
                    }

                    await _unitOfWork.Repository<Brand>().Update(brand, id);
                    await _unitOfWork.CommitAsync();
                    return new BrandsResponse
                    {
                        Id = brand.Id,
                        Name = brand.Name,
                        IconUrl = brand.IconUrl,
                        ImageUrl = brand.ImageUrl,
                        Active = brand.Active,
                        SegmentId = brand.SegmentId,
                        SegmentName = brand.Segment?.Name
                    };
                }
                catch (Exception e)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Update Brand Error!!!", e.InnerException?.Message);
                }
            }
            else
            {
                return null;
            }
        }
    }
}