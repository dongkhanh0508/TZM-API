using System;
using System.Net;

namespace TradeMap.Service.Exceptions
{
    public class FirebaseException : Exception
    {
        public HttpStatusCode Status { get; private set; }

        public FirebaseException(HttpStatusCode status, string msg) : base(msg)
        {
            Status = status;
        }
    }
}