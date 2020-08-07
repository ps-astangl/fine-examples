using System;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using Hl7.Fhir.Rest;
using Mediator.Configurations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mediator.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static void AddFhirClientConfiguration(this IServiceCollection serviceCollection,
            IConfiguration configuration)
        {
            serviceCollection.Configure<FhirClientConfiguration>(nameof(FhirClientConfiguration),
                configuration.GetSection(nameof(FhirClientConfiguration)));
        }

        /// <summary>
        /// Must be added to the service container AFTER <see cref="AddFhirClientConfiguration"/>
        /// </summary>
        public static void AddFhirClient(this IServiceCollection serviceCollection)
        {
            Regex appServiceEndpoint = new Regex(@"^[^.]+\.azure\.[^.]+\.local$");

            serviceCollection.AddScoped<IFhirClient>(provider =>
            {
                var fhirClientConfiguration = serviceCollection
                    .BuildServiceProvider().GetService<IOptions<FhirClientConfiguration>>().Value;

                var client = new FhirClient(fhirClientConfiguration.BaseAddress)
                {
                    PreferredReturn = Prefer.ReturnRepresentation,
                    PreferredFormat = ResourceFormat.Json,
                    Timeout = fhirClientConfiguration.TimeoutMilliseconds
                };

                client.OnBeforeRequest += OnBeforeRequest(fhirClientConfiguration);

                return client;
            });

            EventHandler<BeforeRequestEventArgs> OnBeforeRequest(FhirClientConfiguration fhirClientConfiguration)
            {
                return (sender, args) =>
                {
                    args.RawRequest.ServerCertificateValidationCallback =
                        (reqSender, certificate, chain, policyErrors) =>
                            policyErrors == SslPolicyErrors.None ||
                            appServiceEndpoint.IsMatch(args.RawRequest.RequestUri.Host);

                    args.RawRequest.ClientCertificates.Add(GetCertByThumbprint(fhirClientConfiguration?.Thumbprint));
                };
            }
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