using FluentValidation.Results;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.quiz;
using Journey_of_faith.Domain.interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Journey_of_faith.Application.usecases.quizs.commands
{
    public class SubmitExamHandler : IRequestHandler<SubmitExamCommand, SubmitResult>
    {
        private readonly IExamRepository _repo;
        private readonly IExamQueries _queries;
        private readonly ICurrentUserService _currentUser;
        public SubmitExamHandler(IExamRepository repo, IExamQueries queries, ICurrentUserService currentUserService)
        {
            _repo = repo;
            _queries = queries;
            _currentUser = currentUserService;
        }

        public async Task<SubmitResult> Handle(SubmitExamCommand command, CancellationToken token)
        {

            if(!Guid.TryParse(_currentUser.UserId, out Guid userId))
            {
                throw new UnauthorizationException("Người dùng không hợp lệ, không thể thực hiện chấm bài");
            }
            var quiz = await _queries.GetQuizDetailsAsync(command.QuizId, token);

            if(quiz is null)
            {
                throw new NotFoundException("Đề thi không hợp lệ");
            }

            int correctAnswer = 0;
            int wrongAnswer = 0;
            foreach(var quesiton in quiz.Questions)
            {
                if(command.QuestionAnswer.TryGetValue(quesiton.Id, out var answer))
                {
                    var dataCorrect = quesiton.Answers.FirstOrDefault(e => e.IsCorrect);

                    if(dataCorrect is not null && answer == dataCorrect.Id)
                    {
                        correctAnswer++;
                    } else
                    {
                        wrongAnswer++;
                    }
                }
            }

            var totalCount = (double)correctAnswer / quiz.QuestionCount * 10;
            
            var quizAttempt = QuizAttempt.Create(quiz.Id, userId, DateTime.UtcNow, DateTime.UtcNow, (int)totalCount);

            

            foreach(var quesiton in quiz.Questions)
            {
                if(command.QuestionAnswer.TryGetValue(quesiton.Id, out var answer))
                {
                    var correctAnswr = quesiton.Answers.FirstOrDefault(e => e.IsCorrect);

                    bool userAnswer = correctAnswr is not null && correctAnswr.Id == answer;

                    quizAttempt.AddAttemptAnswer(quizAttempt.Id, quesiton.Id, answer, userAnswer);
                }
            }

            await _repo.SaveScoreTest(quizAttempt);
            return new SubmitResult
            {
                CorrectQuestionCount = correctAnswer,
                FailQuestionCount = wrongAnswer,
                Score = totalCount,
                Message = "Hoàn thành bài thi."
            };

        }
    }
}
