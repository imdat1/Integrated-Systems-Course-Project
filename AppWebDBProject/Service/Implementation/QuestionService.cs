using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Domain;
using Repository.Interface;
using Service.Interface;

namespace Service.Implementation
{
    public class QuestionService : IQuestionService
    {
        private readonly IRepository<Question> _questionRepository;
        private readonly IRepository<Database> _databaseRepository;

        public QuestionService(IRepository<Question> questionRepository, IRepository<Database> databaseRepository)
        {
            _questionRepository = questionRepository;
            _databaseRepository = databaseRepository;
        }

        public void AskQuestion(string questionText, Guid dbId, string questionAnswer)
        {

            var question = new Question
            {
                Id = Guid.NewGuid(),
                QuestionText = questionText,
                QuestionAnswer = questionAnswer
            };
            _questionRepository.Insert(question);
            var db = _databaseRepository.Get(dbId);
            if(db.Questions == null)
            {
                db.Questions = new List<Question>();
            }
            db.Questions.Add(question);
            _databaseRepository.Update(db);

        }

        public void DeleteQuestion(Guid questionId, Guid dbId)
        {
            var question = _questionRepository.Get(questionId);
            var db = _databaseRepository.Get(dbId);

            _questionRepository.Delete(question);
            db.Questions?.Remove(question);
            _databaseRepository.Update(db);
        }

        public List<Question> GetAllQuestions()
        {
            return _questionRepository.GetAll().ToList();
        }

        public Question GetQuestionById(Guid id)
        {
            return _questionRepository.Get(id);
        }

        public List<Question> GetQuestionsForDatabase(Guid id)
        {
            Database db = _databaseRepository.Get(id);
            var dbQuestions = db.Questions;
            if(dbQuestions == null)
            {
                dbQuestions = new List<Question>();
            }
            return dbQuestions.ToList();
        }
    }
}
