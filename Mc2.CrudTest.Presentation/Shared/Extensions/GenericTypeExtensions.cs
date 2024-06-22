using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.Extensions
{
    public static class GenericTypeExtensions
    {
        public static bool IsDefault<T>(this T value) =>
            Equals(value, default(T));

        public static string GetGenericTypeName(this object @object)
        {
            var type = @object.GetType();

            if (!type.IsGenericType)
                return type.Name;

            var genericTypes = string.Join(",", type.GetGenericArguments().Select(t => t.Name).ToArray());

            return $"{type.Name.Remove(type.Name.IndexOf('`'))}<{genericTypes}>";
        }
    }
}
