using RentCalculator;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RentEstimator
{
    /// <summary>
    /// Interaction logic for utilityAllowance.xaml
    /// </summary>
    public partial class utilityAllowance : Window
    {

        public utilityAllowance()
        {
            InitializeComponent();
            //NOTE: FILE
            try
            {
                List<UtilitiesModel> utilityAllowance = new ReadandParseJsonFile(@"assets/utilityAllowance.json").ExtractUtilitiesData();

                dataGrid.ItemsSource = utilityAllowance;
            }
            catch (FileNotFoundException err)
            {
                List<UtilitiesModel> sampleData = new List<UtilitiesModel>();

                for(int i=0; i<= 4; i++)
                {
                    sampleData.Add(new UtilitiesModel());
                    sampleData[i].Bedroom = i;
                    sampleData[i].Electricity = 0;
                    sampleData[i].Water = 0;
                    sampleData[i].Sewer = 0;
                    sampleData[i].Fridge = 0;
                    sampleData[i].Cooking = 0;
                    sampleData[i].Microwave = 0;
                }

                dataGrid.ItemsSource = sampleData;

                Console.WriteLine($"{err} was not found");
            }

        }

        int count = 0;

        private void DataGrid_Error(object sender, ValidationErrorEventArgs e)
        {
            if (e.Action == ValidationErrorEventAction.Added)
            {
                button.IsEnabled = false;
                count++;
            }
            if (e.Action == ValidationErrorEventAction.Removed)
            {
                count--;
                if (count == 0) button.IsEnabled = true;
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            List<UtilitiesModel> utilityAllowance = new List<UtilitiesModel>();

            bool check = IsValid(dataGrid);
             if(check)
            {
                foreach (var item in dataGrid.Items)
                {
                    if (item is UtilitiesModel allowanceModel)
                    {
                        utilityAllowance.Add(allowanceModel);
                    }
                }

                //ValidationRule current = new UtilitiesValidationRule();
                //ValidationResult verifying =  current.Validate(utilityAllowance);

                new ReadandParseJsonFile(@"assets/utilityAllowance.json").StreamWrite(utilityAllowance);

                //verify that file was updated corectly
                MessageBox.Show("Update completed.");
            }
            else
            {
                MessageBox.Show("data could not be updated");
            }

            
        }

        public bool IsValid(DependencyObject parent)
        {
            if (Validation.GetHasError(parent))
                return false;

            // Validate all the bindings on the children
            for (int i = 0; i != VisualTreeHelper.GetChildrenCount(parent); ++i)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (!IsValid(child)) { return false; }
            }

            return true;
        }

        
    }
}
