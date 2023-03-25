using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using TryParsing.Models;
using HtmlAgilityPack;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Net;
using System.Text;
using System.IO;
using System;

namespace TryParsing.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
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


        






        //mine
        public IActionResult Index()
        {
            string url = "https://www.scrapingbee.com/blog/web-scraping-csharp/";
            var response = CallUrl(url).Result;
            WriteToCsv(ParseHtml(response));
            return View();
        }
        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(fullUrl);
            return response;
        }
        private List<string> ParseHtml(string html)
        {
            HtmlDocument htmlDoc = new HtmlDocument();
            htmlDoc.LoadHtml(html);

            var programmerLinks = htmlDoc.DocumentNode.Descendants("li")
                .Where(node => !node.GetAttributeValue("class", "").Contains("tocsection"))
                .ToList();

            List<string> wikiLink = new List<string>();

            foreach (var link in programmerLinks)
            {
                if (link.FirstChild.Attributes.Count > 0) wikiLink.Add("https://www.scrapingbee.com" + link.FirstChild.Attributes[0].Value);
            }

            return wikiLink;
           //var programmerLinks = htmlDoc.DocumentNode.SelectNodes("//li[not(contains(@class, 'tocsection'))]") Xpathov karja linum bayc petqa dranic glux ahnel
        }
        private void WriteToCsv(List<string> links)
        {
            FileStream fs = new FileStream("C:\\Users\\user\\OneDrive\\Рабочий стол\\link.cvs", FileMode.OpenOrCreate);
            StreamWriter sw = new StreamWriter(fs);
            StringBuilder sb = new StringBuilder();
            foreach (var link in links)
            {
                sb.AppendLine(link);
            }
            sw.WriteLine(sb);
            sw.Close();
            fs.Close();
        }
        
    }
}