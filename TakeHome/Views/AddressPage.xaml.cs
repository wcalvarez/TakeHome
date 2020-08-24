using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public partial class AddressPage : ContentPage
    {
        public static ObservableCollection<ShippingInfo> addresses { get; set; }
        readonly DataManager manager = new DataManager();
        IList<CountryDTO> countrylist;
        IList<StateDTO> statelist;

        public AddressPage()
        {
            ListView lstView = new ListView();

            lstView.ItemSelected += OnSelection;
            lstView.ItemTapped += OnTap;


            //countrypicker.SetBinding(Picker.SelectedItemProperty, "SelectedCountry");

            var temp = new DataTemplate(typeof(TextCell));
            lstView.ItemTemplate = temp;
 
            addresses = new ObservableCollection<ShippingInfo>(App.AddressRepo.GetAllAddresses());

            lstView.ItemsSource = addresses;
            Content = lstView;
            BindingContext = addresses;
            InitializeComponent();
            //City.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeWord);
            //Recipient.Keyboard = Keyboard.Create(KeyboardFlags.CapitalizeSentence);
        }

        protected override async void OnAppearing()
        {

            // Turn on network indicator
            this.IsBusy = true;

            try
            {
                var countries = await manager.GetCountries();
                countrylist = JsonConvert.DeserializeObject<IList<CountryDTO>>(countries);

                countrypicker.ItemsSource = countrylist.ToList();

                if (addresses.Count() == 0)
                {
                    if (App.user != null && App.user.IsLoggedIn)
                    {
                        string recipientname = App.user.FirstName;
                        recipientname += ' ';
                        recipientname += App.user.LastName;
                        Recipient.Text = recipientname;
                        EmailAddress.Text = App.user.Email;
                        PhoneNumber.Text = App.user.PhoneNumber;
                    }

                }
            }
            catch (Exception err)
            {
                await DisplayAlert("Ooops..", "Unable to retrieve Addresses data" + err.Message, "OK");
                this.IsBusy = false;
                await Navigation.PopAsync();
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public async void OnPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            // Turn on network indicator
            this.IsBusy = true;

            try
            {
                var picker = (Picker)sender;
                int selectedIndex = picker.SelectedIndex;

                if (selectedIndex != -1)
                {
                    CountryDTO selectedCountry = (CountryDTO)countrypicker.SelectedItem;
                    //retrieve States/Provinces for selected Country
                    var states = await manager.GetStates(selectedCountry.CountryID);
                    statelist = JsonConvert.DeserializeObject<IList<StateDTO>>(states);
                    statepicker.ItemsSource = statelist.ToList();
                }
            }
            catch (Exception err)
            {
                this.IsBusy = false;
                await DisplayAlert("Ooops..", "Unable to load States/Provinces" + err.Message, "OK");
                await Navigation.PopToRootAsync();
            }
            finally
            {
                this.IsBusy = false;
            }
        }

        public async void OnAddAddress(object sender, System.EventArgs e)
        {

            CountryDTO selectedCountry = (CountryDTO)countrypicker.SelectedItem;
            StateDTO selectedState = (StateDTO)statepicker.SelectedItem;
            string state = "";
            if (selectedCountry != null && selectedState != null)
            {
                if (selectedCountry.isoalphacode3 == "USA")
                {
                    state = selectedState.Code;
                }
                else
                {
                    state = selectedState.Name;
                }
                if (Recipient.Text == null || Address1.Text == null || City.Text == null)
                {
                    await DisplayAlert("Alert", "Recipient Name, Address & City are required.", "OK");
                    return;
                }
            }
            else
            {
                await DisplayAlert("Alert", "Select Country and State/Province", "OK");
                return;
            }

            if (EmailAddress.Text == null && PhoneNumber.Text == null)
            {
                await DisplayAlert("Alert", "At least Email or Phonenumber is required.", "OK");
                return;
            }

            var address = new ShippingInfo
            {
                Recipient = Recipient.Text,
                Address1 = Address1.Text,
                City = City.Text,
                State = state,
                Zipcode = Zipcode.Text,
                Country = selectedCountry.Name,
                EmailAddress = EmailAddress.Text,
                PhoneNumber = PhoneNumber.Text
            };
            address.FullAddress = Address1.Text + ", " + City.Text + ", " + state + ", " + Zipcode.Text;

            // Turn on network indicator
            this.IsBusy = true;

            try
            {
                ShippingInfo newsendto = await manager.PostShippingInfo(address);
                App.AddressRepo.AddNewAddress(newsendto);
                addresses.Add(newsendto);
                //clear input fields
                Recipient.Text = "";
                Address1.Text = "";
                City.Text = "";
                //State.Text = "";
                Zipcode.Text = "";
                //Country.Text = ""; 
                EmailAddress.Text = "";
                PhoneNumber.Text = "";
                //if user, added a new address, that meant, they want this new one as shipto
                App.shipto = newsendto;
            }
            catch (Exception err)
            {
                this.IsBusy = false;
                await DisplayAlert("Ooops..", "Unable to save new Address" + err.Message, "OK");
                await Navigation.PopAsync();
            }
            finally
            {
                this.IsBusy = false;
            }


            await Navigation.PushAsync(new CheckOutPage());
        }

        async void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {

                App.shipto = (ShippingInfo)e.SelectedItem;
                //comment out if you want to keep selections
                ListView lst = (ListView)sender;
                lst.SelectedItem = null;
                //await Navigation.PushAsync(newPage);
                //Grab Location, for Referral info
                //
                //int id = App.OrderRepo.GetOrderLocationID().LocationID;
                //string stringLocation = await manager.GetLocation(App.OrderRepo.GetOrderLocationID().LocationID);
                //App.OrderLocation = JsonConvert.DeserializeObject<Location>(stringLocation);
                //
                await Navigation.PushAsync(new CheckOutPage());
                //return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }


        }
        void OnTap(object sender, ItemTappedEventArgs e)
        {
            DisplayAlert("Item Tapped", e.Item.ToString(), "Ok");
        }

        void OnMore(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
            //Do something here... e.g. Navigation.pushAsync(new specialPage(item.commandParameter));
            //page.DisplayAlert("More Context Action", item.CommandParameter + " more context action", 	"OK");
        }

        private async void OnDelete(object sender, System.EventArgs e)
        {
            // Turn on network indicator
            this.IsBusy = true;

            try
            {
                var menuItem = ((MenuItem)sender);
                ShippingInfo address = (ShippingInfo)menuItem.CommandParameter;
                var isDeleted = await manager.DeleteShippingInfo(address.ShippingInfoID);
                isDeleted = true; //debu only
                if (isDeleted)
                {
                    addresses.Remove(address);
                    App.AddressRepo.DeleteAddress(address);
                }
            }
            catch (Exception err)
            {
                this.IsBusy = false;
                await DisplayAlert("Ooops..", "Unable to Delete Address" + err.Message, "OK");
            }
            finally
            {
                this.IsBusy = false;
            }

        }
    }
}