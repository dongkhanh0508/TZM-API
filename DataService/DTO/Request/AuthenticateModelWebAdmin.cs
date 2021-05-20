using System.ComponentModel.DataAnnotations;

namespace TradeMap.Service.DTO.Request
{
    public class AuthenticateModelWebAdmin
    {
        [Required]
        public string IdToken { get; set; }

        public string FcmToken { get; set; }
    }
}