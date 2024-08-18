using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace Mc2.CrudTest.Presentation.Shared.Validators
{
    public class IbanValidator
    {
        public static bool IsValidIban(string iban)
        {
            // Remove all non-alphanumeric characters
            iban = new string(iban.Where(char.IsLetterOrDigit).ToArray());

            // Check if the length is appropriate (IBAN lengths vary by country)
            if (iban.Length < 15 || iban.Length > 34)
            {
                return false;
            }

            // Move the first four characters to the end
            string rearrangedIban = iban.Substring(4) + iban.Substring(0, 4);

            // Convert letters to numbers (A=10, B=11, ..., Z=35)
            string numericIban = string.Empty;
            foreach (char ch in rearrangedIban)
            {
                if (char.IsLetter(ch))
                {
                    numericIban += (ch - 'A' + 10).ToString();
                }
                else
                {
                    numericIban += ch;
                }
            }

            // Convert to a BigInteger and perform the MOD-97-10 check
            BigInteger ibanNumber = BigInteger.Parse(numericIban);
            return ibanNumber % 97 == 1;
        }
    }
}
