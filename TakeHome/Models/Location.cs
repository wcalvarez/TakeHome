using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    public class Location
    {
        [PrimaryKey]
        public int LocationID { get; set; }
        public int AddresssID { get; set; }
        public int AppUserID { get; set; }
        public int MemberID { get; set; }
        public string MemberNumber { get; set; }
        public string AccountName { get; set; }
        public string LocationName { get; set; }
        public string LocationType { get; set; }
        public int BusinessHours { get; set; }
        public string TaxBase { get; set; }
        public string TaxRate { get; set; }
        public bool Delivery { get; set; }
        public bool MembershipRequired { get; set; }
        public bool CustomerPricing { get; set; }
        public bool SalesOrderEntry { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public decimal MinimumCreditOrder { get; set; }
        public decimal MinimumDeliverOrder { get; set; }
        public decimal DefaultPrice { get; set; }
        public string languageCode { get; set; }
        public decimal ReferralPercent { get; set; }
        public string PhoneNumber { get; set; }
        public bool Active { get; set; }
        public string Address { get; set; }
        public string StreetAddress { get; set; } //Address1 + Address2
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string CityStateZip { get; set; }
        public string CountryCode { get; set; }
        public DateTime LastVisit { get; set; }
        public int CountryID { get; set; }
        public double Distance { get; set; }
    }
}
