namespace Journey_of_faith.Application.common.dtos;

public class QuestionView
    {
        public int Id { get; set; }
        public int LevelId { get; set; }
        public string LevelName {get; set;}
        public string QuestionContent { get; set; } = string.Empty;
        public int TypeId { get; set; }
        public string TypeName {get; set;}
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool IsDeleted {get; set;}

        public List<AnswerView> Answers { get; set; } = new();
    }
    public class AnswerView
    {
        public int Id { get; set; }
        public int QuestionId { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
        // public string ImageUrl { get; set; } = string.Empty;
        // public string Explanation { get; set; } = string.Empty;
    }

public sealed class QuizLevelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public int Score { get; set; }
}

public sealed class QuestionTypeDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public sealed class QuestionCategoryDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int CountOfCategory { get; set; }
}

