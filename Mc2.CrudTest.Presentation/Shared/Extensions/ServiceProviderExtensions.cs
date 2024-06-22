using Mc2.CrudTest.Presentation.Shared.SharedKernel;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using System;


namespace Mc2.CrudTest.Presentation.Shared.Extensions
{
    public static class ServiceProviderExtensions
    {
        public static TOptions GetOptions<TOptions>(this IServiceProvider serviceProvider)
            where TOptions : class, IAppOptions =>
            serviceProvider.GetService<IOptions<TOptions>>()?.Value;
    }
}
