using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeHome.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TakeHome.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenu : MasterDetailPage
    {

            public List<MainMenuItem> MainMenuItems { get; set; }

            public MainMenu()
            {
                // Set the binding context to this code behind.
                BindingContext = this;

                // Build the Menu
                if (App.user != null && !App.user.IsLoggedIn)
                {
                    MainMenuItems = new List<MainMenuItem>()
                {
                    new MainMenuItem() { Title = "Store Finder", Icon = "menu_inbox.png", TargetType = typeof(LocationsPage) },
                    //new MainMenuItem() { Title = "My Profile", Icon = "menu_stock.png", TargetType = typeof(MyProfilePage) },
                    new MainMenuItem() { Title = "Login", Icon = "menu_stock.png", TargetType = typeof(LoginPage) }
                };
                }

                if (App.user != null && App.user.IsLoggedIn)
                {
                    MainMenuItems = new List<MainMenuItem>()
                {
                    new MainMenuItem() { Title = "Store Finder", Icon = "menu_inbox.png", TargetType = typeof(LocationsPage) },
                    //new MainMenuItem() { Title = "My Profile", Icon = "menu_stock.png", TargetType = typeof(MyProfilePage) },
                    new MainMenuItem() { Title = "Logout", Icon = "menu_stock.png", TargetType = typeof(LocationsPage) }
                };
                }


                if (App.user == null)
                {
                    MainMenuItems = new List<MainMenuItem>()
                {
                    new MainMenuItem() { Title = "Store Finder", Icon = "menu_inbox.png", TargetType = typeof(LocationsPage) },
                    //new MainMenuItem() { Title = "My Profile", Icon = "menu_stock.png", TargetType = typeof(MyProfilePage) },
                    new MainMenuItem() { Title = "SignUp", Icon = "menu_stock.png", TargetType = typeof(SignUpPage) }
                };
                }
                else
                {
                    MainMenuItems = new List<MainMenuItem>()
                {
                    new MainMenuItem() { Title = "Store Finder", Icon = "menu_inbox.png", TargetType = typeof(LocationsPage) },
                    //new MainMenuItem() { Title = "My Store Credits", Icon = "menu_stock.png", TargetType = typeof(StoreCreditsDashboard) },
                    //new MainMenuItem() { Title = "My Profile", Icon = "menu_stock.png", TargetType = typeof(MyProfilePage) },
                    new MainMenuItem() { Title = "Sign Out", Icon = "menu_stock.png", TargetType = typeof(LocationsPage) }
                };
                }
                // Set the default page, this is the "home" page.
                Detail = new NavigationPage(new LocationsPage());
                NavigationPage.SetHasNavigationBar(this, false);

                InitializeComponent();
                //this.Title = "Home";
            }

        // When a MenuItem is selected.
        public void MainMenuItem_Selected(object sender, SelectedItemChangedEventArgs e)
        {
            var item = e.SelectedItem as MainMenuItem;
            if (item != null)
            {
                if (item.Title.Equals("Store Finder"))
                {
                    Detail = new NavigationPage(new LocationsPage());
                }
                else if (item.Title.Equals("My Store Credits"))
                {
                   // Detail = new NavigationPage(new StoreCreditsDashboard());
                }
                else if (item.Title.Equals("My Profile"))
                {
                    Detail = new NavigationPage(new MyProfilePage());
                }
                else if (item.Title.Equals("Login"))
                {
                    Detail = new NavigationPage(new LoginPage());
                }
                else if (item.Title.Equals("Sign Out"))
                {
                    App.user.IsLoggedIn = false;
                    App.IsUserLoggedIn = false;
                    App.DataRepo.UpdateAppUser(App.user);
                    Detail = new NavigationPage(new LocationsPage());
                }

                MenuListView.SelectedItem = null;
                // IsPresented = false;
            }
        }
        }
    }