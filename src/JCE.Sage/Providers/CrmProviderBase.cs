namespace JCE.Sage.Providers
{
    #region Using Statements
    using JCE.Sage;
    #endregion

    /// <summary>
    /// The base CRM provider class.
    /// </summary>
    public abstract class CrmProviderBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CrmProviderBase"/> class.
        /// </summary>
        /// <param name="crmConnector">The CRM connector.</param>
        protected CrmProviderBase(ICrmConnector crmConnector)
        {
            this.CrmConnector = crmConnector;
        }

        /// <summary>
        /// Gets or sets the CRM connector.
        /// </summary>
        /// <value>
        /// The CRM connector.
        /// </value>
        protected ICrmConnector CrmConnector
        {
            get;
            set;
        }
    }
}