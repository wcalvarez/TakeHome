using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using TakeHome.Models;

namespace TakeHome.Services
{
    public class ZOrderDetailRepository
    {

        SQLiteConnection conn;
        public string StatusMessage { get; set; }
        // SQLiteCommand sqlite_cmd;

        public ZOrderDetailRepository(string dbPath)
        {
            conn = new SQLiteConnection(dbPath);
            // conn.DropTable<ZOrderDetail>();
            conn.CreateTable<ZOrderDetail>();
        }
        public void AddNewZOrderDetail(ZOrderDetail ordDetail)
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

        public string UpdateZOrderDetail(ZOrderDetail orderDetail)
        {
            int result = 0;
            try
            {
                //basic validation to ensure a name was entered
                if (orderDetail.Quantity > 0)
                {
                    result = conn.Update(orderDetail);
                }
                StatusMessage = "ZOrderDetail Updated Successfully.";
            }
            catch (Exception ex)
            {
                StatusMessage = "ZOrderDetail Update unsucessfull.";
            }

            return StatusMessage;
        }

        public void DeleteZOrderDetail(ZOrderDetail orderDetail)
        {
            conn.Delete(orderDetail);
        }

        public void DeleteAllZOrderDetail()
        {
            //  conn.DropTable<ZOrderDetail>();
            conn.CreateTable<ZOrderDetail>();
        }

        public List<ZOrderDetail> GetAllZOrderDetails(int zorderID)
        {
            try
            {
                return conn.Table<ZOrderDetail>().Where(d => d.ZOrderHeaderID == zorderID).ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<ZOrderDetail>();
        }

        public ZOrderDetail GetOrderLocationID()
        {
            try
            {
                return conn.Table<ZOrderDetail>().First();
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
