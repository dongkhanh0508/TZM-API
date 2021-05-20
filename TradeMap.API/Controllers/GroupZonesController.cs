using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Helpers;
using TradeMap.Service.Servies.ImplService;

namespace TradeMap.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class GroupZonesController : Controller
    {
        private readonly IGroupZoneServices _groupZoneServices;

        public GroupZonesController(IGroupZoneServices groupZoneServices)
        {
            _groupZoneServices = groupZoneServices;
        }

        /// <summary>
        /// Get GroupZone
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet]
        public async Task<ActionResult<CustomFeatureCollection>> GetGroupZone()
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value);
            var rs = await _groupZoneServices.GetGroupZone(brandId);
            return Ok(rs);
        }

        /// <summary>
        /// Post GroupZone
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpPost]
        public async Task<ActionResult<GroupZoneResponse>> PostGroupZone([FromBody] GroupZoneRequest model)
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value);
            var rs = await _groupZoneServices.PostGroupZone(model, brandId);
            return Ok(rs);
        }

        /// <summary>
        /// Update GroupZone
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpPut("{id}")]
        public async Task<ActionResult<GroupZoneResponse>> PutGroupZone(int id, [FromBody] PutGroupZoneRequest model)
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value);
            var rs = await _groupZoneServices.PutGroupZone(id, model, brandId);
            return Ok(rs);
        }

        /// <summary>
        /// Delete GroupZone
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<GroupZoneResponse>> DeleteGroupZone(int id)
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value);
            var rs = await _groupZoneServices.DeleteGroupZone(id, brandId);
            return Ok(rs);
        }

        /// <summary>
        /// Get GroupZone by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("{id}")]
        public async Task<ActionResult<GroupZoneResponse>> GetGroupZoneById(int id)
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value);
            var rs = await _groupZoneServices.GetGroupZoneByID(id, brandId);
            return Ok(rs);
        }

        /// <summary>
        /// Get Store TradeZone by GroupZone Id
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("{id}/stores-tradezones")]
        public async Task<ActionResult<StoreTradeZoneForMapResponse>> GetStoresByGroupZoneId(int id, [FromQuery] int tradeverisonId)
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value);
            var rs = await _groupZoneServices.GetStoresByGroupZoneId(id, brandId, tradeverisonId);
            return Ok(rs);
        }

        /// <summary>
        /// Get Tradezone by Groupzone Id
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("{id}/trade-zones")]
        public async Task<ActionResult<List<ZoneResponse>>> GetTradeZoneByGroupZoneId(int id, int tradezoneVersionId)
        {
            var rs = await _groupZoneServices.GetTradeZoneByGroupZoneId(id, tradezoneVersionId);
            return Ok(rs);
        }

        /// <summary>
        /// Get  Free Ward
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("free-wards")]
        public async Task<ActionResult<CustomFeatureCollection>> GetFreeWards()
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value);
            var rs = await _groupZoneServices.GetFreeWard(brandId);
            return Ok(rs);
        }

        /// <summary>
        /// Get  Free 
        /// ct
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("free-districts")]
        public async Task<ActionResult<CustomFeatureCollection>> GetFreeDistricts()
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value);
            var rs = await _groupZoneServices.GetFreeDistrict(brandId);
            return Ok(rs);
        }

        /// <summary>
        /// Get  Free Systemzone
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("free-systemzones")]
        public async Task<ActionResult<CustomFeatureCollection>> GetFreeSystemzones()
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value);
            var rs = await _groupZoneServices.GetFreeSystemzone(brandId);
            return Ok(rs);
        }
    }
}