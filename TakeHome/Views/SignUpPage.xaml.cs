using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeHome.Models;
using TakeHome.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TakeHome.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SignUpPage : ContentPage
    {
        readonly DataManager manager = new DataManager();

        public SignUpPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.Title = "Sign Up";
            //if (App.BrowsingLocation != null && App.BrowsingLocation.MembershipRequired)
            //{
            //    //emailLabel.IsVisible = false;
            //    //emailEntry.IsVisible = false;
            //    memberLabel.IsVisible = true;
            //    memberEntry.IsVisible = true;
            //}
            //else
            //{
            //    memberLabel.IsVisible = false;
            //    memberEntry.IsVisible = false;
            //    //emailLabel.IsVisible = true;
            //    //emailEntry.IsVisible = true;
            //}
        }
        async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            //if (CrossConnectivity.Current.IsConnected)
            //{
            // validate Entries
            if (App.BrowsingLocation != null && App.BrowsingLocation.MembershipRequired)
            {
                //if (string.IsNullOrEmpty(memberEntry.Text) || string.IsNullOrEmpty(emailEntry.Text) || string.IsNullOrEmpty(passwordEntry.Text) || string.IsNullOrEmpty(firstnameEntry.Text)
                //    || string.IsNullOrEmpty(lastnameEntry.Text) || string.IsNullOrEmpty(phonenumberEntry.Text))
                //{
                //    await DisplayAlert("Required Entries:", "Member#, Email, Password, Firstname, Lastname, Phone#", "Ok");
                //    return;
                //}
            }
            else
            {
                if (string.IsNullOrEmpty(emailEntry.Text) || string.IsNullOrEmpty(passwordEntry.Text) || string.IsNullOrEmpty(firstnameEntry.Text)
                || string.IsNullOrEmpty(lastnameEntry.Text) || string.IsNullOrEmpty(phonenumberEntry.Text))
                {
                    await DisplayAlert("Required Entries:", "Email, Password, Firstname, Lastname, Phone#", "Ok");
                    return;
                }
            }
            var user = new AppUser()
            {
                // Username = usernameEntry.Text,
                Password = passwordEntry.Text,
                Email = emailEntry.Text,
                FirstName = firstnameEntry.Text,
                LastName = lastnameEntry.Text,
                IsLoggedIn = true,
                PhoneNumber = phonenumberEntry.Text
            };

            bool uniqueEmail = true;


            if (AreDetailsValid(user))
            {

                AppUser fuser = new AppUser();

                fuser.Email = emailEntry.Text;
                fuser.Password = "forgot";

                this.IsBusy = true;

                try
                {

                    var users = await manager.GetAppUser(fuser);
                    var newUsers = JsonConvert.DeserializeObject<IList<AppUser>>(users); //always returns 1 only

                    if (newUsers.Count > 0)
                    {
                        await DisplayAlert("Invalid Registration:", "Email already exists", "Ok");
                        messageLabel.Text = "Sign up failed";
                        uniqueEmail = false;
                    }

                    if (uniqueEmail)
                    {
                        var locs = await manager.GetLocationsByType("mobileStore");
                        var locList = JsonConvert.DeserializeObject<IList<Models.Location>>(locs);
 
                        user.LocationID = locList.First().LocationID;
                        // user.AppUserTypeID = 3;
                        var types = await manager.GetAppUserType("mobileOrders");
                        var typelist = JsonConvert.DeserializeObject<IList<AppUserType>>(types);
                        user.AppUserTypeID = typelist.First().AppUserTypeID;

                        user.IsLoggedIn = false;
                        user.Active = true;
                        user.PhoneNumber = phonenumberEntry.Text;
                        var newuser = await manager.RegisterAppUser(user);
                        newuser.IsLoggedIn = true;

                        App.user = newuser;

                        App.user.IsLoggedIn = true;
                        App.IsUserLoggedIn = true;
                        App.MainMenu.Master = new MainMenuMaster();
                        //Navigation.InsertPageBefore(new LocationsPage(), Navigation.NavigationStack.First());
                        await Navigation.PopToRootAsync();
                    }
                }
                catch (Exception err)
                {
                    this.IsBusy = false;
                    //await DisplayAlert("Ooops..", "Unable complete your Sign-Up" + err.Message, "OK");

                    var current = Connectivity.NetworkAccess;

                    if (current == NetworkAccess.Internet)
                    {
                        // Connection to internet is available
                        await DisplayAlert("Ooops..", "Unable to complete your Sign-Up, " + err.Message, "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ooops..", "Network Error: " + "You lost internet connection, try again", "OK");

                    }
                }
                finally
                {
                    this.IsBusy = false;
                }
            }
        }
        bool AreDetailsValid(AppUser user)
        {
            return (!string.IsNullOrWhiteSpace(user.Password) && !string.IsNullOrWhiteSpace(user.Email) && user.Email.Contains("@"));
        }
    }
}