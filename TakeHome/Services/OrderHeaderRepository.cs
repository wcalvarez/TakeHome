using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using TakeHome.Models;

namespace TakeHome.Services
{
    public class OrderHeaderRepository
    {
        SQLiteConnection conn;
        public string StatusMessage { get; set; }
        // SQLiteCommand sqlite_cmd;
        public OrderHeaderRepository(string dbPath)
        {
            conn = new SQLiteConnection(dbPath);
            //conn.DropTable<OrderHeader>();
            conn.CreateTable<OrderHeader>();
        }

        public int AddNewOrderHeader(OrderHeader orderHeader)
        {
            int result = 0;
            try
            {
                //basic validation to ensure a name was entered


                result = conn.Insert(orderHeader);


            }
            catch (Exception ex)
            {
                //StatusMessage = string.Format("Failed to add {0}. Error: {1}", orderHeader.ItemName, ex.Message);
            }
            return result;
        }

        //public string UpdateOrderHeader(OrderHeader orderHeader)
        //{
        //    int result = 0;
        //    try
        //    {
        //        //basic validation to ensure a name was entered
        //        if (orderHeader.Quantity > 0)
        //        {
        //            result = conn.Update(orderHeader);
        //        }
        //        StatusMessage = "OrderHeader Updated Successfully.";
        //    }
        //    catch (Exception ex)
        //    {
        //        StatusMessage = "OrderHeader Update unsucessfull.";
        //    }

        //    return StatusMessage;
        //}

        public void DeleteOrderHeader(OrderHeader orderHeader)
        {
            conn.Delete(orderHeader);
        }

        public void DeleteAllOrderHeader()
        {
            conn.DropTable<OrderHeader>();
            conn.CreateTable<OrderHeader>();
        }

        public List<OrderHeader> GetAllOrderHeaders()
        {
            try
            {
                return conn.Table<OrderHeader>().OrderBy(o => o.OrderDate).ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<OrderHeader>();
        }

        public OrderHeader GetOrderLocationID()
        {
            try
            {
                return conn.Table<OrderHeader>().First();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
