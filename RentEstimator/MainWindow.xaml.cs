﻿using PdfSharp;
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
        private RentCalculations calculate = new RentCalculations();
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
                decimal FMR = calculate.GetFMR();
                decimal estimatedGrossRent = calculate.MaxRentIncome();
                decimal topRent = Math.Max(estimatedGrossRent - utilityAllowance, FMR - utilityAllowance);
                decimal lowestRent = calculate.LowestRent();  //Math.Min(estimatedGrossRent - utilityAllowance, FMR - utilityAllowance);


                decimal lowestTotalHAP = calculate.CalculateTotalHAP(lowestRent);
                decimal lowestHAPOwner = Math.Min(lowestRent, lowestTotalHAP);

                decimal highestTotalHAP = calculate.CalculateTotalHAP(topRent);
                decimal highestHAPOwner = Math.Min(topRent, highestTotalHAP);

                int includedUtility = calculate.GetTotalUtilities(calculate.VoucherSize, true, true, false, false, true, false);
                decimal utilTopRent = Math.Max(estimatedGrossRent - includedUtility, FMR - includedUtility);
                decimal utitlyTotalHAP = calculate.CalculateTotalHAP(utilTopRent, includedUtility);
                decimal utitlyHAPOwner = Math.Min(utilTopRent, utitlyTotalHAP);

                decimal utilLowRent = Math.Min(estimatedGrossRent - includedUtility, FMR - includedUtility);
                decimal utitlyLowTotalHAP = calculate.CalculateTotalHAP(utilLowRent, includedUtility);
                decimal utitlyLowHAPOwner = Math.Min(utilLowRent, utitlyLowTotalHAP);

                decimal pagoMin = Math.Max(0, lowestRent - lowestTotalHAP);
                decimal pagoMax = Math.Max(0, topRent - highestTotalHAP);
                decimal pagoUtilMin = Math.Max(0, utilLowRent - utitlyLowTotalHAP);
                decimal pagoUtilMax = Math.Max(0, utilTopRent - utitlyTotalHAP);

                replacemntValues.Add("{{alquilerMin}} - {{alquilerMax}}", $" ${decimal.Truncate(Math.Floor(lowestRent / 10) * 10)}.00 - ${decimal.Truncate(Math.Floor(topRent / 10) * 10)}.00 ");
                replacemntValues.Add("{{pagoMin}} - {{pagoMax}}", $" ${decimal.Truncate(pagoMin)}.00 - ${decimal.Truncate(pagoMax)}.00 ");
                replacemntValues.Add("{{subsidioMin}} - {{subsidioMax}}", $" ${decimal.Truncate(lowestHAPOwner)}.00 - ${decimal.Truncate(highestHAPOwner)}.00 ");

                replacemntValues.Add("{{alquilerUtilMin}} - {{alquilerUtilMax}}", $" ${decimal.Truncate(Math.Floor(utilLowRent/10)*10)}.00 - ${decimal.Truncate(Math.Floor(utilTopRent / 10) * 10)}.00 ");
                replacemntValues.Add("{{pagoUtilMin}} - {{pagoUtilMax}}", $" ${decimal.Truncate(pagoUtilMin)}.00 - ${decimal.Truncate(pagoUtilMax)}.00 ");
                replacemntValues.Add("{{subsidioUtilMin}} - {{subsidioUtilMax}}", $" ${decimal.Truncate(utitlyLowHAPOwner)}.00 - ${decimal.Truncate(utitlyHAPOwner)}.00 ");

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
            rentCalculator openSelectedMenu = new rentCalculator(calculate);
            openSelectedMenu.Show();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
                this.DragMove();
        } 
    }
}
