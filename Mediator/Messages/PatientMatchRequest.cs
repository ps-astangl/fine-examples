using CRISP.WebClient.PatientMatch.Models;
using MediatR;

namespace Mediator.Messages
{
    public class PatientMatchRequest : IRequest<MpiDemographics>
    {
        public string Eid { get; set; }
        public static PatientMatchRequest CreateInstance(string eid)
        {
            return new PatientMatchRequest { Eid = eid };
        }
    }
}