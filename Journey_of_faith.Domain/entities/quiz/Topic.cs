using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.quiz
{
    public class Topic : AuditableEntity
    {
        public string TopicName { get; set; } = string.Empty;
        public int? QuizCount { get; set; }
        public ICollection<Quiz> Quizs { get; set; } = [];

        public Topic()
        {

        }

        public Topic(string name, int? quizCount)
        {
            if(string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name), "Tên kh đucợ phép null");
            }
            TopicName = name;
            QuizCount = quizCount;
        }
    }
}
