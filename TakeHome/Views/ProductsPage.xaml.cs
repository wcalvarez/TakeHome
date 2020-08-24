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
using TakeHome.ViewModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TakeHome.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class ProductsPage : ContentPage
    {
        ProductsViewModel viewModel;
        public static IList<Product> productslist = new ObservableCollection<Product>();
        public static ObservableCollection<Grouping<string, Product>> ProductsGrouped { get; set; }
        public static ObservableCollection<OrderDetail> OrderLines { get; set; }
        private IList<Product> products = new ObservableCollection<Product>();
        GroupedProductModel group;
        public ObservableCollection<GroupedProductModel> grouped;
        public ObservableCollection<Product> Products { get; set; }
        readonly DataManager manager = new DataManager();

        int locationID;
        public ProductsPage(int locID)
        {
            locationID = locID;
            InitializeComponent();
            viewModel = new ProductsViewModel(locID);
            BindingContext = viewModel;
        }

        protected async override void OnAppearing()
        {

            storename.Text = App.BrowsingLocation.LocationName;
            //lstView.ItemsSource = viewModel.grouped;
            base.OnAppearing();

            //Cart orderlines indicator
            //NavigationBarView.Title = "Products";

            OrderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());

            int qty = 0;
            if (OrderLines.Count > 0)
            {
                qty = OrderLines.Select(o => o.Quantity).Aggregate((x, y) => x + y);
            }
            else
            {

            }

            NavigationBarView.FirstNameLabel.Text = qty.ToString();

            //load products from backend all-the-time
            if (products.Count == 0)
            {
                //productslist = (IList<Product>)await downloadProducts(id);


                // Turn on network indicator
                this.IsBusy = true;

                try
                {
                    var prods = await manager.GetProducts(locationID);
                    var productList = JsonConvert.DeserializeObject<IList<Product>>(prods);
                    //productsList = JsonConvert.DeserializeObject<IList<Product>>(prods);
                    if (productList.Count == 0)
                    {
                        await DisplayAlert("Sorry:", "No Products found for Store Location", "Ok");
                        this.IsBusy = false;
                        await Navigation.PopAsync();
                        return;
                    }


                    foreach (Product product in productList)
                    {
                        if (product.PhotoBase64 != null)
                        {
                            product.SmallImage = System.Convert.FromBase64String(product.PhotoBase64);
                        }


                        if (App.BrowsingLocation.CustomerPricing && product.ConversionFactor > 1)
                        {
                            IList prices = product.UomPrices.Split('/').ToList();
                            var price = prices[0];
                            string uom = prices[1].ToString();
                            string sprice = App.CurrentCustomer.DefaultPrice.ToString();
                            System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(product.languageCode);
                            string curr = NumberFormatInfo.CurrentInfo.CurrencySymbol;
                            product.UomPrices = curr + " " + sprice + "/" + uom;
                        }

                        product.Location = App.BrowsingLocation.LocationName;
                        products.Add(product);
                    //    Products.Add(product);

                        //App.ProductRepo.AddNewProduct(product);
                    }
                    //BindingContext = viewModel;
                    //
                    //
                    //products = Products.ToList();
                    //productslist = Products;



                    //this.GroupProducts(products);
                    var sorted = from product in products
                                 orderby product.DisplaySort, product.Name
                                 group product by product.Category into productGroup
                                 select new Grouping<string, Product>(productGroup.Key, productGroup);



                    //if (App.BrowsingLocation.SalesOrderEntry)
                    //{
                    //    storename.Text = products[0].Location + ":" + App.CurrentCustomer.Name;
                    //}

                    var categories = new ObservableCollection<Grouping<string, Product>>(sorted);
                    grouped = new ObservableCollection<GroupedProductModel>();

                    foreach (Grouping<string, Product> items in categories)
                    {
                        string prodCategory = items.Key;

                        group = new GroupedProductModel() { LongName = prodCategory, ShortName = "" };
                        grouped.Add(group);
                        foreach (Product item in items)
                        {
                            ProductModel pmodel = new ProductModel();

                            pmodel.ProductID = item.ProductID;
                            pmodel.LocationID = item.LocationID;
                            pmodel.ProductPriceID = item.ProductPriceID;
                            pmodel.Location = item.Location;
                            pmodel.GroupName = item.GroupName;
                            pmodel.Category = item.Category;
                            pmodel.DisplaySort = item.DisplaySort;
                            pmodel.Name = item.Name;
                            pmodel.Description = item.Description;
                            pmodel.PhotoBase64 = item.PhotoBase64;
                            pmodel.UomPrices = item.UomPrices;

                            //if (App.BrowsingLocation.CustomerPricing && item.ConversionFactor > 1)
                            //{
                            //    IList prices = pmodel.UomPrices.Split('/').ToList();
                            //    var price = prices[0];
                            //    string uom = prices[1].ToString();
                            //    string sprice = App.CurrentCustomer.DefaultPrice.ToString();
                            //    System.Threading.Thread.CurrentThread.CurrentCulture = new CultureInfo(item.languageCode);
                            //    string curr = NumberFormatInfo.CurrentInfo.CurrencySymbol;
                            //    pmodel.UomPrices = curr + " " + sprice + "/" + uom;
                            //}

                            pmodel.languageCode = item.languageCode;
                            pmodel.QuantityOnHand = item.QuantityOnHand;
                            pmodel.ConversionFactor = item.ConversionFactor;
                            pmodel.Featured = item.Featured;
                            pmodel.MembersOnly = item.MembersOnly;

                            if (item.PhotoBase64 != null)
                            {
                                pmodel.SmallImage = System.Convert.FromBase64String(item.PhotoBase64);
                            }
                            group.Add(pmodel);
                        }

                    }
                    lstView.ItemsSource = grouped;

                    //
                    //
                }
                catch (Exception err)
                {
                    this.IsBusy = false;
                    //await DisplayAlert("Ooops..", "Unable to load Products" + err.Message, "OK");
                    var current = Connectivity.NetworkAccess;

                    if (current == NetworkAccess.Internet)
                    {
                        // Connection to internet is available
                        await DisplayAlert("Ooops..", "Unable to Load Products: " + err.Message, "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ooops..", "Unable to load Products: " + "You lost internet connection, try again", "OK");

                    }
                    await Navigation.PopAsync();
                    return;
                }
                finally
                {
                    this.IsBusy = false;
                }

            }


             //NavigationBarView.Title = "Products";

            //OrderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());

            //int qty = 0;
            //if (OrderLines.Count > 0)
            //{
            //    qty = OrderLines.Select(o => o.Quantity).Aggregate((x, y) => x + y);
            //}
            //else
            //{

            //}

            //NavigationBarView.FirstNameLabel.Text = qty.ToString();
            //this.NavigationBarView.FirstNameLabel.Text = qty.ToString();

            // }
            //async void ListView_ItemTapped(object sender, SelectedItemChangedEventArgs e)
            //{
            //Hack for non selected menu-item
            //    if (e.SelectedItem == null) return;
            //}
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();

        }


        async void ProductSelected(object sender, SelectedItemChangedEventArgs e)
        {
            if (e.SelectedItem != null)
            {
                var pmodel = (ProductModel)e.SelectedItem;
                var prod = new Product();

                prod.ProductID = pmodel.ProductID;
                prod.LocationID = pmodel.LocationID;
                prod.ProductPriceID = pmodel.ProductPriceID;
                prod.Location = pmodel.Location;
                prod.GroupName = pmodel.GroupName;
                prod.Category = pmodel.Category;
                prod.Name = pmodel.Name;
                prod.Description = pmodel.Description;
                prod.PhotoBase64 = pmodel.PhotoBase64;
                prod.SmallImage = pmodel.SmallImage;
                prod.UomPrices = pmodel.UomPrices;
                prod.languageCode = pmodel.languageCode;
                prod.QuantityOnHand = pmodel.QuantityOnHand;
                prod.ConversionFactor = pmodel.ConversionFactor;
                prod.Featured = pmodel.Featured;
                prod.MembersOnly = pmodel.MembersOnly;

                // clear selected item
                lstView.SelectedItem = null;

                await Navigation.PushAsync(new OrderItemPage((prod)));

            }
        }
    }

}