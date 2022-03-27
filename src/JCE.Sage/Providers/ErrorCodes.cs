using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JCE.Sage.Providers
{
    /// <summary>
    /// The list of possible error codes
    /// </summary>
    public enum ErrorCodes
    {
        /// <summary>
        /// The Error Code is unspecified.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// The Error Code is "Transaction not found"
        /// </summary>
        TransactionNotFound = 1,

        /// <summary>
        /// The Error Code is "Unmatched Signatures"
        /// </summary>
        UnmatchedSignatures = 2,

        /// <summary>
        /// The Error Code is "NameResolutionFailure"
        /// </summary>
        NameResolutionFailure = 3,

        /// <summary>
        /// The Error Code is "GeneralError"
        /// </summary>
        GeneralError = 4,

        /// <summary>
        /// The Error Code is "PaymentIdMissing"
        /// </summary>
        PaymentIdMissing = 5,

        /// <summary>
        /// The Error Code is "Malformed"
        /// </summary>
        MalformedPost = 6,

        /// <summary>
        /// The Error Code is "Invalid"
        /// </summary>
        InvalidPost = 7,

        /// <summary>
        /// The Error Code is "GeneralPostError"
        /// </summary>
        GeneralPostError = 8,

        /// <summary>
        /// The Error Code is "NotAuthorised"
        /// </summary>
        NotAuthorised = 9,

        /// <summary>
        /// The Error Code is "Aborted"
        /// </summary>
        Aborted = 10,

        /// <summary>
        /// The Error Code is "Rejected"
        /// </summary>
        Rejected = 11,

        /// <summary>
        /// The Error Code is "PaymentError"
        /// </summary>
        PaymentError = 12,

        /// <summary>
        /// The Error Code is "UnspecifiedPaymentError"
        /// </summary>
        UnspecifiedPaymentError = 13,

        /// <summary>
        /// The Error Code is "InvalidContactDetails"
        /// </summary>
        InvalidContactDetails = 14,


        ErrorWithCRM=15
    }
}