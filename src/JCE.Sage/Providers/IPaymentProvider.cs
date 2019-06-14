namespace JCE.Sage.Providers
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using JCE.Crm.Plugins.Entities;
    #endregion

    /// <summary>
    /// The interface implmented by Payment Providers
    /// </summary>
    public interface IPaymentProvider
    {
        /// <summary>
        /// Gets the payment.
        /// </summary>
        /// <param name="paymentId">The payment id.</param>
        /// <returns>
        /// A payment record.
        /// </returns>
        lss_payment GetPayment(Guid paymentId);

        /// <summary>
        /// Gets the payment.
        /// </summary>
        /// <param name="vendorTransactionCode">The vendor transaction code.</param>
        /// <param name="vpstxId">The VPSTX id.</param>
        /// <returns>A payment record.</returns>
        lss_payment GetPayment(string lss_vendorTxCode, string vpstxId);

        /// <summary>
        /// Saves the payment.
        /// </summary>
        /// <param name="payment">The payment.</param>
        void SavePayment(lss_payment payment);
    }
}
