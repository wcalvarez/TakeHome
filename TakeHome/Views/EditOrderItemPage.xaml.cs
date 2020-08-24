using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeHome.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TakeHome.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class EditOrderItemPage : ContentPage
    {
        OrderDetail editDetail = new OrderDetail();
        public static ObservableCollection<OrderDetail> OrderLines { get; set; }


        Product prod = new Product();

        public EditOrderItemPage(OrderDetail orderDetail)
        {
            editDetail = orderDetail;
            prod = App.ProductRepo.GetProduct(orderDetail.ProductID);
            OrderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());
            BindingContext = orderDetail;
            InitializeComponent();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            NavigationBarView.Title = "Edit Order";
            EditQuantity.Focus();
            int qty = OrderLines.Sum(P => P.Quantity);
            storename.Text = App.BrowsingLocation.LocationName;
            NavigationBarView.FirstNameLabel.Text = qty.ToString();
        }

        public async void OnClickCancel(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }

        public async void OnClickSave(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(EditQuantity.Text))
            {
                await DisplayAlert("Error:", "Quantity can not be blank", "Ok");
                return;
            }

            if (int.Parse(EditQuantity.Text) < 1)
            {
                await DisplayAlert("Error:", "Quantity should be at least 1", "Ok");
                return;
            }

            editDetail.Quantity = int.Parse(EditQuantity.Text);
            editDetail.UOMQuantity = editDetail.Quantity;

            IList prices = editDetail.PriceUOM.Split('/').ToList();
            var price = prices[0];
            var sprice = price.ToString();

            //char currencySymbol = sprice.ToCharArray()[0];
            string currencySymbol = sprice.ToCharArray()[0].ToString();
            decimal currencyValue;
            CultureInfo currentCulture = CultureInfo.CreateSpecificCulture(editDetail.languageCode);
            if (decimal.TryParse(sprice, NumberStyles.Currency, currentCulture, out currencyValue))
            {
                string amount = currencyValue.ToString("C", currentCulture);
                //if (prod.ConversionFactor > 1)
                //{
                //    editDetail.LineAmounts = editDetail.Quantity.ToString() + "X" + prod.ConversionFactor.ToString() + " @" + amount + "/" + prices[1];
                //}
                //else
                //{
                    editDetail.LineAmounts = editDetail.Quantity.ToString() + " @" + amount + "/" + prices[1];
              //  }

                editDetail.UnitPrice = currencyValue;
                editDetail.Amount = (editDetail.Quantity * currencyValue);
                editDetail.currencyAmount = editDetail.Amount.ToString("C", currentCulture);
            }
            //

            App.OrderRepo.UpdateOrderDetail(editDetail);
            await Navigation.PopAsync();
        }
    }
}