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
    public partial class ForgotPasswordPage : ContentPage
    {
        readonly DataManager manager = new DataManager();

        public ForgotPasswordPage()
        {
            InitializeComponent();
        }

        public async void OnSubmitButtonClicked(object sender, EventArgs e)
        {

            AppUser fuser = new AppUser();

            fuser.Email = EmailEntered.Text;
            fuser.Password = "forgot";

            if (!ValidEmail(fuser))
            {
                await DisplayAlert("Input Required:", "Enter a valid Email", "Ok");
                return;
            }
            else
            {

                AppUser validUser = null;

                // BACKEND process
                // Turn on network indicator
                this.IsBusy = true;

                try
                {
                    //if (!CrossConnectivity.Current.IsConnected)
                    //{
                    //    await DisplayAlert("Not Connected:", "You have no internet connection", "Ok");
                    //    await Navigation.PopAsync();
                    //}
                    //else
                    //{
                    //
                    //retrieve AppUser from backend w same email
                    var users = await manager.GetAppUser(fuser);
                    var newUsers = JsonConvert.DeserializeObject<IList<AppUser>>(users); //always returns 1 only

                    if (newUsers != null)
                    {
                        foreach (AppUser nuser in newUsers)
                        {
                            validUser = nuser;
                        }
                    }
                    //}

                    //if email has AppUser match, make a POST to AuthTokensController
                    //create callbackURL w/ token,userid
                    if (validUser != null)
                    {
                        //generate AuthToken for this user,and email with callbackurl (all from backend)
                        AuthToken token = new AuthToken();
                        token.AppUserID = validUser.AppUserID;
                        token = await manager.PostAuthToken(token);

                        await DisplayAlert("Email Sent:", "Check your Emails for Password Reset", "Ok");
                    }
                    else
                    {
                        await DisplayAlert("Invalid Email", "No Account found with this Email", "Ok");
                        return;
                    }
                }
                catch (Exception err)
                {
                    this.IsBusy = false;
                    await DisplayAlert("Ooops..", "Unable to process process your request" + err.Message, "OK");
                    return;
                }
                finally
                {
                    this.IsBusy = false;
                }
            }
            await Navigation.PopAsync();
        }

        bool ValidEmail(AppUser user)
        {
            return (!string.IsNullOrWhiteSpace(user.Email) && user.Email.Contains("@"));
        }
    }
}