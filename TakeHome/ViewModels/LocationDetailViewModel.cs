using System;
using System.Collections.Generic;
using System.Text;
using TakeHome.Models;
using Xamarin.Forms;

namespace TakeHome.ViewModels
{
    public class LocationDetailViewModel
    {
        public Location Location = new Location();
        Location locDetail = new Location();
        public BusinessHour businessHours = new BusinessHour();
        public LocationDetailViewModel(Location loc)
        {
            // retrieve BusinessHours
            Location = loc;
            

            
        }
    }
}
