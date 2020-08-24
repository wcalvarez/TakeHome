using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using TakeHome.Models;

namespace TakeHome.Services
{
    public class AddressRepository
    {
        SQLiteConnection conn;
        public string StatusMessage { get; set; }

        public ShippingInfo selectedRecipient = new ShippingInfo();

        public AddressRepository(string dbPath)
        {
            conn = new SQLiteConnection(dbPath);
            //conn.DropTable<ShippingInfo>();
            conn.CreateTable<ShippingInfo>();
        }

        public void AddNewAddress(ShippingInfo address)
        {
            int result = 0;
            try
            {
                //basic validation to ensure a name was entered
                if (string.IsNullOrEmpty(address.Recipient))
                    throw new Exception("Valid name required");

                result = conn.Insert(address);

                StatusMessage = string.Format("{0} record(s) added [Recipient: {1})", result, address.Recipient);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", address.Recipient, ex.Message);
            }
        }

        public void DeleteAddress(ShippingInfo address)
        {
            conn.Delete(address);
        }

        public List<ShippingInfo> GetAllAddresses()
        {
            try
            {
                return conn.Table<ShippingInfo>().ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<ShippingInfo>();
        }

    }
}
