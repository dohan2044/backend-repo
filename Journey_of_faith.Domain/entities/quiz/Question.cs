using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace Journey_of_faith.Domain.entities.quiz
{
    public class Question
    {
        public int Id { get; set; }
        public int LevelId { get; private set; }
        public string QuestionContent { get; private set; } = string.Empty;
        public int? TypeId { get; private set; }
        public int? CategoryId { get; private set; }
        public string? ImageUrl { get; private set; }
        public bool? IsActive { get; private set; } 
        public DateTime? CreatedTime { get; private set; }
        public QuizLevel Level { get; set; } = null!;
        public QuestionType? Type { get; set; }
        public QuestionCategory? Category { get; set; }

        private readonly List<Answer> _answers = new();
        private readonly List<QuizQuestion> _quizQuestions = new();
        private readonly List<AttemptAnswer> _attemptAnswers = new();

        public IReadOnlyCollection<Answer> Answers => _answers.AsReadOnly();
        public IReadOnlyCollection<QuizQuestion> QuizQuestions => _quizQuestions.AsReadOnly();
        public IReadOnlyCollection<AttemptAnswer> AttemptAnswers => _attemptAnswers.AsReadOnly();

        private Question() { }
        public Question(int levelId, string questionContent, int typeId, int categoryId, string imageUrl)
        {
            if(levelId.Equals(0))
            {
                throw new ArgumentException("Cấp độ câu hỏi không được để trống he");
            }
            if(string.IsNullOrEmpty(questionContent))
            {
                throw new ArgumentNullException(nameof(questionContent));
            }

            if(typeId.Equals(0))
            {
                throw new ArgumentException(nameof(typeId));
            }
            if(categoryId.Equals(0))
            {
                throw new ArgumentException(nameof(categoryId));
            }

            if(string.IsNullOrEmpty(imageUrl))
            {
                throw new ArgumentNullException("Vui lonfh chọn hình ảnh nhé");
            }

            this.LevelId = levelId;
            this.QuestionContent = questionContent;
            this.TypeId = typeId;
            this.CategoryId = categoryId;

            this.ImageUrl = imageUrl;

            this.IsActive = true;
            this.CreatedTime = DateTime.UtcNow;
            this._answers = new List<Answer>();
        }
        public Question(int levelId, string questionContent, int typeId, int categoryId, string imageUrl, string id)
        { 
            this.LevelId = levelId;
            this.QuestionContent = questionContent;
            this.TypeId = typeId;
            this.CategoryId = categoryId;

            this.ImageUrl = imageUrl;

            this.IsActive = true;
            this.CreatedTime = DateTime.UtcNow;
            this._answers = new List<Answer>();
        }
        public static Question Create(int levelId, string questionContent, int typeId, int categoryId, string imageUrl)
            => new Question(levelId, questionContent, typeId, categoryId, imageUrl);

        public void AddAnswer(List<Answer> answers)
        {
            _answers.AddRange(answers);
        }

        public static Question Update(int? LevelId, string? questionContent, int? typeId, int? categoryId, string? imageUrl, int Id)
            => Update(LevelId, questionContent, typeId, categoryId, imageUrl, Id);
        public void UpdateAnswer(Answer anser)
        {
            _answers.Add(anser);
        }
        public void AddAnswer(int questionId, string content, bool isCorrect,  string imageUrl, string explance)
        {
            var answer = Answer.Create(this.Id , content, isCorrect, imageUrl, explance);
            _answers.Add(answer);
        }
        
    }
}
