using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeHome.Models;
using TakeHome.Services;
using TakeHome.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TakeHome.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LocationDetailPage : ContentPage
    {

        public Models.Location location = new Models.Location();
        public LocationDetailViewModel viewModel;
        readonly DataManager manager = new DataManager();
        public BusinessHour bhours = new BusinessHour();
        public LocationDetailPage(Models.Location locDetail)
        {
            InitializeComponent();
            string storename = locDetail.LocationName;
            viewModel = new LocationDetailViewModel(locDetail);
            
            locDetail.PhoneNumber = GetFormattedPhoneNumber(locDetail.PhoneNumber);
            viewModel.Location = locDetail;
            BindingContext =viewModel.Location;
        }

        protected async override void OnAppearing()
        {
            this.Title = "Store Details";
            //      location = viewModel.location;
            businesstime.IsVisible = false;
            if (viewModel.Location.BusinessHours > 0)
            {
                // Turn on network indicator
                this.IsBusy = true;

                try
                {
                    //get store-hours
                    var today = DateTime.Now;
                    string Day = today.ToString("dddd");

                    var storehours = await manager.GetLocationHours(Day);

                    BusinessHour bhours = JsonConvert.DeserializeObject<BusinessHour>(storehours);

                    viewModel.businessHours = bhours;
                    businesstime.IsVisible = true;
                    if (bhours.Open)
                    {
                        var openTime = bhours.StartTime.TimeOfDay;
                        var closeTime = bhours.CloseTime.TimeOfDay;
                        var timeNow = DateTime.Now.TimeOfDay;
                        if (timeNow >= openTime && timeNow <= closeTime)
                        {
                            open_close.Text = "OPEN";
                        }
                        else
                        {
                            open_close.Text = "CLOSE";

                        }
                        openning.Text = string.Format("{0:hh:mm:ss tt}", bhours.StartTime);
                        closetime.Text = string.Format("{0:hh:mm:ss tt}", bhours.CloseTime);
                    }
                    else  //Closed Today
                    {
                        open_close.Text = "CLOSED on " +  bhours.WeekDay +"(s)";
                    }
                }
                catch (Exception err)
                {
                    this.IsBusy = false;
                    var current = Connectivity.NetworkAccess;

                    if (current == NetworkAccess.Internet)
                    {
                        // Connection to internet is available
                        await DisplayAlert("Ooops..", "Unable to load Location details. " + err.Message, "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ooops..", "Unable to load Location details: " + "You lost internet connection, try again", "OK");

                    }
                    //NOTE: if No BusinessHours, it implies store (mobileStore) accepts orders 27/7, BusinessHours applies to Restaurants
                    //await DisplayAlert("Ooops..", "Unable to verify store hours." + err.Message, "OK");

                    return;
                }
                finally
                {
                    this.IsBusy = false;
                }
            }

        }
        public string GetFormattedPhoneNumber(string phone)
        {
            if (phone != null && phone.Trim().Length == 10)
                return string.Format("({0}) {1}-{2}", phone.Substring(0, 3), phone.Substring(3, 3), phone.Substring(6, 4));
            return phone;
        }
    }
}