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
        RentCalculations calculate = new RentCalculations();
        public rentCalculator(int voucherSize, decimal annualIncome, int dependantsAmount, bool elderlyOrHandicapped)
        {
            InitializeComponent();

            calculate.VoucherSize = voucherSize;
            calculate.AnnualIncome = annualIncome;
            calculate.Dependants = dependantsAmount;
            calculate.isElderlyHandicap = elderlyOrHandicapped;
        }

        private void comboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
    
            decimal FMR = calculate.GetFMR(); ;
            decimal totalTenantPay = calculate.TTPDetermination();
            decimal topSubsidy = FMR - totalTenantPay;
            decimal estimatedGrossRent = topSubsidy + calculate.FortypercentAdjusted();

           
            decimal utilityAllowance = calculate.TotalUtilities(calculate.VoucherSize);
            decimal currentRent = FMR - utilityAllowance;

            ComboBoxItem currentItem = (ComboBoxItem)comboBox.SelectedItem;
            RentgroupBox.Header = currentItem.Content.ToString();

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

            RentTextBlock.Text = currentRent.ToString();
            ApplicableSubsidyTextBox.Text = applicableSubsidy.ToString();
            TotalHAPTextBox.Text = totalHAP.ToString();
            HAPOwnerTextBox.Text = HAPOwner.ToString();

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
