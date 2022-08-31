using ClosedXML.Excel;
using EntityLayer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http.Headers;
using System.Text;
using TcpListenerWeb.Models;

namespace TcpListenerWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        string Baseurl = "https://localhost:7244/";

  //  [Authorize]
        public async Task<IActionResult> Index()
        {
            List<ClientData> CliInfo = new List<ClientData>();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                //client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "ACCESS_TOKEN");

                HttpResponseMessage Res = await client.GetAsync("api/TcpListener");
                if (Res.IsSuccessStatusCode)
                {
                    var CliResponse = Res.Content.ReadAsStringAsync().Result;

                    CliInfo = JsonConvert.DeserializeObject<List<ClientData>>(CliResponse);
                }
                //returning the employee list to view
                return View(CliInfo);
            }

        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Create(ClientData parametre)
        {

            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                //client.DefaultRequestHeaders.Clear();
                string data = JsonConvert.SerializeObject(parametre);
                StringContent content = new StringContent(data, Encoding.UTF8, "application/json");

                HttpResponseMessage response = client.PostAsync(client.BaseAddress + "api/TcpListener", content).Result;
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

                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                ClientData clidata = new ClientData();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = client.GetAsync(client.BaseAddress + "api/TcpListener/" + id).Result;
                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    clidata = JsonConvert.DeserializeObject<ClientData>(data);
                }
                return View(clidata);
            }
        }
        public IActionResult Edit(ClientData parametre)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                var response = client.PutAsJsonAsync(client.BaseAddress + "api/TcpListener/", parametre).Result;
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }
                return View("Details", parametre.ClientId);
            }
        }
        public ActionResult Delete(int id)
        {
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri(Baseurl);
                client.DefaultRequestHeaders.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage result = client.DeleteAsync(client.BaseAddress + "api/TcpListener/?id=" + id).Result;
               
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
                var worksheet = workbook.Worksheets.Add("Blog Listessi");
                worksheet.Cell(1, 1).Value = "Client ID";
                worksheet.Cell(1, 2).Value = "Telephone Number";
                worksheet.Cell(1, 3).Value = "TargetTelephone Number";
                worksheet.Cell(1, 4).Value = "Date";
                worksheet.Cell(1, 5).Value = "Ring Time";
                worksheet.Cell(1, 6).Value = "Call Time";
                worksheet.Cell(1, 7).Value = "Start Time";
                worksheet.Cell(1, 8).Value = "Finish Time";

                int BlogRowCount = 2;
                List<ClientData> CliInfo = new List<ClientData>();
                using (var client = new HttpClient())
                {
                    client.BaseAddress = new Uri(Baseurl);
                    client.DefaultRequestHeaders.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    HttpResponseMessage Res = client.GetAsync("api/TcpListener").Result;
                    if (Res.IsSuccessStatusCode)
                    {
                        var CliResponse = Res.Content.ReadAsStringAsync().Result;

                        CliInfo = JsonConvert.DeserializeObject<List<ClientData>>(CliResponse);

                        foreach (var item in CliInfo)
                        {
                            worksheet.Cell(BlogRowCount, 1).Value = item.ClientId;
                            worksheet.Cell(BlogRowCount, 2).Value = item.TelephoneNumber;
                            worksheet.Cell(BlogRowCount, 3).Value = item.TargetTelephoneNumber;
                            worksheet.Cell(BlogRowCount, 4).Value = item.Date;
                            worksheet.Cell(BlogRowCount, 5).Value = item.RingTime;
                            worksheet.Cell(BlogRowCount, 6).Value = item.CallTime;
                            worksheet.Cell(BlogRowCount, 7).Value = item.StartTime;
                            worksheet.Cell(BlogRowCount, 8).Value = item.FinishTime;
                            BlogRowCount++;
                        }
                    }
                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();
                        return File(content, "application/vdn.openxmlformats-officedocument.spreadsheetml.sheet", "İnnovaExcell.xlsx");
                    }
                }

            }
        }
            public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}