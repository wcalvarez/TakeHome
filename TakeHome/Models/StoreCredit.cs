using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    public class StoreCredit
    {
        public int StoreCreditID { get; set; }
        public int AppUserID { get; set; }
        public int LocationID { get; set; }
        public string StoreName { get; set; }
        public string CultureInfo { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CreditBalance { get; set; }
        public string Balance { get; set; }
    }
}
