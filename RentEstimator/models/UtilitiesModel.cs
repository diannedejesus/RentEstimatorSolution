using C_Validation_ByCustom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RentEstimator
{
    public class UtilitiesModel : ObservableObject
    {
        private int _bedroom;
        private int _electricity;
        private int _water;
        private int _sewer;
        private int _fridge;
        private int _cooking;
        private int _microwave;


        public int Bedroom
        {
            get { return _bedroom; }
            set
            { 
                OnPropertyChanged(ref _bedroom, value);
            }
        }

        public int Electricity
        {
            get { return _electricity; }
            set
            {
                OnPropertyChanged(ref _electricity, value);
            }
        }

        public int Water
        {
            get { return _water; }
            set
            {
                OnPropertyChanged(ref _water, value);
            }
        }

        public int Sewer
        {
            get { return _sewer; }
            set
            {
                OnPropertyChanged(ref _sewer, value);
            }
        }

        public int Fridge
        {
            get { return _fridge; }
            set
            {
                OnPropertyChanged(ref _fridge, value);
            }
        }

        public int Cooking
        {
            get { return _cooking; }
            set
            {
                OnPropertyChanged(ref _cooking, value);
            }
        }

        public int Microwave
        {
            get { return _microwave; }
            set
            {
                OnPropertyChanged(ref _microwave, value);
            }
        }


    }
}
