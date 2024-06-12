using System.Text;
using AppWebDBAdmin.Models;
using AppWebDBAdmin.Models.DTO;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace AppWebDBAdmin.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            if (TempData["InvalidUsers"] != null)
            {
                ViewBag.InvalidUsers = JsonConvert.DeserializeObject<List<UserRegistrationDto>>(TempData["InvalidUsers"].ToString());
            }
            HttpClient client = new HttpClient();
            string URL = "https://localhost:7213/api/Admin/GetAllUsers";

            HttpResponseMessage response = client.GetAsync(URL).Result;
            var data = response.Content.ReadAsAsync<List<DBUser>>().Result;
            return View(data);
        }
        public IActionResult ImportUsersView()
        {
            return View();
        }

        public IActionResult ImportUsers(IFormFile file)
        {
            string directoryPath = $"{Directory.GetCurrentDirectory()}\\files";
            string pathToUpload = $"{directoryPath}\\{file.FileName}";

            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            using (FileStream fileStream = System.IO.File.Create(pathToUpload))
            {
                file.CopyTo(fileStream);
                fileStream.Flush();
            }

            List<UserRegistrationDto> users = getAllUsersFromFile(file.FileName);
            HttpClient client = new HttpClient();
            string URL = "https://localhost:7213/api/Admin/ImportAllUsers";

            HttpContent content = new StringContent(JsonConvert.SerializeObject(users), Encoding.UTF8, "application/json");

            HttpResponseMessage response = client.PostAsync(URL, content).Result;

            if (response.IsSuccessStatusCode)
            {
                var importResult = JsonConvert.DeserializeObject<ImportResultsDto>(response.Content.ReadAsStringAsync().Result);

                if (importResult != null && importResult.InvalidUsers.Any())
                {
                    TempData["InvalidUsers"] = JsonConvert.SerializeObject(importResult.InvalidUsers);
                }
            }

            return RedirectToAction("Index", "User");
        }



        private List<UserRegistrationDto> getAllUsersFromFile(string fileName)
        {
            List<UserRegistrationDto> users = new List<UserRegistrationDto>();
            string filePath = $"{Directory.GetCurrentDirectory()}\\files\\{fileName}";

            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            using (var stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var reader = ExcelDataReader.ExcelReaderFactory.CreateReader(stream))
                {
                    while (reader.Read())
                    {
                        users.Add(new UserRegistrationDto
                        {
                            Email = reader.GetValue(0).ToString(),
                            Password = reader.GetValue(1).ToString(),
                            ConfirmPassword = reader.GetValue(2).ToString(),
                            HuggingFaceAPIToken = reader.GetValue(3).ToString()
                        });
                    }
                }
            }
            return users;
        }

    }
}
