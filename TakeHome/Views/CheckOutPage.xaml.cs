using LinkOS.Plugin;
using LinkOS.Plugin.Abstractions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TakeHome.Models;
using TakeHome.Services;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace TakeHome.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CheckOutPage : ContentPage
    {
        public static ObservableCollection<OrderDetail> orderLines { get; set; }
        private IList<ZOrderDetail> zorderLines = new ObservableCollection<ZOrderDetail>();
        readonly DataManager manager = new DataManager();

        TimeSpan pickupTime;
        public bool pickup;
        DateTime duration;
        Decimal decTaxRate;
        Decimal OrderTotal;
        Decimal splitPayments;
        Decimal subTotal;
        String orderInfo;
        String fullName;
        String chargeAmount;
        String cashAmount;
        String tax;
        Decimal storeCredit;
        Decimal customerCredit;
        CultureInfo cfi;
        OrderHeader newHeader_print;
        OrderHeader newHeader;
        IConnection connection;
        //string printerMacTest = "BT:AC:3F:A4:A5:0F:BD";
        string printerMacTest;
        const string tag = "Dayfree";

        int orderLocID;
        int refByID;
        StoreCredit usc = new StoreCredit();
        CustomerCredit cc = new CustomerCredit();
        CreditCard card;

        public CheckOutPage()
        {
            orderLocID = App.OrderRepo.GetOrderLocationID().LocationID;
            orderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());
            subTotal = orderLines.Sum(P => P.Amount);

            card = new CreditCard();
            BindingContext = card;
            InitializeComponent();
            
        }

        public CheckOutPage(bool ispickup)
        {
            orderLocID = App.OrderRepo.GetOrderLocationID().LocationID;
            orderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());
            subTotal = orderLines.Sum(P => P.Amount);
            pickup = ispickup;
            card = new CreditCard();
            BindingContext = card;
            InitializeComponent();

        }

        protected async override void OnAppearing()
        {
            CardNumber.Focus();

            if (pickup)
            {
                pickup_frame.IsVisible = true;
                _datePicker.Date = DateTime.Today;

                _timePicker.Time = DateTime.Now.TimeOfDay;
                duration = DateTime.Now.AddMinutes(App.storeHours.PickupLeadTime + 2);
                _timePicker.Time = duration.TimeOfDay;

            }

            if ((App.user == null || !App.user.IsLoggedIn))
            {
                overlay.IsVisible = true; //original for TakeHome Guest User

                if(App.guest_user != null)
                {
                    EnteredEmail.Text = App.guest_user.Email;
                    EnteredName.Text = App.guest_user.Name;
                    EnteredPhoneNumber.Text = App.guest_user.PhoneNumber;
                }
            }

            OrderDetail ord = orderLines.First();
            storename.Text = ord.Location;
            cfi = CultureInfo.CreateSpecificCulture(ord.languageCode);

            App.OrderLocation = App.BrowsingLocation;

            if (App.BrowsingLocation.ReferralPercent > 0)
            {
                // apply credits only if online?
                string stringcustomerCredit = await manager.GetCustomerCredit(App.OrderLocation.LocationID, ord.CustomerID);

                if (stringcustomerCredit != null)
                {
                    cc = JsonConvert.DeserializeObject<CustomerCredit>(stringcustomerCredit);

                    customerCredit = cc.CreditBalance;

                    if (customerCredit > 0)
                    {
                        refcredit.IsVisible = true;
                        useamount.IsVisible = true;
                        reflabel.IsVisible = true;
                        refcredit.Text = cc.CreditBalance.ToString("C", cfi);
                    }

                    if (App.OrderLocation.ReferralPercent > 0)
                    {
                        overlayreferral.IsVisible = true;
                    }
                }
                overlay.IsVisible = false;

            }

            subTotal = orderLines.Sum(P => P.Amount);

            string subtotal = subTotal.ToString("C", cfi);
            SubTotalLabel.Text = "Sub Total:......." + subtotal;

            if (orderLines.Sum(q => q.Quantity) > 1)
            {
                Resources["OrderSummary"] = "Order Summary: " + orderLines.Sum(q => q.Quantity).ToString() + " Items";
            }
            else
            {
                Resources["OrderSummary"] = "Order Summary: " + orderLines.Sum(q => q.Quantity).ToString() + " Item";
            }

            string taxbase = App.OrderLocation.TaxBase;
            ZipTaxParam zp = new ZipTaxParam();
            switch (taxbase)
            {
                case "PointOfSale":  //taxrate based on store location
                        if (App.OrderLocationID == App.BrowsingLocation.LocationID)
                        {
                            decTaxRate = decimal.Parse(App.BrowsingLocation.TaxRate);
                        }
                        else
                        {
                            decTaxRate = decimal.Parse(App.OrderLocation.TaxRate);
                        }
                    break;
                case "ShipTo":       //taxrate based on shipto address
                    break;
                default:
                    break;
            }

            Decimal Tax = subTotal * decTaxRate;
            string tax = Tax.ToString("C", cfi);
            TaxLabel.Text = "Sales Tax:......." + tax;


            OrderTotal = subTotal + Tax;
            string ordtotal = OrderTotal.ToString("C", cfi);
            GrandTotalLabel.Text = "Grand Total:....." + ordtotal;
            NavigationBarView.Title = "CheckOut";

            base.OnAppearing();
        }

        void OnTimePickerPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "Time")
            {
                //pickupTime = DateTime.Today + _timePicker.Time;
                //_timePicker.Time = DateTime.Now.TimeOfDay;
                //if (pickupTime < DateTime.Now)
                //{
                //    pickupTime += TimeSpan.FromDays(1);
                //}
            }
        }

        private async Task<int> SubmitSalesOrder()
        {

            //validate salesorderentry payment-entry
            decimal cashValue = 0;
            decimal chargeValue = 0;
            decimal referralcreditValue = 0;

            int postedOrders = 0;

            IList<OrderDetail> orderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());

            var itemsList = new List<OrderDetail>();

            OrderHeader newHeader = new OrderHeader();
            newHeader.LocationID = App.OrderRepo.GetOrderLocationID().LocationID;
            newHeader.CustomerID = App.CurrentCustomer.CustomerID;
            newHeader.ZCustomerID = App.CurrentCustomer.ZCustomerID;
            newHeader.CustomerFirstName = App.CurrentCustomer.Name;
            newHeader.OrderDate = DateTime.Today;
            newHeader.OrderStatus = "Received";
            newHeader.PhoneNumber = App.CurrentCustomer.PhoneNumber;
            newHeader.AppUserID = 5; //1 will be the default Guest Customer (unregistered) shopper
            newHeader.AvailAddress = App.CurrentCustomer.Address;
            newHeader.GrossUOMQuantity = orderLines.Sum(P => P.UOMQuantity);
            newHeader.GrossAmount = orderLines.Sum(P => P.Amount);
            newHeader.CashAmount = cashValue;
            newHeader.ChargedAmount = chargeValue;
            newHeader.AppliedCredit = referralcreditValue;
            newHeader.OrderInfo = "Date: " + newHeader.OrderDate.ToString("MM/dd/yyyy h:mm") + " Total: " + OrderTotal.ToString("C", cfi);

            // Turn on network indicator
            this.IsBusy = true;
            checkingout.IsVisible = true;
            try
            {
                OrderHeader newOrderHeader = await manager.PostOrderHeader(newHeader);
                postedOrders += 1;
                OrderDetail[] items;

                itemsList = new List<OrderDetail>();

                foreach (OrderDetail dtl in orderLines)
                {
                    dtl.OrderHeaderID = newOrderHeader.OrderHeaderID;
                    itemsList.Add(dtl);
                }

                items = itemsList.ToArray();
                await manager.PostOrderDetails(items);

                foreach (OrderDetail d in orderLines)
                {
                    App.OrderRepo.DeleteOrderDetail(d);
                }

                await DisplayAlert("Order Completed", "Your Order has been received", "Thank You");
                await Navigation.PopToRootAsync();
            }
            catch (Exception err)
            {
                this.IsBusy = false;
                //await DisplayAlert("Ooops..", "Unable to process your Order:" + err.Message, "OK");
                var current = Connectivity.NetworkAccess;

                if (current == NetworkAccess.Internet)
                {
                    // Connection to internet is available
                    await DisplayAlert("Ooops..", "Unable to process your Order: " + err.Message, "OK");
                }
                else
                {
                    await DisplayAlert("Ooops..", "Unable to process your Order: " + "You lost internet connection, try again", "OK");

                }

                await Navigation.PopToRootAsync();

            }
            finally
            {
                this.IsBusy = false;
                checkingout.IsVisible = false;
            }

            return postedOrders;

        }
        private async void OnClickSubmitOrder(object sender, System.EventArgs e)
        {


                //validate salesorderentry payment-entry
                decimal cashValue = 0;
                decimal chargeValue = 0;
                decimal referralcreditValue = 0;

                pickupTime = _timePicker.Time;

            IList<OrderDetail> orderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());


            // validate Entries
            //validate future-date pickup

              if (_datePicker.Date != DateTime.Now.Date && _datePicker.Date > DateTime.Now.Date)
                {
                // Turn on network indicator
                this.IsBusy = true;

                try
                {
                    //get store-hours
                    var weekday = _datePicker.Date;
                    string pickday = weekday.ToString("dddd");

                    var storehours = await manager.GetLocationHours(pickday);

                    BusinessHour bhours = JsonConvert.DeserializeObject<BusinessHour>(storehours);
                //    App.storeHours = bhours;

                    if (bhours.Open)
                    {
                        var openTime = bhours.StartTime.TimeOfDay;
                        var closeTime = bhours.CloseTime.TimeOfDay;
                        var timeNow = _timePicker.Time;  
                        
                        if (timeNow >= openTime && timeNow <= closeTime)
                        {

                        }
                        else
                        {
                            if (timeNow >= closeTime)
                            {
                                //TimeSpan ts = bhours.CloseTime.TimeOfDay;
                                DateTime dtTemp = DateTime.ParseExact(bhours.CloseTime.TimeOfDay.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);      
                                await DisplayAlert("Sorry, this store closes at " + dtTemp.ToString("hh:mm tt") + " on " + _datePicker.Date.ToString("MM/dd/yyyy") + ".","Please select an earlier pickup time.", "Ok");
                            }
                            if (timeNow < openTime)
                            {
                                DateTime dtTemp = DateTime.ParseExact(bhours.StartTime.TimeOfDay.ToString(), "HH:mm:ss", CultureInfo.InvariantCulture);
                                await DisplayAlert("Select a later pickup time:", "This store opens at " + dtTemp.ToString("hh:mm tt") + " on " + _datePicker.Date.ToString("MM/dd/yyyy") + ".", "Ok");
                                await Navigation.PopAsync();
                            }
                            return;
                        }
                    }
                    else  //Closed Today
                    {
                        await DisplayAlert("Closed:", "Sorry, this store is Closed today.", "Ok");
                        await Navigation.PopAsync();
                        return;
                    }
                }
                catch (Exception err)
                {
                    this.IsBusy = false;
                    var current = Connectivity.NetworkAccess;

                    if (current == NetworkAccess.Internet)
                    {
                        await DisplayAlert("Ooops..", "No business hours setup for " + DateTime.Now.ToString("dddd") + "(s).", "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ooops..", "Unable to verify store hours: " + "You lost internet connection, try again", "OK");

                    }
                    return;
                }
                finally
                {
                    this.IsBusy = false;
                }

            }
              else
            {
                if(_datePicker.Date < DateTime.Now.Date)
                {
                    await DisplayAlert("Invalid Pickup Date:", "You can't pickup for past date(s)", "ok");
                    return;
                }
            }
            //same-day pickup
            if (_datePicker.Date == DateTime.Now.Date)
            {
                duration = DateTime.Now.AddMinutes(App.storeHours.PickupLeadTime);
                TimeSpan pick20 = duration.TimeOfDay;
                if (_timePicker.Time < pick20)
                {
                    await DisplayAlert("Earliest Pickup:", App.storeHours.PickupLeadTime.ToString() + " minutes after you ordered", "ok");
                    return;
                }

                //pickup can't be past 15 minutes before closing

                duration = App.storeHours.CloseTime.AddMinutes(App.storeHours.LastOrderLeadTime * -1);
                TimeSpan pick15 = duration.TimeOfDay;
                if (_timePicker.Time > pick15)
                {
                    await DisplayAlert("Lastt Pickup:", App.storeHours.LastOrderLeadTime.ToString() + " minutes before " + App.storeHours.CloseTime.ToString(), "ok");
                    return;
                }
            }
            /// var openTime = bhours.StartTime.TimeOfDay;
            //var closeTime = bhours.CloseTime.TimeOfDay;

            if (string.IsNullOrEmpty(CardNumber.Text) || string.IsNullOrEmpty(ExpMMYR.Text) 
                || string.IsNullOrEmpty(Cvc.Text))
            {
                await DisplayAlert("required entries:", "credit card#, expiration month/year, security code", "ok");
                return;
            }


            subTotal = orderLines.Sum(P => P.Amount);
                Decimal Tax = subTotal * decTaxRate;
                OrderTotal = subTotal + Tax;

                //apply referral credits used
                if (App.BrowsingLocation.ReferralPercent > 0)
                {
                    decimal applied_amount = System.Convert.ToDecimal(useamount.Text);

                    if (applied_amount > customerCredit)
                    {
                        await DisplayAlert("Alert", "Balance less then Credit used", "OK");
                        return;
                    }
                    else
                    {
                        cc.CreditBalance = cc.CreditBalance - applied_amount;
                        // OrderTotal -= applied_amount;
                        if (decimal.TryParse(useamount.Text, NumberStyles.Currency, cfi, out referralcreditValue))
                        {
                            splitPayments = splitPayments + referralcreditValue;
                        }
                    }
                }


                var itemsList = new List<OrderDetail>();
                newHeader = new OrderHeader();

            //TAKEHOME specific OrderHeader data

            if (pickup)
            {
                newHeader.ShippingInfoID = null;
                pickupTime = _timePicker.Time;
                newHeader.AvailMode = "Pickup: " + _datePicker.Date.ToString("MM/dd/yyyy") + " " + 
                    new DateTime().Add(pickupTime).ToString("hh:mm tt");
            }
            else
            {
                newHeader.ShippingInfoID = App.shipto.ShippingInfoID;
                newHeader.AvailAddress = App.shipto.FullAddress;
                newHeader.AvailCityStateZip = App.shipto.City + ", " + App.shipto.State;
            }

            if (App.user == null || !App.user.IsLoggedIn)
                {
                    newHeader.Email = App.guest_user.Email;
                }
                else
                {
                    newHeader.Email = App.user.Email;
                }

                if (App.user != null && App.user.IsLoggedIn)
                {
                    newHeader.AppUserID = App.user.AppUserID;
                    newHeader.CustomerFirstName = App.user.FirstName;
                    newHeader.Email = App.user.Email;
                    newHeader.PhoneNumber = App.user.PhoneNumber;
                }
                else
                {
                    newHeader.CustomerFirstName = App.guest_user.Name;
                    newHeader.Email = App.guest_user.Email;
                    newHeader.PhoneNumber = App.guest_user.PhoneNumber;
                }


                //TAKEHOME-End

                newHeader.LocationID = App.OrderRepo.GetOrderLocationID().LocationID;

                newHeader.OrderDate = DateTime.Today;
                newHeader.ReferredByID = refByID;
                newHeader.OrderStatus = "Received";
                newHeader.GrossAmount = orderLines.Sum(P => P.Amount);
                newHeader.Tax = newHeader.GrossAmount * decTaxRate;
                newHeader.GrossUOMQuantity = orderLines.Sum(P => P.UOMQuantity);
                newHeader.GrossAmount = orderLines.Sum(P => P.Amount);
                // newHeader.Tax = newHeader.GrossAmount * decTaxRate;
                newHeader.CashAmount = cashValue;
                cashAmount = newHeader.CashAmount.ToString();
                newHeader.ChargedAmount = chargeValue;
                newHeader.AppliedCredit = referralcreditValue;
                chargeAmount = newHeader.ChargedAmount.ToString();
                newHeader.OrderInfo = "Date: " + newHeader.OrderDate.ToString("MM/dd/yyyy") + " Total: " + OrderTotal.ToString("C", cfi);
                orderInfo = newHeader.OrderInfo;
                if (App.shipto.Zipcode != null)
                {
                    newHeader.AvailCityStateZip += " " + App.shipto.Zipcode;
                }

                if (App.shipto != null)
                {
                    newHeader.Country = App.shipto.Country;
                }

                newHeader_print = newHeader;


            //
            //STRIPE PAYMENT PROCESS HERE
            //
            //
            //Process StripePayment before backend Updates, unsuccessfull Stripe Payment,means no Order at all
            //
            //BusyIndicator
            // Turn on network indicator
            this.IsBusy = true;
            checkingout.IsVisible = true;
            string[] mmyy = ExpMMYR.Text.Split('/');
            try
            {
                if (App.OrderLocation.MinimumCreditOrder > 0)
                {
                    StripeTest stripeTest = new StripeTest();
                    stripeTest.CardNumber = CardNumber.Text;
                    stripeTest.ExpireMonth = mmyy[0];
                    stripeTest.ExpireYear = mmyy[1];
                    stripeTest.Cvc = Cvc.Text;
                    stripeTest.CardIssuer = "Visa";
                    stripeTest.ChargeAmount = OrderTotal;
                    stripeTest.GrossSales = subTotal;

                    stripeTest.LocationID = App.OrderRepo.GetOrderLocationID().LocationID;

                   
                    StripeTest testedStripe = await manager.PostStripeTest(stripeTest);
                    
                    if (!testedStripe.ChargedSuccessfully)
                    {
                        this.IsBusy = false;
                        checkingout.IsVisible = false;
                        await DisplayAlert("Payment Failed:", testedStripe.ErrorMessage, "Ok");
                        return;
                    }
                }

                OrderHeader newOrderHeader = await manager.PostOrderHeader(newHeader);
                OrderDetail[] items;

                itemsList = new List<OrderDetail>();

                foreach (OrderDetail dtl in orderLines)
                {
                    dtl.OrderHeaderID = newOrderHeader.OrderHeaderID;
                    itemsList.Add(dtl);
                }

                items = itemsList.ToArray();
                await manager.PostOrderDetails(items);
            }
            catch (Exception err)
            {
                this.IsBusy = false;
                var current = Connectivity.NetworkAccess;

                if (current == NetworkAccess.Internet)
                {
                    await DisplayAlert("Ooops..", "Unable to process your Order: " + err.Message, "OK");
                }
                else
                {
                    await DisplayAlert("Ooops..", "Unable to process your Order: " + "You lost internet connection, try again", "OK");
                    
                }

                return;
            }
            finally
            {
                this.IsBusy = false;
                checkingout.IsVisible = false;
            }

            App.OrderRepo.DeleteAllOrderDetail();
            await DisplayAlert("Order Completed", "Your Order has been received", "Thank You");
            await Navigation.PopToRootAsync();

        }

        private async void OnOKButtonClicked(object sender, EventArgs args) //Guest AppUser entries
        {
            if ((EnteredEmail.Text == null || EnteredName.Text == null || EnteredPhoneNumber.Text == null))
            {
                await DisplayAlert("Required:", "Email, Name & Phone#", "Ok");
                return;
            }

            App.guest_user.Email = EnteredEmail.Text;
            App.guest_user.Name = EnteredName.Text;
            App.guest_user.PhoneNumber = EnteredPhoneNumber.Text;
            overlay.IsVisible = false;
        }

        private async void OnOKReferralClicked(object sender, EventArgs args)
        {
            //make sure Order.CustomerID != referByID
            refByID = int.Parse(EnteredCustomerID.Text);
            if (refByID == orderLines.First().CustomerID)
            {
                await DisplayAlert("Invalid:", "Customer can not refer itself", "Ok");
                return;
            }
            overlayreferral.IsVisible = false;
            //skip if non-entered
            if (!string.IsNullOrWhiteSpace(EnteredCustomerID.Text))
            {
                // Turn on network indicator
                this.IsBusy = true;
                try
                {
                    refByID = int.Parse(EnteredCustomerID.Text);
                    Customer refCustomer = App.CustomerRepo.GetCustomer(refByID);
                    if (refCustomer == null)
                    {
                        await DisplayAlert("Invalid:", "Customer # not found", "Ok");
                        overlayreferral.IsVisible = true;
                    }
                    else
                    {
                        string referrer = refCustomer.Name;
                        await DisplayAlert("Customer referred by:", referrer, "Ok");
                    }
                }
                catch (Exception err)
                {
                    this.IsBusy = false;

                    var current = Connectivity.NetworkAccess;

                    if (current == NetworkAccess.Internet)
                    {
                        await DisplayAlert("Ooops..", "Unable to verify Referrer: " + err.Message, "OK");
                    }
                    else
                    {
                        await DisplayAlert("Ooops..", "Network Error: " + "You lost internet connection, try again", "OK");

                    }
                    return;
                }
                finally
                {
                    this.IsBusy = false;
                }
            }
            else
            {
                await DisplayAlert("Input Required", "Referrer's Customer#", "OK");
                return;
            }
        }

        void OnSkipReferralClicked(object sender, EventArgs args)
        {
            overlayreferral.IsVisible = false;
            return;
        }

        private async void OnSkipPrinter(object sender, EventArgs args)
        {
            loadprinter.IsVisible = false;
            await Navigation.PopToRootAsync();
        }

        private async void TapGestureRecognizer_OnTapped(object sender, EventArgs e)
        {
            await Navigation.PopModalAsync();
        }

        private async void OnCancelButtonClicked(object sender, EventArgs args)
        {
            overlay.IsVisible = false;
            await Navigation.PopAsync();
        }


        private async void OnSaveMacAddress(object sender, EventArgs args)
        {
            Printer sprinter = new Printer
            {
                Name = PrinterName.Text,
                MacAddress = MacAddress.Text
            };

            if (sprinter.MacAddress == null)
            {
                await DisplayAlert("Required Entries:", "Printer MacAddress", "Ok");
                return;
            }

            //print customerDetail



            printerMacTest = sprinter.MacAddress;

            Print(printerMacTest);
            //
            //
            if (App.SavedPrinter != null)
            {
                App.CustomerRepo.UpdatePrinter(sprinter);
            }
            else
            {
                App.CustomerRepo.AddPrinter(sprinter);
            }

            App.SavedPrinter = sprinter;
            loadprinter.IsVisible = false;
            await Navigation.PopToRootAsync();
        }

        private async void OnPrint(OrderHeader newHeader)
        {
            //load-display printer
            loadprinter.IsVisible = true;
            App.SavedPrinter = App.CustomerRepo.GetPrinter();

            if (App.SavedPrinter != null)
            {
                PrinterName.Text = App.SavedPrinter.Name;
                MacAddress.Text = App.SavedPrinter.MacAddress;
                printButton1.Text = "Print";
            };

        }

        private void Print(string address)
        {
            //the coordinates for these aren't 100% fine-tuned yet because I ran out of paper mid-testing
            //but that's the only thing missing, the exact coordinates
            IList<OrderDetail> orderLines = new ObservableCollection<OrderDetail>(App.OrderRepo.GetAllOrderDetails());
            cfi = CultureInfo.CreateSpecificCulture(orderLines.First().languageCode);
            //
            //
            //
            NumberFormatInfo LocalFormat = (NumberFormatInfo)NumberFormatInfo.CurrentInfo.Clone();
            // Replace the default currency symbol with the local currency symbol.
            LocalFormat.CurrencySymbol = "P";
            //
            string total = OrderTotal.ToString("C", LocalFormat);
            var vtotal = OrderTotal.ToString("C", cfi);
            string currentPrice = String.Format("\u20B1{0}", OrderTotal);
            int line = 420;

            string zpl;

            if (newHeader_print.ChargedAmount > 0)
            {
                // "^XA^LL410^FO50,250^GB700,1,3^FS^CFA,30^FO50,100^FD" + "Name: " + fullName + "^FS" +
                zpl = "^XA^LL410^FO50,330^GB700,1,3^FS^CFA,30^FO50,100^FD" + "Store: " + App.BrowsingLocation.LocationName + "^FS" +
                    "^FO50,140^FD" + "Customer: " + fullName + "^FS" +
                    "^FO50,180^FD" + "Date: " + newHeader_print.OrderDate.ToString("MM/dd/yyyy") + "^FS" +
                    "^FO50,220^FD" + "Total: " + total + "^FS^" +
                    "^FO50,260^FD" + "Amount Paid:" + newHeader_print.CashAmount.ToString("C", LocalFormat) + "^FS^" +
                    "^FO50,300^FD" + "Charge Amount:" + newHeader_print.ChargedAmount.ToString("C", LocalFormat) + "^FS^" +
                    "^FO50,340^FD" + "Sales Details" + "^FS^";
                line = 340;
                //"^FO50,300^GB700,1,3^FS^XZ";
            }
            else
            {
                //"^XA^LL200^FO50,260^GB700,1,3^FS^CFA,30^FO50,100^FD" + "Store: " + App.BrowsingLocation.LocationName + "^FS" +
                DateTime orDate = DateTime.Now;
                zpl = "^XA^LL410^FO50,290^GB700,1,3^FS^CFA,30^FO50,100^FD" + "Store: " + App.BrowsingLocation.LocationName + "^FS" +
                    "^FO50,140^FD" + "Customer: " + fullName + "^FS" +
                     "^FO50,180^FD" + orDate.ToString("MM/dd/yyyy h:mm tt") + "^FS" +
                     "^FO50,220^FD" + "Total: " + total + "^FS^" +
                    "^FO50,260^FD" + "Amount Paid:" + newHeader_print.CashAmount.ToString("C", LocalFormat) + "^FS^" +
                    "^FO50,300^FD" + "Sales Details" + "^FS^";
                line = 300;
                //"^FO50,460^GB700,1,3^FS^XZ";
            }
            // line = 300;
            foreach (OrderDetail d in orderLines)
            {
                line += 40;
                string amount = d.Amount.ToString("C", LocalFormat);
                zpl += "^FO50," + line.ToString() + "^FD" + d.Quantity.ToString() + " " + d.ItemName + " " + amount + "^FS";
            }
            line += 40;
            //zpl += "^FO50," + line.ToString() + "^GB700,1,3^FS^XZ";
            zpl += "^XZ";
            try
            {
                if ((connection == null) || (!connection.IsConnected))
                {
                    connection = ConnectionBuilder.Current.Build(address);
                    connection.Open();
                }
                if ((SetPrintLanguage(connection)) && (CheckPrinterStatus(connection)))
                {
                    connection.Write(Encoding.ASCII.GetBytes(zpl));
                }
            }
            catch (Exception e)
            {
                Debug.WriteLine(tag, e.ToString());

            }
            finally
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    printButton1.IsEnabled = true;
                });
            }
        }

        private bool SetPrintLanguage(IConnection connection)
        {
            string setLanguage = "! U1 setvar \"device.languages\" \"zpl\"\r\n\r\n! U1 getvar \"device.languages\"\r\n\r\n";
            byte[] response = connection.SendAndWaitForResponse(Encoding.ASCII.GetBytes(setLanguage), 500, 500);
            string s = Encoding.ASCII.GetString(response);
            if (!s.Contains("zpl"))
            {
                Debug.WriteLine(tag, "Not A ZPL Printer.");
                return false;
            }
            return true;
        }

        private bool CheckPrinterStatus(IConnection connection)
        {
            IZebraPrinter printer = ZebraPrinterFactory.Current.GetInstance(PrinterLanguage.ZPL, connection);
            IPrinterStatus status = printer.CurrentStatus;
            if (!status.IsReadyToPrint)
            {
                Debug.WriteLine(tag, "Printer in Error: " + status.ToString());
            }
            return true;
        }
    }
}