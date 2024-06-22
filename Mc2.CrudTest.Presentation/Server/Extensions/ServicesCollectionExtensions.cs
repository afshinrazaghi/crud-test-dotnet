using Mc2.CrudTest.Presentation.Infrastructure.Command.Context;
using Microsoft.EntityFrameworkCore;
using Mc2.CrudTest.Presentation.Shared.Extensions;
using Mc2.CrudTest.Presentation.Shared.AppSettings;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.OpenApi.Models;
using Asp.Versioning;

namespace Mc2.CrudTest.Presentation.Server.Extensions
{
    public static class ServicesCollectionExtensions
    {
        private const int DbMaxRetryCount = 3;
        private const int DbCommandTimeout = 35;
        private const string DbMigrationAssembleName = "Mc2.CrudTest.Presentation.Server";
        public static IServiceCollection AddWriteDbContext(this IServiceCollection services)
        {
            services.AddDbContext<WriteDbContext>((serviceProvider, optionsBuilder) =>
                ConfigureDbContext<WriteDbContext>(serviceProvider, optionsBuilder, QueryTrackingBehavior.TrackAll));
            return services;
        }

        public static void AddSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(swaggerOptions =>
            {
                swaggerOptions.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "Shop (e-commerce)",
                    Description = "ASP.NET Core C# CQRS Event Sourcing, REST API, DDD, BBD, TTD",
                    Contact = new OpenApiContact
                    {
                        Name = "Afshin Razaghi",
                        Email = "afshin.razaghi.net@gmail.com"
                    },
                    License = new OpenApiLicense
                    {
                        Name = "MIT License"
                    }
                });
            });
        }

        public static void AddVersioning(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1);
                options.ReportApiVersions = true;
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ApiVersionReader = ApiVersionReader.Combine(
                    new UrlSegmentApiVersionReader(),
                    new HeaderApiVersionReader("X-Api-Version"));
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'V";
                options.SubstituteApiVersionInUrl = true;
            });
        }


        private static void ConfigureDbContext<TContext>(
           IServiceProvider serviceProvider,
           DbContextOptionsBuilder optionBuilder,
           QueryTrackingBehavior queryTrackingBehavior)
           where TContext : DbContext
        {
            var logger = serviceProvider.GetRequiredService<ILogger<TContext>>();
            var options = serviceProvider.GetOptions<ConnectionOptions>();

            optionBuilder
                .UseSqlServer(options.SqlConnection, sqlServerOptions =>
                {
                    sqlServerOptions.MigrationsAssembly(DbMigrationAssembleName);
                    sqlServerOptions.EnableRetryOnFailure(DbMaxRetryCount);
                    sqlServerOptions.CommandTimeout(DbCommandTimeout);
                })
                .UseQueryTrackingBehavior(queryTrackingBehavior)
                .LogTo((eventId, _) => eventId.Id == CoreEventId.ExecutionStrategyRetrying, eventData =>
                {
                    if (eventData is not ExecutionStrategyEventData retryEventData)
                        return;


                    var exceptions = retryEventData.ExceptionsEncountered;

                    logger.LogWarning(
                        "----- DbContext: Retry #{Count} with delay {Delay} due to error: {Message}",
                        exceptions.Count,
                        retryEventData.Delay,
                        exceptions[^1].Message);
                });

            var environment = serviceProvider.GetRequiredService<IHostEnvironment>();

            if (!environment.IsProduction())
            {
                optionBuilder.EnableDetailedErrors();
                optionBuilder.EnableSensitiveDataLogging();
            }
        }
    }
}
