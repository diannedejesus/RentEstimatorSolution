using RentEstimator;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Security;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace RentCalculator
{
    public class RentCalculations
    {
        //NOTE:FILES
        string FMRFilePath = @"assets/paymentStandard.json";
        string UtilityAllowanceFilePath = @"assets/utilityAllowance.json";

        private Dictionary<int, int> paymentStandard { get; set; }
        private List<UtilitiesModel> utilityAllowance { get; set; }

        public decimal MinimumRent { get; set; } = 50;

        private int _voucherSize = 0;
        private int _dependants = 0;
        private decimal _annualIncome = 0;
        private bool _isElderlyHandicap = false;

        public RentCalculations()
        {
            bool isFMRFile = File.Exists(FMRFilePath);
            bool isUtilityAllowanceFile = File.Exists(UtilityAllowanceFilePath);

            if(isFMRFile && isUtilityAllowanceFile)
            {
                paymentStandard = new ReadandParseJsonFile(@"assets/paymentStandard.json").ExtractFMRData();
                utilityAllowance = new ReadandParseJsonFile(@"assets/utilityAllowance.json").ExtractUtilitiesData();
            }
            else
            {
                string missingFile = "";

                if (!isFMRFile) 
                    missingFile = FMRFilePath;
                else if (!isUtilityAllowanceFile)
                    missingFile = UtilityAllowanceFilePath;

                if (!isFMRFile && !isUtilityAllowanceFile) 
                    missingFile = FMRFilePath + " and " + UtilityAllowanceFilePath;

                throw new FileNotFoundException("Missing files", missingFile);
            }
        }
        public int VoucherSize
        {
            get => _voucherSize;
            set { _voucherSize = value;}
        }
        public int Dependants
        {
            get => _dependants;
            set { _dependants = value; }
        }
        public decimal AnnualIncome
        {
            get => _annualIncome;
            set { _annualIncome = value; }
        }
        public bool isElderlyHandicap
        {
            get => _isElderlyHandicap;
            set { _isElderlyHandicap = value; }
        }

        public decimal AdjustedAnnualIncome(bool elderlyOrDisabled, decimal dependantsAmount, decimal annualIncome)
        {
            const decimal elderlyDisabled = 400;
            const decimal childDependant = 480;
            decimal adjustedAnnualIncome = annualIncome - (dependantsAmount * childDependant);

            if (elderlyOrDisabled){ adjustedAnnualIncome -= elderlyDisabled; }

            if(adjustedAnnualIncome < 0){ adjustedAnnualIncome = 0; }

            return adjustedAnnualIncome;
        }
        public decimal AdjustedAnnualIncome()
        {
            const decimal elderlyHadicapCredit = 400;
            const decimal childDependantCredit = 480;
            decimal adjustedAnnualIncome = _annualIncome - (_dependants * childDependantCredit);

            if (_isElderlyHandicap) { adjustedAnnualIncome -= elderlyHadicapCredit; }

            if (adjustedAnnualIncome < 0) { adjustedAnnualIncome = 0; }

            return adjustedAnnualIncome;
        }
        public decimal TTPDetermination(decimal annualIncome, decimal adjustedAnnualIncome, decimal minimumRent)
        {
            decimal thirtyPercentAdjusted = (adjustedAnnualIncome * .30m) / 12;
            decimal tenPercentIncome = (annualIncome * .10m) / 12;
            decimal highestAmount;

            if(thirtyPercentAdjusted > tenPercentIncome)
            {
                highestAmount = thirtyPercentAdjusted;
            }
            else
            {
                highestAmount = tenPercentIncome;
            }
            
            if(highestAmount < minimumRent)
            {
                highestAmount = minimumRent;
            }

            return highestAmount;
        }
        public decimal TTPDetermination(decimal annualIncome, decimal adjustedAnnualIncome)
        {
            decimal thirtyPercentAdjusted = (adjustedAnnualIncome * .30m) / 12;
            decimal tenPercentIncome = (annualIncome * .10m) / 12;
            decimal highestAmount;

            if (thirtyPercentAdjusted > tenPercentIncome)
            {
                highestAmount = thirtyPercentAdjusted;
            }
            else
            {
                highestAmount = tenPercentIncome;
            }

            if (highestAmount < MinimumRent)
            {
                highestAmount = MinimumRent;
            }

            return highestAmount;
        }
        public decimal TTPDetermination()
        {
            decimal thirtyPercentAdjusted = (AdjustedAnnualIncome() * .30m) / 12;
            decimal tenPercentIncome = (_annualIncome * .10m) / 12;
            decimal highestAmount;

            if (thirtyPercentAdjusted > tenPercentIncome)
            {
                highestAmount = thirtyPercentAdjusted;
            }
            else
            {
                highestAmount = tenPercentIncome;
            }

            if (highestAmount < MinimumRent)
            {
                highestAmount = MinimumRent;
            }

            return highestAmount;
        }
        public int GetFMR(int voucherSize)
         {
            return paymentStandard[voucherSize];
         }
        public int GetFMR()
        {
            return paymentStandard[_voucherSize];
        }
        public int TotalUtilities(int voucherSize)
        {
            int utilitesTotal = 0;

            foreach (var item in utilityAllowance)
            {
                if (item.Bedroom == voucherSize)
                {
                    utilitesTotal += item.Water;
                    utilitesTotal += item.Electricity;
                    utilitesTotal += item.Fridge;
                    utilitesTotal += item.Microwave;
                    utilitesTotal += item.Sewer;
                    utilitesTotal += item.Cooking;

                    return utilitesTotal;
                }
            }

            return utilitesTotal;
        }
        public int GetTotalUtilities(int voucherSize, bool includesWater, bool includesElectricity, bool includesFridge, bool includesMirowave, bool hasSewer, bool includesCooking )
        {
            int utilitesTotal = 0;

            foreach(var item in utilityAllowance)
            {
                if (!includesWater && item.Bedroom == voucherSize) 
                {
                    if (!includesWater) { utilitesTotal += item.Water; }
                    if (!includesElectricity) { utilitesTotal += item.Electricity; }
                    if (!includesFridge) { utilitesTotal += item.Fridge; }
                    if (!includesMirowave) { utilitesTotal += item.Microwave; }
                    if (!hasSewer) { utilitesTotal += item.Sewer; }
                    if (!includesCooking) { utilitesTotal += item.Cooking; }
                }
            }

            return utilitesTotal;
        }
        public int GetUtilityAmount(int voucherSize, string utilityName)
        {
            foreach (var item in utilityAllowance)
            {
                if (item.Bedroom == voucherSize)
                {
                    if (utilityName == "water") { return item.Water; }
                    if (utilityName == "electricity") { return item.Electricity; }
                    if (utilityName == "fridge") { return item.Fridge; }
                    if (utilityName == "microwave") { return item.Microwave; }
                    if (utilityName == "sewer") { return item.Sewer; }
                    if (utilityName == "cooking") { return item.Cooking; }
                }
            }

            return 0;
        }
        public decimal MaxSubsidy()
        {
            decimal FMR = GetFMR();
            decimal TTP = TTPDetermination();

            return FMR - TTP;
        }
        public decimal MaxSubsidy(decimal FMR, decimal TTP)
        {
            return FMR - TTP;
        }
        public decimal FortypercentAdjusted()
        {
            decimal monthlyAdjusted = AdjustedAnnualIncome() / 12;
            return monthlyAdjusted * .4m;
        }
        public decimal GrossRent()
        {
            decimal adjustedMonthlyx4 = FortypercentAdjusted();
            decimal maxSubsidy = MaxSubsidy();

            return adjustedMonthlyx4 + maxSubsidy;

        }
        public decimal LowestRent()
        {
            decimal totalUtilities = TotalUtilities(_voucherSize);
            decimal minGrossRent = GrossRent() - totalUtilities;
            decimal minPaymentStandard = GetFMR() - totalUtilities;

            return Math.Min(minGrossRent, minPaymentStandard);
        }
        public static int RoundOff(decimal i)
        {
            return ((int)Math.Floor(i / 10)) * 10;
        }


    }
}
