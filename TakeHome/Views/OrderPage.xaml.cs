using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
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
    public partial class OrderPage : ContentPage
    {
        public static ObservableCollection<OrderDetail> OrderLines { get; set; }
        
        readonly DataManager manager = new DataManager();
        OrderDetail detailForEdit = new OrderDetail();
        CultureInfo currentCulture;

        public OrderPage()
        {
            ListView lstView = new ListView();

            lstView.ItemSelected += OnSelection;
            lstView.ItemTapped += OnTap;
            lstView.ItemsSource = OrderLines;

            var temp = new DataTemplate(typeof(TextCell));
            lstView.ItemTemplate = temp;
            Content = lstView;
            OrderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());
            if(OrderLines.Count == 0)
            {
                //NoItems();
            }
            BindingContext = OrderLines;

            InitializeComponent();
        }

        protected override void   OnAppearing()
        {

            base.OnAppearing();
            OrderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());
            
            if (OrderLines.Count > 0)
            {
                OrderDetail ord = OrderLines.First();
                storename.Text = ord.Location;
                this.Title = this.Title + "-" + ord.Location;
                if (App.BrowsingLocation.SalesOrderEntry)
                {
                    storename.Text = ord.Location + ":" + ord.ProductName;
                }
                currentCulture = CultureInfo.CreateSpecificCulture(ord.languageCode);
                string curr = currentCulture.NumberFormat.CurrencySymbol;

                NavigationBarView.Title = "Your Order";

                OrderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());
                BindingContext = OrderLines;

                Decimal subTotal = OrderLines.Sum(P => P.Amount);
                string amount = subTotal.ToString("C", currentCulture);

                Resources["SubTotal"] = "Subtotal:" + amount;

                int qty = OrderLines.Sum(P => P.Quantity);

                NavigationBarView.FirstNameLabel.Text = qty.ToString();
            }
            else
            {

                NoItems();
            }
        }

        private async void NoItems()
        {
            await DisplayAlert("Error:", "No items on your shopping cart.", "Ok");
            //await Shell.Current.GoToAsync("//main/stores");
            return;
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            
        }
        public async void OnSelection(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem == null)
            {
                return; //ItemSelected is called on deselection, which results in SelectedItem being set to null
            }
            //overlay.IsVisible = true;

            detailForEdit = (OrderDetail)e.SelectedItem;
            await Navigation.PushAsync(new EditOrderItemPage(detailForEdit));
            //OrderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());
            //BindingContext = OrderLines;
            //Decimal subTotal = OrderLines.Sum(P => P.Amount);
            //string amount = subTotal.ToString("C", currentCulture);

            //Resources["SubTotal"] = "Subtotal:" + amount;
            ListView lst = (ListView)sender;
            lst.SelectedItem = null;
        }
        void OnTap(object sender, ItemTappedEventArgs e)
        {
            DisplayAlert("Item Tapped", e.Item.ToString(), "Ok");
        }

        void OnMore(object sender, EventArgs e)
        {
            var item = (MenuItem)sender;
        }

        private async void OnDelete(object sender, System.EventArgs e)
        {
            var menuItem = ((MenuItem)sender);
            OrderDetail ordDetail = (OrderDetail)menuItem.CommandParameter;
            App.OrderLocationID = ordDetail.LocationID;
            OrderLines.Remove(ordDetail);
            App.OrderRepo.DeleteOrderDetail(ordDetail);
            OrderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());

            int qty = OrderLines.Sum(P => P.Quantity);
            if (qty == 0)
            {
                await Navigation.PopAsync();
                await Navigation.PushAsync(new ProductsPage(App.OrderLocationID));
            }

            BindingContext = OrderLines;
            this.NavigationBarView.FirstNameLabel.Text = qty.ToString();

            Decimal subTotal = OrderLines.Sum(P => P.Amount);
            Resources["SubTotal"] = "Subtotal:" + String.Format("{0:C2}", subTotal);
        }

        async void OnClickCheckout(object sender, EventArgs e)
        {

            if (OrderLines.Count == 0)
            {
                await DisplayAlert("Error:", "No items on your shopping cart.", "Ok");
                return;
            }
            Location loc = App.LocationRepo.GetLocation(OrderLines.First().LocationID);

            if (App.BrowsingLocation.Delivery)
            {
                var action = await DisplayActionSheet("Deliver or Pickup?", "Cancel", null, "Pickup", "Deliver");
                //Debug.WriteLine("Action: " + action);

                if (action == "Deliver")
                {
                    await Navigation.PushAsync(
                    new AddressPage());
                }
                else
                {
                    if (action != "Cancel")
                    {
 
                        bool pickup = true;
                        await Navigation.PushAsync(new CheckOutPage(pickup));
                    }
                    else //Cancel
                    {
                        await Navigation.PopAsync();
                    }
                }
            }
            else
            {
                //if (!App.BrowsingLocation.SalesOrderEntry)
                //{
                //    await Navigation.PushAsync(new AddressPage());
                //}
                //else
                //{
                    await Navigation.PushAsync(new CheckOutPage(true));
                //}

            }
        }
    }
}