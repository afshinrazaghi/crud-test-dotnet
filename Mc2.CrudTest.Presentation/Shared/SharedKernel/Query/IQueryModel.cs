using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.SharedKernel.Query
{
    public interface IQueryModel { };

    public interface IQueryModel<out TKey> : IQueryModel
        where TKey : IEquatable<TKey>
    {
        TKey Id { get; }
    }
}
