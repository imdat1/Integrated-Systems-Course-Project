using System.Net.Http;
using System.Text.Json;
using Domain.Domain;
using Domain.Domain.DTO;
using Domain.Identity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages;
using Service.Interface;

namespace AppWebDB.Controllers.API
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {

        private readonly IDatabaseService _databaseService;
        private readonly IQuestionService _questionService;
        private readonly IUserService _userService;
        private readonly UserManager<DBUser> _userManager;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _flaskClient;

        public AdminController(IDatabaseService databaseService, IQuestionService questionService, IUserService userService, UserManager<DBUser> userManager, IHttpClientFactory httpClientFactory, HttpClient flaskClient)
        {
            _databaseService = databaseService;
            _questionService = questionService;
            _userService = userService;
            _userManager = userManager;
            _httpClientFactory = httpClientFactory;
            _flaskClient = flaskClient;
        }

        [HttpGet("[action]")]
        public List<Database> GetAllDatabases()
        {
            return this._databaseService.GetAllDatabases();
        }

        [HttpPost("[action]")]
        public Database GetDetailsForDatabase(BaseEntity id)
        {
            return this._databaseService.GetDetailsForDatabase(id);
        }

        [HttpGet("[action]")]
        public List<DBUser> GetAllUsers()
        {
            return this._userService.GetUsers();
        }

        [HttpGet("[action]")]
        public DBUser GetDetailsForUser(string id)
        {
            return this._userService.GetUser(id);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<ImportResultDto>> ImportAllUsers(List<UserRegistrationDto> model)
        {
            bool status = true;
            List<UserRegistrationDto> invalidUsers = new List<UserRegistrationDto>();

            foreach (var item in model)
            {
                if (!await ValidateHuggingFaceTokenAsync(item.HuggingFaceAPIToken))
                {
                    invalidUsers.Add(item);
                    continue;
                }

                var userCheck = _userManager.FindByEmailAsync(item.Email).Result;

                if (userCheck == null)
                {
                    var user = new DBUser
                    {
                        UserName = item.Email,
                        NormalizedUserName = item.Email,
                        Email = item.Email,
                        HuggingFaceAPIToken = item.HuggingFaceAPIToken,
                        EmailConfirmed = true,
                        Databases = new List<Database>()
                    };

                    var result = _userManager.CreateAsync(user, item.Password).Result;
                    status = status && result.Succeeded;
                }
            }

            var importResult = new ImportResultDto
            {
                Status = status,
                InvalidUsers = invalidUsers
            };

            return Ok(importResult);
        }

        [HttpPost("[action]")]
        public async Task<ActionResult<ImportResultDatabaseDto>> ImportDatabasesForUser(List<Database> model)
        {
            bool status = true;
            List<Database> invalidDatabases = new List<Database>();

            foreach (var item in model)
            {
                var formData = new Dictionary<string, string>
                {
                    { "dbUsername", item.Username },
                    { "dbPass", item.Password },
                    { "dbHost", item.Host },
                    { "dbName", item.DatabaseName }
                };

                var connectResponse = await _flaskClient.PostAsync("http://localhost:5000/connect", new FormUrlEncodedContent(formData));

                if (!connectResponse.IsSuccessStatusCode)
                {
                    invalidDatabases.Add(item);
                    status = false;
                    continue;
                }

                var databaseCheck = _databaseService.GetDetailsForDatabase(item.Id);

                if (databaseCheck == null)
                {
                    var db = new Database
                    {
                        Id = Guid.NewGuid(),
                        Name = item.Name,
                        Username = item.Username,
                        Password = item.Password,
                        Host = item.Host,
                        DatabaseName = item.DatabaseName,
                        OwnerId = item.OwnerId,
                        Owner = _userService.GetUser(item.OwnerId),
                        Questions = new List<Question>()
                    };

                    _databaseService.CreateNewDatabase(db, item.OwnerId);
                }
            }

            var importResult = new ImportResultDatabaseDto
            {
                Status = status,
                InvalidDatabases = invalidDatabases
            };

            return Ok(importResult);
        }

        private async Task<bool> ValidateHuggingFaceTokenAsync(string token)
        {
            var client = _httpClientFactory.CreateClient();
            var request = new HttpRequestMessage(HttpMethod.Get, "https://huggingface.co/api/whoami-v2");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var response = await client.SendAsync(request);
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var jsonResponse = JsonDocument.Parse(content);
                return true;
            }

            return false;
        }

    }
}
