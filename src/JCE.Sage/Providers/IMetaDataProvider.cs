namespace JCE.Sage.Providers
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using JCE.Crm.Plugins.Entities;
    using Microsoft.Xrm.Sdk.Metadata;
    #endregion

    /// <summary>
    /// The interface implmented by Metadata Providers
    /// </summary>
    public interface IMetaDataProvider
    {
        /// <summary>
        /// Retrieves the option set meta data collection.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>A list of option metadata</returns>
        OptionMetadataCollection RetrieveOptionSetMetaDataCollection(string entityName, string attributeName);
    }
}
