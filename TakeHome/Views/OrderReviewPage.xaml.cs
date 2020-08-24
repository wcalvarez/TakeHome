using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeHome.Models;
using TakeHome.Services;
using TakeHome.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TakeHome.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class OrderReviewPage : ContentPage
    {

        OrderReviewViewModel viewModel;
      //  public static ObservableCollection<OrderDetail> OrderLines { get; set; }
        readonly DataManager manager = new DataManager();
        OrderDetail detailForEdit = new OrderDetail();
        CultureInfo currentCulture;

        public OrderReviewPage()
        {
            InitializeComponent();

            ListView lstView = new ListView();

            lstView.ItemSelected += OnSelection;
            lstView.ItemTapped += OnTap;
           // viewModel = new OrderReviewViewModel();
            

            var temp = new DataTemplate(typeof(TextCell));
            lstView.ItemTemplate = temp;
            Content = lstView;
          //  viewModel = new OrderReviewViewModel();
          //  lstView.ItemsSource = viewModel.OrderLines;
          //  BindingContext = viewModel.OrderLines;
            BindingContext = viewModel = new OrderReviewViewModel();
            lstView.ItemsSource = viewModel.OrderLines;
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

             OrderDetail ord = viewModel.OrderLines.First();
            storename.Text = ord.Location;
            currentCulture = CultureInfo.CreateSpecificCulture(ord.languageCode);
            string curr = currentCulture.NumberFormat.CurrencySymbol;

         //   OrderLines = viewModel.OrderLines;
            BindingContext = viewModel.OrderLines;

            Decimal subTotal = viewModel.OrderLines.Sum(P => P.Amount);
            string amount = subTotal.ToString("C", currentCulture);

            Resources["SubTotal"] = "Subtotal:" + amount;
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
            //Do something here... e.g. Navigation.pushAsync(new specialPage(item.commandParameter));
            //page.DisplayAlert("More Context Action", item.CommandParameter + " more context action", 	"OK");
        }

        private async void OnDelete(object sender, System.EventArgs e)
        {
            var menuItem = ((MenuItem)sender);
            OrderDetail ordDetail = (OrderDetail)menuItem.CommandParameter;
            App.OrderLocationID = ordDetail.LocationID;
            viewModel.OrderLines.Remove(ordDetail);
            App.OrderRepo.DeleteOrderDetail(ordDetail);
            //OrderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());

            int qty = viewModel.OrderLines.Sum(P => P.Quantity);
            if (qty == 0)
            {
                await Navigation.PopAsync();
                await Navigation.PushAsync(new ProductsPage(viewModel.OrderLines.First().LocationID));
            }

            //BindingContext = OrderLines;
           // this.NavigationBarView.FirstNameLabel.Text = qty.ToString();

            Decimal subTotal = viewModel.OrderLines.Sum(P => P.Amount);
            Resources["SubTotal"] = "Subtotal:" + String.Format("{0:C2}", subTotal);
        }

        async void OnClickCheckout(object sender, EventArgs e)
        {
            Location loc = App.LocationRepo.GetLocation(viewModel.OrderLines.First().LocationID);

            if (loc.Delivery)
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
                        await Navigation.PushAsync(new CheckOutPage());
                    }
                    else //Cancel
                    {
                        await Navigation.PopAsync();
                    }
                }
            }
            else
            {
                if (!App.BrowsingLocation.SalesOrderEntry)
                {
                    await Navigation.PushAsync(new AddressPage());
                }
                else
                {
                    await Navigation.PushAsync(new CheckOutPage());
                }

            }
        }
    }
}