using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    public class OrderHeader
    {
        public int OrderHeaderID { get; set; }
        public int OrderNumber { get; set; }
        public int LocationID { get; set; }
        public int AppUserID { get; set; }
        [PrimaryKey, AutoIncrement]
        public int ZOrderHeaderID { get; set; }
        public int ZCustomerID { get; set; }
        public int? CustomerID { get; set; }
        public string MemberNumber { get; set; }
        public int ReferredByID { get; set; }
        public int? ShippingInfoID { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }
        public string PaymentMethod { get; set; }
        public bool Printed { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string AvailAddress { get; set; }
        public string AvailCityStateZip { get; set; }
        public string Country { get; set; }
        public string Currency { get; set; }
        public decimal AppliedCredit { get; set; }
        public string AvailMode { get; set; }
        public string CustomerFirstName { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal Tax { get; set; }
        //LPG App changes
        public string UnitOfMeasure { get; set; } //Kg for LPGApp
        public int GrossQuantity { get; set; } //assumption:all in Kg (LPGApp)
        public decimal GrossUOMQuantity { get; set; }
        public string currencyGrossAmount { get; set; }
        public decimal ChargedAmount { get; set; }
        public string currencyChargedAmount { get; set; }
        public decimal CashAmount { get; set; }
        public string currencyCashAmount { get; set; }
        public string OrderInfo { get; set; }
        public int CompetitorItems { get; set; }
        public string CompetitorName { get; set; }
        public bool Posted { get; set; }
    }
}
