using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace TakeHome.Models
{
    public class AppMenuItem
    {
        [PrimaryKey, AutoIncrement, Column("_Id")]
        public int AppMenuItemID { get; set; }
        [Column("AppID")]
        public int AppID { get; set; }
        [Column("AppName")]
        public string AppName { get; set; }
        [Column("UserStatus")]
        public string UserStatus { get; set; }
        [Column("MenuItemName")]
        public string MenuItemName { get; set; }
        [Column("MenuItemPage")]
        public string MenuItemPage { get; set; }
        [Column("DisplaySequence")]
        public int DisplaySequence { get; set; }
    }
}
