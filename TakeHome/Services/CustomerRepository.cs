using SQLite;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TakeHome.Models;

namespace TakeHome.Services
{
    public class CustomerRepository
    {
        SQLiteConnection conn;
        public string StatusMessage { get; set; }
        private IList<Customer> searched_customers = new ObservableCollection<Customer>();

        // SQLiteCommand sqlite_cmd;
        public CustomerRepository(string dbPath)
        {
            conn = new SQLiteConnection(dbPath);
            //conn.DropTable<Customer>();
            conn.CreateTable<Customer>();
            conn.CreateTable<Printer>();
            conn.CreateTable<State>();
        }

        public void AddNewCustomer(Customer customer)
        {
            int result = 0;
            try
            {
                //basic validation to ensure a name was entered
                if (string.IsNullOrEmpty(customer.Name))
                    throw new Exception("Customer name required");

                result = conn.Insert(customer);

                StatusMessage = string.Format("{0} record(s) added [Customer: {1})", result, customer.Name);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", customer.Name, ex.Message);
            }
        }

        public string UpdateCustomer(Customer customer)
        {
            int result = 0;
            try
            {
                result = conn.Update(customer);
                StatusMessage = "OrderDetail Updated Successfully.";
            }
            catch (Exception ex)
            {
                StatusMessage = "OrderDetail Update unsucessfull.";
            }

            return StatusMessage;
        }

        public List<Customer> GetAllCustomers(int locID)
        {
            try
            {
                if (App.user.AppUserType == "AccountRep")
                {
                    return conn.Table<Customer>().Where(c => c.LocationID == locID && ((c.AppUserID == App.user.AppUserID && App.user.LocationID == c.LocationID))).ToList();
                }
                else
                {
                    return conn.Table<Customer>().Where(c => c.LocationID == locID).ToList();
                }
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<Customer>();
        }

        public Customer GetZCustomer(int zid)
        {
            Customer zc = new Customer();

            try
            {
                zc = conn.Table<Customer>().Where(l => l.ZCustomerID == zid).Single();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return zc;
        }

        public Customer GetCustomer(int id)
        {
            Customer c = new Customer();

            try
            {
                c = conn.Table<Customer>().Where(l => l.CustomerID == id).Single();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return c;
        }

        public IList<Customer> SearchZCustomer(string searchText)
        {
            Customer zc = new Customer();

            try
            {
                searched_customers = conn.Table<Customer>().Where(l => l.Name.Contains(searchText)).ToList();

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return searched_customers;
        }

        public void DeleteAllCustomers()
        {
            conn.DropTable<Customer>();
            conn.CreateTable<Customer>();
        }

        public List<Customer> GetDownloadedCustomers()
        {
            try
            {
                return conn.Table<Customer>().OrderByDescending(l => l.Name).ToList();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<Customer>();
        }

        public void AddPrinter(Printer printer)
        {
            int result = 0;
            try
            {
                //basic validation to ensure a name was entered
                if (string.IsNullOrEmpty(printer.MacAddress))
                    throw new Exception("Valid MacAddress required");

                result = conn.Insert(printer);

                StatusMessage = string.Format("{0} record(s) added [Name: {1})", result, printer.MacAddress);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", printer.MacAddress, ex.Message);
            }
        }

        public string UpdatePrinter(Printer printer)
        {
            int result = 0;
            try
            {
                //basic validation to ensure a name was entered
                if (printer.MacAddress != null)
                {
                    result = conn.Update(printer);
                }
                StatusMessage = "Printer Updated Successfully.";
            }
            catch (Exception ex)
            {
                StatusMessage = "Printer Update unsucessfull.";
            }

            return StatusMessage;
        }

        public Printer GetPrinter()
        {
            Printer printer = new Printer();

            try
            {
                printer = conn.Table<Printer>().FirstOrDefault();
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return printer;

        }

        //
        //STATES

        public List<State> GetStates(int countryID)
        {
            try
            {

                return conn.Table<State>().Where(c => c.CountryID == countryID).OrderBy(c => c.Name).ToList();

            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to retrieve data. {0}", ex.Message);
            }

            return new List<State>();
        }
        public void AddState(State state)
        {
            int result = 0;
            try
            {
                //basic validation to ensure a name was entered
                if (string.IsNullOrEmpty(state.Name))
                    throw new Exception("State name required");

                result = conn.Insert(state);

                StatusMessage = string.Format("{0} record(s) added [State: {1})", result, state.Name);
            }
            catch (Exception ex)
            {
                StatusMessage = string.Format("Failed to add {0}. Error: {1}", state.Name, ex.Message);
            }
        }

        public void DeleteStates()
        {
            conn.DropTable<State>();
            conn.CreateTable<State>();
        }

    }
}
