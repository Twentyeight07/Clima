using Clima.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace Clima.Data
{
    public class Dday
    {
        public ObservableCollection<Mday> ShowDays()
        {
            return new ObservableCollection<Mday>()
            {
                new Mday()
                {
                    Day="Hoy",
                    BackgroundColor="#686868",
                    Selected=true,
                },
                new Mday()
                {
                    Day="Mañana",
                    BackgroundColor="Transparent",
                    Selected=false,
                },
            };
        }
    }
}
