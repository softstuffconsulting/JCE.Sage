namespace JCE.Sage.Providers
{
    using JCE.Crm.Plugins.Entities;
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    #endregion

    /// <summary>
    /// The interface implmented by Contact Providers
    /// </summary>
    public interface IContactProvider
    {
        /// <summary>
        /// Gets the contact.
        /// </summary>
        /// <param name="contactId">The contact id.</param>
        /// <returns>A contact record</returns>
        Contact GetContact(Guid contactId);
    }
}
