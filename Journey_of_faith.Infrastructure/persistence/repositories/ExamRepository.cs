using Dapper;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Domain.entities.quiz;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.dtos.quiz;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Infrastructure.repositories
{
    public class ExamRepository(IDbConnectionFactory _factory, IOptions<TableSchemaName> _name) : IExamRepository
    {
        private readonly TableSchemaName name = _name.Value;
        public async Task<int> CreateQuiz(Quiz quiz, int HardQuestion, int MediumQuestion, int EasyQuestion)
        {
            using var connection = _factory.CreateConnection();
            connection.Open();

            using var transaction = connection.BeginTransaction();
            try
            {
                var Id = await connection.ExecuteScalarAsync<int>("CreateQuiz", new
                {
                    quiz.Title,
                    quiz.Description,
                    quiz.TimeLimit,
                    quiz.QuestionCount,
                    HardQuestion,
                    MediumQuestion,
                    EasyQuestion
                },
                transaction,
                commandType: System.Data.CommandType.StoredProcedure);

                transaction.Commit();
                return Id;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }
        }

        public async Task<long> SaveScoreTest(QuizAttempt quizAttempt)
        {
            using var connection = _factory.CreateConnection();
            connection.Open();
            using var transaction = connection.BeginTransaction();
            try
            {
                var quizAttemp = await connection
                .ExecuteScalarAsync<long>($@"
                    Insert into [{name.Schema}].[{QuizTalbe.QuizAttempt}] (QuizId, UserId, StartTime, EndTime, Score)
                    Output inserted.Id
                    VALUES(@QuizId, @UserId, @StartTime, @EndTime, @Score)
                ", new CreateQuizAttempt
                {
                    QuizId = quizAttempt.QuizId,
                    UserId = quizAttempt.UserId.ToString(),
                    StartTime = quizAttempt.StartTime,
                    EndTime = quizAttempt.EndTime,
                    Score = quizAttempt.Score,
                }, transaction: transaction);

                foreach (var attemptAnswer in quizAttempt.AttemptAnswers)
                {
                    await connection.ExecuteAsync($@"
                        Insert into [{name.Schema}].[{QuizTalbe.AttemptAnswer}] (AttemptId, QuestionId, AnswerId, IsCorrect)
                        Values(@AttemptId, @QuestionId, @AnswerId, @IsCorrect)
                    ", new CreateAttemptAnswer
                    {
                        AttemptId = quizAttemp,
                        QuestionId = attemptAnswer.QuestionId,
                        AnswerId = attemptAnswer.AnswerId,
                        IsCorrect = attemptAnswer.IsCorrect
                    }, transaction);
                }

                transaction.Commit();
                return quizAttemp;
            }
            catch (Exception ex) when (ex is SqlException)
            {
                transaction.Rollback();
                throw;
            }
        }
        public async Task<bool> DeleteQuiz(int Id)
        {
            using var connection = _factory.CreateConnection();
            var isDelete = await connection.ExecuteAsync($@"
                UPDATE [{name.Schema}].[{QuizTalbe.Quiz}] SET
                IsDeleted = @IsDeleted, DeletedAt = @DeletedAt
                Where Id = @Id and IsDeleted = 0
            ", new { IsDeleted = true, DeletedAt = DateTime.Now, Id = Id });

            return isDelete > 0;
        }

        #region Topic
        public async Task<int> CreateTopicAsync(Topic topic)
        {
            using var connection = _factory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>(@$"
                insert into [{name.Schema}].[{QuizTalbe.Topic}] (TopicName, QuizCount, CreationTime)
                values (@TopicName, @QuizCount, Getdate())
            ", new { TopicName = topic.TopicName, QuizCount = topic.QuizCount });
        }


        public async Task<int> DeleteTopicAsync(int id)
        {
            using var connection = _factory.CreateConnection();
            return await connection.ExecuteScalarAsync<int>($@"
                UPDATE [{name.Schema}].[{QuizTalbe.Topic}] Set
                    DeletedAt = GETDATE(),
                    IsDeleted = 1
                Where Id = @id
            ", new { id = id });
        }

        #endregion
    }
    public static class QuizTalbe
    {
        public const string Quiz = "Quiz";
        public const string QuizQuestion = "QuizQuestion";
        public const string QuizAttempt = "QuizAttempt";
        public const string AttemptAnswer = "AttemptAnswer";
        public const string Topic = "Topic";
    }

}


