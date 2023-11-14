using RentCalculator;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for EditText.xaml
    /// </summary>
    public partial class EditText : Window
    {
        string filePath = @"assets\firstpage.json";
        public EditText()
        {
            InitializeComponent();
            Dictionary<string, string> firstpageData = new ReadandParseJsonFile(filePath).ExtractFirstPageData();

            if (firstpageData != null)
            {
                footer1Textbox.Text = firstpageData["footer2"];
                footer2Textbox.Text = firstpageData["footer1"];
                logopathTextbox.Text = firstpageData["logopath"];
            }
            else
            {
                footer1Textbox.Text = "";
                footer2Textbox.Text = "";
                logopathTextbox.Text = "";
            }
        }

        private void UpdateTextButton_Click(object sender, RoutedEventArgs e)
        {
            string footer1 = string.IsNullOrWhiteSpace(footer1Textbox.Text) ? "" : footer1Textbox.Text;
            string footer2 = string.IsNullOrWhiteSpace(footer2Textbox.Text) ? "" : footer2Textbox.Text;
            string logopath = string.IsNullOrWhiteSpace(logopathTextbox.Text) ? "" : logopathTextbox.Text;

            Dictionary<string, string> pagetext = new Dictionary<string, string>();
            pagetext.Add("logopath", logopath);
            pagetext.Add("footer1", footer1);
            pagetext.Add("footer2", footer2);

            //serialize to json and save to file
            new ReadandParseJsonFile(filePath).StreamWrite(pagetext);

            //verify that file was updated corectly
            MessageBox.Show("Update completed.");
        }

        private void LogopathButton_Click(object sender, RoutedEventArgs e)
        {
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            openFileDialog.DefaultExt = ".png";
            openFileDialog.Filter = "JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = openFileDialog.ShowDialog();

            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = openFileDialog.FileName;
                logopathTextbox.Text = filename;
            }
        }
    }
}
