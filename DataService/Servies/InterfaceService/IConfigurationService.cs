using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.Servies.InterfaceService
{
    public interface IConfigurationService
    {
        Task<List<ConfigurationResponse>> GetConfiguration(int version);
        Task<ConfigurationResponse> PutConfiguration(int id, ConfigurationRequest model);
        Task<bool> ChangeVersion(ChangeVersionConfig model);
        Task<ListVersionConfigResponse> GetListVersion();
    }
}
