using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.Models
{
    public sealed class ApiErrorResponse
    {
        [JsonConstructor]
        public ApiErrorResponse(string message)
        {
            Message = message;
        }

        public string Message { get; set; }

        public override string ToString() => Message;
    }
}
