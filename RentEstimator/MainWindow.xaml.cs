using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using PdfSharp.Pdf.Content;
using PdfSharp.Pdf.IO;
using RentCalculator;
using System;
using System.CodeDom;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;

namespace RentEstimator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            //check if template file exist

            Dictionary<string, string> replacemntValues = new Dictionary<string, string>();

            //form values / user input
            int voucherSize = string.IsNullOrWhiteSpace(vouchersizeTextBox.Text) ? 0 : Convert.ToInt16(vouchersizeTextBox.Text);
            decimal annualIncome = string.IsNullOrWhiteSpace(annualincomeTextBox.Text) ? 0m : Convert.ToDecimal(annualincomeTextBox.Text);
            int dependantsAmount = string.IsNullOrWhiteSpace(dependentsTextBox.Text) ? 0 : Convert.ToInt16(dependentsTextBox.Text);
            bool elderlyOrHandicapped = eldelydisabledCheckBox.IsChecked.Value;

            ///
            ///calculations
            ///


            //payment standard [pulled from json]
            //ttp max[10% income, 30% adjusted income, min rent]
            //maxsubsidy [payment standard - ttp]
            //40% income [40% of monthly adjusted income]
            //gross rent [40% income + maxsubsidy]
            //min rent min & roundedNearestTens[payment stardard - utilities, maxsubsidy - utilities]
            //max rent roundedNearestTens[gross rent]

            try
            {
                RentCalculations calculate = new RentCalculations();

                calculate.VoucherSize = voucherSize;
                calculate.AnnualIncome = annualIncome;
                calculate.Dependants = dependantsAmount;
                calculate.isElderlyHandicap = elderlyOrHandicapped;


                replacemntValues.Add("{{estander}}", $"${calculate.GetFMR()}.00");
                replacemntValues.Add("{{pagoInquilino}}", $"${Math.Round(calculate.TTPDetermination(), 2, MidpointRounding.AwayFromZero):0.00}");
                replacemntValues.Add("{{subsidioMaximo}}", $"${(calculate.MaxSubsidy()):0.00}");
                replacemntValues.Add("{{fortyPercent}}", $"${Math.Round(calculate.FortypercentAdjusted(), 2, MidpointRounding.AwayFromZero):0.00}"); //--------------------------------
                replacemntValues.Add("{{rentaMaximo}}", $"${(calculate.MaxRentIncome()):0.00}");
                //replacemntValues.Add("{{rentEstimada}}", $"${RentCalculations.RoundOff(calculate.LowestRent())}.00 - ${RentCalculations.RoundOff(calculate.MaxRentIncome())}.00");

                replacemntValues.Add("{{cooking}}", $"${calculate.GetUtilityAmount(voucherSize, "cooking")}.00");
                replacemntValues.Add("{{electricity}}", $"${calculate.GetUtilityAmount(voucherSize, "electricity")}.00");
                replacemntValues.Add("{{airconditioning}}", $"$0.00");
                replacemntValues.Add("{{waterheater}}", $"$0.00");
                replacemntValues.Add("{{water}}", $"${calculate.GetUtilityAmount(voucherSize, "water")}.00");
                replacemntValues.Add("{{sewer}}", $"${calculate.GetUtilityAmount(voucherSize, "sewer")}.00");
                replacemntValues.Add("{{microwave}}", $"${calculate.GetUtilityAmount(voucherSize, "microwave")}.00");
                replacemntValues.Add("{{refridgerator}}", $"${calculate.GetUtilityAmount(voucherSize, "fridge")}.00");

                int utilityAllowance = calculate.TotalUtilities(calculate.VoucherSize);
                //decimal customRentValue = Decimal.Parse(customRentTextBox.Text);
                decimal FMR = calculate.GetFMR();
                decimal totalTenantPay = calculate.TTPDetermination();
                decimal topSubsidy = FMR - totalTenantPay;
                decimal estimatedGrossRent = topSubsidy + calculate.FortypercentAdjusted();
                decimal topRent = Math.Max(estimatedGrossRent - utilityAllowance, FMR - utilityAllowance);
                //decimal rent = customRentValue > 0 ? customRentValue : topRent;
                //decimal rent = topRent;
                //decimal grossRent = rent + utilityAllowance;
                //decimal totalHAP = Math.Min(calculate.GetFMR(), grossRent) - totalTenantPay;
                //decimal HAPowner = Math.Min(rent, totalHAP);
                //decimal tenantRent = rent - HAPowner < 0 ? 0 : rent - HAPowner;
                //decimal utilityReimbursement = 0;
                decimal lowestRent = Math.Min(estimatedGrossRent - utilityAllowance, FMR - utilityAllowance);


                //if (rent - HAPowner <= 0)
                //{
                //    utilityReimbursement = Math.Min(totalHAP - HAPowner, utilityAllowance);
                //}

                decimal lowestGrossRent = lowestRent + utilityAllowance;
                decimal lowestApplicableSubsidy = Math.Min(lowestGrossRent, FMR);
                decimal lowestTotalHAP = lowestApplicableSubsidy - totalTenantPay;
                decimal lowestHAPOwner = Math.Min(lowestRent, lowestTotalHAP);

                decimal highestGrossRent = topRent + utilityAllowance;
                decimal highestApplicableSubsidy = Math.Min(highestGrossRent, FMR);
                decimal highestTotalHAP = highestApplicableSubsidy - totalTenantPay;
                decimal highestHAPOwner = Math.Min(topRent, highestTotalHAP);

                decimal includedUtility = calculate.GetTotalUtilities(calculate.VoucherSize, true, true, false, false, true, false);
                decimal utilTopRent = Math.Max(estimatedGrossRent - includedUtility, FMR - includedUtility);
                decimal utitlyGrossRent = utilTopRent + includedUtility;
                decimal utitlyApplicableSubsidy = Math.Min(utitlyGrossRent, FMR);
                decimal utitlyTotalHAP = utitlyApplicableSubsidy - totalTenantPay;
                decimal utitlyHAPOwner = Math.Min(utilTopRent, utitlyTotalHAP);

                decimal utilLowRent = Math.Min(estimatedGrossRent - includedUtility, FMR - includedUtility);
                decimal utitlyLowGrossRent = utilLowRent + includedUtility;
                decimal utitlyLowApplicableSubsidy = Math.Min(utitlyLowGrossRent, FMR);
                decimal utitlyLowTotalHAP = utitlyLowApplicableSubsidy - totalTenantPay;
                decimal utitlyLowHAPOwner = Math.Min(utilLowRent, utitlyLowTotalHAP);

                //TTPTextBlock.Text = $"${Math.Round(totalTenantPay, 2, MidpointRounding.AwayFromZero):0.00}";
                //maxsubsidyTextBox.Text = $"${(topSubsidy):0.00}";
                //fortypercentTextBox.Text = $"${Math.Round(calculate.FortypercentAdjusted(), 2, MidpointRounding.AwayFromZero):0.00}";
                //maxrentTextBox.Text = $"${(calculate.MaxRentIncome() - utilityAllowance):0.00}";
                //maxrentsubsTextBox.Text = $"${FMR - utilityAllowance:0.00}";

                //lowestRentTextBlock.Text = $"${lowestRent:0.00}";
                //highestRentTextBlock.Text = $"${topRent:0.00}";
                //utilitiesRentTextBlock.Text = $"${utilTopRent:0.00}";

                decimal pagoMin = Math.Max(0, lowestRent - lowestTotalHAP);
                decimal pagoMax = Math.Max(0, topRent - highestTotalHAP);
                decimal pagoUtilMin = Math.Max(0, utilLowRent - utitlyLowTotalHAP);
                decimal pagoUtilMax = Math.Max(0, utilTopRent - utitlyTotalHAP);

                replacemntValues.Add("{{alquilerMin}} - {{alquilerMax}}", $" ${Math.Floor(lowestRent / 10) * 10}.00 - ${Math.Floor(topRent / 10) * 10}.00 ");
                replacemntValues.Add("{{pagoMin}} - {{pagoMax}}", $" ${pagoMin}.00 - ${pagoMax}.00 ");
                replacemntValues.Add("{{subsidioMin}} - {{subsidioMax}}", $" ${lowestHAPOwner}.00 - ${highestHAPOwner}.00 ");

                replacemntValues.Add("{{alquilerUtilMin}} - {{alquilerUtilMax}}", $" ${Math.Floor(utilLowRent/10)*10}.00 - ${Math.Floor(utilTopRent/10)*10}.00 ");
                replacemntValues.Add("{{pagoUtilMin}} - {{pagoUtilMax}}", $" ${pagoUtilMin}.00 - ${pagoUtilMax}.00 ");
                replacemntValues.Add("{{subsidioUtilMin}} - {{subsidioUtilMax}}", $" ${utitlyLowHAPOwner}.00 - ${utitlyHAPOwner}.00 ");
                
                //calculate highest rent tenants pay or utility payment
                if (topRent - highestHAPOwner > 0) 
                {
                    //rentUtiliesHighBlock.Text = $"${topRent - highestTotalHAP:0.00}";
                    //rentUtiliesHighLabel.Content = "Tenant Rent";
                }
                else
                {
                    //rentUtiliesHighBlock.Text = $"${Math.Min(highestTotalHAP - highestHAPOwner, utilityAllowance):0.00}";
                    //rentUtiliesHighLabel.Content = "Utility Reimbursement";
                }

                //calculate lowest rent tenants pay or utility payment
                if (lowestRent - lowestHAPOwner > 0)
                {
                    //rentUtiliesLowBlock.Text = $"${lowestRent - lowestTotalHAP:0.00}";
                    //rentUtiliesLowLabel.Content = "Tenant Rent";
                }
                else
                {
                    //rentUtiliesLowBlock.Text = $"${Math.Min(lowestTotalHAP - lowestHAPOwner, utilityAllowance):0.00}";
                    //rentUtiliesLowLabel.Content = "Utility Reimbursement";
                }

                //calculate included utility rent tenants pay or utility payment
                if (utilTopRent - utitlyHAPOwner > 0)
                {
                    //utilitiesRentUtiliesTextBlock.Text = $"${utilTopRent - utitlyTotalHAP:0.00}";
                    //utilitiesRentUtiliesLabel.Content = "Tenant Rent";
                }
                else
                {
                    //utilitiesRentUtiliesTextBlock.Text = $"${Math.Min(utitlyTotalHAP - utitlyHAPOwner, includedUtility):0.00}";
                    //utilitiesRentUtiliesLabel.Content = "Utility Reimbursement";
                }




                //NOTE: FILES
                new PdfTemplateBuilder(@"assets/template.json", @"assets/firstpage.json", replacemntValues);
            }
            catch (FileNotFoundException err)
            {
                MessageBox.Show(err.Message + ": " + err.FileName);
            }
        }
        private void MenuOpenPaymentStandard_Click(object sender, RoutedEventArgs e)
        {
            paymentStandard openSelectedMenu = new paymentStandard();
            openSelectedMenu.Show();
        }

        private void MenuOpenUtilitiesAllowance_Click(object sender, RoutedEventArgs e)
        {
            utilityAllowance openSelectedMenu = new utilityAllowance();
            openSelectedMenu.Show();
        }

        private void MenuOpenEditText_Click(object sender, RoutedEventArgs e)
        {
            EditText openSelectedMenu = new EditText();
            openSelectedMenu.Show();
        }

        private void MenuOpenRentCalculator_Click(object sender, RoutedEventArgs e)
        {
            //EditText openSelectedMenu = new EditText();
            //openSelectedMenu.Show();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        }

        
    }
}
