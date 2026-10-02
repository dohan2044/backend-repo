// using Journey_of_faith.Application.common.dtos;
// using Journey_of_faith.Domain.interfaces;
// using MediatR;
// using System;
// using System.Collections.Generic;
// using System.Text;

// namespace Journey_of_faith.Application.usecases.questions.queries
// {
//     public class GetDetailsQuestionQuery : IRequest<QuestionView?> 
//     {
//         public int Id { get; set; }
//     }


//     public class GetDetailsQuestionHandler : IRequestHandler<GetDetailsQuestionQuery, QuestionView?>
//     {
//         private readonly IQuestionRepository _repo;
//         public GetDetailsQuestionHandler(IQuestionRepository repo)
//         {
//             _repo = repo; 
//         }

//         public async Task<QuestionView?> Handle(GetDetailsQuestionQuery query, CancellationToken token)
//         {
//             return await _repo.GetDetailsQuestion(query.Id);
//         }
//     }
// }
