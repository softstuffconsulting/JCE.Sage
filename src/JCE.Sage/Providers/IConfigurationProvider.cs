namespace JCE.Sage.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// The configuration provider interface.
    /// </summary>
    public interface IConfigurationProvider
    {
        /// <summary>
        /// Gets the name of the CRM server.
        /// </summary>
        /// <value>
        /// The CRM server name.
        /// </value>
        Uri CrmPublicAddress { get; }

        /// <summary>
        /// Gets the name of the CRM organization.
        /// </summary>
        /// <value>
        /// The CRM organization name.
        /// </value>
        string CrmOrganization { get; }

        /// <summary>
        /// Gets the name of the user to use to access CRM.
        /// </summary>
        /// <value>
        /// The CRM user name.
        /// </value>
        string CrmUserName { get; }

        /// <summary>
        /// Gets the domain of the user to use to access CRM.
        /// </summary>
        /// <value>
        /// The CRM user domain.
        /// </value>
        string CrmUserDomain { get; }

        /// <summary>
        /// Gets the password of the user to use to access CRM.
        /// </summary>
        /// <value>
        /// The CRM password.
        /// </value>
        string CrmPassword { get; }

        /// <summary>
        /// Gets a value indicating whether the CRM server is operating in IFD mode.
        /// </summary>
        /// <value>
        ///   <c>true</c> if CRM is operating in IFD mode; otherwise, <c>false</c>.
        /// </value>
        bool CrmIfdMode { get; }

        /// <summary>
        /// Gets the sage pay web service address.
        /// </summary>
        string SagePayWebServiceAddress {get;}

        /// <summary>
        /// Gets the order failed URL.
        /// </summary>
        string OrderFailedUrl { get; }

        /// <summary>
        /// Gets the order successful URL.
        /// </summary>
        string OrderSuccessfulUrl { get; }

        /// <summary>
        /// Gets the name of the vendor.
        /// </summary>
        /// <value>
        /// The name of the vendor.
        /// </value>
        string VendorName {get;}

        /// <summary>
        /// Gets the notification URL.
        /// </summary>
        string NotificationUrl { get; }
    }
}
