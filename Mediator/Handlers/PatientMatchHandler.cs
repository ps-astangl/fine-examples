using System;
using System.Threading;
using System.Threading.Tasks;
using CRISP.WebClient.PatientMatch;
using CRISP.WebClient.PatientMatch.Consent;
using CRISP.WebClient.PatientMatch.Models;
using Mediator.Clients;
using Mediator.Messages;
using MediatR;

namespace Mediator.Handlers
{
    /// <summary>
    /// The handler takes care of the response and request. Dependencies (services) can be placed at the level of the
    /// handler
    /// </summary>
    public class PatientMatchHandler : IRequestHandler<PatientMatchRequest, MpiDemographics>
    {
        private readonly IPatientMatchClient _patientMatchClient;
        public PatientMatchHandler(IPatientMatchClient patientMatchClient)
        {
            _patientMatchClient = patientMatchClient;
        }

        // Handles the call to patient match for the demographics
        public Task<MpiDemographics> Handle(PatientMatchRequest patientMatchRequest, CancellationToken cancellationToken)
        {
            try
            {
                var mpiDemos = _patientMatchClient.GetPatientDemographics(patientMatchRequest.Eid);
                return mpiDemos;
            }
            catch (Exception exception)
            {
                return Task.FromResult(new MpiDemographics(exception.Message));
            }
        }
    }
}