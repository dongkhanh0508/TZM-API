using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
    public class HistoryController : Controller
    {
        private readonly IHistoryService _historyService;

        public HistoryController(IHistoryService historyService)
        {
            _historyService = historyService;
        }

        /// <summary>
        /// Get History
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpGet]
        public async Task<ActionResult<PagedResults<HistoryResponse>>> GetHistory([FromQuery] PagingRequestHistory request)
        {
            int role = Convert.ToInt32(User.FindFirst(ClaimTypes.Role)?.Value);
            Guid accountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
            var rs = await _historyService.GetHistory(request, role, accountId);
            return Ok(rs);
        }
        /// <summary>
        /// Archive History
        /// </summary>
        /// <returns></returns>
        // [Authorize(Roles = Role.Surveyor + "," + Role.Admin)]
        [HttpGet("archive")]
        public ActionResult ArchiveHistory()
        {
            _historyService.ArchiveHistory();
            return Ok();
        }
    }
}