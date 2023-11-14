using C_Validation_ByCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentEstimator
{
    public class PaymentStandardModel : ObservableObject
    {
        private int _paymentStandardBR0;
        private int _paymentStandardBR1;
        private int _paymentStandardBR2;
        private int _paymentStandardBR3;
        private int _paymentStandardBR4;

        public int PaymentStandardBR0
        {
            get { return _paymentStandardBR0; }
            set
            {
                OnPropertyChanged(ref _paymentStandardBR0, value);
            }
        }

        public int PaymentStandardBR1
        {
            get { return _paymentStandardBR1; }
            set
            {
                OnPropertyChanged(ref _paymentStandardBR1, value);
            }
        }

        public int PaymentStandardBR2
        {
            get { return _paymentStandardBR2; }
            set
            {
                OnPropertyChanged(ref _paymentStandardBR2, value);
            }
        }

        public int PaymentStandardBR3
        {
            get { return _paymentStandardBR3; }
            set
            {
                OnPropertyChanged(ref _paymentStandardBR3, value);
            }
        }

        public int PaymentStandardBR4
        {
            get { return _paymentStandardBR4; }
            set
            {
                OnPropertyChanged(ref _paymentStandardBR4, value);
            }
        }
    }
}
