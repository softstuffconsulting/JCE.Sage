using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using JCE.Sage.Providers;
using System.Web.Security;
using JCE.Crm.Plugins.Entities;

namespace JCE.Sage
{
    public partial class NotificationPage : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

            string vendorTxCode = Request.Form["VendorTxCode"];
            string vpstxId = Request.Form["VPSTxId"];

            ConfigurationProvider configuration = new ConfigurationProvider();
            CrmConnector connector = new CrmConnector(Properties.Settings.Default.ConnectionString);
            PaymentProvider paymentProvider = new PaymentProvider(connector);

            // Check we have a payment for the transaction code and id
            var payment = paymentProvider.GetPayment(vendorTxCode, vpstxId);
            if (payment == null)
            {
                HandleError(configuration, ErrorCodes.TransactionNotFound);
                return;
            }
            else
            {
                ReadFormFields(payment);

                // Before we check that the signatures are correct, we should just check if the user cancelled the transaction or an error occurred
                string returnStatus = String.Empty;
                string redirectURL = String.Empty;
                StatusCodes statusCode = HandleStatus(payment.lss_notificationstatus);
                switch (statusCode)
                {
                    case StatusCodes.Abort:
                        payment.lss_paystatus.Value = (int)PaymentProvider.PaymentStatus.Unpaid;
                        returnStatus = "OK";
                        redirectURL = configuration.OrderFailedUrl + "?errorcode=" + ((int)ErrorCodes.Aborted).ToString();
                        break;
                    case StatusCodes.Unspecified:
                        payment.lss_paystatus.Value = (int)PaymentProvider.PaymentStatus.Failed;
                        returnStatus = "OK";
                        redirectURL = configuration.OrderFailedUrl + "?errorcode=" + ((int)ErrorCodes.UnspecifiedPaymentError).ToString();
                        break;
                    case StatusCodes.Error:
                        payment.lss_paystatus.Value = (int)PaymentProvider.PaymentStatus.Failed;
                        returnStatus = "INVALID";
                        redirectURL = configuration.OrderFailedUrl + "?errorcode=" + ((int)ErrorCodes.PaymentError).ToString();
                        break;
                }

                // Return the status if one has occurred already
                if (!String.IsNullOrEmpty(returnStatus))
                {
                    paymentProvider.SavePayment(payment);
                    Response.Write("Status=" + returnStatus + System.Environment.NewLine);
                    Response.Write("RedirectURL=" + redirectURL);
                    Response.End();
                    return;
                }

                // Rebuild the post message, so we can then hash it with the security key, and then check against VPSSignature
                string postMessage = vpstxId + vendorTxCode + payment.lss_notificationstatus + payment.lss_txauthno.ToString() + configuration.VendorName + payment.lss_avscv2 + payment.lss_securitykey +
                    payment.lss_addressresult + payment.lss_postcoderesult + payment.lss_cv2result + payment.lss_giftaid + payment.lss_securestatus3d + payment.lss_cavv +
                    payment.lss_addressstatus + payment.lss_payerstatus + payment.lss_cardtype + payment.lss_last4digits;

                string hashedPostMessage = FormsAuthentication.HashPasswordForStoringInConfigFile(postMessage, "MD5");
                if (payment.lss_vpssignature != hashedPostMessage)
                {
                    // The signatures don't match up, so this could indicate the order has been tampered with.
                    payment.lss_paystatus.Value = (int)PaymentProvider.PaymentStatus.Failed;
                    payment.lss_notificationstatus = "INVALID";
                    payment.lss_notificationstatusdetail = "TAMPER WARNING! Signatures do not match for this Payment.  The Payment was Cancelled.";
                    paymentProvider.SavePayment(payment);

                    HandleError(configuration, ErrorCodes.UnmatchedSignatures);
                    return;
                }
                else
                {
                    Response.Clear();
                    Response.ContentType = "text/plain";

                    // Signatures match, so this is Good :)  Now let's find out what actually happened
                    switch (statusCode)
                    {
                        case StatusCodes.Ok:
                        case StatusCodes.Authenticated:
                        case StatusCodes.Registered:
                            payment.lss_datepaid = DateTime.Now;
                            payment.lss_paystatus.Value = (int)PaymentProvider.PaymentStatus.Successful;
                            Response.Write("Status=OK" + System.Environment.NewLine);
                            Response.Write("RedirectURL=" + configuration.OrderSuccessfulUrl);
                            break;
                        case StatusCodes.Abort:
                            payment.lss_paystatus.Value = (int)PaymentProvider.PaymentStatus.Failed;
                            Response.Write("Status=OK" + System.Environment.NewLine);
                            Response.Write("RedirectURL=" + configuration.OrderFailedUrl + "?errorcode=" + ((int)ErrorCodes.Aborted).ToString());
                            break;
                        case StatusCodes.NotAuthed:
                            payment.lss_paystatus.Value = (int)PaymentProvider.PaymentStatus.Declined;
                            Response.Write("Status=OK" + System.Environment.NewLine);
                            Response.Write("RedirectURL=" + configuration.OrderFailedUrl + "?errorcode=" + ((int)ErrorCodes.NotAuthorised).ToString());
                            break;
                        case StatusCodes.Rejected:
                            payment.lss_paystatus.Value = (int)PaymentProvider.PaymentStatus.Rejected;
                            Response.Write("Status=OK" + System.Environment.NewLine);
                            Response.Write("RedirectURL=" + configuration.OrderFailedUrl + "?errorcode=" + ((int)ErrorCodes.Rejected).ToString());
                            break;
                        case StatusCodes.Unspecified:
                            payment.lss_paystatus.Value = (int)PaymentProvider.PaymentStatus.Failed;
                            Response.Write("Status=OK" + System.Environment.NewLine);
                            Response.Write("RedirectURL=" + configuration.OrderFailedUrl + "?errorcode=" + ((int)ErrorCodes.UnspecifiedPaymentError).ToString());
                            break;
                        case StatusCodes.Error:
                            payment.lss_paystatus.Value = (int)PaymentProvider.PaymentStatus.Failed;
                            Response.Write("Status=INVALID" + System.Environment.NewLine);
                            Response.Write("RedirectURL=" + configuration.OrderFailedUrl + "?errorcode=" + ((int)ErrorCodes.PaymentError).ToString());
                            break;
                    }

                    paymentProvider.SavePayment(payment);

                    Response.End();
                }
            }
        }

        /// <summary>
        /// Reads the form fields.
        /// </summary>
        /// <param name="payment">The payment.</param>
        private void ReadFormFields(lss_payment payment)
        {
            payment.lss_notificationstatus = Request.Form["Status"];
            payment.lss_vpssignature = Request.Form["VPSSignature"];
            payment.lss_notificationstatusdetail = Request.Form["StatusDetail"];

            int txAuthNo = 0;
            Int32.TryParse(Request.Form["TxAuthNo"], out txAuthNo);
            payment.lss_txauthno = txAuthNo;

            payment.lss_avscv2 = Request.Form["AVSCV2"];
            payment.lss_addressresult = Request.Form["AddressResult"];
            payment.lss_postcoderesult = Request.Form["PostCodeResult"];
            payment.lss_cv2result = Request.Form["CV2Result"];
            payment.lss_giftaid = Request.Form["GiftAid"];
            payment.lss_securestatus3d = Request.Form["3DSecureStatus"];
            payment.lss_cavv = Request.Form["CAVV"];
            payment.lss_addressstatus = Request.Form["AddressStatus"];
            payment.lss_payerstatus = Request.Form["PayerStatus"];
            payment.lss_cardtype = Request.Form["CardType"];
            payment.lss_last4digits = Request.Form["Last4Digits"];
        }

        /// <summary>
        /// Handles the status.
        /// </summary>
        /// <param name="status">The status.</param>
        /// <returns></returns>
        private StatusCodes HandleStatus(string status)
        {
            switch (status)
            {
                case "OK":
                    return StatusCodes.Ok;
                case "NOTAUTHED":
                    return StatusCodes.NotAuthed;
                case "ABORT":
                    return StatusCodes.Abort;
                case "REJECTED":
                    return StatusCodes.Rejected;
                case "AUTHENTICATED":
                    return StatusCodes.Authenticated;
                case "REGISTERED":
                    return StatusCodes.Registered;
                case "ERROR":
                    return StatusCodes.Error;
                default:
                    return StatusCodes.Unspecified;
            }
        }

        /// <summary>
        /// Handles the error.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="errorCode">The error code.</param>
        private void HandleError(ConfigurationProvider configuration, ErrorCodes errorCode)
        {
            string statusDetail = String.Empty;
            switch (errorCode)
            {
                case ErrorCodes.TransactionNotFound:
                    statusDetail = "Unable to find the transaction in the system.";
                    break;
                case ErrorCodes.UnmatchedSignatures:
                    statusDetail = "Cannot match the MD5 Hash. Transaction might have been tampered with.";
                    break;
                default:
                    statusDetail = "An unknown error occurred.";
                    break;
            }
            Response.Clear();
            Response.ContentType = "text/plain";
            Response.Write("Status=INVALID" + System.Environment.NewLine);
            Response.Write("RedirectURL=" + configuration.OrderFailedUrl + "?errorcode=" + ((int)errorCode).ToString() + System.Environment.NewLine);
            Response.Write("StatusDetail=" + statusDetail + System.Environment.NewLine);
            Response.End();
        }
    }
}