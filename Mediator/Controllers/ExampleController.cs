using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using CRISP.WebClient.PatientMatch.Models;
using Mediator.Messages;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Mediator.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ExampleController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ExampleController(IMediator mediator)
        {
            _mediator = mediator;
        }


        [HttpGet, Route("[action]")]
        public async Task<MpiDemographics> Patient([FromQuery, Required] string eid)
        {
            // This simply runs a ask...
            await _mediator.Publish(ConsentEvent.CreateInstance(eid));
            Task.WaitAll();

            var patientMatchRequest = PatientMatchRequest.CreateInstance(eid);

            var mpiDemographics = _mediator.Send(patientMatchRequest);

            return await mpiDemographics;
        }
    }
}