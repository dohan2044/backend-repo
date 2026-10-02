using Journey_of_faith.Application.common.interfaces;
using Journey_of_faith.Application.exceptions;
using Journey_of_faith.Domain.entities.quiz;
using Journey_of_faith.Domain.interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Journey_of_faith.Application.usecases.questions.commands
{
    public class CreateQuizLevelHandler(IQuestionRepository questionRepository, IQuestionQueries questionQueries)
        : IRequestHandler<CreateQuizLevelCommand, bool> 
    {
        private readonly IQuestionRepository _questionRepository = questionRepository;
        private readonly IQuestionQueries _questionQueries = questionQueries;
        public async Task<bool> Handle(CreateQuizLevelCommand command, CancellationToken token)
        {
            if(await _questionQueries.NameExistsAsync(command.Name, "QuizLevel", token))
            {
                throw new ConfictException("Tên Cấp độ đã tồn tại không thể thực hiện tạo thêm");
            }
            var quiz = new QuizLevel(command.Name, command.Code, command?.Score ?? 1);
            await _questionRepository.CreateQuizLevel(quiz);
            return quiz.Id > 0;
        }
    }

    public class CreateQuestionTypeHandler(IQuestionRepository question, IQuestionQueries questionQueries) : IRequestHandler<CreateQuestionTypeCommand, bool>
    {
        private readonly IQuestionRepository _question = question;
        private readonly IQuestionQueries _questionQueries = questionQueries;
        public async Task<bool> Handle(CreateQuestionTypeCommand command, CancellationToken token)
        {

            if (await _questionQueries.NameExistsAsync(command.Name, "QuestionType", token))
            {
                throw new ConfictException("Tên Kiêu câu hỏi đã tồn tại không thể thực hiện tạo thêm");
            }
            var questionType = new QuestionType(command.Name, command.Code, command?.Description ?? string.Empty);
            await _question.CreateQuestionType(questionType);
            return questionType.Id > 0;
        }
    }


    public class CreateQuestionCategoryHandler(IQuestionRepository question, IQuestionQueries questionQueries) : IRequestHandler<CreateQuestionCategoryCommand, bool>
    {
        private readonly IQuestionRepository _questions = question;
        private readonly IQuestionQueries _questionQueries = questionQueries;
        public async Task<bool> Handle(CreateQuestionCategoryCommand command, CancellationToken token)
        {
            if (await _questionQueries.NameExistsAsync(command.Name, "QuestionCategory", token))
            {
                throw new ConfictException("Tên Chủ đề đã tồn tại không thể thực hiện tạo thêm");
            }
            var questionType = new QuestionCategory(command.Name, command.Code, command?.Description ?? string.Empty);
            await _questions.CreateQuestionCategory(questionType);
            return questionType.Id > 0;
        }
    }

    public class CreateQuestionHandler(IQuestionRepository question, IQuestionQueries questionQueries, IDbConnectionFactory connection)
        : IRequestHandler<CreateQuestionCommand, bool>
    {
        private readonly IQuestionRepository _question = question;
        private readonly IQuestionQueries _questionQueries = questionQueries;
        private readonly IDbConnectionFactory _conneciton = connection;

        public async Task<bool> Handle(CreateQuestionCommand command, CancellationToken token)
        {

            List<ValidateInput> metadata = new List<ValidateInput>()
            {
                new ValidateInput { Id = command.LevelId, TableName = "QuizLevel", ErrorMessage = "Cấp độ câu hỏi không tồn tại."},
                new ValidateInput { Id = command.TypeId, TableName = "QuestionType", ErrorMessage = "Kiểu câu hỏi không tồn tại."},
                new ValidateInput { Id = command.CategoryId, TableName = "QuestionCategory", ErrorMessage = "Kiểu câu hỏi không tồn tại."},
            };

            foreach(var item in metadata)
            {
                if(!await _questionQueries.IdExistsAsync(item.Id, item.TableName, token))
                {
                    throw new NotFoundException(item.ErrorMessage);
                }
            }
            if(!command.Items.Any(e => e.IsCorrect))
            {
                throw new BadRequestException("Phải có ít nhất một đáp án đúng");
            }
            if(command.Items.Count < 3)
            {
                throw new BadRequestException("Phải có ít nhất từ 3 đáp án");
            }

            if(await _questionQueries.QuestionContentExistsAsync(command.QuestionContent, token))
            {
                throw new UnprocessableEntityException("Không thể thêm câu hỏi trùng nội dung");
            }
            var question = Question.Create(levelId: command.LevelId, questionContent: command.QuestionContent, 
                typeId: command.TypeId, categoryId: command.CategoryId, imageUrl: command.ImageUrl);

            foreach(var item in command.Items)
            {
                question.AddAnswer(questionId: question.Id,content: item.Content,isCorrect:item.IsCorrect,
                   imageUrl: item.ImageUrl, explance: item.Explanation);
            }
            await _question.CreateQuestionAsync(question);
            
            return true;
        }

    }
    [AttributeUsage(AttributeTargets.Property)]
    public class ValidateIdAttribute: Attribute
    {
        public string TableName { get; set; }
        public string ErrorMessage { get; set; }
        public ValidateIdAttribute(string tableName, string errorMessage)
        {
            TableName = tableName;
            ErrorMessage = errorMessage;
        }
    }

    public class ValidateInput
    {
        public int Id { get; set; }
        public string TableName { get; set; }
        public string ErrorMessage { get; set; }
    }
}
