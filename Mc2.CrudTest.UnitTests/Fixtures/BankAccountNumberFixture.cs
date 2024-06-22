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
            var random = new Random();
            return (random.Next(11, 99).ToString()) + " " + (random.Next(11, 99).ToString()) + " " + (random.Next(11, 99).ToString()) + " " + (random.Next(111, 999).ToString());
        }
    }
}
