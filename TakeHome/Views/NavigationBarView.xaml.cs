using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeHome.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TakeHome.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class NavigationBarView : ContentView
    {
        public static ObservableCollection<OrderDetail> OrderLines { get; set; }
        public NavigationBarView()
        {
            InitializeComponent();
            //this.FirstNameLabel.Text = App.OrderRepo.GetAllOrderDetails().Count.ToString();
        }

        public Label FirstNameLabel
        {
            get
            {
                return labelText;
            }
        }
        private void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

        public static readonly BindableProperty TitleProperty =
        BindableProperty.Create(
        "Title",
        typeof(string),
        typeof(NavigationBarView),
        "this is Title",
        propertyChanged: OnTitlePropertyChanged
        );
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        static void OnTitlePropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var thisView = bindable as NavigationBarView;
            var title = newValue.ToString();
            thisView.lblTitle.Text = title;
        }

        async void Tapcart_OnTapped(object sender, EventArgs e)
        {
            OrderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());
            if (OrderLines.Count > 0)
            {
                await Navigation.PushAsync(new OrderPage());
            }


        }
    }
}