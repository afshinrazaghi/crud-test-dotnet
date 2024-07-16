using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Mc2.CrudTest.Presentation.Shared.Models
{
    public partial class ApiResponse
    {
        [JsonConstructor]
        public ApiResponse(bool success, string successMessage, int statusCode, IEnumerable<ApiErrorResponse> errors)
        {
            Success = success;
            SuccessMessage = successMessage;
            StatusCode = statusCode;
            Errors = errors;

        }

        public ApiResponse()
        {

        }

        public bool Success { get; protected set; }
        public string SuccessMessage { get; protected set; }
        public int StatusCode { get; protected set; }
        public IEnumerable<ApiErrorResponse> Errors { get; private set; }


        public static ApiResponse Ok() =>
           new ApiResponse() { Success = true, StatusCode = (int)HttpStatusCode.OK };

        public static ApiResponse Ok(string successMessage) =>
            new ApiResponse() { Success = true, StatusCode = (int)HttpStatusCode.OK, SuccessMessage = successMessage };

        public static ApiResponse BadRequest() =>
            new ApiResponse() { Success = false, StatusCode = (int)HttpStatusCode.BadRequest };

        public static ApiResponse BadRequest(string errorMessage) =>
        new() { Success = false, StatusCode = (int)HttpStatusCode.BadRequest, Errors = CreateErrors(errorMessage) };

        public static ApiResponse BadRequest(IEnumerable<ApiErrorResponse> errors) =>
            new() { Success = false, StatusCode = (int)HttpStatusCode.BadRequest, Errors = errors };

        public static ApiResponse Unauthorized() =>
            new() { Success = false, StatusCode = (int)HttpStatusCode.Unauthorized };

        public static ApiResponse Unauthorized(string errorMessage) =>
            new() { Success = false, StatusCode = (int)HttpStatusCode.Unauthorized, Errors = CreateErrors(errorMessage) };

        public static ApiResponse Unauthorized(IEnumerable<ApiErrorResponse> errors) =>
            new() { Success = false, StatusCode = (int)HttpStatusCode.Unauthorized, Errors = errors };

        public static ApiResponse Forbidden() =>
            new() { Success = false, StatusCode = (int)HttpStatusCode.Forbidden };

        public static ApiResponse Forbidden(string errorMessage) =>
            new() { Success = false, StatusCode = (int)HttpStatusCode.Forbidden, Errors = CreateErrors(errorMessage) };

        public static ApiResponse Forbidden(IEnumerable<ApiErrorResponse> errors) =>
            new() { Success = false, StatusCode = (int)HttpStatusCode.Forbidden, Errors = errors };

        public static ApiResponse NotFound() =>
            new() { Success = false, StatusCode = (int)HttpStatusCode.NotFound };

        public static ApiResponse NotFound(string errorMessage) =>
            new() { Success = false, StatusCode = (int)HttpStatusCode.NotFound, Errors = CreateErrors(errorMessage) };

        public static ApiResponse NotFound(IEnumerable<ApiErrorResponse> errors) =>
            new() { Success = false, StatusCode = (int)HttpStatusCode.NotFound, Errors = errors };

        public static ApiResponse InternalServerError(string errorMessage) =>
            new() { Success = false, StatusCode = (int)HttpStatusCode.InternalServerError, Errors = CreateErrors(errorMessage) };

        public static ApiResponse InternalServerError(IEnumerable<ApiErrorResponse> errors) =>
            new() { Success = false, StatusCode = (int)HttpStatusCode.InternalServerError, Errors = errors };

        private static ApiErrorResponse[] CreateErrors(string errorMessage) =>
            new[] { new ApiErrorResponse(errorMessage) };

        public override string ToString() =>
            $"Success: {Success} | StatusCode: {StatusCode} | HasErrors: {Errors.Any()}";
    }
}
