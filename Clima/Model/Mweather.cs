using Clima.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clima.Model
{
    public class MWeather:ViewModelBase
    {
        // Objects
        string _CurrentTemp;
        string _FeelsLike;
        string _MaxTemp;
        string _MinTemp;
        string _ChanceOfRain;
        string _Icon;
        public string CurrentTemp
        {
            get { return _CurrentTemp; }
            set { SetValue(ref _CurrentTemp, value); }
        }
        public string FeelsLike
        {
            get { return _FeelsLike; }
            set { SetValue(ref _FeelsLike, value); }
        }
        public string MaxTemp
        {
            get { return _MaxTemp; }
            set { SetValue(ref _MaxTemp, value); }
        }
        public string MinTemp
        {
            get { return _MinTemp; }
            set { SetValue(ref _MinTemp, value); }
        }
        public string Icon
        {
            get { return _Icon; }
            set { SetValue(ref _Icon, value); }
        }
        public string ChanceOfRain
        {
            get { return _ChanceOfRain;}
            set { SetValue(ref _ChanceOfRain, value); }
        }
    }
}
