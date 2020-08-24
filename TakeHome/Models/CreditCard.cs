using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    public class CreditCard
    {
        public string Number { get; set; }
        public string ExpireYear { get; set; }
        public string ExpirationMonth { get; set; }
        public string Cvc { get; set; }
        public string Zipcode { get; set; }
    }
}
