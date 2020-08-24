using System;
using System.Collections.Generic;
using System.Text;
using TakeHome.Views;

namespace TakeHome.Models
{
    public class MainMenuMenuItem
    {
        public MainMenuMenuItem()
        {
            TargetType = typeof(MainMenuDetail);
        }
        public int Id { get; set; }
        public string Title { get; set; }
        public string Page { get; set; }
        public Type TargetType { get; set; }
    }
}
