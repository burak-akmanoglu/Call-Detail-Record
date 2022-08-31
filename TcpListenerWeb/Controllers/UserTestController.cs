using ClosedXML.Excel;
using EntityLayer;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace TcpListenerWeb.Controllers
{
    public class UserTestController : Controller
    {
        string Baseurl = "https://localhost:7244/";


        public async Task<IActionResult> Index()
        {

            var accessToken = HttpContext.Session.GetString("JWToken");
            List<User> CliInfo = new List<User>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                HttpResponseMessage Res = await client.GetAsync("api/User/Admins");
                if (Res.IsSuccessStatusCode)
                {
                    var CliResponse = Res.Content.ReadAsStringAsync().Result;

                    CliInfo = JsonConvert.DeserializeObject<List<User>>(CliResponse);
                }
                //returning the employee list to view
                if (accessToken != null)
                {
                    return View(CliInfo);
                }
                return Redirect("~/Login/LoginUser");
               
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            if (accessToken != null)
            {
                return View();
            }
            return null;
        }
        [HttpPost]
        public IActionResult Create(User parametre)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                //client.DefaultRequestHeaders.Clear();
                string data = JsonConvert.SerializeObject(parametre);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(client.BaseAddress + "api/User/", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                return View();
            }
        }
        public IActionResult Details(int id)
        {
            using (var client = new HttpClient())
            {
                var accessToken = HttpContext.Session.GetString("JWToken");
                
               
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                User userdata = new User();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(client.BaseAddress + "api/User/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    userdata = JsonConvert.DeserializeObject<User>(data);
                }
                if (accessToken != null)
                {
                    return View(userdata);
                }
                return null;
            }
        }
        public IActionResult Edit(User parametre)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                var response = client.PutAsJsonAsync(client.BaseAddress + "api/User/", parametre).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                return View("Details", parametre.UserModelId);
            }
        }
        public ActionResult Delete(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);

                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage result = client.DeleteAsync(client.BaseAddress + "api/User/?id=" + id).Result;

                if (result.IsSuccessStatusCode)
                {

                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }
        public IActionResult ExportStaticExcellBlogList()
        {
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Deneme");
                worksheet.Cell(1, 1).Value = "User ID";
                worksheet.Cell(1, 2).Value = "User Name";
                worksheet.Cell(1, 3).Value = "Password";
                worksheet.Cell(1, 4).Value = "E mail Adress";
                worksheet.Cell(1, 5).Value = "Role";
                worksheet.Cell(1, 6).Value = "Surname";
                worksheet.Cell(1, 7).Value = "Given Name";
              

                int BlogRowCount = 2;
                List<User> UserInfo = new List<User>();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = client.GetAsync("api/User/Admins").Result;
                    if (Res.IsSuccessStatusCode)
                    {
                        var UserResponse = Res.Content.ReadAsStringAsync().Result;

                        UserInfo = JsonConvert.DeserializeObject<List<User>>(UserResponse);

                        foreach (var item in UserInfo)
                        {
                            worksheet.Cell(BlogRowCount, 1).Value = item.UserModelId;
                            worksheet.Cell(BlogRowCount, 2).Value = item.Username;
                            worksheet.Cell(BlogRowCount, 3).Value = item.Password;
                            worksheet.Cell(BlogRowCount, 4).Value = item.EmailAddress;
                            worksheet.Cell(BlogRowCount, 5).Value = item.Role;
                            worksheet.Cell(BlogRowCount, 6).Value = item.Surname;
                            worksheet.Cell(BlogRowCount, 7).Value = item.GivenName;
                            
                            BlogRowCount++;
                        }
                    }
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vdn.openxmlformats-officedocument.spreadsheetml.sheet", "İnnovaUserExcell.xlsx");
                    }
                }

            }
        }
        [HttpGet]
        public async Task<List<User>> GetUser()
        {
            var accessToken = HttpContext.Session.GetString("JWToken");
            var url = "https://localhost:7244/api/User/Admins";
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            string jsonStr = await client.GetStringAsync(url);
            var res = JsonConvert.DeserializeObject<List<User>>(jsonStr).ToList();
            if (accessToken != null)
            {
                return res;
            }
            else
            {
                return null;
            }
          
        }
     
    }
    }

