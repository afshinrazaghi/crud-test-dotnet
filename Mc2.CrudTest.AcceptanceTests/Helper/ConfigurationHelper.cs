using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.AcceptanceTests.Helper
{
    public static class ConfigurationHelper
    {
        public static IConfiguration GetConfiguration()
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
           .SetBasePath(Directory.GetCurrentDirectory())
           .AddJsonFile("appsettings.test.json", optional: false, reloadOnChange: true);
            return builder.Build();
        }
    }
}
