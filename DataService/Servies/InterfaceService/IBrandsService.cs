using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.Servies.InterfaceService
{
    public interface IBrandsService
    {
        Task<List<BrandsResponse>> GetBrands();
        Task<BrandsResponse> PostBrand(PostBrandRequest model, int role);
        Task<BrandsResponse> PutBrand(int id, PostBrandRequest model);
        Task<BrandsResponse> DeleteBrand(int id);
        Task<BrandsResponse> GetBrandByID(int id);
    }
}