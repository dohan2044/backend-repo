using Journey_of_faith.Domain.entities.quiz;
namespace Journey_of_faith.Domain.interfaces
{
    public class SubmitResult
    {
        public double Score { get; set; }
        public int FailQuestionCount { get; set; }
        public int CorrectQuestionCount { get; set; }
        public string Message { get; set; } = string.Empty;
    }

    public interface IExamRepository
    {
        Task<int> CreateQuiz(Quiz quiz, int HardQuestion, int MediumQuestion, int EasyQuestion);
        Task<long> SaveScoreTest(QuizAttempt quiz);
        Task<bool> DeleteQuiz(int Id);

        Task<int> CreateTopicAsync(Topic topic);
        Task<int> DeleteTopicAsync(int id);
    }
}
