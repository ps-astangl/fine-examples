using MediatR;

namespace Mediator.Messages
{
    public class ConsentEvent : INotification
    {
        public string Eid { get; set; }
        public static ConsentEvent CreateInstance(string eid)
        {
            return new ConsentEvent { Eid = eid };
        }
    }
}