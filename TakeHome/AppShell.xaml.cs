using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using TakeHome.Models;
using TakeHome.Views;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace TakeHome
{
    public partial class AppShell : Shell
    {
        //  Random rand = new Random();
        Dictionary<string, Type> routes = new Dictionary<string, Type>();
        public Dictionary<string, Type> Routes { get { return routes; } }
        public static ObservableCollection<OrderDetail> OrderLines { get; set; }

        public ICommand HelpCommand => new Command<string>((url) => Launcher.CanOpenAsync(new Uri(url)));
      ////  public ICommand RandomPageCommand => new Command(async () => await NavigateToRandomPageAsync());

        public AppShell()
        {
            InitializeComponent();
            RegisterRoutes();
            OrderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());
            //MessagingCenter.Subscribe<object>(this, "hi", (sender) => {
            //    logout_item.Text = "Yuki";
            //});
            //RegisterRoutes();
            //BindingContext = this;
        }

        void RegisterRoutes()
        {
            routes.Add("locations", typeof(LocationsPage));
            routes.Add("products", typeof(ProductsPage));
            routes.Add("orderitem", typeof(OrderItemPage));
            routes.Add("orderpage", typeof(OrderPage));
            routes.Add("checkout", typeof(CheckOutPage));

            foreach (var item in routes)
            {
                Routing.RegisterRoute(item.Key, item.Value);
            }
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            //MessagingCenter.Subscribe<object>(this, "hi", (sender) => {
            //    cart.Title = "Yuki";
            //});
            OrderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());

        }
        //void RegisterRoutes()
        //{
        //    routes.Add("itemdetails", typeof(ItemDetailPage));
        //    routes.Add("itemspage", typeof(ItemsPage));
        //    routes.Add("newitempage", typeof(NewItemPage));

        //    foreach (var item in routes)
        //    {
        //        Routing.RegisterRoute(item.Key, item.Value);
        //    }
        //}

        //async Task NavigateToRandomPageAsync()
        //{
        //    string destinationRoute = routes.ElementAt(rand.Next(0, routes.Count)).Key;
        //    string animalName = null;

        //    switch (destinationRoute)
        //    {
        //        case "monkeydetails":
        //            animalName = MonkeyData.Monkeys.ElementAt(rand.Next(0, MonkeyData.Monkeys.Count)).Name;
        //            break;
        //        case "beardetails":
        //            animalName = BearData.Bears.ElementAt(rand.Next(0, BearData.Bears.Count)).Name;
        //            break;
        //        case "catdetails":
        //            animalName = CatData.Cats.ElementAt(rand.Next(0, CatData.Cats.Count)).Name;
        //            break;
        //        case "dogdetails":
        //            animalName = DogData.Dogs.ElementAt(rand.Next(0, DogData.Dogs.Count)).Name;
        //            break;
        //        case "elephantdetails":
        //            animalName = ElephantData.Elephants.ElementAt(rand.Next(0, ElephantData.Elephants.Count)).Name;
        //            break;
        //    }

        //    ShellNavigationState state = Shell.Current.CurrentState;
        //    await Shell.Current.GoToAsync($"{state.Location}/{destinationRoute}?name={animalName}");
        //    Shell.Current.FlyoutIsPresented = false;
        //}

        void OnNavigating(object sender, ShellNavigatingEventArgs e)
        {
            // Cancel any back navigation
            //if (e.Source == ShellNavigationSource.Pop)
            //{
            //    e.Cancel();
            //}
        }

        void OnNavigated(object sender, ShellNavigatedEventArgs e)
        {
        }
    }
}

