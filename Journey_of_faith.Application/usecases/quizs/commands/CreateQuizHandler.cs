using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.quiz;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Application.usecases.questions;
using MediatR;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace Journey_of_faith.Application.usecases.quizs.commands
{
    public class CreateQuizHandler : IRequestHandler<CreateQuizCommand, int>
    {
        private readonly IExamRepository _examRepository;
        private readonly IQuestionQueries _questionQueries;
        public CreateQuizHandler(IExamRepository examRepository, IQuestionQueries questionQueries)
        {
            _examRepository = examRepository;
            _questionQueries = questionQueries;
        }

        public async Task<int> Handle(CreateQuizCommand command, CancellationToken token)
        {
            if(await _questionQueries.GetQuestionCountAsync(token) < command.QuestionCount)
            {
                throw new BadRequestException("Không đủ số câu hỏi để tạo đề thi");
            }
            var quiz = Quiz.Create(command.Title, command.Description, command.TimeLimit, command.QuestionCount);
            var result = await _examRepository.CreateQuiz(quiz, command.HardQuestion, command.MediumQuestion, EasyQuestion: command.EasyQuestion);
            return result;
        }

    }
}
