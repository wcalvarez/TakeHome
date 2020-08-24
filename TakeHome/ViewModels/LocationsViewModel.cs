using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Text;
using TakeHome.Models;
using Xamarin.Forms;

namespace TakeHome.ViewModels
{
    public class LocationsViewModel : BaseViewModel
    {
        public IList<Location> Locations = new ObservableCollection<Location>();
       // public ObservableCollection<Location> Locations{ get; set; }
        public Command LoadLocationsCommand { get; set; }
        
        public LocationsViewModel()
        {
            Title = "Locations";

            Locations = new ObservableCollection<Location>();
            //var locations = new List<Location>()
            //{ 
            //    new Location   {
            //        LocationID= 1,
            //        AccountName= "Dayfree",
            //        LocationName= "Kingsport Subdivision",
            //        LocationType= "Marketing",
            //        BusinessHours= 0,
            //        TaxBase= "None",
            //        TaxRate= null,
            //        Delivery= false,
            //        MembershipRequired= false,
            //        CustomerPricing= true,
            //        SalesOrderEntry= true,
            //        Latitude= 0,
            //        Longitude= 0,
            //        MinimumCreditOrder= 0,
            //        MinimumDeliverOrder= 0,
            //        DefaultPrice= 51,
            //        languageCode= "en-PH",
            //        ReferralPercent= 5,
            //        Phonenumber= "011639568512079",
            //        Active= true,
            //        Address= null,
            //        StreetAddress= "29 Francisco Compound",
            //        City= "Quezon City",
            //        State= "Metro Manila",
            //        Zipcode= "12345",
            //        CityStateZip= "Quezon City, Metro Manila 12345",
            //        CountryCode= "Philippines",
            //        CountryID= 2
            //    },
            //    new Location   {
            //        LocationID= 5,
            //        AccountName= "Dayfree",
            //        LocationName= "Assurance",
            //        LocationType= "Plant",
            //        BusinessHours= 0,
            //        TaxBase= "None",
            //        TaxRate= null,
            //        Delivery= true,
            //        MembershipRequired= false,
            //        CustomerPricing= true,
            //        SalesOrderEntry= false,
            //        Latitude= 0,
            //        Longitude= 0,
            //        MinimumCreditOrder= 20,
            //        MinimumDeliverOrder= 0,
            //        DefaultPrice= 48,
            //        languageCode= "en-PH",
            //        ReferralPercent= 0,
            //        Phonenumber= "011639568512079",
            //        Active= true,
            //        Address= null,
            //        StreetAddress= "Lot 1 Block 3",
            //        City= "General Trial",
            //        State= "Cavite",
            //        Zipcode= "01123",
            //        CityStateZip= "General Trial, Cavite 01123",
            //        CountryCode= "Philippines",
            //        CountryID= 2
            //    },
            //    new Location  {
            //        LocationID= 4,
            //        AccountName= "Dayfree",
            //        LocationName= "TANZA",
            //        LocationType= "Marketing",
            //        BusinessHours= 0,
            //        TaxBase= "PointOfSale",
            //        TaxRate= null,
            //        Delivery= false,
            //        MembershipRequired= false,
            //        CustomerPricing= true,
            //        SalesOrderEntry= true,
            //        Latitude= 0,
            //        Longitude= 0,
            //        MinimumCreditOrder= 0,
            //        MinimumDeliverOrder= 0,
            //        DefaultPrice= 48,
            //        languageCode= "en-PH",
            //        ReferralPercent= 0,
            //        Phonenumber= "09660835376",
            //        Active= true,
            //        Address= null,
            //        StreetAddress= "Paradahan 1",
            //        City= "Tanza",
            //        State= "Cavite",
            //        Zipcode= "04117",
            //        CityStateZip= "Tanza, Cavite 04117",
            //        CountryCode= "Philippines",
            //        CountryID= 2
            //    }
            // };
            //Locations = locations;
            var visitedlocations = App.LocationRepo.GetVisitedLocations();

            if (visitedlocations.Count > 0)
            {
                foreach (Location loc in visitedlocations)
                {
                    Locations.Add(loc);
                }
            }
            else //get Locations from backend
            {

            }


        }


    }
}
