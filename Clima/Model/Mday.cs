using Clima.ViewModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clima.Model
{
    public class Mday:ViewModelBase
    {
        public string Day { get; set; }
        //Objetos
        string _BackgroundColor;
        bool _Selected;
        public bool Selected
        {
            get { return _Selected; }
            set { SetValue(ref _Selected, value); }
        }
        public string BackgroundColor
        {
            get { return _BackgroundColor; }
            set { SetValue(ref _BackgroundColor, value); }
        }
    }
}
