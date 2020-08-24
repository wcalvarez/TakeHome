using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace TakeHome.Models
{
    public class ProductModel
    {
        public int ProductID { get; set; }
        public int LocationID { get; set; }
        public int ProductPriceID { get; set; }
        public string Location { get; set; }
        public string GroupName { get; set; }
        public string Category { get; set; }
        public int DisplaySort { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string PhotoBase64 { get; set; }
        public byte[] SmallImage { get; set; }
        public string UomPrices { get; set; }
        public string languageCode { get; set; }
        public int QuantityOnHand { get; set; }
        public decimal ConversionFactor { get; set; }
        public bool Featured { get; set; }
        public bool MembersOnly { get; set; }
        public ProductModel()
        {
        }

    }
    // Products Grouped by Category
    public class GroupedProductModel : ObservableCollection<ProductModel>
    {
        public string LongName { get; set; }
        public string ShortName { get; set; }
        public int DisplaySort { get; set; }
    }
}
