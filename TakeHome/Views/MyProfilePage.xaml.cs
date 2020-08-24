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
    public partial class MyProfilePage : ContentPage
    {
        AppUser user = new AppUser();
        readonly DataManager manager = new DataManager();

        public MyProfilePage()
        {
            InitializeComponent();
            user = App.user;

            mobileOrdersNumber.Text = App.user.AppUserID.ToString();
            emailEntry.Text = user.Email;
            // passwordEntry.Text = user.Password;
            firstnameEntry.Text = user.FirstName;
            lastnameEntry.Text = user.LastName;
            phonenumberEntry.Text = user.PhoneNumber;
        }

        //public async void OnSaveButtonClicked(object sender, EventArgs e)
        //{
        //    if (CrossConnectivity.Current.IsConnected)
        //    {
        //        string updatemsg = "";
        //        AppUser updUser = App.user;
        //        updUser.Email = emailEntry.Text;
        //       // updUser.Password = passwordEntry.Text;
        //        updUser.FirstName = firstnameEntry.Text;
        //        updUser.LastName = lastnameEntry.Text;
        //        updUser.PhoneNumber = phonenumberEntry.Text;
        //        AppUser updatedUser = await manager.UpdateAppUserProfile(updUser);

        //        if (updatedUser != null)
        //        {
        //            updatemsg = App.DataRepo.UpdateAppUser(updUser);
        //        }
        //        await DisplayAlert("Update Status:", updatemsg, "Ok");

        //    }else
        //    {
        //        await DisplayAlert("Not Connected:", "You have no internet connection", "Ok");
        //    }

        //    await Navigation.PopToRootAsync();
        //}
        public async void OnExitButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PopToRootAsync();
        }

    }
}