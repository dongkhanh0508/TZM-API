using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TradeMap.Data.Entity;
using TradeMap.Data.UnitOfWork;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Exceptions;
using TradeMap.Service.Helpers;
using TradeMap.Service.Servies.InterfaceService;
using static TradeMap.Service.Helpers.StatusEnum;
using static TradeMap.Service.Helpers.TypeAssetEnum;

namespace TradeMap.Service.Servies.ImplService
{
    public class AssetsService : IAssetsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public AssetsService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<string> AssetAuthen(AssetAuthenRequest request)
        {
            var asset = await _unitOfWork.Repository<Asset>().GetAll().Where(x => x.Id == request.AssetId && x.StoreId == request.StoreId && x.Store.BrandId == request.BrandId && x.IsDeleted == false).FirstOrDefaultAsync();
            if (asset == null)
            {
                return null;
            }

            try
            {
                string jwt = GenerateJwtToken(asset);
                return jwt;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Authen Asset Error!!!", e.InnerException?.Message);
            }
        }

        private string GenerateJwtToken(Asset asset)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, asset.Id.ToString()),
                new Claim(ClaimTypes.Name , asset.Name),
                 new Claim(ClaimTypes.Role , "Asset"),
                new Claim("BrandId" , string.IsNullOrEmpty(asset.Store?.BrandId?.ToString())?"":asset.Store?.BrandId?.ToString()),
                new Claim("StoreId" ,string.IsNullOrEmpty(asset.StoreId.ToString())?"":asset.StoreId.ToString()),
                new Claim("BrandName" , string.IsNullOrEmpty(asset.Store?.Brand?.Name)?"":asset.Store?.Brand?.Name),
                new Claim("StoreName" ,string.IsNullOrEmpty(asset.Store.Name)?"":asset.Store.Name),
                new Claim("Type" ,asset.Type.ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["AppSettings:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["AppSettings:Issuer"],
                _config["AppSettings:Issuer"],
                claims,
                expires: DateTime.UtcNow.AddHours(7).Date.AddDays(7),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AssetResponse> DeleteAsset(Guid id)
        {
            var asset = _unitOfWork.Repository<Asset>().GetAll().Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefault();
            if (asset != null)
            {
                asset.IsDeleted = true;
                try
                {
                    await _unitOfWork.Repository<Asset>().UpdateGuid(asset, id);
                    await _unitOfWork.CommitAsync();
                    return new AssetResponse
                    {
                        Name = asset.Name,
                        StoreId = asset.StoreId,
                        Id = asset.Id,
                        Type = asset.Type,
                        IsDeleted = asset.IsDeleted
                    };
                }
                catch (Exception e)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Delete Asset Error!!!", e.InnerException?.Message);
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<PagedResults<AssetResponse>> GetAsset(PagingAssetRequest request, int brandId)
        {
            List<AssetResponse> list = null;
            try
            {
                List<Asset> assets = null;
                if (request.StoreId != 0)
                {
                    assets = await _unitOfWork.Repository<Asset>()
                           .GetAll()
                               .Where(x => x.Name.ToLower()
                           .Contains(request.KeySearch.ToLower())
                           && x.IsDeleted == false && x.StoreId == request.StoreId).ToListAsync();
                }
                else
                {
                    var stores = await _unitOfWork.Repository<Store>().GetAll().Where(x => x.BrandId == brandId
                    && x.Status != (int)Status.Deleted && x.Status != (int)Status.Reject && x.Status != (int)Status.NeedApproval).ToListAsync();
                    assets = await _unitOfWork.Repository<Asset>()
                           .GetAll()
                               .Where(x => x.Name.ToLower()
                           .Contains(request.KeySearch.ToLower())
                           && x.IsDeleted == false).ToListAsync();
                    assets = assets.Where(x => stores.AsEnumerable().Any(f => x.StoreId == f.Id)).ToList();
                }
                if (request.TypeAsset != 0)
                {
                    assets = assets.Where(x => x.Type == (int)request.TypeAsset).ToList();
                }
                IEnumerable<AssetResponse> rs = assets.Select(x => new AssetResponse
                {
                    Name = x.Name,
                    StoreId = x.StoreId,
                    Id = x.Id,
                    Type = x.Type,
                    IsDeleted = x.IsDeleted,
                    StoreName = x.Store?.Name
                }).AsEnumerable();

                list = PageHelper<AssetResponse>.Sorting(request.SortType, rs, request.ColName);
                var result = PageHelper<AssetResponse>.Paging(list, request.Page, request.PageSize);
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Asset Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<AssetResponse> GetAssetByID(Guid id)
        {
            var asset = await _unitOfWork.Repository<Asset>().GetAll().Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
            if (asset != null)
            {
                return new AssetResponse
                {
                    Name = asset.Name,
                    StoreId = asset.StoreId,
                    Id = asset.Id,
                    Type = asset.Type,
                    IsDeleted = asset.IsDeleted,
                    StoreName = asset.Store?.Name
                };
            }
            else
            {
                return null;
            }
        }

        public async Task<PagedResults<LogViolationResponse>> GetAssetLogViolation(PagingRequestLogViolation request, int brandId)
        {
            List<LogViolationResponse> rs = null;
            List<LogViolationResponse> list = null;
            try
            {
                if (request.StoreId != 0)
                {
                    var store = await _unitOfWork.Repository<Store>().GetAll().Where(x => x.Id == request.StoreId
                    && x.Status != (int)Status.Deleted && x.Status != (int)Status.Reject && x.Status != (int)Status.NeedApproval).FirstOrDefaultAsync();
                    if (store == null)
                    {
                        return null;
                    }

                    var assets = store.Assets.Where(x => x.StoreId == store.Id && x.IsDeleted != true).ToList();
                    assets.ForEach(x =>
                    {
                        if (list == null)
                        {
                            list = new List<LogViolationResponse>();
                        }
                        var temp = x.ViolationLogs?.Select(a => new LogViolationResponse
                        {
                            Id = a.Id,
                            AssetId = x.Id,
                            AssetName = x.Name,
                            Description = a.Description,
                            EndTime = a.EndTime,
                            StartTime = a.StartTime,
                            StoreId = x.StoreId,
                            StoreName = x.Store?.Name,
                            Geom = a.Geom?.AsText(),
                            Severity = a.Severity,
                            TypeViolation = a.TypeViolation
                        }).ToList();
                        list.AddRange(temp);
                    });
                }
                else
                {
                    var stores = await _unitOfWork.Repository<Store>().GetAll().Where(x => x.BrandId == brandId
                    && x.Status != (int)Status.Deleted && x.Status != (int)Status.Reject && x.Status != (int)Status.NeedApproval).ToListAsync();
                    if (stores.Count == 0)
                    {
                        return null;
                    }

                    var assets = await _unitOfWork.Repository<Asset>().GetAll().Where(x => !x.IsDeleted).ToListAsync();
                    assets = assets.Where(x => stores.Any(f => x.StoreId == f.Id)).ToList();
                    assets.ForEach(x =>
                    {
                        if (list == null)
                        {
                            list = new List<LogViolationResponse>();
                        }
                        var temp = x.ViolationLogs?.Select(a => new LogViolationResponse
                        {
                            Id = a.Id,
                            AssetId = x.Id,
                            AssetName = x.Name,
                            Description = a.Description,
                            EndTime = a.EndTime,
                            StartTime = a.StartTime,
                            StoreId = x.StoreId,
                            StoreName = x.Store?.Name,
                            Geom = a.Geom?.AsText(),
                            Severity = a.Severity,
                            TypeViolation = a.TypeViolation
                        }).ToList();
                        list.AddRange(temp);
                    });
                }
                rs = PageHelper<LogViolationResponse>.Sorting(request.SortType, list.AsEnumerable(), request.ColName);
                var result = PageHelper<LogViolationResponse>.Paging(rs, request.Page, request.PageSize);
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Asset Log violation Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<List<AssetReportResponse>> GetAssetReport(int brandId)
        {
            var stores = await _unitOfWork.Repository<Store>().GetAll().Where(x => x.BrandId == brandId
                    && x.Status != (int)Status.Deleted && x.Status != (int)Status.Reject && x.Status != (int)Status.NeedApproval).ToListAsync();
            List<AssetReportResponse> list = new List<AssetReportResponse>();
            foreach (var item in Enum.GetValues(typeof(TypeAsset)))
            {
                if ((int)item != 0)
                {
                    var count = await _unitOfWork.Repository<Asset>().GetAll().Where(x => x.Type == (int)item).ToListAsync();
                    count = count.Where(x => stores.Any(f => x.StoreId == f.Id)).ToList();
                    list.Add(new AssetReportResponse
                    {
                        TypeAsset = (TypeAsset)item,
                        Total = count.Count
                    });
                }
            }
            return list;
        }

        public async Task<AssetResponse> PostAsset(AssetRequest model)
        {
            Asset asset = new Asset
            {
                Id = Guid.NewGuid(),
                StoreId = model.StoreId,
                IsDeleted = false,
                Name = model.Name,
                Type = model.Type
            };
            try
            {
                await _unitOfWork.Repository<Asset>().InsertAsync(asset);
                await _unitOfWork.CommitAsync();
                var assetsInsert = await _unitOfWork.Repository<Asset>().GetAll().Where(x => x.Id == asset.Id).Include(s => s.Store).FirstOrDefaultAsync();
                return new AssetResponse
                {
                    Name = assetsInsert.Name,
                    StoreId = assetsInsert.StoreId,
                    Id = assetsInsert.Id,
                    Type = assetsInsert.Type,
                    IsDeleted = assetsInsert.IsDeleted,
                    StoreName = assetsInsert.Store?.Name
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Asset Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<AssetResponse> PutAsset(Guid id, AssetRequest model)
        {
            var asset = await _unitOfWork.Repository<Asset>().GetAll().Where(x => x.Id == id && x.IsDeleted == false).FirstOrDefaultAsync();
            if (asset != null)
            {
                try
                {
                    asset.Name = model.Name;
                    asset.StoreId = model.StoreId;
                    asset.Type = model.Type;
                    await _unitOfWork.Repository<Asset>().UpdateGuid(asset, id);
                    await _unitOfWork.CommitAsync();
                    return new AssetResponse
                    {
                        Name = asset.Name,
                        StoreId = asset.StoreId,
                        Id = asset.Id,
                        Type = asset.Type,
                        IsDeleted = asset.IsDeleted,
                        StoreName = asset.Store?.Name
                    };
                }
                catch (Exception e)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Update Asset Error!!!", e.InnerException?.Message);
                }
            }
            else
            {
                return null;
            }
        }

        public async Task<LogViolationResponse> GetAssetLogViolationById(int id)
        {
            var rs = await _unitOfWork.Repository<ViolationLog>().GetById(id);
            if (rs != null)
            {
                return new LogViolationResponse
                {
                    Id = rs.Id,
                    AssetId = rs.AssetId,
                    AssetName = rs.Asset.Name,
                    Description = rs.Description,
                    EndTime = rs.EndTime,
                    StartTime = rs.StartTime,
                    StoreId = rs.Asset.StoreId,
                    StoreName = rs.Asset.Store?.Name,
                    Severity = rs.Severity,
                    TypeViolation = rs.TypeViolation,
                    Geometry = rs.Geom
                };
            }
            else
            {
                return null;
            }
        }
    }
}