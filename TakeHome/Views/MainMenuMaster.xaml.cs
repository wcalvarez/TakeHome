using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TakeHome.Services;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using TakeHome.Models;

namespace TakeHome.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class MainMenuMaster : ContentPage
    {
        public ListView ListView;
        readonly DataManager manager = new DataManager();

        public MainMenuMaster()
        {
            InitializeComponent();
            BindingContext = new MainMenuMasterViewModel();
            ListView = MenuItemsListView;
        }

        public void MainMenuItem_Selected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var item = (MainMenuMenuItem)e.SelectedItem;
                switch (item.Title)
                {
                    case "Store Finder":
                        App.NavigationPage.Navigation.PopToRootAsync();
                        App.MenuIsPresented = false;
                        break;

                    case "Showrooms/Dealers Finder":
                        App.NavigationPage.Navigation.PushAsync(new LocationsPage());
                        App.MenuIsPresented = false;
                        break;

                    case "My Profile":
                        App.NavigationPage.Navigation.PushAsync(new MyProfilePage());
                        App.MenuIsPresented = false;
                        break;

                    case "Change Password":
                        App.NavigationPage.Navigation.PushAsync(new ChangePasswordPage());
                        App.MenuIsPresented = false;
                        break;

                    case "SignUp":
                        App.NavigationPage.Navigation.PushAsync(new SignUpPage());
                        App.MainMenu.Master = new MainMenuMaster();
                        App.MenuIsPresented = false;
                        break;

                    case "Login":
                        App.NavigationPage.Navigation.PushAsync(new LoginPage());
                        App.MainMenu.Master = new MainMenuMaster();
                        App.MenuIsPresented = false;
                        break;

                    case "LogOut":
                        App.IsUserLoggedIn = false;
                        App.user.IsLoggedIn = false;
                        App.DataRepo.UpdateAppUser(App.user);
                        App.MainMenu.Master = new MainMenuMaster();
                        App.NavigationPage.Navigation.PopToRootAsync();
                        App.MenuIsPresented = false;
                        break;

                    default:
                        break;
                }

                ListView.SelectedItem = null;
            }

        }

        class MainMenuMasterViewModel : INotifyPropertyChanged
        {
            public ObservableCollection<MainMenuMenuItem> MenuItems { get; set; }

            public MainMenuMasterViewModel()
            {
                MenuItems = new ObservableCollection<MainMenuMenuItem>();

                if (App.user != null && !App.user.IsLoggedIn)  //LoggedOut
                {
                    switch (App.AppName)
                    {
                        case "mobileOrders":

                            MenuItems = new ObservableCollection<MainMenuMenuItem>(new[]
                            {
                                new MainMenuMenuItem { Id = 0, Title = "Store Finder" },
                                new MainMenuMenuItem { Id = 1, Title = "My Profile" },
                                new MainMenuMenuItem { Id = 2, Title = "Login" }
                            });
                            break;

                        case "Kuyang":

                            MenuItems = new ObservableCollection<MainMenuMenuItem>(new[]
                            {
                                new MainMenuMenuItem { Id = 0, Title = "Lodge Locator" },
                                new MainMenuMenuItem { Id = 1, Title = "Member Verification" },
                                new MainMenuMenuItem { Id = 2, Title = "Login" }
                            });

                            break;
                        case "Dayfree":

                            MenuItems = new ObservableCollection<MainMenuMenuItem>(new[]
                            {
                                new MainMenuMenuItem { Id = 0, Title = "Post Orders" },
                                new MainMenuMenuItem { Id = 1, Title = "Showrooms/Dealers Finder" }
                            });

                            break;
                        default:

                            MenuItems = new ObservableCollection<MainMenuMenuItem>(new[]
                           {
                                new MainMenuMenuItem { Id = 2, Title = "Login" }
                            });
                            break;
                    }
                }

                if (App.user != null && App.user.IsLoggedIn)
                {
                    switch (App.AppName)
                    {
                        case "mobileOrders":

                            MenuItems = new ObservableCollection<MainMenuMenuItem>(new[]
                            {
                                new MainMenuMenuItem { Id = 0, Title = "Store Finder" },
                               // new MainMenuMenuItem { Id = 1, Title = "My Store Credits" },
                                new MainMenuMenuItem { Id = 2, Title = "My Profile" },
                                new MainMenuMenuItem { Id = 3, Title =  "Change Password"},
                                new MainMenuMenuItem { Id = 4, Title = "LogOut" }
                            });
                            break;

                        case "Kuyang":

                            MenuItems = new ObservableCollection<MainMenuMenuItem>(new[]
                            {
                                new MainMenuMenuItem { Id = 0, Title = "Member Profile" },
                                new MainMenuMenuItem { Id = 1, Title = "Dues & Donations" },
                                new MainMenuMenuItem { Id = 2, Title = "Lodge Locator" },
                                new MainMenuMenuItem { Id = 3, Title = "Member Verification" },
                                new MainMenuMenuItem { Id = 4, Title = "LogOut" }
                            });

                            break;
                        case "Dayfree":

                            MenuItems = new ObservableCollection<MainMenuMenuItem>(new[]
                            {
                                new MainMenuMenuItem { Id = 0, Title = "Post Orders" },
                                new MainMenuMenuItem { Id = 1, Title = "Showrooms/Dealers Finder" },
                                new MainMenuMenuItem { Id = 2, Title =  "Change Password"},
                                new MainMenuMenuItem { Id = 3, Title = "Printer Settings"},
                                new MainMenuMenuItem { Id = 4, Title = "LogOut" }
                            });

                            break;
                        default:

                            MenuItems = new ObservableCollection<MainMenuMenuItem>(new[]
                           {
                                new MainMenuMenuItem { Id = 2, Title = "LogOut" }
                            });
                            break;
                    }
                    //
                    //
                    //

                }
                else  //new User, not signed up yet or re-installed App
                {
                    if (App.user == null)
                    {
                        switch (App.AppName)
                        {
                            case "mobileOrders":
                                MenuItems = new ObservableCollection<MainMenuMenuItem>(new[]
                                {
                                    new MainMenuMenuItem { Id = 0, Title = "Store Finder" },
                                    new MainMenuMenuItem { Id = 1, Title = "Login" },
                                    new MainMenuMenuItem { Id = 2, Title = "SignUp" }
                                });
                                break;

                            case "Kuyang":
                                MenuItems = new ObservableCollection<MainMenuMenuItem>(new[]
 {
                                    new MainMenuMenuItem { Id = 0, Title = "Lodge Locator" },
                                    new MainMenuMenuItem { Id = 2, Title = "SignUp" }
                                });
                                break;
                            case "Dayfree":

                                MenuItems = new ObservableCollection<MainMenuMenuItem>(new[]
                                {
                                new MainMenuMenuItem { Id = 0, Title = "Post Orders" },
                                new MainMenuMenuItem { Id = 1, Title = "Showrooms/Dealers Finder" }
                            });

                                break;
                        }
                    }

                }

            }

            #region INotifyPropertyChanged Implementation
            public event PropertyChangedEventHandler PropertyChanged;
            void OnPropertyChanged([CallerMemberName] string propertyName = "")
            {
                if (PropertyChanged == null)
                    return;

                PropertyChanged.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
            #endregion
        }
    }
}