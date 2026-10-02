// using Journey_of_faith.Application.exceptions;
// using Journey_of_faith.Domain.interfaces;
// using MediatR;
// using System;
// using System.Collections.Generic;
// using System.Text;

// namespace Journey_of_faith.Application.usecases.questions.commands
// {
//     public class DeleteQuestionHandler : IRequestHandler<DeleteQuestionCommand, bool>
//     {
//         private readonly IQuestionRepository _repo;
//         public DeleteQuestionHandler(IQuestionRepository repo)
//         {
//             _repo = repo; 
//         }

//         public async Task<bool> Handle(DeleteQuestionCommand command, CancellationToken cancellationToken)
//         {

//             if(await _repo.GetDetailsQuestion(command.Id) is null)
//             {
//                 throw new NotFoundException("Không tìm thấy câu hỏi.");
//             }
//             return await _repo.DeleteQuestion(command.Id);
//         }
//     }
// }
