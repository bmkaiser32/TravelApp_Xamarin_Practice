using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TravelApp
{
    public partial class App : Application
    {

        public static string DatabaseLocation = string.Empty;
      
        // messes up the initial loading
        public App()
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());
        }

        public App(string databaseLocation)
        {
            InitializeComponent();

            MainPage = new NavigationPage(new MainPage());

            DatabaseLocation = databaseLocation;
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
