// using Journey_of_faith.Application.exceptions;
// using Journey_of_faith.Domain.entities.quiz;
// using Journey_of_faith.Domain.interfaces;
// using MediatR;
// using System;
// using System.Collections.Generic;
// using System.Text;

// namespace Journey_of_faith.Application.usecases.questions.commands
// {
//     public class UpdateQuestionHandler : IRequestHandler<UpdateQuestionCommand, Unit>
//     {
//         private readonly IQuestionRepository _repo;
//         public UpdateQuestionHandler(IQuestionRepository repo)
//         {
//             _repo = repo;
//         }

//         public async Task<Unit> Handle(UpdateQuestionCommand command, CancellationToken token)
//         {
//             if(await _repo.GetDetailsQuestion(command.Id) is null)
//             {
//                 throw new NotFoundException("Không tìm thấy câu hỏi.");
//             }
//             var question = Question.Update(command.LevelId, command.QuestionContent, command.TypeId, command.CategoryId, command.ImageUrl, command.Id);

//             foreach(var answer in command.Answers)
//             {
//                 var ansert = Answer.Update(answer.Content, answer.IsCorrect, answer.ImageUrl, answer.Explanation);
//                 question.UpdateAnswer(ansert);
//             }
//             await _repo.UpdateQuestion(question);

//             return Unit.Value;

//         }
//     }
// }
