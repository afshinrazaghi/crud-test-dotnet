using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.Interfaces
{
    public interface IGenerator { }
    public interface IGenerator<T> : IGenerator
    {
        T Generate();
    }
}
