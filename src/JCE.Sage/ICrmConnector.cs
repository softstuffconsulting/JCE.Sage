namespace JCE.Sage
{
    #region Using Statements
    using System;
    using JCE.Crm.Plugins.Entities;
    using Microsoft.Xrm.Sdk.Client;
    #endregion

    /// <summary>
    /// The interface implemented by classes capable of connecting and communicating with 
    /// CRM 2011 instances.
    /// </summary>
    public interface ICrmConnector : IDisposable
    {
        /// <summary>
        /// Creates a data context capable of communicating with the configured CRM organization.
        /// The DataContext returned should be disposed when it goes out of scope.
        /// </summary>
        /// <returns>The data context instance.</returns>
      DataContext CreateDataContext();
    }
}
