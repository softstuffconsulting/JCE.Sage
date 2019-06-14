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
    /// The implementation of the Contact Provider
    /// </summary>
    public class ContactProvider : CrmProviderBase, IContactProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContactProvider"/> class.
        /// </summary>
        /// <param name="crmConnector">The CRM connector.</param>
        public ContactProvider(ICrmConnector crmConnector)
            : base(crmConnector)
        {
        }

        /// <summary>
        /// Gets the contact.
        /// </summary>
        /// <param name="contactId">The contact id.</param>
        /// <returns>
        /// A contact record
        /// </returns>
        public Contact GetContact(Guid contactId)
        {
            using (var context = this.CrmConnector.CreateDataContext())
            {
                return context.ContactSet.FirstOrDefault(c => c.Id == contactId);
            }
        }
    }
}
