namespace JCE.Sage
{
    #region Using Statements
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Security;
    using System.Security.Cryptography.X509Certificates;
    using System.ServiceModel.Description;
    using JCE.Crm.Plugins.Entities;
    using JCE.Sage.Providers;

    using Microsoft.Xrm.Sdk;
    using Microsoft.Xrm.Sdk.Client;
    using Microsoft.Xrm.Sdk.Discovery;
    using Microsoft.Xrm.Tooling.Connector;
    #endregion

    public class CrmConnector : ICrmConnector
    {
        /// <summary>
        /// The cache of organization endpoints looked up using the discovery service.
        /// </summary>
        private static Dictionary<string, Uri> organizationEndpoints = new Dictionary<string, Uri>();

        /// <summary>
        /// The thread synchronization object - used to synchronize access to the organizationEndpoints dictionary.
        /// </summary>
        private static object syncObj = new object();

        /// <summary>
        /// The current discovery service.
        /// </summary>
        private DiscoveryServiceProxy discoveryService;

        /// <summary>
        /// The current organization service.
        /// </summary>
        private OrganizationServiceProxy organizationService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CrmConnector"/> class.
        /// </summary>
        /// <param name="configuration">The configuration provider.</param>
     


        public CrmConnector(string CrmServiceConnectionString)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            CrmServiceClient conn = new CrmServiceClient(CrmServiceConnectionString);
            OrganizationServiceProxy orgServiceProxy = conn.OrganizationServiceProxy;
            var _crmService = (IOrganizationService)orgServiceProxy;
            //_crmService = new OrganizationService(connection);
            var _crmServiceContext = new OrganizationServiceContext(_crmService);


            this.organizationService = orgServiceProxy;
            this.organizationService.EnableProxyTypes();
        }
    

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Creates a data context capable of communicating with the configured CRM organization.
        /// The DataContext returned should be disposed when it goes out of scope.
        /// </summary>
        /// <returns>
        /// The data context instance.
        /// </returns>
        public DataContext CreateDataContext()
        {
            return new DataContext(this.organizationService);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.discoveryService != null)
                {
                    this.discoveryService.Dispose();
                    this.discoveryService = null;
                }

                if (this.organizationService != null)
                {
                    this.organizationService.Dispose();
                    this.organizationService = null;
                }
            }
        }

        /// <summary>
        /// Creates the discovery service configuration.
        /// </summary>
        /// <param name="configuration">The configuration provider.</param>
        /// <returns>The constructed configuration instance.</returns>
        private static IServiceConfiguration<IDiscoveryService> CreateDiscoveryConfiguration(IConfigurationProvider configuration)
        {
            var discoveryUri = new Uri(configuration.CrmPublicAddress, "/XRMServices/2011/Discovery.svc");
            return ServiceConfigurationFactory.CreateConfiguration<IDiscoveryService>(discoveryUri);
        }

        /// <summary>
        /// A policy override for the web services that prevents invalid certificates from killing the connection.
        /// </summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="certificate">The certificate at the end of the connection.</param>
        /// <param name="chain">The certificate chain.</param>
        /// <param name="sslPolicyErrors">The SSL policy errors.</param>
        /// <returns>This method always returns <c>true</c>.</returns>
        private static bool AcceptAllCertificatePolicy(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        /// <summary>
        /// Creates the organization service configuration.
        /// </summary>
        /// <param name="configuration">The configuration provider.</param>
        /// <returns>The constructed configuration instance.</returns>
        private IServiceConfiguration<IOrganizationService> CreateServiceConfiguration(IConfigurationProvider configuration)
        {
            return ServiceConfigurationFactory.CreateConfiguration<IOrganizationService>(this.GetOrganizationAddress(configuration.CrmOrganization));
        }

        /// <summary>
        /// Configures the connector in IFD mode.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        private void ConfigureIfdConnection(IConfigurationProvider configuration)
        {
            var userCredentials = new ClientCredentials();
            userCredentials.UserName.UserName = configuration.CrmUserDomain + @"\" + configuration.CrmUserName;
            userCredentials.UserName.Password = configuration.CrmPassword;

            var discoveryConfiguration = CreateDiscoveryConfiguration(configuration);
            var userResponseWrapper = discoveryConfiguration.Authenticate(userCredentials);
            this.discoveryService = new DiscoveryServiceProxy(discoveryConfiguration, userResponseWrapper);

            var serviceConfiguration = this.CreateServiceConfiguration(configuration);
            this.organizationService = new OrganizationServiceProxy(serviceConfiguration, userResponseWrapper);
            this.organizationService.EnableProxyTypes();
        }

        /// <summary>
        /// Configures the connector in active directory mode.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        private void ConfigureActiveDirectoryConnection(IConfigurationProvider configuration)
        {
            var userCredentials = new ClientCredentials();
            userCredentials.Windows.ClientCredential.UserName = configuration.CrmUserName;
            userCredentials.Windows.ClientCredential.Domain = configuration.CrmUserDomain;
            userCredentials.Windows.ClientCredential.Password = configuration.CrmPassword;

            var discoveryConfiguration = CreateDiscoveryConfiguration(configuration);
            this.discoveryService = new DiscoveryServiceProxy(discoveryConfiguration, userCredentials);
            this.discoveryService.Authenticate();

            var serviceConfiguration = this.CreateServiceConfiguration(configuration);
            this.organizationService = new OrganizationServiceProxy(serviceConfiguration, userCredentials);
            this.organizationService.EnableProxyTypes();
            this.organizationService.Authenticate();
        }

        /// <summary>
        /// Gets the organization address for the given organization name. This uses the currently 
        /// configured discovery service.
        /// </summary>
        /// <param name="orgName">Name of the organization.</param>
        /// <returns>The organization's endpoint URI.</returns>
        private Uri GetOrganizationAddress(string orgName)
        {
            Uri endpoint;
            if (!organizationEndpoints.TryGetValue(orgName, out endpoint))
            {
                lock (syncObj)
                {
                    if (!organizationEndpoints.TryGetValue(orgName, out endpoint))
                    {
                        // Obtain information about the organizations that the system user belongs to.
                        var orgRequest = new RetrieveOrganizationsRequest();
                        var orgResponse = (RetrieveOrganizationsResponse)this.discoveryService.Execute(orgRequest);
                        var org = orgResponse.Details.FirstOrDefault(
                            o => o.UniqueName.Equals(orgName, StringComparison.OrdinalIgnoreCase));

                        if (org != null)
                        {
                            endpoint = new Uri(org.Endpoints[EndpointType.OrganizationService]);
                            organizationEndpoints[orgName] = endpoint;
                        }
                        else
                        {
                            throw new InvalidOperationException("Organization " + orgName +
                                                                "does not exist on the server.");
                        }
                    }
                }
            }

            return endpoint;
        }
    }
}
