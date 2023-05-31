﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Clima.ViewModel
{
    internal class VMMainMenu : ViewModelBase
    {
        #region VARIABLES
        string _Ciudad;
        string _DateTime;
        #endregion
        #region CONSTRUCTOR
        public VMMainMenu(INavigation navigation)
        {
            Navigation = navigation;
            _DateTime = DateTime.Now.ToString();
        }
        #endregion
        #region OBJETOS
        public string Ciudad
        {
            get { return _Ciudad; }
            set { SetValue(ref _Ciudad, value); }
        }
        public string Datetime
        {
            get { return _DateTime; }
        }
        #endregion
        #region PROCESOS
        public async Task ProcesoAsyncrono()
        {

        }
        public void ProcesoSimple()
        {
            
        }
        #endregion
        #region COMANDOS
        public ICommand ProcesoAsyncommand => new Command(async () => await ProcesoAsyncrono());
        public ICommand ProcesoSimpcommand => new Command(ProcesoSimple);
        #endregion
    }
}
