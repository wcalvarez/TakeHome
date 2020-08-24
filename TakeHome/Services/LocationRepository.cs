using System;
using System.Collections.Generic;
using System.Text;
using SQLite;
//using mobileOrdersApp.Data;
using System.Linq;
using TakeHome.Models;

namespace TakeHome.Services
{
    public class LocationRepository
    {
        SQLiteConnection conn;
        public string StatusMessage { get; set; }

        public LocationRepository(string dbPath)
        {
            conn = new SQLiteConnection(dbPath);
            conn.DropTable<Location>();  //Note:commentout for Production release
            conn.CreateTable<Location>();
        }

        public string AddLocation(Location location)
        {
            var loc = from l in conn.Table<Location>()
                      where l.LocationID.Equals(location.LocationID)
                      select l;
            int result = 0;
            Location visitedlocation = loc.FirstOrDefault();
            if (visitedlocation != null)
            {
                visitedlocation.LastVisit = DateTime.Now;
                result = conn.Update(location);
            }
            else
            {
                try
                {
                    //basic validation to ensure a name was entered
                    if (string.IsNullOrEmpty(location.LocationName))
                        throw new Exception("Valid name required");
                    int ctr = conn.Table<Location>().Count();
                    if (conn.Table<Location>().Count() >= 20)
                    {
                        Location toploc = conn.Table<Location>().First();
                        conn.Table<Location>().Delete(l => l.LocationID == toploc.LocationID);
                    }

                    location.LastVisit = DateTime.Now;
                    if (App.IsUserLoggedIn)
                    {
                        if (App.BrowsingLocation != null && App.BrowsingLocation.MembershipRequired)
                        {
                            location.AppUserID = App.user.AppUserID;

                        }
                    }
                    result = conn.Insert(location);

                    StatusMessage = "success";
                }
                catch (Exception ex)
                {
                    StatusMessage = ex.Message;

                }
            }

            return StatusMessage;

        }

        public List<Location> GetVisitedLocations()
        {
            try
            {
                return conn.Table<Location>().OrderByDescending(l => l.LastVisit).Take(10).ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<Location>();
        }

        public Location GetLocation(int locID)
        {
            Location loc = new Location();

            try
            {
                loc = conn.Table<Location>().Where(l => l.LocationID == locID).Single();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return loc;
        }

        public void DeleteLocation(Location location)
        {
            conn.Delete(location);
        }

    }
}
