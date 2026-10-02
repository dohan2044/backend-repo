using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Infrastructure.dtos.quiz
{
    public class CreateQuizAttempt
    {
        public int QuizId { get; set; }
        public string UserId { get; set; }
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; }
        = DateTime.Now;

        public int Score { get; set; }
    }
}
