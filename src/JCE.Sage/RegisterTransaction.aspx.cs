using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;
using System.IO;
using System.Configuration;
using System.Collections.Specialized;
using System.ComponentModel;
using JCE.Sage.Providers;
using JCE.Crm.Plugins.Entities;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk;

namespace JCE.Sage
{
    public partial class RegisterTransaction : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ConfigurationProvider configuration = new ConfigurationProvider();

            Guid paymentId;
            if (!Guid.TryParse(Request["paymentid"], out paymentId))
            {
                Response.Redirect(configuration.OrderFailedUrl + "?errorcode=" + (int)ErrorCodes.PaymentIdMissing);
            }
            
            CrmConnector connector = new CrmConnector(Properties.Settings.Default.ConnectionString);
            if(!connector.isReady()) Response.Redirect(configuration.OrderFailedUrl + "?errorcode=" + (int)ErrorCodes.ErrorWithCRM);
            PaymentProvider paymentProvider = new PaymentProvider(connector);
            var payment = paymentProvider.GetPayment(paymentId);

            if (payment == null)
            {
                throw new ArgumentException("PaymentId is incorrect.");
            }

            try
            {
                WebRequest request = WebRequest.Create(configuration.SagePayWebServiceAddress);
                request.Method = "Post";

                string postData = CreatePostRequest(configuration, connector, payment);

                // If the postData in invalid contact then ensure the Contact has the right details.
                if (postData == "INVALIDCONTACT")
                {
                    Response.Redirect(configuration.OrderFailedUrl + "?errorcode=" + ((int)ErrorCodes.InvalidContactDetails).ToString());
                }

                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                request.ContentType = "application/x-www-form-urlencoded";
                request.ContentLength = byteArray.Length;

                Stream dataStream = request.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                WebResponse response = request.GetResponse();
                dataStream = response.GetResponseStream();

                StreamReader reader = new StreamReader(dataStream);
                string responseFromSagePay = reader.ReadToEnd();

                reader.Close();
                dataStream.Close();
                response.Close();

                string nextUrl = CheckSagePayResponse(configuration, connector, payment, responseFromSagePay);
                if (!String.IsNullOrEmpty(nextUrl))
                {
                    Response.Redirect(nextUrl);
                }
            }
            catch (WebException webException)
            {
                string error = String.Empty;
                ErrorCodes errorCode;
                if (webException.Status == WebExceptionStatus.NameResolutionFailure)
                {
                    errorCode = ErrorCodes.NameResolutionFailure;
                    error = @"Your server was unable to register this transaction with Sage Pay.
Check that you do not have a firewall restricting the POST and 
that your server can correctly resolve the address " + configuration.SagePayWebServiceAddress;
                }
                else
                {
                    errorCode = ErrorCodes.GeneralError;
                    error = @"An Error has occurred whilst trying to register this transaction.<BR>
The Error is: " + webException;
                }

                payment.lss_responsestatus = errorCode.ToString();
                payment.lss_responsestatusdetail = (error.Length > 2000) ? error.Substring(0, 2000) : error;
                payment.lss_paystatus.Value = (int)PaymentProvider.PaymentStatus.Failed;
                paymentProvider.SavePayment(payment);

                Response.Redirect(configuration.OrderFailedUrl + "?errorcode=" + ((int)errorCode).ToString());
            }
        }

        /// <summary>
        /// Checks the response from SagePay
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="connector">The connector.</param>
        /// <param name="payment">The payment.</param>
        /// <param name="responseFromSagePay">The response from sage pay.</param>
        /// <returns>
        /// Returns the next url to go to if the status is ok
        /// </returns>
        private string CheckSagePayResponse(ConfigurationProvider configuration, CrmConnector connector, lss_payment payment, string responseFromSagePay)
        {
            string nextUrl = String.Empty;

            string[] responses = responseFromSagePay.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (string response in responses)
            {
                int splitIndex = response.IndexOf("=");
                if (splitIndex != -1)
                {
                    string name = response.Substring(0, splitIndex);
                    string value = response.Substring(splitIndex + 1);

                    switch (name)
                    {
                        case "VPSProtocol":
                            payment.lss_vpsprotocol = value;
                            break;
                        case "Status":
                            payment.lss_responsestatus = value;
                            break;
                        case "StatusDetail":
                            payment.lss_responsestatusdetail = value;
                            break;
                        case "VPSTxId":
                            payment.lss_vpstxid = value;
                            break;
                        case "SecurityKey":
                            payment.lss_securitykey = value;
                            break;
                        case "NextURL":
                            payment.lss_nexturl = value;
                            break;
                    }
                }
            }

            if (payment.lss_responsestatus != null)
            {
                switch (payment.lss_responsestatus)
                {
                    case "OK":
                        // If the status is OK then we can return the Next Url
                        nextUrl = payment.lss_nexturl == null ? String.Empty : payment.lss_nexturl;
                        break;
                    case "MALFORMED":
                        payment.lss_paystatus.Value = (int)PaymentProvider.PaymentStatus.Failed;
                        nextUrl = configuration.OrderFailedUrl + "?errorcode=" + ((int)ErrorCodes.MalformedPost).ToString();
                        break;
                    case "INVALID":
                        payment.lss_paystatus.Value = (int)PaymentProvider.PaymentStatus.Failed;
                        nextUrl = configuration.OrderFailedUrl + "?errorcode=" + ((int)ErrorCodes.InvalidPost).ToString();
                        break;
                    default:
                        payment.lss_paystatus.Value = (int)PaymentProvider.PaymentStatus.Failed;
                        nextUrl = configuration.OrderFailedUrl + "?errorcode=" + ((int)ErrorCodes.GeneralPostError).ToString();
                        break;
                }
            }

            PaymentProvider paymentProvider = new PaymentProvider(connector);
            paymentProvider.SavePayment(payment);

            return nextUrl;
        }

        /// <summary>
        /// Creates the post request.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="connector">The connector.</param>
        /// <param name="paymentId">The payment id.</param>
        /// <returns></returns>
        private static string CreatePostRequest(ConfigurationProvider configuration, CrmConnector connector, lss_payment payment)
        {
            OrderProvider orderProvider = new OrderProvider(connector);
            ContactProvider contactProvider = new ContactProvider(connector);
            PaymentProvider paymentProvider = new PaymentProvider(connector);

            var order = orderProvider.GetOrder(payment.lss_orderid.Id);
            var contact = contactProvider.GetContact(order.CustomerId.Id);

            StringBuilder postRequest = new StringBuilder();

            payment.lss_vendortxcode = CreateVendorTransactionCode(order.Name);

            // Save the record so the payment has a name
            payment.lss_name = "Payment: " + payment.lss_vendortxcode + " - £" + payment.lss_amount.Value.ToString("#.##");
            paymentProvider.SavePayment(payment);

            // Check the Contact has the relevant details for a sucessful transaction
            if (String.IsNullOrEmpty(contact.FirstName) ||
                String.IsNullOrEmpty(contact.LastName) ||
                String.IsNullOrEmpty(contact.Address1_Line1) ||
                String.IsNullOrEmpty(contact.Address1_City) ||
                String.IsNullOrEmpty(contact.Address1_PostalCode))
            {
                return "INVALIDCONTACT";
            }

            postRequest.Append("VPSProtocol=4.00&TxType=PAYMENT&Vendor=");
            postRequest.Append(configuration.VendorName);
            postRequest.Append("&VendorTxCode=" + payment.lss_vendortxcode);
            postRequest.Append("&Amount=" + payment.lss_amount.Value.ToString("#.##"));
            postRequest.Append("&Currency=GBP");
            postRequest.Append("&Description=" + order.Name);
            postRequest.Append("&NotificationURL=" + configuration.NotificationUrl);
            postRequest.Append(BuildContactDetails(contact));
            postRequest.Append("&CustomerEMail=" + (String.IsNullOrEmpty(contact.EMailAddress1) ? String.Empty : contact.EMailAddress1.Trim()));
            postRequest.Append(BuildBasket(connector, orderProvider, order));
            postRequest.Append("&AllowGiftAid=0&ApplyAVSCV2=0&Apply3DSecure=0&Profile=NORMAL&AccountType=M");

            return postRequest.ToString();
        }

        /// <summary>
        /// Builds the contact details.
        /// </summary>
        /// <param name="contact">The contact.</param>
        /// <returns>The contact details concatenated into a string.</returns>
        private static string BuildContactDetails(Contact contact)
        {
            StringBuilder contactDetails = new StringBuilder();

            contactDetails.Append("&BillingSurname=" + (String.IsNullOrEmpty(contact.LastName) ? String.Empty : contact.LastName));
            contactDetails.Append("&BillingFirstnames=" + (String.IsNullOrEmpty(contact.FirstName) ? String.Empty : contact.FirstName));
            contactDetails.Append("&BillingAddress1=" + (String.IsNullOrEmpty(contact.Address1_Line1) ? String.Empty : MaxLength(contact.Address1_Line1,50)));
            contactDetails.Append("&BillingAddress2=" + (String.IsNullOrEmpty(contact.Address1_Line2) ? String.Empty : MaxLength(contact.Address1_Line2,50)));
            contactDetails.Append("&BillingCity=" + (String.IsNullOrEmpty(contact.Address1_City) ? String.Empty : contact.Address1_City));
            contactDetails.Append("&BillingPostCode=" + (String.IsNullOrEmpty(contact.Address1_PostalCode) ? String.Empty : contact.Address1_PostalCode));
            contactDetails.Append("&BillingCountry=GB");

            contactDetails.Append("&DeliverySurname=" + (String.IsNullOrEmpty(contact.LastName) ? String.Empty : contact.LastName));
            contactDetails.Append("&DeliveryFirstnames=" + (String.IsNullOrEmpty(contact.FirstName) ? String.Empty : contact.FirstName));
            contactDetails.Append("&DeliveryAddress1=" + (String.IsNullOrEmpty(contact.Address1_Line1) ? String.Empty : MaxLength(contact.Address1_Line1,50)));
            contactDetails.Append("&DeliveryAddress2=" + (String.IsNullOrEmpty(contact.Address1_Line2) ? String.Empty : MaxLength(contact.Address1_Line2,50)));
            contactDetails.Append("&DeliveryCity=" + (String.IsNullOrEmpty(contact.Address1_City) ? String.Empty : contact.Address1_City));
            contactDetails.Append("&DeliveryPostCode=" + (String.IsNullOrEmpty(contact.Address1_PostalCode) ? String.Empty : contact.Address1_PostalCode));
            contactDetails.Append("&DeliveryCountry=GB");

            return contactDetails.ToString();
        }


        private static string MaxLength(string value, int length)
        {
            if (value.Length < length) return value;

            value = value.Substring(0, length);
            return value;
        }

        /// <summary>
        /// Builds the basket.
        /// </summary>
        /// <param name="connector">The connector.</param>
        /// <param name="order">The order.</param>
        /// <returns>
        /// A string containing the contents of the order
        /// </returns>
        private static string BuildBasket(CrmConnector connector, OrderProvider orderProvider, SalesOrder order)
        {

            StringBuilder basket = new StringBuilder();

            // A basket item comprises of Product Name, Quantity, Unit Cost, Tax, Cost inc. Tax, and Total Cost (Cost*Quantity).  These are all seperated with colons (i.e. %3A).
            string basketItem = String.Empty;
            string seperator = "%3A";

            // Get a list of the names for the level Option Set.
            MetaDataProvider metaDataProvider = new MetaDataProvider(connector);
            OptionMetadataCollection levelOptionSetLabels = metaDataProvider.RetrieveOptionSetMetaDataCollection(SalesOrderDetail.EntityLogicalName, "lss_level");

            int basketCount = 0;
            List<SalesOrderDetail> productExtras;
            List<SalesOrderDetail> orderDetails = orderProvider.GetOrderDetail(order.Id, ProductType.Course);
            foreach (SalesOrderDetail detail in orderDetails)
            {
                string basketItemName = GetBasketNameOfProduct(detail, levelOptionSetLabels);

                // Add name of basket item with a hard-coded Quantity of 1
                basketItem = seperator + basketItemName + seperator + "1" + seperator;

                // For each product check if it has any related products
                productExtras = orderProvider.GetProductExtras(detail.Id);
                decimal extraUnitCost = 0;
                decimal extraTax = 0;
                decimal extraTotal = 0;
                foreach (SalesOrderDetail extra in productExtras)
                {
                    extraUnitCost += extra.BaseAmount == null ? 0 : extra.BaseAmount.Value;
                    extraTax += extra.Tax == null ? 0 : extra.Tax.Value;
                    extraTotal += extra.ExtendedAmount == null ? 0 : extra.ExtendedAmount.Value;
                }

                // Add unit cost
                basketItem += (detail.BaseAmount.Value + extraUnitCost).ToString("#.##") + seperator;

                // Add Tax
                basketItem += detail.Tax == null ? "0" : (detail.Tax.Value + extraTax).ToString("#.##") + seperator;

                // Add Cost inc. Tax
                basketItem += (detail.ExtendedAmount.Value + extraTotal).ToString("#.##") + seperator;

                // Add Total Cost 
                basketItem += (detail.ExtendedAmount.Value + extraTotal).ToString("#.##");

                basket.Append(basketItem);
                basketCount++;
            }

            basket.Insert(0, "&Basket=" + basketCount.ToString());

            return basket.ToString();
        }

        /// <summary>
        /// Gets the basket name of product.
        /// </summary>
        /// <param name="detail">The detail.</param>
        /// <param name="optionSetLabels">The option set labels.</param>
        /// <returns></returns>
        private static string GetBasketNameOfProduct(SalesOrderDetail detail, OptionMetadataCollection levelOptionSetLabels)
        {
            string basketName = detail.lss_courseid == null ? String.Empty : detail.lss_courseid.Name;
            basketName += " - " + ReplaceOptionSet(levelOptionSetLabels, detail.lss_Level) + " - ";
            basketName += detail.lss_Centre == null ? String.Empty : detail.lss_Centre.Name;

            return basketName;
        }

        /// <summary>
        /// Replaces the option set.
        /// </summary>
        /// <param name="localContext">The local context.</param>
        /// <param name="optionSetLabels">The option set labels.</param>
        /// <param name="optionSetValue">The option set value.</param>
        /// <returns></returns>
        private static string ReplaceOptionSet(OptionMetadataCollection optionSetLabels, OptionSetValue optionSetValue)
        {
            if (optionSetValue != null)
            {
                foreach (OptionMetadata optionMetadata in optionSetLabels)
                {
                    if (optionMetadata.Value == optionSetValue.Value)
                    {
                        return optionMetadata.Label.UserLocalizedLabel.Label;
                    }
                }
            }

            return String.Empty;
        }

        /// <summary>
        /// Creates the vendor transaction code.
        /// </summary>
        /// <param name="orderName">Name of the order.</param>
        /// <returns>
        /// A new vendor transaciton code
        /// </returns>
        private static string CreateVendorTransactionCode(string orderName)
        {
            DateTime currentDate = DateTime.Now;

            return orderName + "-"
                + currentDate.Year.ToString()
                + currentDate.Month.ToString()
                + currentDate.Day.ToString()
                + currentDate.Hour.ToString()
                + currentDate.Minute.ToString()
                + currentDate.Second.ToString();
        }

    }
}