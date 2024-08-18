using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.Models
{
    public partial class ApiResponse<T> : ApiResponse
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
           new() { Success = true, StatusCode = (int)HttpStatusCode.OK, Result = result };

        public static ApiResponse<T> Ok(T result, string successMessage) =>
            new() { Success = true, StatusCode = (int)HttpStatusCode.OK, Result = result, SuccessMessage = successMessage };
    }
}
