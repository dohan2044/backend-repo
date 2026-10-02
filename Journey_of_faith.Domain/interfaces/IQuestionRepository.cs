using Journey_of_faith.Domain.dtos;
using Journey_of_faith.Domain.entities.quiz;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace Journey_of_faith.Domain.interfaces;

    public interface IQuestionRepository 
    {
        Task<bool> CreateQuizLevel(QuizLevel quiz);
        Task<bool> CreateQuestionType(QuestionType questionType);
        Task<bool> CreateQuestionCategory(QuestionCategory questionCategory);
        Task<bool> CreateQuestionAsync(Question question);
        Task<bool> InsertBulkQuestionAsync(string jsonValue);
        Task<bool> UpdateQuestion(Question question);
        Task<bool> DeleteQuestion(int Id);

        Task<bool> InsertMultipleCategories(string valuesInsert);
    }
