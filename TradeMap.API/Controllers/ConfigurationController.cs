using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Helpers;
using TradeMap.Service.Servies.InterfaceService;

namespace TradeMap.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class ConfigurationController : Controller
    {
        private readonly IConfigurationService _configurationService;

        public ConfigurationController(IConfigurationService configurationService)
        {
            _configurationService = configurationService;
        }

        /// <summary>
        /// Get Configs
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet]
        public async Task<ActionResult<List<ConfigurationResponse>>> GetConfigs(int version)
        {
            var rs = await _configurationService.GetConfiguration(version);
            return Ok(rs);
        }
        /// <summary>
        /// Get List version
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("list-version")]
        public async Task<ActionResult<ListVersionConfigResponse>> GetListVersion()
        {
            var rs = await _configurationService.GetListVersion();
            return Ok(rs);
        }


        /// <summary>
        /// Update Config
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPut("{id}")]
        public async Task<ActionResult<ConfigurationResponse>> UpdateConfig(int id, [FromBody] ConfigurationRequest model)
        {
            var rs = await _configurationService.PutConfiguration(id, model);
            return Ok(rs);
        }
        /// <summary>
        /// Change Version
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPut("change-version")]
        public async Task<ActionResult<ConfigurationResponse>> ChangeVersion([FromBody] ChangeVersionConfig model)
        {
            var rs = await _configurationService.ChangeVersion(model);
            return Ok(rs);
        }

    }
}