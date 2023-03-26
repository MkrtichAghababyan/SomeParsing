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
using PuppeteerSharp;
using Microsoft.Extensions.Logging;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Internal;

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

        //only for html 

        /*public IActionResult Index()
        {
            string url = "https://www.scrapingbee.com/blog/web-scraping-csharp/";
            var response = CallUrl(url).Result;
            WriteToCsv(ParseHtml(response));
            return View();
        }*/

        //TODO:jsov 1in dzev ogtagorcuma puppeteer nutgate


        public async Task<IActionResult> Index()
        {
            string fullUrl = "https://en.wikipedia.org/wiki/List_of_programmers";

            List<string> programmerLinks = new();

            var options = new LaunchOptions()
            {
                Headless = true,
                ExecutablePath = "C:/Program Files/Google/Chrome/Application/chrome.exe"
            };

            var browser = await Puppeteer.LaunchAsync(options, null);
            var page = await browser.NewPageAsync();
            await page.GoToAsync(fullUrl);

            var links = @"Array.from(document.querySelectorAll('li:not([class^=""toc""]) a')).map(a => a.href);";
            var urls = await page.EvaluateExpressionAsync<string[]>(links);

            foreach (string url in urls)
            {
                programmerLinks.Add(url);
            }

            WriteToCsv(programmerLinks);

            return View();
        }

        //jsov 2rd dzevy ogtagorcuma Selenium nutgatey inchvor bana pakasum toli chgitem chi ashxatum

        //public async Task<IActionResult> Index()
        //{
        //    string fullUrl = "https://en.wikipedia.org/wiki/List_of_programmers";
        //    List<string> programmerLinks = new List<string>();

        //    var options = new ChromeOptions()
        //    {
        //        BinaryLocation = "C:\\Program Files (x86)\\Google\\Chrome\\Application\\chrome.exe"
        //    };

        //    options.AddArguments(new List<string>() { "headless", "disable-gpu" });

        //    var browser = new ChromeDriver(options);
        //    browser.Navigate().GoToUrl(fullUrl);

        //    var links = browser.FindElementsByXPath("//li[not(contains(@class, 'tocsection'))]/a[1]");
        //    foreach (var url in links)
        //    {
        //        programmerLinks.Add(url.GetAttribute("href"));
        //    }

        //    WriteToCsv(programmerLinks);

        //    return View();
        //}
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
                if (link.FirstChild.Attributes.Count > 0) wikiLink.Add("https://en.wikipedia.org" + link.FirstChild.Attributes[0].Value);
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