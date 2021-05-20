using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceStack.Redis;
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
    public class AssetsController : Controller
    {
        private readonly IAssetsService _assetsService;

        private readonly IRedisClientsManager _distributedCache;

        private readonly IViolationLogService _violationLogService;

        public AssetsController(IAssetsService assetsService, IRedisClientsManager distributedCache, IViolationLogService violationLogService)
        {
            _assetsService = assetsService;
            _distributedCache = distributedCache;
            _violationLogService = violationLogService;
        }

        /// <summary>
        /// Get Asset
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet]
        public async Task<ActionResult<PagedResults<AssetResponse>>> GetAssets([FromQuery] PagingAssetRequest request)
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value);
            var rs = await _assetsService.GetAsset(request, brandId);
            return Ok(rs);
        }

        /// <summary>
        /// Check Asset Violation
        /// </summary>
        /// <returns></returns>
        [HttpGet("Violation")]
        public ActionResult CheckAssetViolation()
        {
            _violationLogService.CheckViolation();
            return Ok();
        }

        ///// <summary>
        ///// Get Asset
        ///// </summary>

        ///// <returns></returns>
        // //  [Authorize(Roles = Role.Brand)]
        //[HttpGet]
        //public ActionResult<PagedResults<AssetResponse>> GetAssets()
        //{
        //    _violationLogService.CheckViolation();
        //    return Ok();
        //}

        /// <summary>
        /// Asset Authen
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        //[Authorize(Roles = "Asset")]
        [HttpPost("authenticator")]
        public async Task<ActionResult<string>> AssetAuthen([FromBody] AssetAuthenRequest request)
        {
            var rs = await _assetsService.AssetAuthen(request);
            return Ok(rs);
        }

        /// <summary>
        /// Get asset violation log
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("violation-logs")]
        public async Task<ActionResult<PagedResults<LogViolationResponse>>> GetAssetsLogViolation([FromQuery] PagingRequestLogViolation request)
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value);
            var rs = await _assetsService.GetAssetLogViolation(request, brandId);
            return Ok(rs);
        }

        /// <summary>
        /// Get amount for unit asset
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("amount-unit")]
        public async Task<ActionResult<List<AssetReportResponse>>> GetAmountUnitAsset()
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value);
            var rs = await _assetsService.GetAssetReport(brandId);
            return Ok(rs);
        }

        /// <summary>
        /// Post Asset
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpPost]
        public async Task<ActionResult<AssetResponse>> PostAsset([FromBody] AssetRequest model)
        {
            var rs = await _assetsService.PostAsset(model);
            return Ok(rs);
        }

        /// <summary>
        /// Update Asset
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin)]
        [HttpPut("{id}")]
        public async Task<ActionResult<AssetResponse>> PutAsset(Guid id, [FromBody] AssetRequest model)
        {
            var rs = await _assetsService.PutAsset(id, model);
            return Ok(rs);
        }

        /// <summary>
        /// Delete Asset
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<AssetResponse>> DeleteAsset(Guid id)
        {
            var rs = await _assetsService.DeleteAsset(id);
            return Ok(rs);
        }

        /// <summary>
        /// Get Asset by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("{id}")]
        public async Task<ActionResult<AssetResponse>> GetAssetById(Guid id)
        {
            var rs = await _assetsService.GetAssetByID(id);
            return Ok(rs);
        }

        /// <summary>
        /// Get Asset by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("violation-logs/{id}")]
        public async Task<ActionResult<LogViolationResponse>> GetAssetLogViolationById(int id)
        {
            var rs = await _assetsService.GetAssetLogViolationById(id);
            return Ok(rs);
        }

        /// <summary>
        /// Set Asset Location
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Asset")]
        [HttpPost("Assets-Location")]
        public ActionResult<AssetRedis> SetAssetLocation([FromBody] AssetLocationRequest request)
        {
            AssetRedis asset = new AssetRedis()
            {
                Id = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value),
                BrandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value),
                StoreId = Convert.ToInt32(User.FindFirst("StoreId")?.Value),
                Name = User.FindFirst(ClaimTypes.Name)?.Value,
                BrandName = User.FindFirst("BrandName")?.Value,
                StoreName = User.FindFirst("StoreName")?.Value,
                Type = Convert.ToInt32(User.FindFirst("Type")?.Value),
                Location = request.Location,
            };
            var rs = RedisHelper.SetAssestPoint(_distributedCache, asset.Id, asset);
            return Ok(rs);
        }

        /// <summary>
        /// Get Assets Location
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("Assets-Location")]
        public ActionResult<List<AssetRedisResponse>> GetAssetByBrandId()
        {
            var brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value);
            var rs = RedisHelper.GetAssetRedis(brandId, _distributedCache);
            return Ok(rs);
        }
    }
}