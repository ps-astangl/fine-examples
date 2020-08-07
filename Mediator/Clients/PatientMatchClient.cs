using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using CRISP.Common.Models;
using CRISP.WebClient.PatientMatch.Consent;
using CRISP.WebClient.PatientMatch.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonConverter = System.Text.Json.Serialization.JsonConverter;

namespace Mediator.Clients
{
    public interface IPatientMatchClient
    {
        public Task<MpiDemographics> GetPatientDemographics(string eid);
        public Task<string> GetOptOut(string eid);
    }

    public class PatientMatchClient : IPatientMatchClient
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<PatientMatchClient> _logger;

        public PatientMatchClient(IHttpClientFactory httpClientFactory, ILogger<PatientMatchClient> logger)
        {
            _httpClientFactory = httpClientFactory;
            _logger = logger;
        }

        public async Task<MpiDemographics> GetPatientDemographics(string eid)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("PatientMatch");
                var request = new HttpRequestMessage(HttpMethod.Get, "/api/Demographics/" + eid + "?IncludeSMRNs=true");
                _logger.LogInformation($"Performing Request {request.RequestUri}");
                var response = await client.SendAsync(request);
                return JsonConvert.DeserializeObject<MpiDemographics>(await response.Content.ReadAsStringAsync());
            }
            catch (Exception exception)
            {
                return new MpiDemographics(exception.Message);
            }
        }

        public async Task<string> GetOptOut(string eid)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("PatientMatch");
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "/api/Consent/" + eid);
                HttpResponseMessage response = await client.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception exception)
            {
                _logger.LogWarning(exception, exception.Message);
                return "Error";
            }
        }
    }
}