using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.System.Utility
{
    public class Response : IResponse
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public bool Succeeded { get; set; }

        public static Response Success(string message = "Operation succeeded", string status = "200")
        {
            return new Response
            {
                Succeeded = true,
                Message = message,
                Status = status
            };
        }

        public static Response Failure(string message = "Operation failed", string status = "400")
        {
            return new Response
            {
                Succeeded = false,
                Message = message,
                Status = status
            };
        }
    }

    public class Response<T> : IResponse<T>
    {
        public string Message { get; set; }
        public string Status { get; set; }
        public bool Succeeded { get; set; }
        public T Data { get; set; }

        // Success methods
        public static Response<T> Success(T data, string message = "Operation succeeded", string status = "200")
        {
            return new Response<T>
            {
                Succeeded = true,
                Data = data,
                Message = message,
                Status = status
            };
        }
        

        public static async Task<Response<T>> SuccessAsync(T data, string message = "Operation succeeded", string status = "200")
        {
            return await Task.FromResult(Success(data, message, status));
        }

        // Failure methods
        public static Response<T> Failure(string message = "Operation failed", string status = "400")
        {
            return new Response<T>
            {
                Succeeded = false,
                Data = default,
                Message = message,
                Status = status
            };
        }

        public static async Task<Response<T>> FailureAsync(string message = "Operation failed", string status = "400")
        {
            return await Task.FromResult(Failure(message, status));
        }
    }

}
