using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Helpers;
using TradeMap.Service.Servies.InterfaceService;

namespace TradeMap.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class CampusController : Controller
    {
        private readonly ICampusService _campusService;

        public CampusController(ICampusService campusService)
        {
            _campusService = campusService;
        }

        /// <summary>
        /// Insert Campus
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        public async Task<ActionResult<CampusResponse>> InsertCampus([FromBody] PostCampusRequest model)
        {
            var rs = await _campusService.CreateCampus(model);
            return Ok(rs);
        }

        /// <summary>
        /// Delete campus
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<CampusResponse>> DeleteCampus(int id)
        {
            var rs = await _campusService.DeleteCampus(id);
            if (rs == null) return NotFound();
            return Ok(rs);
        }
        /// <summary>
        /// Update campus
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<CampusResponse>> PutCampus(int id, [FromBody] PutCampusRequest model)
        {
            var rs = await _campusService.PutCampus(id, model);
            if (rs == null) return NotFound();
            return Ok(rs);
        }

        /// <summary>
        /// Get campus by id
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<CampusResponse>> GetSystemzoneById(int id)
        {
            var rs = await _campusService.GetCampusById(id);
            return Ok(rs);
        }
    }
}