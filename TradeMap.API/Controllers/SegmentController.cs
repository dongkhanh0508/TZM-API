using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
    public class SegmentController : ControllerBase
    {
        private readonly ISegmentService _SegmentService;

        public SegmentController(ISegmentService SegmentService)
        {
            _SegmentService = SegmentService;
        }

        /// <summary>
        /// Get Segments
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpGet]
        public async Task<ActionResult<List<SegmentResponse>>> GetSegment()
        {
            var rs = await _SegmentService.GetSegment();
            return Ok(rs);
        }

        /// <summary>
        /// Post Segment
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPost]
        public async Task<ActionResult<SegmentResponse>> PostSegment([FromBody] SegmentRequest model)
        {
            var rs = await _SegmentService.PostSegment(model);
            return Ok(rs);
        }

        /// <summary>
        /// Update Segment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpPut("{id}")]
        public async Task<ActionResult<SegmentResponse>> PutSegment(int id, [FromBody] SegmentRequest model)
        {
            var rs = await _SegmentService.PutSegment(id, model);
            return Ok(rs);
        }

        /// <summary>
        /// Delete Segment
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<SegmentResponse>> DeleteSegment(int id)
        {
            var rs = await _SegmentService.DeleteSegment(id);
            return Ok(rs);
        }

        /// <summary>
        /// Get Segment by Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Admin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<SegmentResponse>> GetSegmentById(int id)
        {
            var rs = await _SegmentService.GetSegmentByID(id);
            return Ok(rs);
        }
    }
}