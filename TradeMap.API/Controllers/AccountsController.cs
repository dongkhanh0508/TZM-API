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
    public class AccountsController : ControllerBase
    {
        private readonly IAccountsService _accountService;

        public AccountsController(IAccountsService accountService)
        {
            _accountService = accountService;
        }

        /// <summary>
        /// Get Account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin)]
        [HttpGet]
        public async Task<ActionResult<PagedResults<AccountResponse>>> GetAccount([FromQuery] PagingRequest request)
        {
            int brandId = Convert.ToInt32(User.FindFirst("BrandId")?.Value != "" ? User.FindFirst("BrandId")?.Value : "0");
            int role = Convert.ToInt32(User.FindFirst(ClaimTypes.Role)?.Value != "" ? User.FindFirst(ClaimTypes.Role)?.Value : "0");
            if (role == 0)
            {
                brandId = 0;
            }
            var rs = await _accountService.GetAccount(request, brandId);
            return Ok(rs);
        }

        /// <summary>
        /// Get Account By Id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin)]
        [HttpGet("{id}")]
        public async Task<ActionResult<AccountResponse>> GetAccountById(Guid id)
        {
            var rs = await _accountService.GetAccountById(id);
            return Ok(rs);
        }

        /// <summary>
        /// Get All Surveyor
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand)]
        [HttpGet("surveyors")]
        public async Task<ActionResult<SurveyorResponse>> GetSurveyor()
        {
            try
            {
                var rs = await _accountService.GetSurveyor();
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("get-jwt/{id}")]
        public async Task<ActionResult<string>> GetJwt(Guid id)
        {
            var rs = await _accountService.GetJwt(id);
            return Ok(rs);
        }

        /// <summary>
        /// Login
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<ActionResult<string>> AuthenticateWebAdminAsync([FromBody] AuthenticateModelWebAdmin model)
        {
            var authenticateResponse = await _accountService.AuthenticateAsync(model);

            if (authenticateResponse == null)
                return Unauthorized();
            return Ok(authenticateResponse);
        }

        /// <summary>
        /// Verify jwt token
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("verify-jwt")]
        public async Task<ActionResult<VerifyResponse>> VerifyJwtTradeZoneMap([FromBody] VerifyJwtRequest model)
        {
            var rs = await _accountService.VerifyJwtTraddeZoneMap(model.Jwt);
            return Ok(rs);
        }

        /// <summary>
        /// Update Account
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin)]
        [HttpPost]
        public async Task<ActionResult<AccountResponse>> InsertAccount([FromBody] PostAccountRequest request)
        {
            try
            {
                var rs = await _accountService.PostAccount(request);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Update Account
        /// </summary>
        /// <param name="id"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin + "," + Role.Surveyor)]
        [HttpPut("{id}")]
        public async Task<ActionResult<PutAccountResponse>> UpdateAccount(Guid id, [FromBody] PutAccountRequest model)
        {
            try
            {
                Guid CurentAccountId = new Guid(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                var rs = await _accountService.PutAccount(id, model, CurentAccountId);
                return Ok(rs);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Delete Account
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = Role.Brand + "," + Role.Admin)]
        [HttpDelete("{id}")]
        public async Task<ActionResult<AccountResponse>> DeleteAccount(Guid id)
        {
            var rs = await _accountService.DeleteAccount(id);
            if (rs == null) return NoContent();
            else return Ok(rs);
        }
    }
}