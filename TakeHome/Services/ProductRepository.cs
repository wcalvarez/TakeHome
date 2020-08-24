using SQLite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TakeHome.Models;

namespace TakeHome.Services
{
    public class ProductRepository
    {
        SQLiteConnection conn;
        public string StatusMessage { get; set; }

        // SQLiteCommand sqlite_cmd;
        public ProductRepository(string dbPath)
        {
            conn = new SQLiteConnection(dbPath);
            //conn.DropTable<Product>();
            conn.CreateTable<Product>();
        }

        public void AddNewProduct(Product product)
        {
            int result = 0;
            try
            {
                //basic validation to ensure a name was entered
                if (string.IsNullOrEmpty(product.Name))
                    throw new Exception("Product name required");

                result = conn.Insert(product);

                StatusMessage = string.Format("{0} record(s) added [Product: {1})", result, product.Name);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", product.Name, ex.Message);
            }
        }

        public void DeleteAllProducts()
        {
            conn.DropTable<Product>();
            conn.CreateTable<Product>();
        }

        public List<Product> GetAllProducts(int locID)
        {
            try
            {
                return conn.Table<Product>().Where(p => p.LocationID == locID).ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<Product>();
        }

        public Product GetProduct(int productID)
        {
            Product prod = new Product();

            try
            {
                prod = conn.Table<Product>().Where(l => l.ProductID == productID).Single();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return prod;
        }

        public List<Product> GetDownloadedProducts()
        {
            try
            {
                return conn.Table<Product>().OrderByDescending(l => l.Name).ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<Product>();
        }
    }
}
