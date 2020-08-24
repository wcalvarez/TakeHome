using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    public class State
    {
        public int StateID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string Country3Alpha { get; set; }
        public string CountryName { get; set; }
        public int CountryID { get; set; }
    }
}
