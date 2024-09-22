using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RentCalculator;

namespace RentEstimator.models
{
    public class CalculateModel : INotifyPropertyChanged
    {
        RentCalculations calculate = new RentCalculations();
        
        private int _voucherSize;
        private decimal _annualIncome;
        private int _dependantsAmount;
        private bool _elderlyOrHandicapped;

        public int VoucherSize
        {
            get { return _voucherSize; }
            set { 
                _voucherSize = value;
                OnPropertyChanged(nameof(VoucherSize));
            }
        }

        public decimal AnnualIncome
        {
            get { return _annualIncome; }
            set
            {
                _annualIncome = value;
                OnPropertyChanged(nameof(AnnualIncome));
            }
        }

        public int DependantsAmount
        {
            get { return _dependantsAmount; }
            set
            {
                _dependantsAmount = value;
                OnPropertyChanged(nameof(DependantsAmount));
            }
        }

        public bool ElderlyOrHandicapped
        {
            get { return _elderlyOrHandicapped; }
            set
            {
                _elderlyOrHandicapped = value;
                OnPropertyChanged(nameof(ElderlyOrHandicapped));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
