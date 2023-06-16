using Clima.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Clima.Data
{
    public class DWeather
    {
        public ObservableCollection<MWeather> ShowWeather()
        {
            return new ObservableCollection<MWeather>()
            {
                new MWeather()
                {
                    CurrentTemp = "",
                    FeelsLike ="",
                    MaxTemp ="",
                    MinTemp = "",
                    ChanceOfRain="",
                    Icon=""

                }
            };
        }
    }
}
