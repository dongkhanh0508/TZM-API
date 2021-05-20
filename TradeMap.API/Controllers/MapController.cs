using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Helpers;
using TradeMap.Service.Servies.ImplService;
using TradeMap.Service.Servies.InterfaceService;

namespace TradeMap.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class MapController : ControllerBase
    {
        private readonly IMapService _mapService;

        public MapController(IMapService mapService)
        {
            _mapService = mapService;
        }

        /// <summary>
        /// get building for map
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost("building")]
        public async Task<ActionResult<CustomFeatureCollection>> GetBuildingForMap([FromBody] PostMapRequest request)
        {
            try
            {
                int role = Convert.ToInt32(User.FindFirst(ClaimTypes.Role)?.Value);
                var rs = await _mapService.GetBuildingForMap(request.CoordinateString, role);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// get building surveyor by surveyorId for map
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Surveyor)]
        [HttpGet("building/surveyor")]
        public async Task<ActionResult<CustomFeatureCollection>> GetBuildingOfSurveyorForMap()
        {
            Guid surveyorId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rs = await _mapService.GetBuildingOfSurveyorForMap(surveyorId);
            return Ok(rs);
        }

        /// <summary>
        /// get store surveyor by surveyorId for map
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Surveyor)]
        [HttpGet("store/surveyor")]
        public async Task<ActionResult<CustomFeatureCollection>> GetStoreOfSurveyorForMap()
        {
            Guid surveyorId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rs = await _mapService.GetStoreOfSurveyorForMap(surveyorId);
            return Ok(rs);
        }

        /// <summary>
        /// Get Campus for Map
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost("campus")]
        public async Task<ActionResult<CustomFeatureCollection>> GetCampusForMap([FromBody] PostMapRequest request)
        {
            var rs = await _mapService.GetCampusForMap(request.CoordinateString);
            return Ok(rs);
        }

        /// <summary>
        /// get
        /// ct for map
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost("district")]
        public async Task<ActionResult<CustomFeatureCollection>> GetDistrictForMap([FromBody] PostMapRequest request)
        {
            var rs = await _mapService.GetDistrictForMap(request.CoordinateString);
            return Ok(rs);
        }

        /// <summary>
        /// Get Store for map
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
       // [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost("store")]
        public async Task<ActionResult<CustomFeatureCollection>> GetStoreForMap([FromBody] PostMapRequest request)
        {
            int role = Convert.ToInt32(User.FindFirst(ClaimTypes.Role)?.Value);
            var result = await _mapService.GetStoreForMap(request.CoordinateString, role);
            return Ok(result);
        }

        /// <summary>
        /// Get Store by brand id for map
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("store/brand")]
        public async Task<ActionResult<CustomFeatureCollection>> GetStoreByBrandIdForMap()
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value);
            var result = await _mapService.GetStoreByBrandIdForMap(brandId);
            return Ok(result);
        }

        /// <summary>
        /// Get System Zone for Map
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost("system-zone")]
        public async Task<ActionResult<CustomFeatureCollection>> GetSystemZoneForMap([FromBody] PostMapRequest request)
        {
            var rs = await _mapService.GetSystemZoneForMap(request.CoordinateString);
            return Ok(rs);
        }

        /// <summary>
        /// Get System Zone for Map by Surveyor Id
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Surveyor)]
        [HttpGet("system-zone/surveyor")]
        public async Task<ActionResult<CustomFeatureCollection>> GetSystemZoneForMapBySurveyorId()
        {
            Guid surveyorId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rs = await _mapService.GetSystemZoneForMapBySurveyorId(surveyorId);
            return Ok(rs);
        }

        /// <summary>
        /// Get Ward for map
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost("ward")]
        public async Task<ActionResult<CustomFeatureCollection>> GetWadrForMap([FromBody] PostMapRequest request)
        {
            var rs = await _mapService.GetWardBoundaryForMap(request.CoordinateString);
            return Ok(rs);
        }

        /// <summary>
        /// Check Ward contains Multi polygon
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost("ward/check-system-zone")]
        public async Task<ActionResult<WardWithDistrict>> GetWadrForMap([FromBody] PostMultiPolygon requestDTO)
        {
            try
            {
                var rs = await _mapService.GetWardIdByLocation(requestDTO.CoordinateString);
                if (rs != null)
                {
                    return Ok(rs);
                }
                else
                {
                    return NoContent();
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Check New Campus valid
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost("ward/check-new-campus")]
        public ActionResult<bool> CheckNewCampus([FromBody] PostMultiPolygon requestDTO)
        {
            try
            {
                var rs = _mapService.CheckCampus(requestDTO.CoordinateString);
                return rs;
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Check Building Valid
        /// </summary>
        /// <returns></returns>
        ///
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost("ward/check-building-valid")]
        public async Task<ActionResult<CheckBuidlingResponse>> CheckBuilding([FromBody] PostMultiPolygon requestDTO)
        {
            try
            {
                Guid surveyorId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var rs = await _mapService.CheckBuildingInCampus(requestDTO.CoordinateString, surveyorId);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [Authorize(Roles = Role.Brand)]
        [HttpPost("ward/check-group-zone-valid")]
        public ActionResult<bool> CheckGroupZone([FromBody] CheckGroupZoneRequest request)
        {
            try
            {
                int brandId = (bool)(!User.FindFirst("BrandId")?.Value.Equals("")) ? Convert.ToInt32(User.FindFirst("BrandId")?.Value) : 0;
                var rs = _mapService.IsValidGroupZone(request.ListZoneId, brandId, request.Type);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Get Ward for map
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost("ward-full")]
        public async Task<ActionResult<CustomFeatureCollection>> GetWardForMap([FromBody] PostMapRequest request)
        {
            var rs = await _mapService.GetWardForMap(request.CoordinateString);
            return Ok(rs);
        }

        /// <summary>
        /// Check store valid
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpGet("store-valid")]
        public async Task<ActionResult<bool>> CheckStoreValid(string coordinateString)
        {
            Guid surveyorId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rs = await _mapService.CheckStoreInSystemzoneAsync(surveyorId, coordinateString);
            return Ok(rs);
        }
    }
}