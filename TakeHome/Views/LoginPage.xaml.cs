using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeHome.Models;
using TakeHome.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TakeHome.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        readonly DataManager manager = new DataManager();
        AppUser validUser = new AppUser();
        // private object refcredit;

        public LoginPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            //   OrgName.Text = App.BrowsingLocation.LocationName;

            if (App.BrowsingLocation != null && App.BrowsingLocation.MembershipRequired)
            {
                // emailLabel.IsVisible = false;
                // emailEntry.IsVisible = false;
                //memberLabel.IsVisible = true;
                //memberEntry.IsVisible = true;
            }
            else
            {
                //memberLabel.IsVisible = false;
                //memberEntry.IsVisible = false;
            }

        }

        private async void OnBackButtonPressed()
        {
            await Shell.Current.GoToAsync("//stores");
        }

        async void OnSignUpButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SignUpPage());
        }

        async void OnForgotButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(
                new ForgotPasswordPage());
        }

        async void OnLoginButtonClicked(object sender, EventArgs e)
        {


            var user = new AppUser
            {
                Email = emailEntry.Text,
                Password = passwordEntry.Text
            };

            bool loginerror = false;

            if (AreDetailsValid(user))
            {

                validUser = App.DataRepo.AppUserExistsOnDevice(user);
                if (validUser == null)
                {

                    this.IsBusy = true;

                    try
                    {
                        //if (CrossConnectivity.Current.IsConnected)
                        //{
                        //retrieve AppUser from backend w same email/password
                        var users = await manager.GetAppUser(user);
                        var newUsers = JsonConvert.DeserializeObject<IList<AppUser>>(users); //always returns 1 only

                        if (newUsers.Count > 0)
                        {
                            foreach (AppUser nuser in newUsers)
                            {


                                validUser = nuser;
                            }

                        } //if null:it crashes
                        else
                        {
                            await DisplayAlert("Not found:", "Invalid Email/Password", "Ok");
                            passwordEntry.Text = string.Empty;
                            loginerror = true;
                            return;
                        }
                    }
                    catch (Exception err)
                    {

                        this.IsBusy = false;
                        await DisplayAlert("Ooops..", "Unable to log you in:" + err.Message, "OK");
                        return;
                    }
                    finally
                    {
                        this.IsBusy = false;
                    }
                }

                if (validUser != null)
                {
                    user.IsLoggedIn = true;
                    validUser.IsLoggedIn = true;
                    App.IsUserLoggedIn = true;
                    App.user = user;
                    if (App.DataRepo.AppUserExistsOnDevice(validUser) != null)
                    {
                        App.DataRepo.UpdateAppUser(validUser);
                    }
                    else
                    {
                        App.DataRepo.AddAppUser(validUser);
                    }

                    App.user = validUser;
                    App.user.IsLoggedIn = true;
                    App.MainMenu.Master = new MainMenuMaster();
                    await Navigation.PopAsync();
                }
                else
                {
                    //messageLabel.Text = "Login failed";
                    if (loginerror)
                    {
                        await DisplayAlert("Login Failed:", "Password is invalid", "Ok");
                        passwordEntry.Text = string.Empty;
                    }
                    //return;
                }
            }
            else
            {
                if (!loginerror)
                {
                    await DisplayAlert("Input Required:", "Enter a valid Email, Password", "Ok");
                }
                else
                {
                    loginerror = false;
                }
            }

        }

        bool AreDetailsValid(AppUser user)
        {
            return (!string.IsNullOrWhiteSpace(user.Password) && !string.IsNullOrWhiteSpace(user.Email) && user.Email.Contains("@"));
        }
    }
}