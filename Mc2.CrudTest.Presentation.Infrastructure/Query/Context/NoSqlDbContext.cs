using Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query;
using MongoDB.Driver;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Mc2.CrudTest.Presentation.Shared.SharedKernel.Query;
using Mc2.CrudTest.Presentation.Shared.AppSettings;
using Mc2.CrudTest.Presentation.Domain.Entities.CustomerAggregate;
using Mc2.CrudTest.Presentation.Application.Models;
using System.Reflection;
using System.Linq.Expressions;
using Polly;

namespace Mc2.CrudTest.Presentation.Infrastructure.Query.Context
{
    public class NoSqlDbContext : IReadDbContext, ISynchronizeDb
    {
        #region Constructor
        private const string DatabaseName = "EShop";
        private const int RetryCount = 2;

        private static readonly ReplaceOptions DefaultReplaceOptions = new ReplaceOptions()
        {
            IsUpsert = true
        };

        public static readonly CreateIndexOptions DefaultCreateIndexOptions = new CreateIndexOptions
        {
            Unique = true,
            Sparse = true
        };

        private readonly IMongoDatabase _database;
        private readonly ILogger<NoSqlDbContext> _logger;
        private readonly AsyncRetryPolicy _mongoRetryPolicy;


        public NoSqlDbContext(IOptions<ConnectionOptions> options, ILogger<NoSqlDbContext> logger)
        {
            ConnectionString = options.Value.NoSqlConnection;

            var mongoClient = new MongoClient(options.Value.NoSqlConnection);
            _database = mongoClient.GetDatabase(DatabaseName);
            _logger = logger;
            _mongoRetryPolicy = CreateRetryPolicy(logger);
        }



        #endregion

        #region IReadDbContext

        public string ConnectionString { get; }


        public IMongoCollection<TQueryModel> GetCollection<TQueryModel>()
            where TQueryModel : IQueryModel
        => _database.GetCollection<TQueryModel>(typeof(TQueryModel).Name);

        public async Task CreateCollectionsAsync()
        {
            using var asyncCursor = await _database.ListCollectionNamesAsync();
            var collections = await asyncCursor.ToListAsync();

            foreach (var collectionName in GetCollectionNamesFromAssembly())
            {
                if (!collections.Exists(db => db.Equals(collectionName, StringComparison.InvariantCultureIgnoreCase)))
                {
                    _logger.LogInformation("----- MongoDb: creating the Collection {Name}", collectionName);
                    await _database.CreateCollectionAsync(collectionName);
                }
                else
                {
                    _logger.LogInformation("----- MongoDb: the {Name} collection allready exists", collectionName);
                }
            }

            await CreateIndexAsync();
        }

        private async Task CreateIndexAsync()
        {
            var indexDefinition = Builders<CustomerQueryModel>.IndexKeys.Ascending(model => model.Email);

            var indexModel = new CreateIndexModel<CustomerQueryModel>(indexDefinition, DefaultCreateIndexOptions);

            var collection = GetCollection<CustomerQueryModel>();
            await collection.Indexes.CreateOneAsync(indexModel);

        }

        private static List<string> GetCollectionNamesFromAssembly() =>
            Assembly
                .GetExecutingAssembly()
                .GetAllTypesOf<IQueryModel>()
                .Select(impl => impl.Name)
                .Distinct()
                .ToList();



        #endregion

        #region ISynchronizeDb
        public async Task UpsertAsync<TQueryModel>(TQueryModel queryModel, Expression<Func<TQueryModel, bool>> upsertFilter)
            where TQueryModel : IQueryModel
        {
            var collection = GetCollection<TQueryModel>();
            await _mongoRetryPolicy.ExecuteAsync(async () =>
                await collection.ReplaceOneAsync(upsertFilter, queryModel, DefaultReplaceOptions));

        }


        public async Task DeleteAsync<TQueryModel>(Expression<Func<TQueryModel, bool>> deleteFilter)
            where TQueryModel : IQueryModel
        {
            var collection = GetCollection<TQueryModel>();
            await _mongoRetryPolicy.ExecuteAsync(async () =>
                await collection.DeleteOneAsync(deleteFilter));
        }

        private static AsyncRetryPolicy CreateRetryPolicy(ILogger logger)
        {
            return Policy
                .Handle<MongoException>()
                .WaitAndRetryAsync(RetryCount, SleepDurationProvider, OnRetry);

            void OnRetry(Exception ex, TimeSpan _) =>
                logger.LogError(ex, "An unexpected exception occurred while saving to MongoDb: {Message}", ex.Message);

            TimeSpan SleepDurationProvider(int retryAttempt)
            {
                var sleepDuration =
                    TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)) + TimeSpan.FromMicroseconds(Random.Shared.Next(0, 1000));

                logger.LogInformation("----- MongoDB: Retry #{Count} with delay {Delay}", retryAttempt, sleepDuration);

                    return sleepDuration;
            }
        }
        #endregion


    }
}
