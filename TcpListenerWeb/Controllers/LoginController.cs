using EntityLayer;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;

namespace TcpListenerWeb.Controllers
{
    public class LoginController : Controller
    {
        string Baseurl = "https://localhost:7244/";
        [HttpGet]
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Index(User parametre)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                // client.DefaultRequestHeaders.Clear();
                string data = JsonConvert.SerializeObject(parametre);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(client.BaseAddress + "api/Login", content).Result;
                
                string data2 = response.Content.ReadAsStringAsync().Result;
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + data2);

             //   HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "UserTest",data2);
                }

                return View();
            }
        }
        [HttpGet]
        public IActionResult LoginUser()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> LoginUser(UserLogin user)
        {
            using (var httpClient= new HttpClient())
            {
                StringContent stringContent = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                using (var response = await httpClient.PostAsync("https://localhost:7244/api/Login",stringContent))
                {
                    string token = await response.Content.ReadAsStringAsync();
                    if (token=="Invalid credentials")
                    {
                        ViewBag.Message = "Incorrent UserId or Password!";
                        return Redirect("~/Home/Index");
                    }
                    HttpContext.Session.SetString("JWToken", token);
                }
                return Redirect("~/UserTest/Index");
            }

        }
        public IActionResult Logoff()
        {
            HttpContext.Session.Clear();
            return Redirect("~/Home/Index");
        }
       
    }
}
