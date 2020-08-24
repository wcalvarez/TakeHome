using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    public class CountryDTO
    {
        public int CountryID { get; set; }
        public int CurrencyID { get; set; }
        public string Name { get; set; }
        public string isoalphacode2 { get; set; }
        public string isoalphacode3 { get; set; }
        public string numeric3 { get; set; }
        public string languageCode { get; set; }
    }
}
