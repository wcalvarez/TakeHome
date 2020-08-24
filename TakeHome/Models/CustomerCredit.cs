using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    public class CustomerCredit
    {
        public int CustomerCreditID { get; set; }
        public int CustomerID { get; set; }
        public int LocationID { get; set; }
        public string LocationName { get; set; }
        public string CultureInfo { get; set; }
        public string CurrencyCode { get; set; }
        public decimal CreditBalance { get; set; }
        public string Balance { get; set; }
    }
}
