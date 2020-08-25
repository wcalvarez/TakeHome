using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using TakeHome.Models;
using TakeHome.Services;
using TakeHome.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TakeHome.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationsPage : ContentPage
    {
        public IList<Models.Location> locations = new ObservableCollection<Models.Location>();
        public IList<Models.Location> geo_locations = new ObservableCollection<Models.Location>();
        public IList<Models.Location> sorted_geo_locations = new ObservableCollection<Models.Location>();
        public IList<Models.Location> Locations = new ObservableCollection<Models.Location>();

        LocationsViewModel viewModel;


        readonly DataManager manager = new DataManager();

        public ICommand LocationDetailCommand
        {
            get
            {
                return new Command((e) =>
                {
                    var item = (e as Models.Location);
                    // delete logic on item
                });
            }
        }

        public LocationsPage()
        {
            InitializeComponent();
            viewModel = new LocationsViewModel();
 
            BindingContext = viewModel;
        }
        private async void SearchBar_SearchButtonPressed(object sender, EventArgs e)
        {

            // Turn on network indicator
            this.IsBusy = true;


            string searchQry = searchText.Text;
            try
            {
                var locs = await manager.GetLocations(searchQry);
                var locList = JsonConvert.DeserializeObject<IList<Models.Location>>(locs);
                if (locList.Count == 0)
                {
                    this.IsBusy = false;
                    await DisplayAlert("Alert", "No Locations found for:" + searchQry, "OK");
                    return;
                }
                locations.Clear();

                foreach (Models.Location loc in locList)
                {

                    locations.Add(loc);
                }
                viewModel.Locations = locations;
            }
            catch (Exception err)
            {
                this.IsBusy = false;
                //await DisplayAlert("Ooops..", err.Message, "OK");
                var current = Connectivity.NetworkAccess;

                if (current == NetworkAccess.Internet)
                {
                    // Connection to internet is available
                    await DisplayAlert("Ooops..", "Unable to Load Locations: " + err.Message, "OK");
                }
                else
                {
                    await DisplayAlert("Ooops..", "Network Error: " + "No internet connection, try again", "OK");

                }
                return;
            }
            finally
            {
                this.IsBusy = false;
            }
            //   }
        }

        protected  override void OnAppearing()
        {
            base.OnAppearing();
            //locations = viewModel.Locations;
 

            if (locations.Count == 0)
            {
                downloadLocations("WJ3");
            }
            else
            {
                this.IsBusy = false;
                sortGeoLocations();
                lstView.ItemsSource = locations;
                BindingContext = locations;

            }
        }
        protected  void ListItems_Refreshing(object sender, EventArgs e)
        {

            locations = new ObservableCollection<Models.Location>();
            geo_locations = new ObservableCollection<Models.Location>();

            downloadLocations("WJ3");
            lstView.EndRefresh();
        }

        private async void downloadLocations(string searchText)
        {
            locations = new ObservableCollection<Models.Location>();
            geo_locations = new ObservableCollection<Models.Location>();

            //
            //
            // Turn on network indicator
            this.IsBusy = true;


            //string searchQry = App.AppName;
            string searchQry = "WJ3";
            try
            {
                var locs = await manager.GetLocations(searchQry);
                var locList = JsonConvert.DeserializeObject<IList<Models.Location>>(locs);
                if (locList.Count == 0)
                {
                    this.IsBusy = false;
                    await DisplayAlert("Alert", "No Locations found for:" + searchQry, "OK");
                    return;
                }
                // locations.Clear();

                foreach (Models.Location loc in locList)
                {

                    locations.Add(loc);
                    // BindingContext = locations;
                }
                lstView.ItemsSource = locations;
                if (locations.Count > 0)
                {
                    //determine distance from current locaiton and append it to CityStateZip
                    //
                    var request = new GeolocationRequest(GeolocationAccuracy.Best);
                    var geolocation = await Geolocation.GetLocationAsync(request);

                    if (geolocation != null)
                    {
                        if (geolocation.IsFromMockProvider)
                        {
                            // location is from a mock provider
                        }
                        foreach (var loc in locations)
                        {
                            TakeHome.Models.Location l = (TakeHome.Models.Location)loc;
                            Xamarin.Essentials.Location store_geo = new Xamarin.Essentials.Location(l.Latitude, l.Longitude);
                            Xamarin.Essentials.Location current_location = new Xamarin.Essentials.Location(geolocation.Latitude, geolocation.Longitude);
                            double miles = Xamarin.Essentials.Location.CalculateDistance(store_geo, current_location, DistanceUnits.Miles);
                            l.Distance = miles;
                            l.CityStateZip = String.Format("{0:0.##}", miles) + " mi - " + l.CityStateZip;
                            geo_locations.Add(l);
                        }
                    }
                    //
                    //
                    this.IsBusy = false;
                    sortGeoLocations();

                    lstView.ItemsSource = locations;

                }
            }
            catch (Exception err)
            {
                this.IsBusy = false;
                var current = Connectivity.NetworkAccess;

                if (current == NetworkAccess.Internet)
                {
                    // Connection to internet is available
                    await DisplayAlert("Ooops..", "Unable to Load Locations: " + err.Message, "OK");
                }
                else
                {
                    await DisplayAlert("Ooops..", "Network Error: " + "No internet connection, try again", "OK");

                }

                return;
            }
            finally
            {
                this.IsBusy = false;
            }
            lstView.ItemsSource = locations;
            //lstView.ItemsSource = geo_locations;
            if (locations.Count > 0)
            {
                // BindingContext = locations;
            }
            else
            {
                //zerolocations.Text = "No locations downloaded, please try again";
                //zerolocations.IsVisible = true;
                return;
            }
            //
            //
        }

        private void sortGeoLocations()
        {
            if (geo_locations.Count > 0)
            {
                locations = new ObservableCollection<Models.Location>((IEnumerable<Models.Location>)geo_locations.OrderBy(i => i.Distance));
            }
        }
        async void OnImageButtonClicked(object sender, EventArgs e)
        {

            ImageButton button = (ImageButton)sender;
            var loc = button.CommandParameter;
            Models.Location detailLocation = (Models.Location)loc;
            await Navigation.PushAsync(new LocationDetailPage(detailLocation));
        }
        async void ShowProducts(object sender, ItemTappedEventArgs e)
        {
            App.BrowsingLocation = (Models.Location)e.Item;
            lstView.SelectedItem = null;
            await Navigation.PushAsync(new ProductsPage(App.BrowsingLocation.LocationID));
        }

    }
}