using System;

namespace TradeMap.Service.DTO.Response
{
    public class ErrorResponse
    {
        public string Type { get; set; }
        public string Message { get; set; }
        public string StackTrade { get; set; }

        public ErrorResponse(Exception ex, string error)
        {
            Type = ex.GetType().Name;
            Message = ex.Message;
            StackTrade = error;
        }
    }
}