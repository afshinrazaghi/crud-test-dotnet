using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.Extensions
{
    public static class JsonExtensions
    {
        public static readonly Lazy<JsonSerializerOptions> LazyOptions =
            new(() => new JsonSerializerOptions().Configure(), isThreadSafe: true);


        public static T FromJson<T>(this string value) =>
            value != null ? JsonSerializer.Deserialize<T>(value, LazyOptions.Value) : default;


        public static string ToJson<T>(this T value) =>
            !value.IsDefault() ? JsonSerializer.Serialize(value, LazyOptions.Value) : default;

        public static JsonSerializerOptions Configure(this JsonSerializerOptions jsonSettings)
        {
            jsonSettings.WriteIndented = false;
            jsonSettings.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            jsonSettings.ReadCommentHandling = JsonCommentHandling.Skip;
            jsonSettings.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            jsonSettings.TypeInfoResolver = new PrivateConstructorContractResolver();
            jsonSettings.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            return jsonSettings;
        }
    }
}


internal sealed class PrivateConstructorContractResolver : DefaultJsonTypeInfoResolver
{
    public override JsonTypeInfo GetTypeInfo(Type type, JsonSerializerOptions options)
    {
        JsonTypeInfo jsonTypeInfo = base.GetTypeInfo(type, options);

        if (jsonTypeInfo.Kind == JsonTypeInfoKind.Object &&
            jsonTypeInfo.CreateObject is null &&
            jsonTypeInfo.Type.GetConstructors(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).Length == 0)
        {
            jsonTypeInfo.CreateObject = () => Activator.CreateInstance(jsonTypeInfo.Type, true);
        }


        return jsonTypeInfo;
    }
}