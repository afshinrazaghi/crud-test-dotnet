using Mc2.CrudTest.Presentation.Shared.SharedKernel.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query
{
    public interface ISynchronizeDb
    {
        Task UpsertAsync<TQueryModel>(TQueryModel queryModel, Expression<Func<TQueryModel, bool>> upsertFilter)
        where TQueryModel : IQueryModel;

        Task DeleteAsync<TQueryModel>(Expression<Func<TQueryModel, bool>> deleteFilter)
            where TQueryModel : IQueryModel;
    }
}
