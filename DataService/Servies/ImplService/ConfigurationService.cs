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
    public class ConfigurationService : IConfigurationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ConfigurationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<bool> ChangeVersion(ChangeVersionConfig model)
        {
            try
            {
                var rs = await _unitOfWork.Repository<Config>().GetAll().Where(a => a.Version == model.Version)
                    .ToListAsync();
                var rs1 = await _unitOfWork.Repository<Config>().GetAll().Where(a => a.Version != model.Version)
                    .ToListAsync();
                foreach (var item in rs)
                {
                    item.Active = true;
                }
                foreach (var item in rs1)
                {
                    item.Active = false;
                }
                _unitOfWork.Repository<Config>().UpdateRange(rs.AsQueryable());
                _unitOfWork.Repository<Config>().UpdateRange(rs1.AsQueryable());
                await _unitOfWork.CommitAsync();
                return true;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update Version Config Error!!!", e.InnerException?.Message);
            }

        }

        public async Task<List<ConfigurationResponse>> GetConfiguration(int version)
        {
            if (version > 0)
            {
                return await _unitOfWork.Repository<Config>().GetAll().Where(a => a.Version == version)
                .Select(x => new ConfigurationResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Value,
                    Version = x.Version,
                    Active = x.Active,
                    Description = x.Description
                }).ToListAsync();
            }
            else if (version == 0)
            {
                var rs = await _unitOfWork.Repository<Config>().GetAll().Where(x => x.Active == true).FirstOrDefaultAsync();
                return await _unitOfWork.Repository<Config>().GetAll().Where(x => x.Version == rs.Version)
                .Select(x => new ConfigurationResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Value,
                    Version = x.Version,
                    Active = x.Active,
                    Description = x.Description
                }).ToListAsync();
            }
            else
            {

                return await _unitOfWork.Repository<Config>().GetAll()
                .Select(x => new ConfigurationResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Value = x.Value,
                    Version = x.Version,
                    Active = x.Active,
                    Description = x.Description
                }).ToListAsync();
            }

        }

        public async Task<ListVersionConfigResponse> GetListVersion()
        {
            List<int> list = null;
            var rs = _unitOfWork.Repository<Config>().GetAll().AsEnumerable().GroupBy(x => x.Version).ToList();
            foreach (var item in rs)
            {
                if (list == null)
                {
                    list = new List<int>();
                }
                list.Add(item.Key);
            }
            var currentActive = await _unitOfWork.Repository<Config>().GetAll().Where(x => x.Active == true).FirstOrDefaultAsync();
            return new ListVersionConfigResponse
            {
                CurrentActive = currentActive.Version,
                VersionList = list
            };
        }

        public async Task<ConfigurationResponse> PutConfiguration(int id, ConfigurationRequest model)
        {
            var config = await _unitOfWork.Repository<Config>().GetById(id);
            if (config != null)
            {
                try
                {
                    config.Value = model.Value;
                    await _unitOfWork.Repository<Config>().Update(config, id);
                    await _unitOfWork.CommitAsync();
                    return new ConfigurationResponse
                    {
                        Id = config.Id,
                        Name = config.Name,
                        Value = config.Value,
                        Version = config.Version,
                        Active = config.Active,
                        Description = config.Description
                    };
                }
                catch (Exception e)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Update Config Error!!!", e.InnerException?.Message);
                }
            }
            else
            {
                return null;
            }
        }
    }
}
