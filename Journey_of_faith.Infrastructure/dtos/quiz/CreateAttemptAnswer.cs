using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Infrastructure.dtos.quiz
{
    public class CreateAttemptAnswer
    {
        public long AttemptId { get; set; }
        public int QuestionId { get; set; }
        public int AnswerId { get; set; }
        public bool IsCorrect { get; set; }
    }
}
