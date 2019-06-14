namespace JCE.Sage.Providers
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using JCE.Crm.Plugins.Entities;
    using JCE.Sage.Providers;
    using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Metadata;
using Microsoft.Xrm.Sdk.Messages;
    #endregion

    /// <summary>
    /// The implementation of the Metadata Provider
    /// </summary>
    public class MetaDataProvider : CrmProviderBase, IMetaDataProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ContactProvider"/> class.
        /// </summary>
        /// <param name="crmConnector">The CRM connector.</param>
        public MetaDataProvider(ICrmConnector crmConnector)
            : base(crmConnector)
        {
        }

        /// <summary>
        /// Retrieves the option set meta data collection.
        /// </summary>
        /// <param name="entityName">Name of the entity.</param>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns>
        /// A list of option metadata
        /// </returns>
        public OptionMetadataCollection RetrieveOptionSetMetaDataCollection(string entityName, string attributeName)
        {
            using (var context = this.CrmConnector.CreateDataContext())
            {
                OptionMetadataCollection optionMetaData = null;
                RetrieveEntityRequest retrieveEntityRequest = new RetrieveEntityRequest();
                RetrieveEntityResponse retrieveEntityResponse = new RetrieveEntityResponse();

                retrieveEntityRequest.LogicalName = entityName;
                retrieveEntityRequest.EntityFilters = Microsoft.Xrm.Sdk.Metadata.EntityFilters.Attributes;
                retrieveEntityResponse = (RetrieveEntityResponse)context.Execute(retrieveEntityRequest);
                foreach (AttributeMetadata attributeMetaData in retrieveEntityResponse.EntityMetadata.Attributes)
                {
                    if (attributeMetaData.AttributeType == AttributeTypeCode.Picklist && attributeMetaData.LogicalName == attributeName)
                    {
                        optionMetaData = ((PicklistAttributeMetadata)attributeMetaData).OptionSet.Options;
                        break;
                    }
                }

                return optionMetaData;
            }
        }
    }
}
