namespace JCE.Sage.Providers
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using JCE.Crm.Plugins.Entities;
    using JCE.Sage.Providers;
    #endregion

    /// <summary>
    /// The implementation of the Payment Provider
    /// </summary>
    public class PaymentProvider : CrmProviderBase, IPaymentProvider
    {
        /// <summary>
        /// The various types of Payment Status
        /// </summary>
        public enum PaymentStatus
        {
            /// <summary>
            /// The Payment Status is unspecified.
            /// </summary>
            Unspecified = 0,

            /// <summary>
            /// The "Unpaid" Payment Status.
            /// </summary>
            Unpaid = 863600000,

            /// <summary>
            /// The "Successful" Payment Status.
            /// </summary>
            Successful = 863600001,

            /// <summary>
            /// The "Failed" Payment Status.
            /// </summary>
            Failed = 863600002,

            /// <summary>
            /// The "Declined" Payment Status.
            /// </summary>
            Declined = 863600003,

            /// <summary>
            /// The "Rejected" Payment Status.
            /// </summary>
            Rejected = 863600004,
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContactProvider"/> class.
        /// </summary>
        /// <param name="crmConnector">The CRM connector.</param>
        public PaymentProvider(ICrmConnector crmConnector)
            : base(crmConnector)
        {
        }

        /// <summary>
        /// Gets the payment.
        /// </summary>
        /// <param name="paymentId">The payment id.</param>
        /// <returns>
        /// A payment record.
        /// </returns>
        public lss_payment GetPayment(Guid paymentId)
        {
            using (var context = this.CrmConnector.CreateDataContext())
            {
                return context.lss_paymentSet.FirstOrDefault(p => p.Id == paymentId);
            }
        }

        /// <summary>
        /// Gets the payment.
        /// </summary>
        /// <param name="lss_vendorTxCode">The lss_vendor tx code.</param>
        /// <param name="vpstxId">The VPSTX id.</param>
        /// <returns>A payment record.</returns>
        public lss_payment GetPayment(string lss_vendorTxCode, string vpstxId)
        {
            using (var context = this.CrmConnector.CreateDataContext())
            {
                return context.lss_paymentSet.FirstOrDefault(p => p.lss_vendortxcode == lss_vendorTxCode && p.lss_vpstxid == vpstxId);
            }
        }

        /// <summary>
        /// Saves the payment.
        /// </summary>
        /// <param name="payment">The payment.</param>
        public void SavePayment(lss_payment payment)
        {
            using (var context = this.CrmConnector.CreateDataContext())
            {
                context.Attach(payment);
                context.UpdateObject(payment);
                context.SaveChanges();
            }
        }
    }
}
