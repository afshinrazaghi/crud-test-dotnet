using System.Text.Json.Serialization;

namespace Mc2.CrudTest.Presentation.Server.Models
{
    public class ApiResponse<T> : ApiResponse
    {
        [JsonConstructor]
        public ApiResponse(T result, bool success, string successMessage, int statusCode, IEnumerable<ApiErrorResponse> errors)
            : base(success, successMessage, statusCode, errors)
        {
            Result = result;
        }

        public ApiResponse()
        {

        }

        public T Result { get; private init; }

        public static ApiResponse<T> Ok(T result) =>
            new() { Success = true, StatusCode = StatusCodes.Status200OK, Result = result };

        public static ApiResponse<T> Ok(T result, string successMessage) =>
            new() { Success = true, StatusCode = StatusCodes.Status200OK, Result = result, SuccessMessage = successMessage };


    }
}
