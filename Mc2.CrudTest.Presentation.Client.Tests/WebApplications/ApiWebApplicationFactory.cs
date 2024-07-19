using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Client.Tests.WebApplications
{
    public class ApiWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram>
        where TProgram : Mc2.CrudTest.Presentation.Program
    {
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
