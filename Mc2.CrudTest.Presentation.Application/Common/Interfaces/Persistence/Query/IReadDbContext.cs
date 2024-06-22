using Mc2.CrudTest.Presentation.Shared.SharedKernel.Query;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query
{
    public interface IReadDbContext
    {
        string ConnectionString { get; }

        IMongoCollection<TQueryModel> GetCollection<TQueryModel>()
            where TQueryModel : IQueryModel;

        Task CreateCollectionsAsync();
    }
}
