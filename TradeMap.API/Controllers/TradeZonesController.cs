using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
    public class TradeZonesController : Controller
    {
        private readonly ITradeZoneService _tradeZoneService;

        public TradeZonesController(ITradeZoneService tradeZoneService)
        {
            _tradeZoneService = tradeZoneService;
        }

        /// <summary>
        /// Group system-zone
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpPost("group-systemzones")]
        public async Task<ActionResult<StoreTradeZoneResponse>> GroupSystemZones([FromBody] InitTradezoneRequest request)
        {
            int brandId = (bool)(!User.FindFirst("BrandId")?.Value.Equals("")) ? Convert.ToInt32(User.FindFirst("BrandId")?.Value) : 0;
            var rs = await _tradeZoneService.GroupSystemzoneForStoreByDistance(brandId, request.Distance, request.TimeSlot, request.StoresId);
            return Ok(rs);
        }

        /// <summary>
        /// Insert system-zone
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpPost]
        public async Task<ActionResult<List<TradezoneResponse>>> CreateTradezone([FromBody] List<TradezoneRequest> model)
        {
            int brandId = (bool)(!User.FindFirst("BrandId")?.Value.Equals("")) ? Convert.ToInt32(User.FindFirst("BrandId")?.Value) : 0;
            var rs = await _tradeZoneService.InsertTradezone(model, brandId);
            return Ok(rs);
        }
    }
}