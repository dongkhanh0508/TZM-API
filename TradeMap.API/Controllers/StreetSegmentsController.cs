using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Helpers;
using TradeMap.Service.Servies.InterfaceService;

namespace TradeMap.API.Controllers
{
    [Route(Helpers.SettingVersionApi.ApiVersion)]
    [ApiController]
    public class StreetSegmentsController : Controller
    {
        private readonly IStreetSegmentsService _streetSegmentsService;

        public StreetSegmentsController(IStreetSegmentsService streetSegmentsService)
        {
            _streetSegmentsService = streetSegmentsService;
        }

        /// <summary>
        /// Insert Street Segment
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost]
        public async Task<ActionResult<StreetSegmentResponse>> InsertStreetSegment([FromBody] PostStreetSegmentRequest model)
        {
            var rs = await _streetSegmentsService.PostStreetSegment(model);
            return Ok(rs);
        }

        /// <summary>
        /// Get Street Segment in radius by Location
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost("map")]
        public async Task<ActionResult<ListStreetSegmentResponse>> GetSegment([FromBody] PostMultiPolygon requestDTO)
        {
            var rs = await _streetSegmentsService.GetStreetSegmentInRadius(requestDTO.CoordinateString);
            return Ok(rs);
        }

        /// <summary>
        /// Get Street Segment in radius by Location's Store
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPost("store")]
        public async Task<ActionResult<ListStreetSegmentResponse>> GetStreetSegmentForStore([FromBody] PostMultiPolygon requestDTO)
        {
            var rs = await _streetSegmentsService.GetStreetSegmentInRadiusPoint(requestDTO.CoordinateString);
            return Ok(rs);
        }
    }
}