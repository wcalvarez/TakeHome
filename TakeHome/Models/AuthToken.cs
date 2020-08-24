using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    public class AuthToken
    {
        public int AuthTokenID { get; set; }
        public int AppUserID { get; set; }
        public string Token { get; set; }
        public DateTime TimeSent { get; set; }
        public DateTime Expires { get; set; }
        public bool Used { get; set; }
    }
}
