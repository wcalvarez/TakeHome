using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TakeHome.Services;
using TakeHome.Views;
using TakeHome.Models;

namespace TakeHome
{
    public partial class App : Application
    {
        //public static  string Url = "http://192.168.1.10:5000/";
        //public static string Url = "http://10.99.43.56:5000/";
        //public static string Url = "https://www.winmobilesales.com/";
        public static string Url = "https://mobileorders20190611090658.azurewebsites.net/";
        public static bool IsUserLoggedIn { get; set; }
        public static Location OrderLocation { get; set; }
        public static Printer SavedPrinter { get; set; }
        public static NavigationPage NavigationPage { get; set; }
        public static MainMenu MainMenu;
        public static int OrderLocationID { get; set; }
        public static AppUser user = new AppUser();
        public static string AppName = "mobileOrders";
        public static ShippingInfo shipto = new ShippingInfo();
        public static GuestAppUser guest_user = new GuestAppUser();
        public static Location BrowsingLocation { get; set; }
        public static Customer CurrentCustomer { get; set; }
        public static BusinessHour storeHours { get; set; }
        //Repositories
        public static OrderHeaderRepository OrderHeaderRepo { get; set; }
        public static AddressRepository AddressRepo { get; set; }
        public static DataRepository DataRepo { get; set; }
        public static ProductRepository ProductRepo { get; set; }
        public static ZOrderDetailRepository ZOrderRepo { get; set; }
        public static CustomerRepository CustomerRepo { get; set; }
        public static OrderDetailRepository OrderRepo { get; set; }
        public static LocationRepository LocationRepo { get; set; }

        public static bool MenuIsPresented
        {
            get
            {
                return MainMenu.IsPresented;
            }
            set
            {
                MainMenu.IsPresented = value;
            }
        }
        public App(string dbPath)
        {
            InitializeComponent();
            OrderRepo = new OrderDetailRepository(dbPath);
            AddressRepo = new AddressRepository(dbPath);
            LocationRepo = new LocationRepository(dbPath);
            DataRepo = new DataRepository(dbPath);
            ProductRepo = new ProductRepository(dbPath);
            OrderHeaderRepo = new OrderHeaderRepository(dbPath);
            ZOrderRepo = new ZOrderDetailRepository(dbPath);

            NavigationPage = new NavigationPage(new LocationsPage());

            //get AppUser
            user = DataRepo.GetAppUser();

            var menuPage = new MainMenuMaster { Title = "Home", IconImageSource = "icons8_menu_30.png" };

            NavigationPage = new NavigationPage(new LocationsPage());
            //}

            MainMenu = new MainMenu();
            MainMenu.Master = menuPage;
            MainMenu.Detail = NavigationPage;
            NavigationPage.BarBackgroundColor = Color.Tomato;
            MainPage = MainMenu;
        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {
        }

        protected override void OnResume()
        {
        }
    }
}
