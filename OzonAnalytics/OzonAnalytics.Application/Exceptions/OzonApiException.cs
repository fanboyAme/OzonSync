using System;
using System.Collections.Generic;
using System.Text;

namespace OzonAnalytics.Application.Exceptions
{
    public class OzonApiException: Exception
    {
        public int StatusCode { get; }
        public OzonApiException(int statusCode, string message): base(message) 
        {
            StatusCode = statusCode;
        }
    }
}
