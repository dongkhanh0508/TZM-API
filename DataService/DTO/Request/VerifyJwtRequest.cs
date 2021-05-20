using System.ComponentModel.DataAnnotations;

namespace TradeMap.Service.DTO.Request
{
    public class VerifyJwtRequest
    {
        [Required]
        public string Jwt { get; set; }
    }
}