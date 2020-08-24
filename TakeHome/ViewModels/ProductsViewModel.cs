using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TakeHome.Models;
using TakeHome.ViewModels;
using TakeHome.Services;

namespace TakeHome.ViewModels
{
    public class ProductsViewModel : BaseViewModel
    {
        
        public ObservableCollection<Product> Products { get; set; }
        public static IList<Product> productslist = new ObservableCollection<Product>();
        private IList<Product> products = new ObservableCollection<Product>();
        GroupedProductModel group;
        public static ObservableCollection<Grouping<string, Product>> ProductsGrouped { get; set; }
        public ObservableCollection<GroupedProductModel> grouped;

        public ProductsViewModel()
        { 

        }
            public ProductsViewModel(int loccationID)
        {
            Title = "Products";
            Products = new ObservableCollection<Product>();
 


            products = Products.ToList();
            productslist = Products;



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
                var Will = "Awesome";

            }


            //}
            //catch (Exception)
            //{
            //    this.IsBusy = false;
            //    await DisplayAlert("Ooops..", "Unable to load/reload Products", "OK");
            //    await Navigation.PopToRootAsync();
            //}
            //finally
            //{
            //    this.IsBusy = false;
            //}

            //}
        }
    }
}
