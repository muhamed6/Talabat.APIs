﻿
namespace Talabat.APIs.Errors
{
    public class ApiResponse
    {
        public int StatusCode { get; set; }
        public string Message { get; set; }
        public ApiResponse(int statusCode, string? message=null) 
        {
         StatusCode = statusCode;
            Message = message?? GetDefaultMessageForStatusCode(statusCode); 
        }

        private string? GetDefaultMessageForStatusCode(int statusCode)
        {
            return statusCode switch
            {
                400 => "A bad request, you have made",
                404 => "Resource was not found",
                401 => "Authorized, you are not ",
                500 =>  "Errors are the path to the dark side",
                _=>null
               };
        }
    }
}
