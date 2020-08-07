using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using CRISP.Extensions.DependencyInjection;
using CRISP.Telemetry.Clients;
using Microsoft.Extensions.Configuration;

namespace Mediator.Configurations
{
    public static class ClientConfigurations
    {
        public static IEnumerable<HttpClientConfiguration> ConfigureNamedClient(IConfiguration configuration)
        {
            List<ClientConfiguration> clientConfiguration = configuration
                .GetSection(nameof(ClientConfigurations))
                .Get<List<ClientConfiguration>>();

            return clientConfiguration.Select(config => new HttpClientConfiguration(config.EndpointName,
                GetCertByThumbprint(config.CertificateThumbprint), new Uri(config.BaseAddress)));
        }
        private static X509Certificate2 GetCertByThumbprint(string thumbprint)
        {
            X509Store x509Store = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            x509Store.Open(OpenFlags.ReadOnly);
            X509Certificate2Collection certificate2Collection =
                x509Store.Certificates.Find(X509FindType.FindByThumbprint, thumbprint, false);
            return certificate2Collection.Count > 0 ? certificate2Collection[0] : null;
        }
    }
}