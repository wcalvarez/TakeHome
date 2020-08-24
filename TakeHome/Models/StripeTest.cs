using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    public class StripeTest
    {
        public int StripeTestID { get; set; }
        public decimal ChargeAmount { get; set; }
        public decimal GrossSales { get; set; }
        public string Currency { get; set; }
        public string CardNumber { get; set; }
        public string CardIssuer { get; set; }
        public string ExpireMonth { get; set; }
        public string ExpireYear { get; set; }
        public string Cvc { get; set; }
        public string Token { get; set; }
        public bool ChargedSuccessfully { get; set; }
        public string ErrorMessage { get; set; }
        public int LocationID { get; set; }
        public bool Live_Mode { get; set; }
    }
}
