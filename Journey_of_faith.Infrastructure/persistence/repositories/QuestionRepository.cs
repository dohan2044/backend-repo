using Dapper;
using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Domain.entities.quiz;
using Journey_of_faith.Domain.interfaces;
using Journey_of_faith.Infrastructure.common;
using Journey_of_faith.Infrastructure.context;
using Journey_of_faith.Infrastructure.services;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Journey_of_faith.Infrastructure.repositories
{
    public record InsertQuestionDto(int LevelId, string QuestionContent, int? TypeId, int? CategoryId, string? ImageUrl);
    public record InsertAnswerDto(int questionId, string Content, bool isCorrect, string? ImageUrl, string? Explanation);
    public sealed class QuestionRepository : DataHandlerRequest, IQuestionRepository
    {

        public QuestionRepository(IDbConnectionFactory factory, IOptions<TableSchemaName> _options) : base(factory, _options)
        {
        }
        #region another method with table relationship
        // tao moi tao level bai thi moi
        public async Task<bool> CreateQuizLevel(QuizLevel quizLevel)
        {
            return await InsertOnlyName(TableQuestion.QuizLevel, new { Name = quizLevel.Name, Code = quizLevel.Code, Score = quizLevel.Score });
        }
        // tao kieu cau hoi moi
        public async Task<bool> CreateQuestionType(QuestionType questionType)
        {
            return await InsertOnlyName(TableQuestion.QuestionType, new { Name = questionType.Name, Code = questionType.Code, Description = questionType.Description });
        }
        // tao danh muc cau hoi moi
        public async Task<bool> CreateQuestionCategory(QuestionCategory questionCategory)
        {
            return await InsertOnlyName(TableQuestion.QuestionCategory, new { Name = questionCategory.Name, Code = questionCategory.Code, Description = questionCategory.Description });
        }
        #endregion
        #region questions
        public async Task<bool> InsertBulkQuestionAsync(string jsonValue)
        {
            using(var connection = _factory.CreateConnection())
            {
                var parameters = new { JsonData = jsonValue };
                await connection.ExecuteAsync(
                    "sp_UploadQuestionsFromJson",
                    parameters,
                    commandType: CommandType.StoredProcedure
                );

                return true;
            }
        }

        public async Task<bool> InsertMultipleCategories(string valuesInsert)
        {
            using(var connection = _factory.CreateConnection())
            {
                var parameters = new { DataJson = valuesInsert };  
                await connection.ExecuteAsync(
                    "spInsertMultipleCategory", 
                    parameters,
                    commandType: CommandType.StoredProcedure
                );
                return true;
            }
        }
        public async Task<bool> CreateQuestionAsync(Question question)
        {
            try
            {
                using var connection = _factory.CreateConnection();
                DataTable answers = new DataTable();
                answers.Columns.Add("Content", typeof(string));
                answers.Columns.Add("IsCorrect", typeof(bool));
                answers.Columns.Add("ImageUrl", typeof(string));
                answers.Columns.Add("Explanation", typeof(string));


                foreach (var answer in question.Answers)
                {
                    answers.Rows.Add(answer.Content, answer.IsCorrect, answer.ImageUrl, answer.Explanation);
                }


                var parameters = new DynamicParameters();
                parameters.Add("@LevelId", question.LevelId);
                parameters.Add("@QuestionContent", question.QuestionContent);
                parameters.Add("@TypeId", question.TypeId);
                parameters.Add("@CategoryId", question.CategoryId);
                parameters.Add("@ImageUrl", question.ImageUrl);
                parameters.Add("@Answers", answers.AsTableValuedParameter("[dbo].[QuestionAnswerType]"));


                var result = await connection.ExecuteAsync("spCreateQuestionWithAnswert", parameters, commandType: CommandType.StoredProcedure);
                return result > 0;
            }
            catch (SqlException ex)
            {
                throw new InvalidOperationException("An error occurred while creating the question.", ex);
            }
        }
        // public async Task<Question?> GetDetailsQuestion(int id)
        // {
        //     using var connection = _connection.CreateConnection();
        //     var sql = @"
        //                 SELECT * FROM [jcodepro_journey_of_faith].Question WHERE Id = @Id;
        //                 SELECT * FROM Answer WHERE QuestionId = @Id";
        //     using (var multiLine = await connection.QueryMultipleAsync(sql, new { Id = id }))
        //     {
        //         var question = await multiLine.ReadSingleOrDefaultAsync<QuestionView>();
        //         if(question is not null)
        //         {
        //             var answer = (await multiLine.ReadAsync<AnswerView>()).ToList();

        //             question?.Answers = answer.ToList();
        //         }

        //         return question;
        //     }
        // }

        public async Task<bool> UpdateQuestion(Question question)
        {
            using var connection = _factory.CreateConnection();
            using var transaction = connection.BeginTransaction();

            try
            {

                var affectedRows = await connection.ExecuteAsync(
                    $@"Update [jcodepro_journey_of_faith].Question Set 
                                    LevelId = Coalesce(@LevelId, LevelId),  
                                    QuestionContent = Coalesce(@QuestionContent, QuestionContent),
                                    TypeId = Coalesce(@TypeId, TypeId),
                                    CategoryId = Coalesce(@CategoryId, CategoryId),
                                    ImageUrl = Coalesce(@ImageUrl, ImageUrl)
                                    where Id = @Id",
                    new
                    {
                        LevelId = question.LevelId,
                        QuestionContent = question.QuestionContent,
                        TypeId = question.TypeId,
                        CategoryId = question.CategoryId,
                        ImageUrl = question.ImageUrl,
                        Id = question.Id
                    },
                    transaction
                );

                if (affectedRows == 0)
                    throw new NotFoundException($"Question {question.Id} not found.");
                foreach (var answer in question.Answers)
                {
                    await connection.ExecuteAsync($@"
                        UPdate [{_schemaName.Schema}].[{TableQuestion.Answer}] SET
                            Content = Coalesce(@Content, Content),
                            IsCorrect = Coalesce(@IsCorrect, IsCorrect), 
                            ImageUrl = CoaLesce(@ImageUrl, ImageUrl),
                            Explanation = Coalesce(@Explanation, Explanation)
                        where Id = @AnswerId
                    ", new
                    {
                        Content = answer.Content,
                        IsCorrect = answer.IsCorrect,
                        ImageUrl = answer.ImageUrl,
                        Explanation = answer.Explanation,
                        Id = answer.Id,
                    }, transaction);
                }

                transaction.Commit();
                return true;
            }
            catch
            {
                transaction.Rollback();
                throw;
            }

        }

        public async Task<bool> DeleteQuestion(int id)
        {
            using var connection = _factory.CreateConnection();

            var rowsAffected = await connection.ExecuteAsync($@"
                UPDATE [{_schemaName.Schema}].[{TableQuestion.Question}]
                SET IsDeleted = @IsDeleted,
                    DeletedAt = @DeletedAt
                WHERE Id = @Id and IsDeleted = 0",
                        new
                        {
                            IsDeleted = true,
                            DeletedAt = DateTime.Now,
                            Id = id
                        });

            return rowsAffected > 0;
        }

        #endregion
    }
    #region Constants
    public static class TableQuestion
    {
        public const string Answer = "Answer";
        public const string Question = "Question";
        public const string QuizLevel = "QuizLevel";
        public const string QuestionType = "QuestionType";
        public const string QuestionCategory = "QuestionCategory";
    }
    #endregion
    #region Get data generic
    
    #endregion
}
