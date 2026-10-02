using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.quizs.commands
{
    public class DeleteQuizCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
