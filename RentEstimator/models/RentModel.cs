using C_Validation_ByCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentEstimator
{
    public class RentModel : ObservableObject
    {
        private int _voucherSize;
        private decimal _annualIncome;
        private int _dependants;
        private bool _elderlyOrDisabled;

        public int VoucherSize
        {
            get { return _voucherSize; }
            set
            {
                OnPropertyChanged(ref _voucherSize, value);
            }
        }

        public decimal AnnualIncome
        {
            get { return _annualIncome; }
            set
            {
                OnPropertyChanged(ref _annualIncome, value);
            }
        }

        public int Dependants
        {
            get { return _dependants; }
            set
            {
                OnPropertyChanged(ref _dependants, value);
            }
        }

        public bool ElderlyOrDisabled
        {
            get { return _elderlyOrDisabled; }
            set
            {
                OnPropertyChanged(ref _elderlyOrDisabled, value);
            }
        }

    }
}
