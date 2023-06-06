using Clima.Data;
using Clima.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Internals;

namespace Clima.ViewModel
{
    internal class VMMainMenu : ViewModelBase
    {
        #region VARIABLES
        string _Ciudad;
        string _DateTime;
        ObservableCollection<Mday> _Categories;
        #endregion
        #region CONSTRUCTOR
        public VMMainMenu(INavigation navigation)
        {
            Navigation = navigation;
            ListCategories();
            SetDateTime("Hoy");
        }
        #endregion
        #region OBJETOS
        public ObservableCollection<Mday> Categories
        {
            get { return _Categories; }
            set { SetValue(ref _Categories, value); }
        }
        public string Ciudad
        {
            get { return _Ciudad; }
            set { SetValue(ref _Ciudad, value); }
        }
        public string Datetime
        {
            get { return _DateTime; }
            set { SetValue(ref _DateTime, value); }
        }
        #endregion
        #region PROCESOS
        public async Task ProcesoAsyncrono()
        {

        }
        public void ListCategories()
        {
            var function = new Dday();
            Categories = function.ShowDays();
        }
        private void Select(Mday param)
        {
            //We make a list from the days, and find an Index. Just compare the "day" from the 'Dday' and the "day" from the Label I just select
            var index = Categories.ToList().FindIndex(p => p.Day == param.Day);
            if (index > -1)
            {
                //Now we use the index from the var above, and change the properties to make the Label look like selected
                DeSelect();
                Categories[index].Selected = true;
                Categories[index].BackgroundColor = "#686868";
                SetDateTime(Categories[index].Day);
            }
        }
        private void DeSelect()
        {
            //We make a list from the Days list and change properties
            Categories.ForEach((item) =>
            {
                item.Selected = false;
                item.BackgroundColor = "Transparent";
            });
        }
        private void SetDateTime(string day)
        {
            if (day == "Mañana")
            {
                Datetime = DateTime.Today.AddDays(1).ToString("dddd, d MMMM");
            }
            else
            {
                Datetime = DateTime.Now.ToString("d MMMM, HH:mm");
            }
        }
        #endregion
        #region COMANDOS
        public ICommand ProcesoAsyncommand => new Command(async () => await ProcesoAsyncrono());
        public ICommand Selectcommand => new Command<Mday>(Select);
        #endregion
    }
}
