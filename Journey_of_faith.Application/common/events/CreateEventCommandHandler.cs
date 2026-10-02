// using MediatR;

// namespace Journey_of_faith.Application.common.events;

// public class CreateEventCommand : IRequest<Guid>
// {
//     public Guid Id {get; set;}
//     public int ChurchId {get; set;}
//     public string Title {get; set;}
//     public string Body {get; set;}
// }

// public class CreateEventCommandHandler : IRequestHandler<CreateEventCommand, Guid>
// {
//     private readonly Mediator mediator;
//     public CreateEventCommandHandler(Mediator mediator)
//     {
//         this.mediator = mediator;
//     }
//     public async Task<Guid> Handle(CreateEventCommand command, CancellationToken ct)
//     {
//         await mediator.Publish(new EventCreatedEvent
//         {
//             ChurchId = command.ChurchId,
//             EventTitle = command.Title
//         });

//         return 
//     }
// }