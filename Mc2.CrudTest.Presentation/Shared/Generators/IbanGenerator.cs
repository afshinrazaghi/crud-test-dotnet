using Mc2.CrudTest.Presentation.Shared.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.Generators
{
    public class IbanGenerator : IGenerator<string>
    {
        public string Generate()
        {
            string countryCode = "NL";
            string bankCode = "ABNA"; // Example bank code, can be changed
            string accountNumber = GenerateRandomNumber(10); // Random 10-digit account number

            string ibanWithoutChecksum = countryCode + "00" + bankCode + accountNumber;
            string checksum = CalculateChecksum(ibanWithoutChecksum);

            return countryCode + checksum + bankCode + accountNumber;
        }

        private string GenerateRandomNumber(int length)
        {
            Random random = new Random();
            string accountNumber = string.Empty;
            for (int i = 0; i < length; i++)
            {
                accountNumber += random.Next(0, 10).ToString();
            }
            return accountNumber;
        }

        private string CalculateChecksum(string ibanWithoutChecksum)
        {
            // Move the first four characters to the end
            string rearrangedIban = ibanWithoutChecksum.Substring(4) + ibanWithoutChecksum.Substring(0, 4);

            // Replace letters with numbers (A = 10, B = 11, ..., Z = 35)
            string numericIban = string.Empty;
            foreach (char character in rearrangedIban)
            {
                if (char.IsLetter(character))
                {
                    int numericValue = char.ToUpper(character) - 'A' + 10;
                    numericIban += numericValue.ToString();
                }
                else
                {
                    numericIban += character;
                }
            }
            BigInteger ibanNumber = BigInteger.Parse(numericIban);
            int checksum = (int)(98 - (ibanNumber % 97));

            // Return the checksum as a 2-digit string
            return checksum.ToString("D2");
        }
    }
}
