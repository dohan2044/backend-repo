using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.questions.commands
{
    public class DeleteQuestionCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }
}
