using System;
using System.Collections.Generic;
using System.Text;

namespace Journey_of_faith.Domain.entities.quiz
{
    public class QuestionCategory
    {
        public int Id { get; set; }
        public string Name { get; private set; } = string.Empty;
        public string Code {get; set; } = string.Empty;
        public string Description {get; set; } = string.Empty;
        public int CountOfCategory { get; private set; } = 0;
        public ICollection<Question> Questions { get; set; } = [];
        private QuestionCategory() { }
        public QuestionCategory(string name, string code, string description)
        {
            if(string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException($"{name} is required.");
            }

            Name = name;
            Code = code;
            Description = description;
        }
    }
}
