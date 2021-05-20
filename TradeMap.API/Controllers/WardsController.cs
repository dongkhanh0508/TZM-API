using Microsoft.AspNetCore.Mvc;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Servies.InterfaceService;

namespace TradeMap.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class WardsController : Controller
    {
        private readonly IWardsService _wardsService;

        public WardsController(IWardsService wardsService)
        {
            _wardsService = wardsService;
        }

        /// <summary>
        /// Get Ward
        /// </summary>
        /// <returns></returns>
     //   [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpGet]
        public ActionResult<ProvinceResponse> GetWards()
        {
            try
            {
                var rs = _wardsService.GetWard();
                return Ok(rs.Result);
            }
            catch (System.Exception)
            {
                return BadRequest();
            }
        }
    }
}