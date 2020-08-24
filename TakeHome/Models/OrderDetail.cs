using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    [Table("mobileOrders")]

    public class OrderDetail
    {
        [PrimaryKey, AutoIncrement]
        public int OrderDetailID { get; set; }
        public int OrderHeaderID { get; set; }
        public int ZOrderHeaderID { get; set; }
        public int ProductID { get; set; }
        public int LocationID { get; set; }
        public int CustomerID { get; set; }
        public int ProductPriceID { get; set; }
        public string Location { get; set; }
        public string ProductName { get; set; }
        public int LineNumber { get; set; }
        public int Quantity { get; set; }
        public CylinderDeal CylinderDeal { get; set; }
        public string ItemName { get; set; }
        [Column("Price")]
        public decimal UnitPrice { get; set; }
        public decimal UOMQuantity { get; set; }
        public string PriceUOM { get; set; }
        public string LineAmounts { get; set; }
        public string languageCode { get; set; }
        public decimal Amount { get; set; }
        public string currencyAmount { get; set; }
        public string SpecialInstruction { get; set; }
        public int CylinderSwapped { get; set; }
        public int CylinderSold { get; set; }
        public int CylinderLoaned { get; set; }
        public int CylinderRefilled { get; set; }
    }
}
