using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
    public partial class OrderItemPage : ContentPage
    {
        Product prod = new Product();
        OrderDetail currOrd = new OrderDetail();
        public static ObservableCollection<OrderDetail> orderLines { get; set; }

        readonly DataManager manager = new DataManager();
        CylinderDeal cd;
        public OrderItemPage(Product product)
        {
            prod = product;
            BindingContext = product;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationBarView.Title = "Product Details";
            Quantity.Focus();
            orderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());
            storename.Text = App.BrowsingLocation.LocationName;
            int qty = 0;
            if (orderLines.Count > 0)
            {
                qty = orderLines.Select(o => o.Quantity).Aggregate((x, y) => x + y);
            }
            else
            {

            }

            if(prod.SmallImage == null)
            {
                image.IsVisible = false;
            }

            NavigationBarView.FirstNameLabel.Text = qty.ToString();

        }

        public async void OnClickOrder(object sender, EventArgs e)
        {
            // make sure qty > 0
            if (string.IsNullOrWhiteSpace(Quantity.Text))
            {
                await DisplayAlert("Error:", "Quantity can not be blank", "Ok");
                return;
            }
            if (int.Parse(Quantity.Text) < 1 && !string.IsNullOrWhiteSpace(Quantity.Text))
            {
                await DisplayAlert("Error:", "Quantity should be at least 1", "Ok");
                return;
            }

            BusinessHour bhours;

            if (App.BrowsingLocation.BusinessHours > 0 && orderLines.Count == 0)
            {
                // Turn on network indicator
                this.IsBusy = true;

                try
                {
                    var today = DateTime.Now;
                    string Day = today.ToString("dddd");

                    var storehours = await manager.GetLocationHours(Day);

                    bhours = JsonConvert.DeserializeObject<BusinessHour>(storehours);
                    App.storeHours = bhours;

                    if (bhours.Open)
                    {
                        var openTime = bhours.StartTime.TimeOfDay;
                        var closeTime = bhours.CloseTime.TimeOfDay;
                        var timeNow = DateTime.Now.TimeOfDay;
                        if (timeNow >= openTime && timeNow <= closeTime)
                        { 

                        }
                        else
                        {
                            if (timeNow >=  closeTime)
                            {
                                DateTime dtTemp = DateTime.ParseExact(bhours.CloseTime.TimeOfDay.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);
                                await DisplayAlert("Closed:", "Sorry, this store closes at " + dtTemp.ToString("hh:mm tt") + ".", "Ok");
                            }
                            if (timeNow < openTime)
                            {
                                DateTime dtTemp = DateTime.ParseExact(bhours.StartTime.TimeOfDay.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);
                                await DisplayAlert("Closed:", "This store opens at " + dtTemp.ToString("hh:mm tt") + ".", "Ok");
                                await Navigation.PopAsync();
                            }
                            return;
                        }
                    }
                    else  //Closed Today
                    {
                        await DisplayAlert("Closed:", "Sorry, this store is Closed today.", "Ok");
                        await Navigation.PopAsync();
                        return;
                    }
                }
                catch (Exception err)
                {
                    this.IsBusy = false;
                    var current = Connectivity.NetworkAccess;

                    if (current == NetworkAccess.Internet)
                    {
                        await DisplayAlert("Ooops..", "No business hours setup for " + DateTime.Now.ToString("dddd")+"(s).", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ooops..", "Unable to verify store hours: " + "You lost internet connection, try again", "OK");

                    }
                    return;
                }
                finally
                {
                    this.IsBusy = false;
                }
            }

            currOrd = App.OrderRepo.GetOrderLocationID();

            if ((currOrd != null) && (prod.LocationID != currOrd.LocationID))
            {
                await DisplayAlert("Selected Item from " + prod.Location, "Current Order:" + currOrd.Location, "Ok");
            }
            else
            {
                Product product = new Product();
                product = prod;
                OrderDetail ord = new OrderDetail();
                ord.ProductID = product.ProductID;
                ord.LocationID = product.LocationID;
                ord.ProductPriceID = product.ProductPriceID;
                ord.Location = product.Location;
                ord.Quantity = int.Parse(Quantity.Text);
                ord.PriceUOM = prod.UomPrices;
                ord.UOMQuantity = ord.Quantity * prod.ConversionFactor;
                IList prices = product.UomPrices.Split('/').ToList();
                var price = prices[0];
                var sprice = price.ToString();

                string currencySymbol = sprice.ToCharArray()[0].ToString();
                decimal currencyValue;
                CultureInfo currentCulture = CultureInfo.CreateSpecificCulture(prod.languageCode);

                ord.LineAmounts = "";

                if (decimal.TryParse(sprice, NumberStyles.Currency, currentCulture, out currencyValue))
                {


                    string amount = currencyValue.ToString("C", currentCulture);

                    if (prod.ConversionFactor > 1)
                    {
                        ord.LineAmounts = ord.Quantity.ToString() + "X" + prod.ConversionFactor.ToString() + " @" + amount + "/" + prices[1];
                    }
                    else
                    {
                        ord.LineAmounts = ord.Quantity.ToString() + " @" + amount + "/" + prices[1];
                    }

                    ord.UnitPrice = currencyValue;
                    ord.languageCode = prod.languageCode;
                    ord.Amount = ((ord.Quantity * prod.ConversionFactor) * currencyValue);
                    ord.currencyAmount = ord.Amount.ToString("C", currentCulture);
                }

                ord.ItemName = product.Name;

                App.OrderRepo.AddNewOrderDetail(ord);

                orderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());

                await Navigation.PopAsync(); //Remove the page currently on top.

            }
        }
    }
}