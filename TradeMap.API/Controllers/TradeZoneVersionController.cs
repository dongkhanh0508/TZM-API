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
    public class TradeZoneVersionsController : Controller
    {
        private readonly ITradeZoneVersionServices _tradeZoneVersionServices;

        public TradeZoneVersionsController(ITradeZoneVersionServices tradeZoneVersionServices)
        {
            _tradeZoneVersionServices = tradeZoneVersionServices;
        }

        /// <summary>
        /// Create Tradezone version
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpPost]
        public async Task<ActionResult<TradeZoneVersionResponse>> CreateTradezone([FromBody] PostTradeZoneVerison model)
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value != "" ? User.FindFirst("BrandId")?.Value : "0");
            var result = await _tradeZoneVersionServices.CreateTradeZoneVersion(model, brandId);
            return Ok(result);
        }

        /// <summary>
        /// Get List Tradezone version
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet()]
        public async Task<ActionResult<TradeZoneVersionResponse>> GetTradezoneVersionById([FromQuery] string timeSlot = "1111", String dateFilter = "1111111")
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value != "" ? User.FindFirst("BrandId")?.Value : "0");
            var result = await _tradeZoneVersionServices.GetTradeZoneVersionByBrandId(brandId, dateFilter, timeSlot);
            return Ok(result);
        }

        /// <summary>
        /// Get Tradezone version
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<List<TradeZoneVersionDetailResponse>>> GetTradezoneVersionById(int id)
        {
            var result = await _tradeZoneVersionServices.GetTradeZoneVersionById(id);
            return Ok(result);
        }

        /// <summary>
        /// Get groupzone by Tradezone version id
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("{id:int}/group-zones")]
        public async Task<ActionResult<List<ZoneResponse>>> GetGroupzoneByTradezoneVersionId(int id)
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value != "" ? User.FindFirst("BrandId")?.Value : "0");
            var result = await _tradeZoneVersionServices.GetGroupZoneByTradeZoneVersionId(id, brandId);
            return Ok(result);
        }

        /// <summary>
        /// Get Tradezone version active
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("active-version")]
        public async Task<ActionResult<TradeZoneVersionDetailResponse>> GetTradezoneVersionActive()
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value != "" ? User.FindFirst("BrandId")?.Value : "0");
            var result = await _tradeZoneVersionServices.GetTradeZoneVersionActive(brandId);
            return Ok(result);
        }

        /// <summary>
        /// Active Tradezone version
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<TradeZoneVersionResponse>> ChangeFlagActive(int id)
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value != "" ? User.FindFirst("BrandId")?.Value : "0");
            var result = await _tradeZoneVersionServices.ChangeFlagActiveVersion(brandId, id);
            return Ok(result);
        }

        /// <summary>
        /// Active Tradezone version
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<TradeZoneVersionResponse>> DeleteTradezoneVersion(int id)
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value != "" ? User.FindFirst("BrandId")?.Value : "0");
            var result = await _tradeZoneVersionServices.DeleteTradeZoneVersionById(brandId, id);
            return Ok(result);
        }

        /// <summary>
        /// Get GroupZone, Store, Tradezone in Tradezone Version Active
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("trade-zones-version-active/group-zones")]
        public async Task<ActionResult<List<GroupStoreTradeZoneResponse>>> GetGroupStoreTradeZone()
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value != "" ? User.FindFirst("BrandId")?.Value : "0");
            var result = await _tradeZoneVersionServices.GetListGroupStoreTradeZoneByTradeZoneVersionId(brandId);
            return Ok(result);
        }
    }
}