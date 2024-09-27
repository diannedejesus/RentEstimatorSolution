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
using RentCalculator;

namespace RentEstimator
{
    /// <summary>
    /// Interaction logic for rentCalculator.xaml
    /// </summary>
    public partial class rentCalculator : Window
    {
        private RentCalculations calculate;
        public rentCalculator(RentCalculations viewModel)
        {
            InitializeComponent();

            calculate = viewModel;
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
    
            decimal FMR = calculate.GetFMR(); ;
            decimal totalTenantPay = calculate.TTPDetermination();
            decimal topSubsidy = FMR - totalTenantPay;
            decimal estimatedGrossRent = topSubsidy + calculate.FortypercentAdjusted();

            TTPTextBlock.Text = $"${totalTenantPay:0.00}";
            fortypercentTextBox.Text = $"${calculate.FortypercentAdjusted():0.00}";
            estimatedGrossRentTextBox.Text = $"${estimatedGrossRent:0.00}";
            topSubsidyTextBox.Text = $"${topSubsidy:0.00}";

            decimal utilityAllowance = calculate.TotalUtilities(calculate.VoucherSize);
            decimal currentRent = FMR - utilityAllowance;
            maxrentsubsTextBox.Text = $"${currentRent:0.00}";

            ComboBoxItem currentItem = (ComboBoxItem)comboBox.SelectedItem;
            //RentgroupBox.Header = currentItem.Content.ToString();

            if (currentItem.Content != null) 
            {
                if (currentItem.Content.ToString() == "Lowest")
                {
                    currentRent = Math.Min(estimatedGrossRent - utilityAllowance, FMR - utilityAllowance);
                } else if (currentItem.Content.ToString() == "Highest")
                {
                    currentRent = Math.Max(estimatedGrossRent - utilityAllowance, FMR - utilityAllowance);
                } else if (currentItem.Content.ToString() == "Lowest with Utilities")
                {
                    utilityAllowance = calculate.GetTotalUtilities(calculate.VoucherSize, true, true, false, false, true, false);
                    currentRent = Math.Min(estimatedGrossRent - utilityAllowance, FMR - utilityAllowance);
                } else if (currentItem.Content.ToString() == "Highest with Utilities")
                {
                    utilityAllowance = calculate.GetTotalUtilities(calculate.VoucherSize, true, true, false, false, true, false);
                    currentRent = Math.Max(estimatedGrossRent - utilityAllowance, FMR - utilityAllowance);
                }
            }
            
            decimal grossRent = currentRent + utilityAllowance;
            decimal applicableSubsidy = Math.Min(grossRent, FMR);
            decimal totalHAP = applicableSubsidy - totalTenantPay;
            decimal HAPOwner = Math.Min(currentRent, totalHAP);

            GrossRentTextBox.Text = $"${grossRent:0.00}";
            RentTextBlock.Text = $"${currentRent:0.00}";
            ApplicableSubsidyTextBox.Text = $"${applicableSubsidy:0.00}";
            TotalHAPTextBox.Text = $"${totalHAP:0.00}";
            HAPOwnerTextBox.Text = $"${HAPOwner:0.00}";

            if (currentRent - HAPOwner > 0)
            {
                RentUtilitiesTextBlock.Text = $"${currentRent - totalHAP:0.00}";
                RentUtilitiesLabel.Content = "Tenant Rent";
            }
            else
            {
                RentUtilitiesTextBlock.Text = $"${Math.Min(totalHAP - HAPOwner, utilityAllowance):0.00}";
                RentUtilitiesLabel.Content = "Utility Reimbursement";
            }
        }
    }
}
