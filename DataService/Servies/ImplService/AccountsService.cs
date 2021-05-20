using FirebaseAdmin.Auth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TradeMap.Data.Entity;
using TradeMap.Data.UnitOfWork;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;
using TradeMap.Service.Exceptions;
using TradeMap.Service.Helpers;
using TradeMap.Service.Servies.InterfaceService;

namespace TradeMap.Service.Servies.ImplService
{
    public class AccountsService : IAccountsService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public AccountsService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        public async Task<string> AuthenticateAsync(AuthenticateModelWebAdmin model)
        {
            FirebaseToken decodedToken = null;
            try
            {
                decodedToken = await FirebaseAuth.DefaultInstance
        .VerifyIdTokenAsync(model.IdToken);
            }
            catch (Exception)
            {
                throw new FirebaseException(HttpStatusCode.Forbidden, "Unauthorized access to TradeZoneMap!!!");
            }
            string firebaseUid = decodedToken.Uid;
            string email = "";
            string phoneNumber = "";
            string imageUrl = "";
            decodedToken.Claims.TryGetValue("firebase", out object tmp);
            decodedToken.Claims.TryGetValue("email", out tmp);
            if (tmp != null)
            {
                email = tmp.ToString();
            }

            decodedToken.Claims.TryGetValue("phoneNumber", out tmp);
            if (tmp != null)
            {
                phoneNumber = tmp.ToString();
            }

            decodedToken.Claims.TryGetValue("picture", out tmp);
            if (tmp != null)
            {
                imageUrl = tmp.ToString();
            }

            decodedToken.Claims.TryGetValue("name", out tmp);
            string name = tmp.ToString();
            Account account = await _unitOfWork.Repository<Account>().GetAll().Where(x => x.Email == email && x.Active == true).FirstOrDefaultAsync();
            if (account == null)
            {
                throw new FirebaseException(HttpStatusCode.Forbidden, "Unauthorized access to TradeZoneMap!!!");
            }
            else
            {

                account.FcmToken = model.FcmToken;
                if (string.IsNullOrEmpty(account.Fullname))
                {
                    account.Fullname = name;
                }

                if (string.IsNullOrEmpty(account.PhoneNumber))
                {
                    account.PhoneNumber = phoneNumber;
                }

                if (string.IsNullOrEmpty(account.ImageUrl))
                {
                    account.ImageUrl = imageUrl;
                }

                await _unitOfWork.Repository<Account>().UpdateGuid(account, account.Id);
                await _unitOfWork.CommitAsync();
            }
            var jwt = GenerateJwtToken(account);
            return jwt;
        }

