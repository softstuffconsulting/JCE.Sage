using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace JCE.Sage.Providers
{
    /// <summary>
    /// The list of possible status codes
    /// </summary>
    public enum StatusCodes
    {
        /// <summary>
        /// The Status Code is unspecified.
        /// </summary>
        Unspecified = 0,

        /// <summary>
        /// The Status Code is "Ok".
        /// </summary>
        Ok = 1,

        /// <summary>
        /// The Status Code is "NOTAUTHED".
        /// </summary>
        NotAuthed = 2,

        /// <summary>
        /// The Status Code is "Abort".
        /// </summary>
        Abort = 3,

        /// <summary>
        /// The Status Code is "Rejected".
        /// </summary>
        Rejected = 4,

        /// <summary>
        /// The Status Code is "Authenticated".
        /// </summary>
        Authenticated = 5,

        /// <summary>
        /// The Status Code is "Registered".
        /// </summary>
        Registered = 6,

        /// <summary>
        /// The Status Code is "Error".
        /// </summary>
        Error = 7,
    }
}