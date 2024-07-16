using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Context;
using Mc2.CrudTest.Presentation.Infrastructure.Query.Context;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.AcceptanceTests.Support
{
    public static class ServiceCollectionExtensions
    {
        public static void RemoveDbContexts(this IServiceCollection services)
        {
            services.RemoveAll<WriteDbContext>();
            services.RemoveAll<DbContextOptions<WriteDbContext>>();
            services.RemoveAll<EventStoreDbContext>();
            services.RemoveAll<DbContextOptions<EventStoreDbContext>>();
            services.RemoveAll<ISynchronizeDb>();
        }

        public static void AddDbContexts(this IServiceCollection services, string sqlConnectionString)
        {
            services.AddDbContext<WriteDbContext>(options =>
            {
                options.UseSqlServer(new SqlConnection(sqlConnectionString));
            });

            services.AddDbContext<EventStoreDbContext>(options =>
            {
                options.UseSqlServer(new SqlConnection(sqlConnectionString));
            });

            services.AddSingleton<IReadDbContext, NoSqlDbContext>();
            services.AddSingleton<ISynchronizeDb, NoSqlDbContext>();
        }

       
    }
}
