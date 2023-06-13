using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Clima.Views.MainMenu;

namespace Clima
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            MainPage = new MainMenu();
        }

        protected override void OnStart()
        {
        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
