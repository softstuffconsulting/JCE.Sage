namespace JCE.Sage.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Configuration;

    public class ConfigurationProvider : IConfigurationProvider
    {
        public Uri CrmPublicAddress
        {
            get { return new Uri(ConfigurationManager.AppSettings["CrmPublicAddress"]); }
        }

        public string CrmOrganization
        {
            get { return ConfigurationManager.AppSettings["CrmOrganization"]; }
        }

        public string CrmUserName
        {
            get { return ConfigurationManager.AppSettings["CrmUserName"]; }
        }

        public string CrmUserDomain
        {
            get { return ConfigurationManager.AppSettings["CrmUserDomain"]; }
        }

        public string CrmPassword
        {
            get { return ConfigurationManager.AppSettings["CrmPassword"]; }
        }

        public bool CrmIfdMode
        {
            get
            {
                var value = ConfigurationManager.AppSettings["CrmIfdMode"];
                bool convertedValue;
                if (Boolean.TryParse(value, out convertedValue))
                {
                    return convertedValue;
                }

                return false;
            }
        }

        /// <summary>
        /// Gets the name of the vendor.
        /// </summary>
        /// <value>
        /// The name of the vendor.
        /// </value>
        public string VendorName
        {
            get { return ConfigurationManager.AppSettings["VendorName"]; }
        }

        /// <summary>
        /// Gets the sage pay web service address.
        /// </summary>
        public string SagePayWebServiceAddress
        {
            get { return ConfigurationManager.AppSettings["SagePayWebServiceAddress"]; }
        }

        /// <summary>
        /// Gets the order failed URL.
        /// </summary>
        public string OrderFailedUrl
        {
            get { return ConfigurationManager.AppSettings["OrderFailedUrl"]; }
        }

        /// <summary>
        /// Gets the order successful URL.
        /// </summary>
        public string OrderSuccessfulUrl
        {
            get { return ConfigurationManager.AppSettings["OrderSuccessfulUrl"]; }
        }

        /// <summary>
        /// Gets the notification URL.
        /// </summary>
        public string NotificationUrl
        {
            get { return ConfigurationManager.AppSettings["NotificationUrl"]; }
        }
    }
}
