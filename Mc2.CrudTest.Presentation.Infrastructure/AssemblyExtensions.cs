using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetAllTypesOf<TInterface>(this Assembly assembly)
        {
            var isAssignableToTInterface = typeof(TInterface).IsAssignableFrom;
            return assembly
                .GetTypes()
                .Where(type => type.IsClass && !type.IsAbstract && !type.IsInterface && isAssignableToTInterface(type))
                .ToList();
        }
    }
}
