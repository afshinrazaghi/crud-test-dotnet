using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Query
{
    public static class QueryRegistrationServices
    {
        public static IServiceCollection ConfigureQueryServices(this IServiceCollection services)
        {
            return services;
        }
    }
}
