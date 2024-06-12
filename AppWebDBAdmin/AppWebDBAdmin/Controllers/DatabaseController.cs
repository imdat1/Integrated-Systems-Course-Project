using System.Text;
using AppWebDBAdmin.Models;
using AppWebDBAdmin.Models.DTO;
using ClosedXML.Excel;
using GemBox.Document;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppWebDBAdmin.Controllers
{
    public class DatabaseController : Controller
    {
        public DatabaseController() {
            ComponentInfo.SetLicense("FREE-LIMITED-KEY");
        }
        public IActionResult SeeDatabases(string id)
        {
            HttpClient client = new HttpClient();
            string URL = "https://localhost:7213/api/Admin/GetDetailsForUser?id=" + id;

            HttpResponseMessage response = client.GetAsync(URL).Result;
            var data = response.Content.ReadAsAsync<DBUser>().Result;

            var invalidDatabasesJson = TempData["InvalidDatabases"] as string;
            List<Database> invalidDatabases = string.IsNullOrEmpty(invalidDatabasesJson)
                ? new List<Database>()
                : JsonConvert.DeserializeObject<List<Database>>(invalidDatabasesJson);

            ViewBag.InvalidDatabases = invalidDatabases;
            return View(data);
        }

        public IActionResult ImportDatabasesView(string id)
        {
            ViewBag.userId = id;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ImportDatabases(IFormFile file, string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User ID is required.");
            }

            string directoryPath = $"{Directory.GetCurrentDirectory()}\\files";
            string pathToUpload = $"{directoryPath}\\{file.FileName}";

            // Check if the directory exists, and create it if it does not
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Proceed with file upload
            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }

            List<Database> databases = getAllDatabasesFromFile(file.FileName, userId);
            HttpClient client = new HttpClient();
            string URL = "https://localhost:7213/api/Admin/ImportDatabasesForUser";

            HttpContent content = new StringContent(JsonConvert.SerializeObject(databases), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync(URL, content);

            if (response.IsSuccessStatusCode)
            {
                var importResult = JsonConvert.DeserializeObject<ImportResultsDatabaseDto>(await response.Content.ReadAsStringAsync());

                if (importResult != null && importResult.InvalidDatabases.Any())
                {
                    TempData["InvalidDatabases"] = JsonConvert.SerializeObject(importResult.InvalidDatabases);
                }
            }

            return RedirectToAction("SeeDatabases", new { id = userId });
        }

        private List<Database> getAllDatabasesFromFile(string fileName, string userId)
        {
            List<Database> databases = new List<Database>();
            string filePath = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        databases.Add(new Database
                        {
                            Id = Guid.NewGuid(),
                            Name = reader.GetValue(0).ToString(),
                            Username = reader.GetValue(1).ToString(),
                            Password = reader.GetValue(2).ToString(),
                            Host = reader.GetValue(3).ToString(),
                            DatabaseName = reader.GetValue(4).ToString(),
                            OwnerId = userId,
                            Questions = new List<Question>(),
                        });
                    }
                }
            }
            return databases;
        }

        [HttpGet]
        public FileContentResult ExportQuestionsForDatabase(Guid id)
        {
            string fileName = "QuestionsAnswersForDatabaseId" + id.ToString() + ".xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workbook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workbook.Worksheets.Add("Orders");
                worksheet.Cell(1, 1).Value = "Question";
                worksheet.Cell(1, 2).Value = "Answer";
                HttpClient client = new HttpClient();
                string URL = "https://localhost:7213/api/Admin/GetDetailsForDatabase";
                var model = new
                {
                    Id = id
                };

                HttpContent cnt = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(URL, cnt).Result;

                var data = response.Content.ReadAsAsync<Database>().Result;

                var questions = data.Questions.ToList();

                for (int i = 0; i < questions.Count(); i++)
                {
                    var item = questions[i];
                    worksheet.Cell(i + 2, 1).Value = item.QuestionText;
                    worksheet.Cell(i + 2, 2).Value = item.QuestionAnswer;
                }
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, contentType, fileName);
                }
            }

        }
    }
}