        public async Task<PagedResults<AccountResponse>> GetAccount(PagingRequest request, int brandId)
        {
            List<AccountResponse> list = null;
            try
            {
                List<Account> account = null;
                if (brandId != 0)
                {
                    account = await _unitOfWork.Repository<Account>()
                            .GetAll()
                                .Where(x => x.Fullname.ToLower()
                            .Contains(request.KeySearch.ToLower())
                            && x.Active == true && x.BrandId == brandId).ToListAsync();
                }
                else
                {
                    account = await _unitOfWork.Repository<Account>()
                            .GetAll()
                                .Where(x => x.Fullname.ToLower()
                            .Contains(request.KeySearch.ToLower())
                            && x.Active == true).ToListAsync();
                }

                IEnumerable<AccountResponse> rs = account.Select(x => new AccountResponse
                {
                    Active = x.Active,
                    BrandId = x.BrandId,
                    BrandName = x.Brand?.Name,
                    CreateDate = x.CreateDate,
                    Email = x.Email,
                    FcmToken = x.FcmToken,
                    Fullname = x.Fullname,
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    PhoneNumber = x.PhoneNumber,
                    Role = x.Role,
                }).AsEnumerable();

                list = PageHelper<AccountResponse>.Sorting(request.SortType, rs, request.ColName);
                var result = PageHelper<AccountResponse>.Paging(list, request.Page, request.PageSize);
                return result;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Account Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<AccountResponse> GetAccountById(Guid id)
        {
            var account = await _unitOfWork.Repository<Account>().GetAll().Where(f => f.Id == id && f.Active == true).FirstOrDefaultAsync();
            return new AccountResponse
            {
                ImageUrl = account.ImageUrl,
                Active = account.Active,
                BrandId = account.BrandId,
                BrandName = account.Brand?.Name,
                CreateDate = account.CreateDate,
                Email = account.Email,
                FcmToken = account.FcmToken,

                Fullname = account.Fullname,
                Id = account.Id,
                PhoneNumber = account.PhoneNumber,
                Role = account.Role
            };
        }

        public async Task<AccountResponse> PostAccount(PostAccountRequest request)
        {
            try
            {
                Account account = new Account
                {
                    Id = Guid.NewGuid(),
                    Fullname = request.Fullname,
                    Email = request.Email,
                    Active = true,
                    PhoneNumber = request.PhoneNumber,
                    Role = request.Role,
                    CreateDate = DateTime.UtcNow.AddHours(7),
                    ImageUrl = request.ImageUrl
                };
                if (request.BrandId > 0)
                {
                    account.BrandId = request.BrandId;
                }
                await _unitOfWork.Repository<Account>().InsertAsync(account);
                await _unitOfWork.CommitAsync();
                return new AccountResponse
                {
                    ImageUrl = account.ImageUrl,
                    Active = account.Active,
                    BrandId = account.BrandId,
                    BrandName = account.Brand?.Name,
                    CreateDate = account.CreateDate,
                    Email = account.Email,
                    FcmToken = account.FcmToken,

                    Fullname = account.Fullname,
                    Id = account.Id,
                    PhoneNumber = account.PhoneNumber,
                    Role = account.Role,
                };
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Insert Account Error!!!", e.InnerException?.Message);
            }
        }

        public async Task<PutAccountResponse> PutAccount(Guid id, PutAccountRequest model, Guid currentAccountId)
        {
            Account account = await _unitOfWork.Repository<Account>().GetAll().Where(x => x.Id == id && x.Active == true).Include(x => x.Brand).FirstOrDefaultAsync();
            if (account != null)
            {
                if (model.Fullname != "")
                {
                    account.Fullname = model.Fullname;
                }

                if (model.PhoneNumber != "")
                {
                    account.PhoneNumber = model.PhoneNumber;
                }

                if (model.Email != "")
                {
                    account.Email = model.Email;
                }

                if (model.Role > -1)
                {
                    account.Role = model.Role;
                }

                if (model.Role != 1)
                {
                    account.BrandId = null;
                }
                else
                {
                    if (model.BrandId > 0)
                    {
                        account.BrandId = model.BrandId;
                    }
                }
                if (model.ImageUrl != "")
                {
                    account.ImageUrl = model.ImageUrl;
                }

                try
                {
                    await _unitOfWork.Repository<Account>().UpdateGuid(account, id);
                    await _unitOfWork.CommitAsync();
                }
                catch (Exception e)
                {
                    throw new CrudException(HttpStatusCode.BadRequest, "Update Account Error!!!", e.InnerException?.Message);
                }
                return new PutAccountResponse
                {
                    ImageUrl = account.ImageUrl,
                    Active = account.Active,
                    BrandId = account.BrandId,
                    BrandName = account.Brand?.Name,
                    CreateDate = account.CreateDate,
                    Email = account.Email,
                    FcmToken = account.FcmToken,

                    Fullname = account.Fullname,
                    Id = account.Id,
                    PhoneNumber = account.PhoneNumber,
                    Role = account.Role,
                    Jwt = id == currentAccountId ? GenerateJwtToken(account) : null
                };
            }
            else
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Update Account Error!!!", "");
            }
        }

        public async Task<VerifyResponse> VerifyJwtTraddeZoneMap(string jwt)
        {
            try
            {
                var userPrincipal = this.ValidateToken(jwt);

                var valueExp = "";
                Guid accountId = Guid.Empty;
                if (userPrincipal != null)
                {
                    foreach (Claim claim in userPrincipal.Claims)
                    {
                        if (claim.Type == _config["AppSettings:Id"])
                        {
                            accountId = new Guid(claim.Value);
                        }
                        if (claim.Type == "exp")
                        {
                            valueExp = claim.Value;
                        }
                    }
                }
                DateTimeOffset dateTimeOffset = DateTimeOffset.FromUnixTimeSeconds(Convert.ToInt64(valueExp));
                var dayRemaining = dateTimeOffset.DateTime - DateTime.UtcNow.AddHours(7).Date;
                if (dayRemaining.Days < 3)
                {
                    Account account = await _unitOfWork.Repository<Account>().GetAll().Where(x => x.Id == accountId && x.Active == true).FirstOrDefaultAsync();
                    return new VerifyResponse
                    {
                        Jwt = jwt,
                        RefreshToken = GenerateJwtToken(account)
                    };
                }
                else
                {
                    return new VerifyResponse
                    {
                        Jwt = jwt,
                        RefreshToken = ""
                    };
                }
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Error to verify account TradeZoneMap!!!", e.InnerException?.Message);
            }
        }

        private string GenerateJwtToken(Account account)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, account.Id.ToString()),
                new Claim(ClaimTypes.Role , account.Role.ToString()),
                new Claim(ClaimTypes.Name , account.Fullname),
                new Claim(ClaimTypes.Email , account.Email),
                new Claim("FcmToken" , account.FcmToken ?? ""),
                new Claim("ImageUrl", account.ImageUrl),
                new Claim(ClaimTypes.MobilePhone , account.PhoneNumber),
                new Claim("BrandId" , string.IsNullOrEmpty(account.BrandId.ToString())?"":account.BrandId.ToString()),
                new Claim("BrandName" ,account.Brand==null?"":account.Brand.Name),
                new Claim("Active" , account.Active.ToString()),
            };
            var key = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["AppSettings:Secret"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(_config["AppSettings:Issuer"],
                _config["AppSettings:Issuer"],
                claims,
                expires: DateTime.UtcNow.AddHours(7).Date.AddDays(7),
                signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private ClaimsPrincipal ValidateToken(string jwtToken)
        {
            IdentityModelEventSource.ShowPII = true;

            TokenValidationParameters validationParameters = new TokenValidationParameters
            {
                ValidateLifetime = true,

                ValidAudience = _config["AppSettings:Issuer"],
                ValidIssuer = _config["AppSettings:Issuer"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_config["AppSettings:Secret"]))
            };
            ClaimsPrincipal principal = new JwtSecurityTokenHandler().ValidateToken(jwtToken, validationParameters, out _);

            return principal;
        }

        public async Task<List<AccountResponse>> GetAccountSurveyBySystemzoneId(int id)
        {
            try
            {
                var data = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.Id == id).Include(x => x.Account).Select(y => new AccountResponse
                {
                    Id = y.Account.Id,
                    Fullname = y.Account.Fullname,
                    PhoneNumber = y.Account.PhoneNumber,
                    Email = y.Account.Email,
                }).ToListAsync();
                return data;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Surveyor By SystemzoneId error!!!", e.InnerException?.Message);
            }
        }

        public async Task<List<SurveyorResponse>> GetSurveyor()
        {
            try
            {
                var data = await _unitOfWork.Repository<Account>().GetAll().Where(x => x.Role == 2 && x.Active == true).Select(y => new SurveyorResponse
                {
                    Id = y.Id,
                    Fullname = y.Fullname,
                    PhoneNumber = y.PhoneNumber,
                    Email = y.Email,
                    Active = y.Active
                }).ToListAsync();
                return data;
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Surveyor error!!!", e.InnerException?.Message);
            }
        }

        public async Task<List<SurveyorResponse>> GetFreeSurveyor(int id)
        {
            try
            {
                var systemzones = await _unitOfWork.Repository<SystemZone>().GetAll().Where(x => x.Id == id).Include(x => x.Account).SingleOrDefaultAsync();
                return systemzones != null
                    ? _unitOfWork.Repository<Account>().GetAll().Where(x => x.Role == 2 && x.Active == true && x.Id != systemzones.AccountId).Select(y => new SurveyorResponse
                    {
                        Id = y.Id,
                        Fullname = y.Fullname,
                        PhoneNumber = y.PhoneNumber,
                        Email = y.Email,
                        Active = y.Active
                    }).ToList()
                    : await GetSurveyor();
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Get Surveyor error!!!", e.InnerException?.Message);
            }
        }

        public async Task<string> GetJwt(Guid accountId)
        {
            var account = await _unitOfWork.Repository<Account>().GetByIdGuid(accountId);
            return GenerateJwtToken(account);
        }

        public async Task<AccountResponse> DeleteAccount(Guid id)
        {
            try
            {
                var account = await _unitOfWork.Repository<Account>().GetAll().Where(f => f.Id == id && f.Active == true).FirstOrDefaultAsync();
                if (account != null)
                {
                    var accSysytemZone = await _unitOfWork.Repository<Account>().GetAll().Where(x => x.Id == id).ToListAsync();
                    account.Active = false;
                    await _unitOfWork.Repository<Account>().UpdateGuid(account, id);
                    await _unitOfWork.CommitAsync();
                    return new AccountResponse
                    {
                        ImageUrl = account.ImageUrl,
                        Active = account.Active,
                        BrandId = account.BrandId,
                        BrandName = account.Brand?.Name,
                        CreateDate = account.CreateDate,
                        Email = account.Email,
                        FcmToken = account.FcmToken,

                        Id = account.Id,
                        PhoneNumber = account.PhoneNumber,
                        Role = account.Role,
                    };
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new CrudException(HttpStatusCode.BadRequest, "Delete Account Error!!!", e.InnerException?.Message);
            }
        }
    }
}