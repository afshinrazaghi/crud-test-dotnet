using System.Text.Json.Serialization;

namespace Mc2.CrudTest.Presentation.Server.Models
{
    public sealed class ApiErrorResponse
    {
        [method: JsonConstructor]
        public ApiErrorResponse(string message)
        {
            Message = message;
        }

        public string Message { get; set; }

        public override string ToString() => Message;
    }
}
