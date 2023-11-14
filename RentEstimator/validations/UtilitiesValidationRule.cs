using PdfSharp.Pdf.Content.Objects;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Data;

namespace RentEstimator
{
    public class UtilitiesValidationRule : ValidationRule
    {
        public override ValidationResult Validate(object value, CultureInfo cultureInfo)
        {
            int charString;

            if (string.IsNullOrWhiteSpace(value as string))
            {
                Console.WriteLine("null value");
                return new ValidationResult(false, $"Must not be empty");
            }
                

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
            {
                Console.WriteLine($"No negatives.");
                return new ValidationResult(false, $"Must not be negative");
            }
               

            return new ValidationResult(true, null);
        }
    }

    public class DataGridHasErrorsConverter : IValueConverter
    {
        
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Console.WriteLine("executed");

            if (value is DataGrid dataGrid)
            {
                // Check if the DataGrid has any validation errors in its rows and cells.
                foreach (var item in dataGrid.Items)
                {
                    if (Validation.GetHasError(dataGrid.ItemContainerGenerator.ContainerFromItem(item) as DataGridRow))
                    {
                        // DataGrid has validation errors.
                        Console.WriteLine("Error found in grid");
                        return false;
                    }

                    foreach (var column in dataGrid.Columns)
                    {
                        var cellContent = column.GetCellContent(item);
                        if (cellContent != null && Validation.GetHasError(cellContent))
                        {
                            // DataGrid cell has validation errors.
                            Console.WriteLine("Error found in cell");
                            return false;
                        }
                    }
                }
            }

            // DataGrid does not have validation errors.
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
