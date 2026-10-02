using Journey_of_faith.Domain.interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.quizs.commands
{
    public class SubmitExamCommand : IRequest<SubmitResult>
    {
        public int QuizId { get; set; }
        public Dictionary<int, int> QuestionAnswer { get; set; } = new();
    }
}
