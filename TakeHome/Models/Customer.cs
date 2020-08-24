using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    public class Customer
    {
        [PrimaryKey, AutoIncrement]
        public int ZCustomerID { get; set; }
        public int LocationID { get; set; }
        public int? AppUserID { get; set; }
        public int CustomerID { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Address { get; set; }
        public string StreetAddress { get; set; }
        public string City { get; set; }
        public string Province { get; set; }
        public decimal DefaultPrice { get; set; }
        public string PhoneNumber { get; set; }
        public bool ChargePaymentAllowed { get; set; }
        public decimal CreditLimit { get; set; }
    }
}
