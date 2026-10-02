using Journey_of_faith.Domain.exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.quiz
{
    public class AttemptAnswer
    {
        public long Id { get; set; }
        public long AttemptId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public bool IsCorrect { get; set; }

        public AttemptAnswer(long attemptId, int questionId, int answerId, bool isCorrect)
        {
            if(attemptId < 0)
            {
                throw new DomainException("Mã Lần chọn kh hợp lệ");
            }

            if(questionId < 0)
            {
                throw new DomainException("Mã câu hỏi không hợp lệ");
            }

            if(answerId < 0)
            {
                throw new DomainException("Mã đáp án không hợp lệ");
            }
            AttemptId = attemptId;
            QuestionId = questionId;
            AnswerId = answerId;
            IsCorrect = isCorrect;
        }

        public QuizAttempt Attempt { get; set; } = null!;

        public static AttemptAnswer Create(long attemptId, int questionId, int answerId, bool isCorrect)
            => new AttemptAnswer(attemptId, questionId, answerId, isCorrect);
    }
}
