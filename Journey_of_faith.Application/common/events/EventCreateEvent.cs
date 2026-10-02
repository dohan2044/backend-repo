using MediatR;

namespace Journey_of_faith.Application.common.events;

public class EventCreatedEvent : INotification
{
    public int ChurchId {get; set;}
    public string EventTitle {get; set;}
}