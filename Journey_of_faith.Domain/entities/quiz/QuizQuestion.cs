using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.quiz
{
    public class QuizQuestion
    {
        public int Id { get; set; }
        public int QuizId { get; set; }
        public int QuestionId { get; set; }
        public int? OrderIndex { get; set; }
        public Quiz Quiz { get; set; } = null!;
        public Question Question { get; set; } = null!;

        private QuizQuestion() { }
        public QuizQuestion(int quizId, int questionId, int? orderIndex)
        {
            Id = quizId;
            QuizId = questionId;
            QuestionId = questionId;
        }


    }
}
