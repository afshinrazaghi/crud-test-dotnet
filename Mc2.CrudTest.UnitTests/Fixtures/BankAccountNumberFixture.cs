using Mc2.CrudTest.Presentation.Shared.Generators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.UnitTests.Fixtures
{
    public class BankAccountNumberFixture
    {
        public static string Generate()
        {
            var ibanGenerator = new IbanGenerator();
            return ibanGenerator.Generate();
        }
    }
}
