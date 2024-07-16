using Mc2.CrudTest.Presentation.Shared.AppSettings;
using Mc2.CrudTest.Presentation.Shared.SharedKernel;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;

namespace Mc2.CrudTest.Presentation.Shared
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddShared(this IServiceCollection services)
            => services.AddOptionsWithValidation<ConnectionOptions>();

        public static IServiceCollection AddOptionsWithValidation<TOptions>(this IServiceCollection services)
            where TOptions : class, IAppOptions
        {
            return services
                .AddOptions<TOptions>()
                .BindConfiguration(TOptions.ConfigSectionPath, binderOptions => binderOptions.BindNonPublicProperties = true)
                .ValidateDataAnnotations()
                .Services;
        }
    }
}
