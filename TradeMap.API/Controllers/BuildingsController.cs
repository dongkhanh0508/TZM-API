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
using TradeMap.Service.InterfaceService;
using TradeMap.Service.Servies.InterfaceService;

namespace TradeMap.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class BuildingsController : ControllerBase
    {
        private readonly IBuildingService _buildingService;
        private readonly IStreetSegmentsService _streetSegmentsService;

        public BuildingsController(IBuildingService buildingService, IStreetSegmentsService streetSegmentsService)
        {
            _buildingService = buildingService;
            _streetSegmentsService = streetSegmentsService;
        }

        /// <summary>
        /// Get Building by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<BuildingDetailResponse>> GetBuildingById(int id)
        {
            try
            {
                Guid accountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var rs = await _buildingService.GetBuidingById(id, accountId);
                return Ok(rs);
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Building Error!!!", e.InnerException?.Message);
            }
        }

        /// <summary>
        /// Get Building by store id
        /// </summary>
        /// <param name="storeId"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpGet("store-id/{storeId:int}")]
        public async Task<ActionResult<BuildingDetailResponse>> GetBuildingByStoreId(int storeId)
        {
            try
            {
                var rs = await _buildingService.GetBuidingByStoreId(storeId);
                return Ok(rs);
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Building by store id Error!!!", e.InnerException?.Message);
            }
        }

        /// <summary>
        /// Insert Building
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost]
        public async Task<ActionResult<BuildingResponse>> InsertBuiding([FromBody] PostBuildingRequest request)
        {
            Guid accountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rs = await _buildingService.InsertBuilding(request, accountId);
            return Ok(rs);
        }

        /// <summary>
        /// Put Building
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<BuildingResponse>> PutBuiding(int id, [FromBody] PostBuildingRequest request)
        {
            Guid accountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rs = await _buildingService.PutBuilding(id, request, accountId);
            if (rs == null) return NotFound();
            return Ok(rs);
        }

        /// <summary>
        /// mark need survey for building
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
    //    [Authorize(Roles = Role.Admin)]
        [HttpPut("need-survey/{id:int}")]
        public async Task<ActionResult<BuildingResponse>> ChangeFlagNeedSurvey(int id)
        {
            var rs = await _buildingService.ChangeFlagNeedSurvey(id);
            return Ok(rs);
        }

        /// <summary>
        /// Approve building
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPut("approve-building/{id:int}")]
        public async Task<ActionResult<ApproveBuildingResponse>> ApproveBuilding(int id)
        {
            var rs = await _buildingService.ApproveBuilding(id);
            return Ok(rs);
        }

        /// <summary>
        /// Get buildings of surveyor
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Surveyor + "," + Role.Admin)]
        [HttpGet("building-need-survey/surveyor")]
        public async Task<ActionResult<PagedResults<BuildingResponse>>> GetBuidingNeedSurveyByAccountId([FromQuery] PagingRequestGetSurvey request)
        {
            Guid accountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rs = await _buildingService.GetBuidingNeedSurveyByAccountId(accountId, request);
            return Ok(rs);
        }

        /// <summary>
        /// Delete Building
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Surveyor)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<BuildingResponse>> DeleteBuilding(int id)
        {
            try
            {
                Guid accountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var rs = await _buildingService.DeleteBuilding(id, accountId);
                if (rs == null) return NotFound();
                return Ok(rs);
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Delete Error!!!", e.InnerException?.Message);
            }
        }

        /// <summary>
        /// Get List Type
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpGet("types")]
        public async Task<ActionResult<TypeResponse>> GetTypeBuilding()
        {
            try
            {
                var rs = await _buildingService.GetListTypeBuilding();
                return Ok(rs);
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Type Error!!!", e.InnerException?.Message);
            }
        }

        /// <summary>
        /// Get Street Segment By BuildingId
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}/street-segments")]
        public async Task<ActionResult<ListStreetSegmentResponse>> GetStreetSegmentByBuildingId(int id)
        {
            var rs = await _streetSegmentsService.GetStreetSegmentsByBuildingId(id);
            return Ok(rs);
        }

        /// <summary>
        /// Reject building
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPut("reject-building/{id:int}")]
        public async Task<ActionResult<ApproveBuildingResponse>> RejectBuilding(int id)
        {
            var rs = await _buildingService.RejectBuilding(id);
            return Ok(rs);
        }

        [Authorize(Roles = Role.Admin)]
        [HttpGet("delete-building")]
        public ActionResult DeleteBuilding()
        {
            _buildingService.HardDeleteBuilding();
            return Ok();
        }

        /// <summary>
        /// Post CustomerSegment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Surveyor)]
        [HttpPost("analysis")]
        public async Task<ActionResult<CustomerSegmentResponse>> PostCustomerSegment([FromBody] PostCustomerSegmentRequest model)
        {
            var rs = await _buildingService.PostCustomerSegment(model);
            return Ok(rs);
        }

        /// <summary>
        /// Update CustomerSegment
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="SegmentId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPut("analysis/{buildingId}/{SegmentId}")]
        public async Task<ActionResult<CustomerSegmentResponse>> PutCustomerSegment(int buildingId, int SegmentId, [FromBody] PutCustomerSegmentRequest model)
        {
            var rs = await _buildingService.PutCustomerSegment(buildingId, SegmentId, model);
            return Ok(rs);
        }

        /// <summary>
        /// Delete CustomerSegment
        /// </summary>
        /// <param name="buildingId"></param>
        /// <param name="SegmentId"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Surveyor)]
        [HttpDelete("analysis/{buildingId}/{SegmentId}")]
        public async Task<ActionResult<CustomerSegmentResponse>> DeleteCustomerSegment(int buildingId, int SegmentId)
        {
            var rs = await _buildingService.DeleteCustomerSegment(buildingId, SegmentId);
            return Ok(rs);
        }

        /// <summary>
        /// Get Building Segment by building Id
        /// </summary>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpGet("analysis/{buildingId}")]
        public async Task<ActionResult<List<CustomerSegmentResponse>>> GetSegmentById(int buildingId)
        {
            var rs = await _buildingService.GetCustomerSegmentByID(buildingId);
            return Ok(rs);
        }
    }
}