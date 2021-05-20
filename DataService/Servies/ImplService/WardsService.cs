using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TradeMap.Data.Entity;
using TradeMap.Data.UnitOfWork;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Servies.InterfaceService;

namespace TradeMap.Service.Servies.ImplService
{
    public class WardsService : IWardsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public WardsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<List<ProvinceResponse>> GetWard()
        {
            return await _unitOfWork.Repository<Province>().GetAll().Where(x => x.Id == 60)
                .Select(x => new ProvinceResponse
                {
                    Id = x.Id,
                    Name = x.Name,
                    Districts = new List<DistrictResponse>
                (x.Districts.OrderBy(x => x.Name).Select(d => new DistrictResponse { Id = d.Id, Name = d.Name, Wards = new List<WardResponse>(d.Wards.Select(w => new WardResponse { Id = w.Id, Name = w.Name }).ToList()) }))
                }).ToListAsync();
        }
    }
}