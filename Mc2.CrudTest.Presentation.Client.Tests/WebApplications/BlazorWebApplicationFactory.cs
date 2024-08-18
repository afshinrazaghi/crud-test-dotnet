using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Client.Tests.WebApplications
{
    public class BlazorWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
        where TProgram : Mc2.CrudTest.Presentation.Client.Program
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                // Configure HttpClient for Blazor WASM to use the provided API URL
                services.AddScoped(sp => new HttpClient { BaseAddress = new Uri("http://localhost:5099") });
            });

            builder.UseStaticWebAssets();
        }

        protected override IHost CreateHost(IHostBuilder builder)
        {
            IHost dummyHost = builder.Build();

            builder.ConfigureWebHost(webHostBuilder => webHostBuilder.UseKestrel());

            IHost host = builder.Build();
            host.Start();

            return dummyHost;
        }
    }
}
