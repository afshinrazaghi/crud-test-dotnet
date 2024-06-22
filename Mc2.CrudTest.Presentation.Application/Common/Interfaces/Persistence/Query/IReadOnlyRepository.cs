using Mc2.CrudTest.Presentation.Shared.SharedKernel.Query;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Application.Common.Interfaces.Persistence.Query
{
    public interface IReadOnlyRepository<TQueryModel, in TKey>
        where TQueryModel : IQueryModel<TKey>
        where TKey : IEquatable<TKey>
    {

        Task<TQueryModel> GetByIdAsync(TKey id);
    }
}
