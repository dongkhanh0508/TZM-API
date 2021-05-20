using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Helpers;
using TradeMap.Service.Servies.InterfaceService;

namespace TradeMap.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class BrandsController : Controller
    {
        private readonly IBrandsService _brandsService;
        private readonly IStoresService _storesService;

        public BrandsController(IBrandsService brandsService, IStoresService storesService)
        {
            _brandsService = brandsService;
            _storesService = storesService;
        }

        /// <summary>
        /// Get Brands
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<List<BrandsResponse>>> GetBrands()
        {
            var rs = await _brandsService.GetBrands();
            return Ok(rs);
        }

        /// <summary>
        /// Post Brand
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost]
        public async Task<ActionResult<BrandsResponse>> PostBrand([FromBody] PostBrandRequest model)
        {
            int role = Convert.ToInt32(User.FindFirst(ClaimTypes.Role)?.Value);
            var rs = await _brandsService.PostBrand(model, role);
            return Ok(rs);
        }

        /// <summary>
        /// Update Brand
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin)]
        [HttpPut("{id}")]
        public async Task<ActionResult<BrandsResponse>> PutBrand(int id, [FromBody] PostBrandRequest model)
        {
            var rs = await _brandsService.PutBrand(id, model);
            return Ok(rs);
        }

        /// <summary>
        /// Delete Brand
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<BrandsResponse>> DeleteBrand(int id)
        {
            var rs = await _brandsService.DeleteBrand(id);
            return Ok(rs);
        }

        /// <summary>
        /// Get Brand by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + "Asset")]
        [HttpGet("{id}")]
        public async Task<ActionResult<BrandsResponse>> GetBrandById(int id)
        {
            var rs = await _brandsService.GetBrandByID(id);
            return Ok(rs);
        }

        /// <summary>
        /// Get Store by BrandId
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpGet("{id:int}/stores")]
        public async Task<ActionResult<StoreBrandResponse>> GetStoreByBrandId(int id)
        {
            var rs = await _storesService.GetStoreByBrandId(id);
            return Ok(rs);
        }

        /// <summary>
        /// Get Store by BrandId
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("{id:int}/stores-asset")]
        public async Task<ActionResult<StoreBrandResponse>> GetStoreByBrandIdAnonymous(int id)
        {
            var rs = await _storesService.GetStoreByBrandId(id);
            return Ok(rs);
        }

        /// <summary>
        /// Get Store By BrandId fil By TimeSlot
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("stores-time-slot")]
        public async Task<ActionResult<StoreTimeSlotResponse>> GetStoreByBrandIdFilByTimeSlot()
        {
            int brandId = (bool)(!User.FindFirst("BrandId")?.Value.Equals("")) ? Convert.ToInt32(User.FindFirst("BrandId")?.Value) : 0;
            var rs = await _storesService.GetStoreTimeslot(brandId);
            return Ok(rs);
        }
    }
}