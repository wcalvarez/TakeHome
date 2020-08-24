using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TakeHome.Models;
using System.Web;

namespace TakeHome.Services
{
    public class DataManager
    {
        ///api/AppMenuItemsByAppId/{appId:int}
        public async Task<String> GetAppMenuItems(int AppId)
        {
            string Url2 = App.Url + "api/AppMenuItemsByAppid/" + AppId.ToString();
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);
        }
        //
        //LOCATIONS
        //

        public async Task<String> GetLocations(string searchText)
        {
            string Url2 = App.Url + "api/Locations/BySearch/" + searchText;
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);
        }

        public async Task<String> GetLocationsByType(string locType)
        {
            string Url2 = App.Url + "api/Locations/ByType/" + locType;    ///api/Locations/ByType/
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);
        }

        //"~/api/AppUserType/
        public async Task<String> GetAppUserType(string userType)
        {
            string Url2 = App.Url + "/api/AppUserTypes/" + userType;    
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);
        }

        public async Task<String> GetLocationHours(int locId)
        {
            // TODO: use GET to retrieve locations
            var today = DateTime.Now;
            string WeekDay = today.ToString("dddd");

            string Url2 = App.Url + "api/BusinessHours/" + WeekDay + "/" + locId.ToString();
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);
        }

        public async Task<String> GetLocationCustomers(int locId)
        {
            string Url2 = App.Url + "api/locationcustomers/" + locId.ToString();
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);
        }

        public async Task<String> GetCountries()
        {
            string Url2 = App.Url + "api/countries";
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);

        }

        public async Task<String> GetStates(int id)
        {
            string Url2 = App.Url + "api/countrystates/" + id.ToString();
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);

        }

        public async Task<String> GetLocation(int id)
        {
            string Url2 = App.Url + "api/locations/" + id.ToString();
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);

        }

        //
        //PRODUCTS
        //

        public async Task<String> GetProducts(int locID)
        {
            // TODO: use GET to retrieve locations
            string Url2 = App.Url + "api/locations/" + locID.ToString() + "/products";
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);
        }

        //
        //Fees
        //

        public async Task<String> GetFees(int memberId)
        {
            // TODO: use GET to retrieve Fees
            //"~/api/members/{memberId:int}/fees"
            string Url2 = App.Url + "api/members/" + memberId.ToString() + "/fees";
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);
        }

        //
        // APPUSER-MEMBERS
        //
        public async Task<String> GetCustomers()
        {
            string Url2 = App.Url + "api/Products";
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);

        }

        public async Task<String> SearchCustomers(string searchText)
        {
            string Url2 = App.Url + "api/Customers/BySearch/" + searchText;
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);
        }

        //public async Task<String> GetMember(MemberDTO member)
        //{
        //    // TODO: use GET to retrieve Fees
        //    //[Route("~/api/members/{locationId:int}/{memberNumber:int}/membership")]
        //    string Url2 = App.Url + "api/members/" + member.LocationID.ToString() + "/" + member.MembershipNumber + "/" + "membership";
        //    HttpClient client = await GetClient();
        //    return await client.GetStringAsync(Url2);
        //}

        public async Task<AppUser> RegisterAppUser(AppUser user)
        {
            string Url2 = App.Url + "api/AppUsers/";
            HttpClient client = await GetClient();
            //user.AppUserID = 1;
            var myContent = JsonConvert.SerializeObject(user);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(Url2, byteContent);
            var contents = await response.Content.ReadAsStringAsync();
            AppUser newuser = JsonConvert.DeserializeObject<AppUser>(contents);
            return newuser;
        }

        public async Task<AuthToken> PostAuthToken(AuthToken authToken)
        {
            string Url2 = App.Url + "api/AuthTokens/";
            HttpClient client = await GetClient();

            var myContent = JsonConvert.SerializeObject(authToken);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(Url2, byteContent);
            var contents = await response.Content.ReadAsStringAsync();
            AuthToken newtoken = JsonConvert.DeserializeObject<AuthToken>(contents);
            return newtoken;
        }

        public async Task<String> GetAppUser(AppUser user)
        {
            string Url2 = App.Url + "api/validateuser/ByEmail/";
            Url2 += user.Email;
            Url2 += "/";
            Url2 += user.Password;

            HttpClient client = await GetClient();

            return await client.GetStringAsync(Url2);
        }

        public async Task<String> GetAppUserByEmail(AppUser user)
        {
            string Urlemail = App.Url + "api/AppUser/ByEmail/";
            Urlemail += user.Email;

            HttpClient client = await GetClient();

            return await client.GetStringAsync(Urlemail);

        }

        public async Task<AppUser> AppUserExists(int id)
        {
            string UrlId = App.Url + "api/AppUsers/" + id.ToString();

            HttpClient client = await GetClient();

            var response = await client.GetStringAsync(UrlId);
            AppUser updatedUser = JsonConvert.DeserializeObject<AppUser>(response);
            return updatedUser;
        }

        public Customer CustomerExists(int id)
        {

            Customer updateCustomer = App.CustomerRepo.GetCustomer(id);
            return updateCustomer;
        }

        public async Task<string> UpdateAppUser(AppUser user)
        {
            string Url2 = App.Url + "api/AppUsers/" + user.AppUserID.ToString();

            HttpClient client = await GetClient();
            var myContent = JsonConvert.SerializeObject(user);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(Url2, byteContent);
            var contents = await response.Content.ReadAsStringAsync();
            AppUser updatedUser = JsonConvert.DeserializeObject<AppUser>(contents);
            //return updatedUser;
            return contents;
        }

        //public async Task<Fee> UpdateFee(Fee fee)
        //{
        //    string Url2 = App.Url + "api/Fees/" + fee.FeeID.ToString();

        //    HttpClient client = await GetClient();
        //    var myContent = JsonConvert.SerializeObject(fee);
        //    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
        //    var byteContent = new ByteArrayContent(buffer);
        //    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //    var response = await client.PutAsync(Url2, byteContent);
        //    var contents = await response.Content.ReadAsStringAsync();
        //    Fee updatedFee = JsonConvert.DeserializeObject<Fee>(contents);
        //    return updatedFee;
        //}

        public async Task<AppUser> UpdateAppUserProfile(AppUser user)
        {
            //string Url2 = App.Url + "api/AppUsersProfile/" + user.AppUserID.ToString();
            string Url2 = App.Url + "api/AppUsers/" + user.AppUserID.ToString();
            HttpClient client = await GetClient();
            var myContent = JsonConvert.SerializeObject(user);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PutAsync(Url2, byteContent);
            var contents = await response.Content.ReadAsStringAsync();
            AppUser updatedUser = JsonConvert.DeserializeObject<AppUser>(contents);
            return updatedUser;
        }

        //Get AppUser StoreCredits from specific location
        public async Task<String> GetStoreCredit(int locID)
        {
            // api / StoreCredits /{ locationId: int}/{ appuserid: int}
            string Url2 = App.Url + "api/StoreCredits/" + locID.ToString() + "/" + App.user.AppUserID.ToString();
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);
        }

        public async Task<String> GetCustomerCredit(int locID, int customerid)
        {
            // api / StoreCredits /{ locationId: int}/{ appuserid: int}
            string Url2 = App.Url + "api/CustomerCredits/" + locID.ToString() + "/" + customerid.ToString();
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);
        }

        public async Task<String> GetAppUserStoreCredits(int id)
        {
            // TODO: use GET to retrieve locations
            string Url2 = App.Url + "api/StoreCredits/ByAppUser/" + id.ToString();
            HttpClient client = await GetClient();
            return await client.GetStringAsync(Url2);
        }

        //public async Task<AuthToken> PostAuthToken(AuthToken authToken)
        //{
        //    string Url2 = App.Url + "api/AuthTokens/";
        //    HttpClient client = await GetClient();

        //    var myContent = JsonConvert.SerializeObject(authToken);
        //    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
        //    var byteContent = new ByteArrayContent(buffer);
        //    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //    var response = await client.PostAsync(Url2, byteContent);
        //    var contents = await response.Content.ReadAsStringAsync();
        //    AuthToken newtoken = JsonConvert.DeserializeObject<AuthToken>(contents);
        //    return newtoken;
        //}

        //
        // ORDERS Processing
        //
        //public async Task<String> GetTaxRate(ZipTaxParam zp)
        //{
        //    string ziptaxURL = "https://api.zip-tax.com/request/v30";


        //    HttpClient client = await GetClient();
        //    //

        //    var myContent = JsonConvert.SerializeObject(zp);
        //    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
        //    var byteContent = new ByteArrayContent(buffer);
        //    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");

        //    var builder = new UriBuilder("https://api.zip-tax.com/request/v30");
        //    builder.Port = -1;
        //    var query = HttpUtility.ParseQueryString(builder.Query);
        //    query["key"] = "CKJKU6PL3PRG";
        //    query["postalcode"] = "10930";
        //    builder.Query = query.ToString();
        //    string url = builder.ToString();
        //    var jsonString = await client.GetStringAsync(url);
        //    string TaxRate = "8.25";
        //    return TaxRate;
        //}

        public async Task<int> GetQtyOnHand(int productPriceID)
        {
            string Url2 = App.Url + "api/ProductQOH/" + productPriceID.ToString();
            HttpClient client = await GetClient();
            var response = await client.GetStringAsync(Url2);

            int qty = 0;
            Int32.TryParse(response, out qty);
            return qty; // get QOH from backend for P
        }

        public async Task<OrderHeader> PostOrderHeader(OrderHeader orderHeader)
        {
            string Url2 = App.Url + "api/OrderHeaders";
            HttpClient client = await GetClient();

            var myContent = JsonConvert.SerializeObject(orderHeader);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(Url2, byteContent);
            var contents = await response.Content.ReadAsStringAsync();
            OrderHeader newHdr = JsonConvert.DeserializeObject<OrderHeader>(contents);
            return newHdr;
        }

        public async Task<ShippingInfo> PostShippingInfo(ShippingInfo sendto)
        {
            string Url2 = App.Url + "api/ShippingInfos/";
            HttpClient client = await GetClient();

            var myContent = JsonConvert.SerializeObject(sendto);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(Url2, byteContent);
            var contents = await response.Content.ReadAsStringAsync();
            ShippingInfo newsendto = JsonConvert.DeserializeObject<ShippingInfo>(contents);
            return newsendto;
        }

        public async Task<bool> DeleteShippingInfo(int? id)
        {
            string Url2 = App.Url + "api/ShippingInfos/" + id.ToString();
            HttpClient client = await GetClient();

            var response = await client.DeleteAsync(Url2);
            return response.IsSuccessStatusCode;
        }

        public async Task<Customer> PostCustomer(Customer customer)
        {
            string Url2 = App.Url + "api/Customers";
            HttpClient client = await GetClient();

            var myContent = JsonConvert.SerializeObject(customer);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(Url2, byteContent);
            var contents = await response.Content.ReadAsStringAsync();
            Customer newCustomer = JsonConvert.DeserializeObject<Customer>(contents);
            return newCustomer;
        }

        //public async Task<FeePaymentHeader> PostFeePaymentHeader(FeePaymentHeader paymentHeader)
        //{
        //    string Url2 = App.Url + "api/FeePaymentHeaders";
        //    HttpClient client = await GetClient();

        //    var myContent = JsonConvert.SerializeObject(paymentHeader);
        //    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
        //    var byteContent = new ByteArrayContent(buffer);
        //    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //    var response = await client.PostAsync(Url2, byteContent);
        //    var contents = await response.Content.ReadAsStringAsync();
        //    FeePaymentHeader newHdr = JsonConvert.DeserializeObject<FeePaymentHeader>(contents);
        //    return newHdr;
        //}

        //public async Task<Fee> PostFees(FeePaymentDetail[] feeItems)
        //{
        //    string url2 = App.Url + "api/Fees/batchPost";
        //    HttpClient client = await GetClient();

        //    var myContent = JsonConvert.SerializeObject(feeItems);
        //    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
        //    var byteContent = new ByteArrayContent(buffer);
        //    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //    var response = await client.PostAsync(url2, byteContent);
        //    var contents = await response.Content.ReadAsStringAsync();
        //    Fee newFee = JsonConvert.DeserializeObject<Fee>(contents);
        //    return newFee;
        //}

        public async Task<OrderDetail> PostOrderDetails(OrderDetail[] orderItems)
        {
            string url2 = App.Url + "api/OrderDetails/batchPost";
            HttpClient client = await GetClient();

            var myContent = JsonConvert.SerializeObject(orderItems);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(url2, byteContent);
            var contents = await response.Content.ReadAsStringAsync();
            OrderDetail newDtl = JsonConvert.DeserializeObject<OrderDetail>(contents);
            return newDtl;
        }

        //public async Task<ShippingInfo> PostShippingInfo(ShippingInfo sendto)
        //{
        //    string Url2 = App.Url + "api/ShippingInfos/";
        //    HttpClient client = await GetClient();

        //    var myContent = JsonConvert.SerializeObject(sendto);
        //    var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
        //    var byteContent = new ByteArrayContent(buffer);
        //    byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        //    var response = await client.PostAsync(Url2, byteContent);
        //    var contents = await response.Content.ReadAsStringAsync();
        //    ShippingInfo newsendto = JsonConvert.DeserializeObject<ShippingInfo>(contents);
        //    return newsendto;
        //}

        //public async Task<bool> DeleteShippingInfo(int? id)
        //{
        //    string Url2 = App.Url + "api/ShippingInfos/" + id.ToString();
        //    HttpClient client = await GetClient();

        //    var response = await client.DeleteAsync(Url2);
        //    return response.IsSuccessStatusCode;
        //}

        //
        // Stripe Processing
        //
        public async Task<StripeTest> PostStripeTest(StripeTest stripeTest)
        {
            string Url2 = App.Url + "api/StripeTests";
            HttpClient client = await GetClient();

            var myContent = JsonConvert.SerializeObject(stripeTest);
            var buffer = System.Text.Encoding.UTF8.GetBytes(myContent);
            var byteContent = new ByteArrayContent(buffer);
            byteContent.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            var response = await client.PostAsync(Url2, byteContent);
            var contents = await response.Content.ReadAsStringAsync();
            StripeTest testedStripe = JsonConvert.DeserializeObject<StripeTest>(contents);
            return testedStripe;
        }

        //END: Stripe Processing

        private async Task<HttpClient> GetClient()

        {
            //var client = App.app_client;
            // HttpClient client = new HttpClient();
            //NOTE:client with Timeout should be used for Production only
            var client = new HttpClient()
            {
                Timeout = TimeSpan.FromMilliseconds(60000) //1 min
                //  Timeout = TimeSpan.FromSeconds(360)
            };

            //localhost client
            //var client = new HttpClient();


            byte[] authBytes = Encoding.UTF8.GetBytes("kabayan@gmail.com" + ":" + "Test2018!");
            var wca = Convert.ToBase64String(authBytes);
            client.DefaultRequestHeaders.Add("authorization", "Basic " + Convert.ToBase64String(authBytes));
            client.DefaultRequestHeaders.Add("Accept", "application/json");

            return client;
        }
    }
}
