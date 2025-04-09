using System.Net;

namespace ConversationalAIWebsite.API
{
    public class BackendResponse<T>
    {
        public HttpStatusCode StatusCode { get; private set; }
        public T Data { get; private set; }
        public string Message { get; private set; }
        public bool Success { get; private set; }

        private BackendResponse() { }

        public static BackendResponse<T> CreateSuccessResponse(T data, string message = "", HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            return new BackendResponse<T>
            {
                StatusCode = statusCode,
                Data = data,
                Message = message,
                Success = true
            };
        }

        public static BackendResponse<T> CreateFailureResponse(string message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            return new BackendResponse<T>
            {
                StatusCode = statusCode,
                Data = default,
                Message = message,
                Success = false
            };
        }
    }
}
