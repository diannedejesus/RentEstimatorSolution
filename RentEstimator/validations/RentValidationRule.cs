using PdfSharp.Pdf.Content.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace RentEstimator
{
    public class RentValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int charString;

            if (string.IsNullOrWhiteSpace(value as string))
                return new ValidationResult(false, $"Must not be empty");

            bool success = int.TryParse(value as string, out charString);
            if (success)
            {
                Console.WriteLine($"Converted '{value}' to {charString}.");
            }
            else
            {
                Console.WriteLine($"Attempted conversion of '{value ?? "<null>"}' failed.");
                return new ValidationResult(false, $"Can't contain letter, symbols or periods.");
            }

            if (charString < 0)
                return new ValidationResult(false, $"Can't be a negative value");

            return new ValidationResult(true, null);
        }
    }
}
