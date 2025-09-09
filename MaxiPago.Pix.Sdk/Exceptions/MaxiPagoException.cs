using System;

namespace MaxiPago.Pix.Sdk.Exceptions
{
    public class MaxiPagoException : Exception
    {
        public string ErrorCode { get; }

        public MaxiPagoException(string errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }
    }
}