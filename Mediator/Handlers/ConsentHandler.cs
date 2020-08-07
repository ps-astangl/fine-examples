using System.Threading;
using System.Threading.Tasks;
using Mediator.Clients;
using Mediator.Messages;
using MediatR;

namespace Mediator.Handlers
{
    public class ConsentHandler : INotificationHandler<ConsentEvent>
    {
        private readonly IPatientMatchClient _patientMatchClient;

        public ConsentHandler(IPatientMatchClient patientMatchClient)
        {
            _patientMatchClient = patientMatchClient;
        }

        public Task Handle(ConsentEvent notification, CancellationToken cancellationToken)
        {
            var optout =  _patientMatchClient.GetOptOut(notification.Eid);
            return Task.CompletedTask;
        }
    }
}