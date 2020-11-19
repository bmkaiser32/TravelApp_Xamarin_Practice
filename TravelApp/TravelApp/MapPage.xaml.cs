using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.Maps;
using Plugin.Geolocator;
using SQLite;
using TravelApp.Model;

namespace TravelApp
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MapPage : ContentPage
    {

        public bool hasLocationPermmission = false;

        public MapPage()
        {
            InitializeComponent();

            GetPermissions();
        }

        private async void GetPermissions()
        {
            try
            {
                //**** Tutorial Method ****
                // var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Location); 

                var status = await CrossPermissions.Current.CheckPermissionStatusAsync<CalendarPermission>();

                if (status != PermissionStatus.Granted)
                {

                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                    {
                        await DisplayAlert("Need location", "Gimme dat fat location", "OK");
                    }

                    //**** Tutorial Method ****
                    //var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Location);

                    //*** Might need***
                    //PermissionStatus results = await CrossPermissions.Current.RequestPermissionAsync<LocationWhenInUsePermission>();
                    status = await CrossPermissions.Current.RequestPermissionAsync<LocationWhenInUsePermission>();

                    if (status == PermissionStatus.Granted)
                    {
                        hasLocationPermmission = true;
                        locationsMap.IsShowingUser = true;
                    }
                    else
                    {
                        await DisplayAlert("Location Denied", "You didn't give us permission to access location", "Ok");
                    }


                    //**** Tutorial Method ****
                    /*if (results.ContainsKey(Permission.LocationWhenInUse))
                    {
                        status = results[Permission.Location];
                    }*/
                }


            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "Ok");
            }   

        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            if(hasLocationPermmission)
            {
                var locator = CrossGeolocator.Current;
                locator.PositionChanged += Locator_PositionChanged;
                await locator.StartListeningAsync(TimeSpan.Zero, 100);

                /*var position = await locator.GetPositionAsync();

                var center = new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude);
                var span = new Xamarin.Forms.Maps.MapSpan(center, 2, 2);
                locationsMap.MoveToRegion(span);*/

                using (SQLiteConnection conn = new SQLiteConnection(App.DatabaseLocation))
                {
                    conn.CreateTable<Post>();
                    var posts = conn.Table<Post>().ToList();

                    DisplayInMap(posts);
                }

            }

            GetLocation();
        }

        private void DisplayInMap(List<Post> posts)
        {
            foreach(var post in posts)
            {
                try
                {
                    var positon = new Xamarin.Forms.Maps.Position(post.Latitude, post.Longitude);

                    var pin = new Xamarin.Forms.Maps.Pin()
                    {
                        Type = Xamarin.Forms.Maps.PinType.SavedPin,
                        Position = positon,
                        Label = post.VenueName,
                        Address = post.Address
                    };

                    locationsMap.Pins.Add(pin);
                }

                catch(NullReferenceException nre)
                {

                }

                catch(Exception ex) 
                {
                
                }

                   
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

            CrossGeolocator.Current.StopListeningAsync();
            CrossGeolocator.Current.PositionChanged -= Locator_PositionChanged;
        }

        void Locator_PositionChanged(object sender, Plugin.Geolocator.Abstractions.PositionEventArgs e)
        {
            MoveMap(e.Position);
        }

        private async void GetLocation()
        {
            if(hasLocationPermmission)
            {
                var locator = CrossGeolocator.Current;
                var position = await locator.GetPositionAsync();

                MoveMap(position);
            }

        }

        /*private void MoveMap(Plugin.Geolocator.Abstractions.Position position)
        {
            throw new NotImplementedException();
        }*/

        private void MoveMap(Plugin.Geolocator.Abstractions.Position position)
        {
            var center = new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude);
            var span = new Xamarin.Forms.Maps.MapSpan(center, 1, 1);
            locationsMap.MoveToRegion(span);
        }

        /*private void MoveMap(Position position)
        {
            var center = new Xamarin.Forms.Maps.Position(position.Latitude, position.Longitude);
            var span = new Xamarin.Forms.Maps.MapSpan(center, 1, 1);
            locationsMap.MoveToRegion(span);
        }*/

    }
}