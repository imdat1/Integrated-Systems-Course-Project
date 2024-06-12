using System.Security.Claims;
using Domain.Domain;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Repository.Interface;
using Service.Interface;

namespace AppWebDB.Controllers
{
    public class QuestionController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly IDatabaseService _databaseService;
        private readonly IUserRepository _userRepository;
        private readonly HttpClient _flaskClient;

        [JsonProperty("answer")]
        public string Answer { get; set; }

        public QuestionController(IQuestionService questionService, IDatabaseService databaseService, IUserRepository userRepository, HttpClient flaskClient)
        {
            _questionService = questionService;
            _databaseService = databaseService;
            _userRepository = userRepository;
            _flaskClient = flaskClient;
        }

        public IActionResult Index(Guid id, string errorMessage)
        {
            ViewBag.DatabaseId = id;
            ViewBag.ErrorMessage = errorMessage;
            List<Question> questions = _questionService.GetQuestionsForDatabase(id);
            return View(questions);
        }

        public IActionResult AskQuestion(Guid id)
        {
            ViewBag.DatabaseId = id;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AskQuestion(Guid id, [Bind("QuestionText")] Question q)
        {
            if (ModelState.IsValid)
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = _userRepository.Get(userId);
                var huggingFaceToken = user.HuggingFaceAPIToken;

                var db = _databaseService.GetDetailsForDatabase(id);

                // Prepare the data to be sent to the Flask app
                var formData = new Dictionary<string, string>
                {
                    { "dbUsername", db.Username },
                    { "dbPass", db.Password },
                    { "dbHost", db.Host },
                    { "dbName", db.DatabaseName },
                    { "huggingface_token", huggingFaceToken },
                    { "question", q.QuestionText }
                };

                // Send a POST request to the Flask app to ask the question
                var askResponse = await _flaskClient.PostAsync("http://localhost:5000/ask", new FormUrlEncodedContent(formData));

                if (askResponse.IsSuccessStatusCode)
                {
                    // Parse the response from the Flask app
                    var responseString = await askResponse.Content.ReadAsStringAsync();

                    // Parse the JSON response
                    var jsonResponse = JObject.Parse(responseString);

                    // Get the value of the "answer" property
                    var questionAnswer = jsonResponse["answer"].ToString();

                    // Process the answer as needed
                    _questionService.AskQuestion(q.QuestionText, id, questionAnswer);

                    // Redirect to the Index action
                    return RedirectToAction(nameof(Index), new { id = id });
                }
                else
                {
                    // Failed to get answer from Flask app, handle error
                    // For example, return to the Index with an error message
                    var errorMessage = "Failed to get answer from Flask app.";
                    return RedirectToAction(nameof(Index), new { id = id, errorMessage = errorMessage });
                }
            }
            return RedirectToAction(nameof(Index), new { id = id });
        }

        public IActionResult DeleteQuestionAnswer(Guid questionId, Guid databaseId)
        {
            var question = _questionService.GetQuestionById(questionId);
            if (question == null)
            {
                return NotFound();
            }

            ViewBag.DatabaseId = databaseId; // Passing the databaseId to the view if needed
            return View(question); // Assuming you have a view for confirming the deletion
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteQuestionAnswerConfirmed(Guid id, Guid databaseId)
        {
            _questionService.DeleteQuestion(id, databaseId);
            return RedirectToAction(nameof(Index), new { id = databaseId });
        }

    }
}
