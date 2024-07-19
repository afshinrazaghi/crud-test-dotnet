using AutoMapper;
using FluentValidation;
using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Abstractions;
using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
using Mc2.CrudTest.Presentation.Infrastructure.Command;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Context;
using Mc2.CrudTest.Presentation.Infrastructure.Command.Persistence;
using Mc2.CrudTest.Presentation.Infrastructure.Query.Context;
using Mc2.CrudTest.Presentation.Infrastructure.Query.Mappings;
using Mc2.CrudTest.Presentation.Infrastructure.Query.Persistence;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Command;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();


            services.AddValidatorsFromAssembly(assembly);

            #region Context And UnitOfWork
            services.AddScoped<WriteDbContext>()
                    .AddScoped<EventStoreDbContext>()
                    .AddScoped<IUnitOfWork, UnitOfWork>();
            #endregion

            #region Repositories
            services.AddScoped<IEventStoreRepository, EventStoreRepository>()
                .AddScoped<ICustomerWriteOnlyRepository, CustomerWriteOnlyRepository>();
            #endregion

            #region Queries
            services
                .AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(assembly))
                .AddSingleton<IMapper>(new Mapper(new MapperConfiguration(cfg => cfg.AddMaps(assembly))))
                .AddSingleton<ISynchronizeDb, NoSqlDbContext>()
                .AddSingleton<IReadDbContext, NoSqlDbContext>()
                .AddSingleton<NoSqlDbContext>();

            ConfigureMongoDb();

            services.AddScoped<ICustomerReadOnlyRepository, CustomerReadOnlyRepository>();

            #endregion

            return services;
        }

        private static void ConfigureMongoDb()
        {
            try
            {
                BsonSerializer.TryRegisterSerializer(new GuidSerializer(MongoDB.Bson.GuidRepresentation.CSharpLegacy));

                ConventionRegistry.Register("Conventions",
                    new ConventionPack {
                        new CamelCaseElementNameConvention(),
                        new EnumRepresentationConvention(BsonType.String),
                        new IgnoreExtraElementsConvention(true),
                        new IgnoreIfNullConvention(true)
                    }
                    , _ => true);

                new CustomerMap().Configure();
            }
            catch
            {
                //Ingore
            }
        }
    }
}
