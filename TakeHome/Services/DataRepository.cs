using SQLite;
using System;
using System.Collections.Generic;
using System.Text;
using TakeHome.Models;

namespace TakeHome.Services
{
    public class DataRepository
    {
        SQLiteConnection conn;
        public string StatusMessage { get; set; }
        SQLiteCommand sqlite_cmd;
        public static string dbPathString = "";
        public DataRepository(string dbPath)
        {
            conn = new SQLiteConnection(dbPath);

            //conn.DropTable<AppUser>();
            conn.CreateTable<AppUser>();
        }

        public void AddAppUser(AppUser newuser)
        {
            int result = 0;
            try
            {
                //basic validation to ensure a name was entered
                if (string.IsNullOrEmpty(newuser.Email))
                    throw new Exception("Valid name required");

                result = conn.Insert(newuser);

                StatusMessage = string.Format("{0} record(s) added [Email: {1})", result, newuser.Email);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", newuser.Email, ex.Message);
            }
        }

        public string UpdateAppUser(AppUser newuser)
        {
            int result = 0;
            try
            {
                //basic validation to ensure a name was entered
                if (string.IsNullOrEmpty(newuser.Email))
                    throw new Exception("Valid name required");

                result = conn.Update(newuser);

                StatusMessage = "Profile Updated Successfully.";
            }
            catch (Exception ex)
            {
                StatusMessage = "Profile Update unsucessfull.";
            }

            return StatusMessage;
        }

        public AppUser GetAppUser()
        {
            try
            {
                return conn.Table<AppUser>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new AppUser();
        }

        public void DeleteAppUser(AppUser appUser)
        {
            conn.Delete(appUser);
        }

        public AppUser AppUserExistsOnDevice(AppUser user)
        {

            AppUser logginguser = conn.Find<AppUser>(user.AppUserID);

            return logginguser;
        }
    }
}
