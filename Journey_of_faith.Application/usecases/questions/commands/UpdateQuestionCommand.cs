using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Application.usecases.questions.commands
{
    public class UpdateQuestionCommand : IRequest<Unit>
    {
        public int Id { get; set; }
        public int? LevelId { get; set; }
        public string? QuestionContent { get; set; } = string.Empty;
        public int? TypeId { get; set; }
        public int? CategoryId { get; set; }
        public string? ImageUrl { get; set; } = string.Empty;
        public List<UpdateAnswerCommand> Answers { get; set; } = new();
    }

    public class UpdateAnswerCommand
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Explanation { get; set; } = string.Empty;
    }

}
