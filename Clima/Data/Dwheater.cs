using Clima.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Clima.Data
{
    public class DWeater
    {
        public ObservableCollection<MWeater> ShowWeater()
        {
            return new ObservableCollection<MWeater>()
            {
                new MWeater()
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
