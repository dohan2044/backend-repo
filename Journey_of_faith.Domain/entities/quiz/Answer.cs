using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.quiz
{
    public class Answer
    {
        public int Id { get; set; }
        public int QuestionId { get; private set; }
        public string Content { get; private set; } = string.Empty;
        public bool IsCorrect { get; private set; }
        public string? ImageUrl { get; private set; }
        public string? Explanation { get; private set; }
        public Question Question { get; set; } = null!;

        public Answer() { }
        public Answer(int questionId, string content, bool IsCorrect)
        {
            this.QuestionId = questionId;
            this.Content = content;
            this.IsCorrect = IsCorrect;
        }

        public Answer(int questionId, string content, bool IsCorrect, string imageUrl, string explantion)
        {
            this.QuestionId = questionId;
            this.Content = content;
            this.IsCorrect = IsCorrect;
            this.ImageUrl = imageUrl;
            this.Explanation = explantion;
        }

        public Answer(string? content, bool? IsCorrect, string? imageUrl, string? explantion)
        {
            this.Content = content;
            this.IsCorrect = (bool)IsCorrect;
            this.ImageUrl = imageUrl;
            this.Explanation = explantion;
        }
        public static Answer Create(int questionId, string content, bool IsCorrect)
            => new Answer(questionId, content, IsCorrect);

        public static Answer Create(int questionId, string content, bool IsCorrect, string imageUrl, string explantion)
            => new Answer(questionId, content, IsCorrect, imageUrl, explantion);


        public static Answer Update(string? content, bool? isCorrect, string? imageUrl, string? explantion)
         => new Answer(content, isCorrect, explantion, explantion);

    }
}
