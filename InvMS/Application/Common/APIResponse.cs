using System.Net;

namespace Application.Common
{
    public class APIResponse<T>
    {
        public bool Status { get; set; } = true;
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.OK;
        
        public string? Message { get; set; }
        public T? Data { get; set; }
        public string? Error { get; set; }

        public APIResponse()
        {
        }

        public APIResponse(T data, string message = "")
        {
            Data = data;
            Message = message;
        }
    }
}
