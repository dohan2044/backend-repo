using Journey_of_faith.Domain.interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.quizs.commands
{
    public class DeleteQuizHandler : IRequestHandler<DeleteQuizCommand, bool>
    {
        private readonly IExamRepository _repo;
        public DeleteQuizHandler(IExamRepository repo) { _repo = repo; }

        public async Task<bool> Handle(DeleteQuizCommand command, CancellationToken token)
        {
            return await _repo.DeleteQuiz(command.Id);
        }
    }
}
