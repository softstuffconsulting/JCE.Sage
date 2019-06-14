using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Net;
using System.Text;
using System.IO;
using JCE.Sage.Providers;

namespace JCE.Sage
{
    public partial class OrderFailed : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Get the error code which should be an number
            int errorCode;
            if (!Int32.TryParse(Request["errorcode"], out errorCode))
            {
                // Treat it as a general error
                errorCode = (int)ErrorCodes.GeneralError;
            }

            string errorDescription = String.Empty;
            switch (errorCode)
            {
                case (int)ErrorCodes.GeneralError:
                    errorDescription = "An error has occurred whilst trying to register this transaction.";
                    break;
                case (int)ErrorCodes.PaymentIdMissing:
                    errorDescription = "Payment Id is missing or not valid.";
                    break;
                case (int)ErrorCodes.NameResolutionFailure:
                    errorDescription = @"The server was unable to register this transaction with Sage Pay.
Check that you do not have a firewall restricting the POST and 
that your server can correctly resolve the address.";
                    break;
                case (int)ErrorCodes.MalformedPost:
                    errorDescription = "SagePay returned a Malformed status when posting data.";
                    break;
                case (int)ErrorCodes.InvalidPost:
                    errorDescription = "SagePay returned an Invalid status when posting data.";
                    break;
                case (int)ErrorCodes.GeneralPostError:
                    errorDescription = "SagePay returned an Error status when posting data.";
                    break;
                case (int)ErrorCodes.TransactionNotFound:
                    errorDescription = "We were unable to locate this Payment in CRM. The customer has NOT been charged.";
                    break;
                case (int)ErrorCodes.UnmatchedSignatures:
                    errorDescription = "There was a problem validating the result from our Payment Gateway<br/>To protect you we have cancelled the payment.";
                    break;
                case (int)ErrorCodes.Aborted:
                    errorDescription = "The user has pressed Cancel on the payment pages, or the transaction was timed out due to inactivity.<br/>Please try again.";
                    break;
                case (int)ErrorCodes.NotAuthorised:
                    errorDescription = "The transaction was not authorised by the bank.";
                    break;
                case (int)ErrorCodes.Rejected:
                    errorDescription = "The transaction was failed by your 3D-Secure or AVS/CV2 rule-bases.";
                    break;
                case (int)ErrorCodes.UnspecifiedPaymentError:
                    errorDescription = "An unknown status was returned from Sage Pay.";
                    break;
                case (int)ErrorCodes.PaymentError:
                    errorDescription = "There was an error during the payment process.<br/>Please try again.";
                    break;
                case (int)ErrorCodes.InvalidContactDetails:
                    errorDescription = "The customer does not have enough contact information to take a payment.<BR/>Please ensure the customer has a <i>First</i> and <i>Last</i> name specified, and address details that include <i>Address Line 1</i>, <i>City</i> and <i>Postal Code</i>.";
                    break;
                default:
                    errorDescription = "An unknown error has occurred.";
                    break;
            }

            ErrorDescription.Text = errorDescription;

            if (errorCode != (int)ErrorCodes.InvalidContactDetails && errorCode != (int)ErrorCodes.Aborted)
            {
                ErrorHelp.Text = "Please contact your system administrator.";
            }
        }
    }
}