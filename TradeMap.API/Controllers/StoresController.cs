using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Exceptions;
using TradeMap.Service.Helpers;
using TradeMap.Service.Servies.InterfaceService;

namespace TradeMap.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class StoresController : ControllerBase
    {
        private readonly IStoresService _storesService;
        private readonly IStreetSegmentsService _streetSegmentsService;

        public StoresController(IStoresService storesService, IStreetSegmentsService streetSegmentsService)
        {
            _storesService = storesService;
            _streetSegmentsService = streetSegmentsService;
        }

        /// <summary>
        /// Insert Store
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost]
        public async Task<ActionResult<StoreResponse>> InsertStore([FromBody] PostStoresRequest model)
        {
            //insert new store

            Guid accountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rs = await _storesService.PostStore(accountId, model);
            return Ok(rs);
        }

        /// <summary>
        /// Get Store of Brand
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpGet("brand")]
        public async Task<ActionResult<List<StoreBrandResponse>>> GetStoreByBrandId()
        {
            try
            {
                int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value);
                var rs = await _storesService.GetStoreByBrandId(brandId);
                return Ok(rs);
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Store Error!!!", e.InnerException?.Message);
            }
        }

        /// <summary>
        /// Get Store by id
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<StoreDetailResponse>> GetStoreById(int id)
        {
            try
            {
                Guid accountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                int role = Convert.ToInt32(User.FindFirst(ClaimTypes.Role)?.Value);
                var rs = await _storesService.GetStoreById(id, accountId, role);
                return Ok(rs);
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Store Error!!!", e.InnerException?.Message);
            }
        }

        /// <summary>
        /// Update Store
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<StoreResponse>> UpdateStore(int id, [FromBody] PostStoresRequest model)
        {
            Guid accountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rs = await _storesService.PutStore(id, accountId, model);
            if (rs == null) return NotFound();
            return Ok(rs);
        }

        /// <summary>
        /// Update Store For Brand
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpPut("for-brand/{id:int}")]
        public async Task<ActionResult<StoreResponse>> UpdateStoreForBrand(int id, [FromBody] PutStoresForBrandRequest model)
        {
            Guid accountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            int brandId = (bool)(!User.FindFirst("BrandId")?.Value.Equals("")) ? Convert.ToInt32(User.FindFirst("BrandId")?.Value) : 0;
            var rs = await _storesService.PutStoreForBrand(id, accountId, brandId, model);
            if (rs == null) return NotFound();
            return Ok(rs);
        }

        /// <summary>
        /// Delete Store
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<StoreResponse>> DeleteStore(int id)
        {
            int role = Convert.ToInt32(User.FindFirst(ClaimTypes.Role)?.Value);
            int brandId = (bool)(!User.FindFirst("BrandId")?.Value.Equals("")) ? Convert.ToInt32(User.FindFirst("BrandId")?.Value) : 0;
            Guid accountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rs = await _storesService.DeleteStore(id, accountId, role, brandId);
            if (rs.Id == 0) return NotFound();
            return Ok(rs);
        }

        /// <summary>
        /// Get Store by need survey by accountId
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Surveyor)]
        [HttpGet("store-need-survey/surveyor")]
        public async Task<ActionResult<PagedResults<StoreResponse>>> GetStoreNeedSurveyByAccountId([FromQuery] PagingRequestGetSurvey request)
        {
            Guid accountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rs = await _storesService.GetStoreNeedSurveyByAccountId(accountId, request);
            return Ok(rs);
        }

        /// <summary>
        /// mark need survey for store
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPut("need-survey/{id:int}")]
        public async Task<ActionResult<StoreResponse>> ChangeFlagNeedSurvey(int id)
        {
            var rs = await _storesService.ChangeFlagNeedSurvey(id);
            return Ok(rs);
        }

        /// <summary>
        /// Approve Store
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPut("approve-store/{id:int}")]
        public async Task<ActionResult<ApproveStoreResponse>> ApproveStore(int id)
        {
            var rs = await _storesService.ApproveStore(id);
            return Ok(rs);
        }

        /// <summary>
        /// Reject Store
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPut("reject-store/{id:int}")]
        public async Task<ActionResult<ApproveStoreResponse>> RejectStore(int id)
        {
            var rs = await _storesService.RejectStore(id);
            return Ok(rs);
        }

        /// <summary>
        /// Get Store type
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpGet("types")]
        public async Task<ActionResult<string>> GetTypes()
        {
            var rs = await _storesService.GetStoreTypes();
            return Ok(rs);
        }

        /// <summary>
        /// Get building in radius by Location's Store
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost("buildings")]
        public async Task<ActionResult<List<BuildingDetailResponse>>> GetBuildingNearStore([FromBody] PostMultiPolygon request)
        {
            float radius = 500;
            List<BuildingDetailResponse> rs = await _storesService.GetBuildingByStoreLocation(request.CoordinateString, radius);
            return Ok(rs);
        }

        /// <summary>
        /// Get StreetSegment By BuildingId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpGet("{id:int}/street-segments")]
        public async Task<ActionResult<ListStreetSegmentResponse>> GetStreetSegmentByBuildingId(int id)
        {
            var rs = await _streetSegmentsService.GetStreetSegmentsByStoreId(id);
            return Ok(rs);
        }

        /// <summary>
        /// Get Store order by location
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost("store-order")]
        public async Task<ActionResult<StoreResponse>> GetStoreOrder(StoreOrderRequest request)
        {
            int brandId = (bool)(!User.FindFirst("BrandId")?.Value.Equals("")) ? Convert.ToInt32(User.FindFirst("BrandId")?.Value) : 0;
            var rs = await _storesService.GetStoreOrder(brandId, request.CoordinateString, request.TimeOrder, request.DateOrder);
            return Ok(rs);
        }




    }
}