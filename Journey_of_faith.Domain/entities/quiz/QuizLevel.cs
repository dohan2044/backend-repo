using Journey_of_faith.Domain.exceptions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.quiz
{
    public class QuizLevel
    {
        public int Id { get; set; }
        public string Name { get; private set; } = string.Empty;
        public string Code {get; private set; } = string.Empty;
        public int Score { get; private set; } = 0;
        public ICollection<Question> Questions { get; set; } = [];
        private QuizLevel() { }
        public QuizLevel(string name, string code, int score)
        {
            if(string.IsNullOrEmpty(name))
            {
                throw new DomainException($"{name} is required");
            }
            Name = name;
            Code = code;
            Score = score;
        }
    }
}
