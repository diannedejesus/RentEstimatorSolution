using RentCalculator;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace RentEstimator
{
    /// <summary>
    /// Interaction logic for paymentStandard.xaml
    /// </summary>
    public partial class paymentStandard : Window
    {

        public paymentStandard()
        {
            InitializeComponent();

            try
            {
                Dictionary<int, int> paymentStandard = new ReadandParseJsonFile(@"assets/paymentStandard.json").ExtractFMRData();

                bedroom0.Text = paymentStandard[0].ToString();
                bedroom1.Text = paymentStandard[1].ToString();
                bedroom2.Text = paymentStandard[2].ToString();
                bedroom3.Text = paymentStandard[3].ToString();
                bedroom4.Text = paymentStandard[4].ToString();
            }
            catch (FileNotFoundException err)
            {
                bedroom0.Text = "0";
                bedroom1.Text = "0";
                bedroom2.Text = "0";
                bedroom3.Text = "0";
                bedroom4.Text = "0";
                Console.WriteLine($"{err} was not found");
            }
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            
            Dictionary<string, int> jsonData = new Dictionary<string, int>()
            {
                { "0", string.IsNullOrWhiteSpace(bedroom0.Text) ? 0 : Convert.ToInt16(bedroom0.Text) },
                { "1", string.IsNullOrWhiteSpace(bedroom1.Text) ? 0 : Convert.ToInt16(bedroom1.Text) },
                { "2", string.IsNullOrWhiteSpace(bedroom2.Text) ? 0 : Convert.ToInt16(bedroom2.Text) },
                { "3", string.IsNullOrWhiteSpace(bedroom3.Text) ? 0 : Convert.ToInt16(bedroom3.Text) },
                { "4", string.IsNullOrWhiteSpace(bedroom4.Text) ? 0 : Convert.ToInt16(bedroom4.Text) },
            };

            try
            {

                new ReadandParseJsonFile(@"assets/paymentStandard.json").StreamWrite(jsonData);

                //verify that file was updated corectly 
                MessageBox.Show("Update completed.");
            }
            catch (FileNotFoundException err)
            {
                MessageBox.Show(err.Message + ": " + err.FileName);
            }

            
        }
    }
}
