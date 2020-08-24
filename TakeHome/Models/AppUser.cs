using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    public class AppUser
    {
        //NOTE:Don't go crazy, think MVP (mobileOrders basic..not TakeHome thinking...)
        [PrimaryKey]
        public int AppUserID { get; set; }
        public string AppUserType { get; set; }
        public int AppUserTypeID { get; set; }  //does this have to be not-null? for LPG yes
        public int LocationID { get; set; }     // does this have to be not-null? for LPG yes
        public string Email { get; set; }
        public string MemberNumber { get; set; }
        public string Password { get; set; }
        public string secstamp { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Zipcode { get; set; }
        public string Country { get; set; }
        public string PhoneNumber { get; set; }
        public bool IsLoggedIn { get; set; }
        public bool Active { get; set; }
    }
}
