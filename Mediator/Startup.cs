using System.Linq;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using CRISP.Common.Certificates;
using CRISP.Extensions.DependencyInjection;
using CRISP.Providers.Models.EnterpriseData;
using CRISP.Telemetry.Base;
using CRISP.Telemetry.Clients;
using CRISP.WebClient.PatientMatch;
using CRISP.WebClient.PatientMatch.Consent;
using Mediator.Clients;
using Mediator.Configurations;
using Mediator.Extensions;
using Mediator.Formatters;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;


namespace Mediator
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    options.OutputFormatters.Insert(0, new FhirJsonOutputFormatter());
                    options.EnableEndpointRouting = false;
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_3_0)
                .AddJsonOptions(opt => opt.JsonSerializerOptions.IgnoreNullValues = true);

            services.AddControllers();


            // Add Fhir Client Configuration and FHIR Client
            services.AddFhirClientConfiguration(_configuration);
            services.AddFhirClient();

            // Add PatientMatch, Consent, and Enterprise data
            // var configs = ClientConfigurations.ConfigureNamedClient(_configuration);
            services.AddNamedHttpClients(_configuration, new CertProvider());
            services.AddTransient<IPatientMatchClient, PatientMatchClient>();
            // services.AddSingleton<IProvideAuthorization, AuthorizationProvider>();

            // Add Mediator
            services.AddMediatR(Assembly.GetExecutingAssembly());
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseMvc();
            app.UseDeveloperExceptionPage();
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