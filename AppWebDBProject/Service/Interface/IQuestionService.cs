using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Domain;

namespace Service.Interface
{
    public interface IQuestionService
    {
        List<Question> GetAllQuestions();
        List<Question> GetQuestionsForDatabase(Guid id);
        void AskQuestion(string questionText, Guid dbId, string questionAnswer);
        void DeleteQuestion(Guid questionId, Guid dbId);
        Question GetQuestionById(Guid id);
    }
}
