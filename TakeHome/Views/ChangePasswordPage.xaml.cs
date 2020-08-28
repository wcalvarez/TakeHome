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
    public partial class ChangePasswordPage : ContentPage
    {
        readonly DataManager manager = new DataManager();
        AppUser validUser = new AppUser();

        public ChangePasswordPage()
        {
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            this.Title = "Change Password";
        }

        public async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            //if (CrossConnectivity.Current.IsConnected)
            //{ 

            AppUser updUser = App.user;
            //
            //
            var user = new AppUser
            {
                Email = updUser.Email,
                Password = currentpassword.Text
            };

            if (newpassword.Text != confirmpassword.Text)
            {
                await DisplayAlert("Mismatch new & confirm:", "New & Confirm passwords should match", "Ok");
                return;
            }

            if (AreDetailsValid(user))
            {
                // Turn on network indicator
                this.IsBusy = true;

                try
                {
                    //if (CrossConnectivity.Current.IsConnected)
                    //{
                    //retrieve AppUser from backend w same email/password
                    var users = await manager.GetAppUser(user);
                    var newUsers = JsonConvert.DeserializeObject<IList<AppUser>>(users); //always returns 1 only

                    if (newUsers != null)
                    {
                        foreach (AppUser nuser in newUsers)
                        {
                            validUser = nuser;
                        }

                        //user found from backend, make sure newpassword == confirm password, update
                        if (newpassword.Text == confirmpassword.Text)
                        {
                            //update backend AppUser table, store hashed password on device
                            //
                            //AppUser updUser = App.user;
                            updUser.Password = newpassword.Text;
                            string updatemsg = await manager.UpdateAppUser(updUser);
                            //updUser  = await manager.UpdateAppUser(updUser);
                            //
                            //


                            //string updatemsg = "";

                            //if (updUser != null)
                            //{
                            //    updatemsg = App.DataRepo.UpdateAppUser(updUser);

                            //}

                            if (updatemsg == "")
                            {

                                await DisplayAlert("Update Status:", "Password changed Successfully", "Ok");
                            }
                            else
                            {
                                await DisplayAlert("Update Status:", "Password NOT changed Successfully", "Ok");
                            }
                            await Navigation.PopToRootAsync();
                        }
                    } //if null:it crashes
                    else
                    {
                        await DisplayAlert("Not found:", "Current Password, invalid", "Ok");
                        currentpassword.Text = string.Empty;
                        //return;
                        //await Navigation.PopToRootAsync();
                    }
                    //}
                    //else
                    //{
                    //    await DisplayAlert("Not Connected:", "You have no internet connection", "Ok");
                    //    await Navigation.PopToRootAsync();
                    //}
                }
                catch (Exception err)
                {
                    this.IsBusy = false;
                    //await DisplayAlert("Ooops..", "Unable to save password changes" + err.Message, "OK");
                    var current = Connectivity.NetworkAccess;

                    if (current == NetworkAccess.Internet)
                    {
                        // Connection to internet is available
                        await DisplayAlert("Ooops..", "Unable to save password changes, " + err.Message, "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ooops..", "Network Error: " + "You lost internet connection, try again", "OK");

                    }
                    return;
 
                }
                finally
                {
                    this.IsBusy = false;
                }
            }
            else
            {
                await DisplayAlert("Input Required:", "Current Password, and confirmed new Password", "Ok");
            }
            //
            //






            //validate current password, text vs text comparison won't work, with hashed, salted password in back-end
            //string currentpwd = currentpassword.Text;
            //if (currentpwd != updUser.Password)
            //{
            //    await DisplayAlert("Invalid Password", "Invalid current-password", "Ok");
            //}

            //if (newpassword.Text != confirmpassword.Text)
            //{
            //    await DisplayAlert("Mismatch new & confirm:", "New & Confirm passwords should match", "Ok");
            //}

            //if (newpassword.Text == confirmpassword.Text)
            //{
            //    //update backend AppUser table, store hashed password on device
            //    //
            //    //AppUser updUser = App.user;
            //    updUser.Password = newpassword.Text;

            //    updUser = await manager.UpdateAppUser(updUser);
            //    //
            //    //


            //    string updatemsg = "";

            //    if (updUser != null)
            //    {
            //        updatemsg = App.DataRepo.UpdateAppUser(updUser);

            //    }

            //    await DisplayAlert("Update Status:", updatemsg, "Ok");
            //    await Navigation.PopToRootAsync();
            //}

            //}
            //else
            //{
            //    await DisplayAlert("Not Connected:", "You have no internet connection", "Ok");
            //}

            // await Navigation.PopToRootAsync();
        }

        bool AreDetailsValid(AppUser user)
        {
            return (!string.IsNullOrWhiteSpace(user.Password) && !string.IsNullOrWhiteSpace(user.Email) && user.Email.Contains("@"));
        }
    }
}