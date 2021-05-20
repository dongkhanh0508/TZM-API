using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TradeMap.Service.DTO.Request;
using TradeMap.Service.DTO.Response;

namespace TradeMap.Service.Servies.InterfaceService
{
    public interface IAccountsService
    {
        Task<string> AuthenticateAsync(AuthenticateModelWebAdmin model);

        Task<VerifyResponse> VerifyJwtTraddeZoneMap(string jwt);

        Task<AccountResponse> GetAccountById(Guid id);

        Task<PagedResults<AccountResponse>> GetAccount(PagingRequest request, int brandId);

        Task<AccountResponse> DeleteAccount(Guid id);

        Task<AccountResponse> PostAccount(PostAccountRequest request);

        Task<PutAccountResponse> PutAccount(Guid id, PutAccountRequest model, Guid currentAccountId);

        Task<List<AccountResponse>> GetAccountSurveyBySystemzoneId(int id);

        Task<List<SurveyorResponse>> GetSurveyor();

        Task<List<SurveyorResponse>> GetFreeSurveyor(int id);
        Task<string> GetJwt(Guid accountId);
    }
}