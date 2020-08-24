using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using TakeHome.Models;

namespace TakeHome.Services
{
    public class OrderDetailRepository
    {
        SQLiteConnection conn;
        public string StatusMessage { get; set; }
        // SQLiteCommand sqlite_cmd;
        public OrderDetailRepository(string dbPath)
        {
            conn = new SQLiteConnection(dbPath);
            conn.DropTable<OrderDetail>();
            conn.CreateTable<OrderDetail>();
        }

        public void AddNewOrderDetail(OrderDetail ordDetail)
        {
            int result = 0;
            try
            {
                //basic validation to ensure a name was entered
                if (string.IsNullOrEmpty(ordDetail.ItemName))
                    throw new Exception("Valid name required");

                result = conn.Insert(ordDetail);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, ordDetail.ItemName);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", ordDetail.ItemName, ex.Message);
            }
        }

        public string UpdateOrderDetail(OrderDetail orderDetail)
        {
            int result = 0;
            try
            {
                //basic validation to ensure a name was entered
                if (orderDetail.Quantity > 0)
                {
                    result = conn.Update(orderDetail);
                }
                StatusMessage = "OrderDetail Updated Successfully.";
            }
            catch (Exception ex)
            {
                StatusMessage = "OrderDetail Update unsucessfull.";
            }

            return StatusMessage;
        }

        public void DeleteOrderDetail(OrderDetail orderDetail)
        {
            conn.Delete(orderDetail);
        }

        public void DeleteAllOrderDetail()
        {
            conn.DropTable<OrderDetail>();
            conn.CreateTable<OrderDetail>();
        }

        public List<OrderDetail> GetAllOrderDetails()
        {
            try
            {
                return conn.Table<OrderDetail>().Where(d => d.ZOrderHeaderID == 0).ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<OrderDetail>();
        }

        public OrderDetail GetOrderLocationID()
        {
            try
            {
                return conn.Table<OrderDetail>().First();
            }
            catch (Exception ex)
            {
                return null;
            }
        }
    }
}
