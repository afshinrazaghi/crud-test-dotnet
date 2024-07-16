using Mc2.CrudTest.Presentation.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.Generators
{
    public class PhoneNumberGenerator : IGenerator<string>
    {
        public string Generate()
        {
            var random = new Random();
            var number = random.Next(1111111, 9999999);
            string phoneNumber = $"+98919{number}";
            return phoneNumber;
        }
    }
}
