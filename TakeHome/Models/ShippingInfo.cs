using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    public class ShippingInfo
    {
        [PrimaryKey]
        public int? ShippingInfoID { get; set; }
        public string Type { get; set; }
        public string Recipient { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
        public string FullAddress { get; set; }
        public string EmailAddress { get; set; }
        public string PhoneNumber { get; set; }
        //     public Boolean Selected { get; set; }
    }
}
