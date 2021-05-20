using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Geometries;
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
    public class SystemZonesController : ControllerBase
    {
        private readonly ISystemZoneService _systemZoneService;
        private readonly IAccountsService _accountsService;

        public SystemZonesController(ISystemZoneService systemZoneService, IAccountsService accountsService)
        {
            _systemZoneService = systemZoneService;
            _accountsService = accountsService;
        }

        /// <summary>
        /// get system-zone
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Surveyor + "," + Role.Admin)]
        [HttpGet]
        public async Task<ActionResult<PagedResults<SystemZoneResponse>>> GetSystemZone([FromQuery] SystemZonePagingRequest request)
        {
            Guid accountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rs = await _systemZoneService.GetSystemZone(request, accountId);
            return Ok(rs);
        }

        /// <summary>
        /// get system-zone
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Surveyor + "," + Role.Admin)]
        [HttpGet("building-store")]
        public async Task<ActionResult<List<StoreBuildingResponse>>> GetStoreBuildingBySystemZoneId([FromQuery] GetStoreBuildingBySystemZoneIdRequest request)
        {
            var rs = await _systemZoneService.GetStoreBuildingBySystemZoneId(request);
            return Ok(rs);
        }

        /// <summary>
        /// Insert system-zone
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        public async Task<ActionResult<SystemZoneResponse>> InsertSystemZone([FromBody] PostSystemZoneRequset model)
        {
            var rs = await _systemZoneService.PostSystemZone(model);

            return Ok(rs);
        }

        /// <summary>
        /// Update system-zone
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPut("{id:int}")]
        public async Task<ActionResult<SystemZoneResponse>> UpdateSystemZone(int id, [FromBody] PutSystemZone model)
        {
            var rs = await _systemZoneService.PutSystemZone(id, model);
            return Ok(rs);
        }

        /// <summary>
        /// Delete system-zone
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id:int}")]
        public async Task<ActionResult<SystemZoneResponse>> DeleteSystemZone(int id)
        {
            var rs = await _systemZoneService.DeleteSystemZone(id);
            return Ok(rs);
        }

        /// <summary>
        /// Get Surveyors by SystemzoneId
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("surveyors")]
        public async Task<ActionResult<List<AccountResponse>>> GetSurveyor(int id)
        {
            var rs = await _accountsService.GetAccountSurveyBySystemzoneId(id);
            // _systemZoneService.SendNotification(id);
            return Ok(rs);
        }

        /// <summary>
        /// Administrator Assign system-zone
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPost("assign-surveyor")]
        public async Task<ActionResult<AssignSystemZoneRespones>> AssginSystemZone([FromBody] PostAssignSystemZone model)
        {
            var rs = await _systemZoneService.PostAssignSystemZone(model.AccountId, model.SystemZoneId);
            return Ok(rs);
        }

        /// <summary>
        /// Administrator Remove Assign system-zone
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="systemZoneId"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{accountId}/{systemZoneId}")]
        public async Task<ActionResult<AssignSystemZoneRespones>> DeleteAssginSystemZone(Guid accountId, int systemZoneId)
        {
            var rs = await _systemZoneService.DeleteAssignSystemZone(accountId, systemZoneId);
            if (rs != null) return Ok(rs);
            else return NotFound();
        }

        /// <summary>
        /// Get Free Surveyor
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("free-surveyors")]
        public async Task<ActionResult<SurveyorResponse>> GetFreeSurveyor(int systemzoneId)
        {
            try
            {
                var rs = await _accountsService.GetFreeSurveyor(systemzoneId);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Get systemzone by id
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpGet("{id:int}")]
        public async Task<ActionResult<SystemZoneResponse>> GetSystemzoneById(int id)
        {
            var rs = await _systemZoneService.GetSystemZoneById(id);
            return Ok(rs);
        }

        /// <summary>
        /// Check systemzone close by WardId
        /// </summary>
        /// <returns></returns>
        //[Authorize(Roles = Role.Admin)]
        [HttpGet("check-close")]
        public async Task<ActionResult<Geometry>> CheckSystemzoneCloseByWardId(int id)
        {
            var rs = await _systemZoneService.CheckSystemZoneClose(id);
            return Ok(rs);
        }

        /// <summary>
        /// Check Systemzone Full Fill Ward
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("check-full-fill")]
        public async Task<ActionResult<bool>> CheckSystemzoneFullFillWard(int id)
        {
            var rs = await _systemZoneService.CheckSystemzoneFillWard(id);
            return Ok(rs);
        }

        /// <summary>
        /// get building by systemzone id
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Surveyor)]
        [HttpGet("{id:int}/buildings")]
        public async Task<ActionResult<PagedResults<BuildingResponse>>> GetBuildingBySystemZoneId([FromQuery] PagingRequestGetSurvey model, int id)
        {
            Guid curentAccountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rs = await _systemZoneService.GetBuidingBySystemzoneId(model, id, curentAccountId);
            return Ok(rs);
        }

        /// <summary>
        /// get stores by systemzone id
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Surveyor)]
        [HttpGet("{id:int}/stores")]
        public async Task<ActionResult<PagedResults<StoreResponse>>> GetStoreBySystemZoneId([FromQuery] PagingRequestGetSurvey model, int id)
        {
            Guid curentAccountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rs = await _systemZoneService.GeStoreBySystemzoneId(model, id, curentAccountId);
            return Ok(rs);
        }
    }
}