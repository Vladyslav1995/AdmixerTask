using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using RestSharp;
using Newtonsoft.Json.Linq;
using System.IO;
using System.Linq;

namespace TestTask
{
    public class Tests
    {
        static IWebDriver driver;
        private string priceAmount = "20000";
        private const string Uri = "https://swapi.dev/api/people/1/";
        private List<string> filmsWithSkywalker = new List<string>() { "A New Hope", "The Empire Strikes Back", "Return of the Jedi", "Revenge of the Sith" };

        [OneTimeSetUp]
        public void Setup()
        {
            driver = new ChromeDriver();
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait.Add(TimeSpan.FromSeconds(5));
        }
        
        [Test, Order(1)]
        public void VerifyPriceLessThan20000()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            watch.Start();
            driver.Navigate().GoToUrl("https://rozetka.com.ua/mobile-phones/c80003/preset=smartfon/");
            driver.FindElement(Page.Apple).Click();
            driver.FindElement(Page.Samsung).Click();
            driver.FindElement(Page.UntilPrice).Clear();
            driver.FindElement(Page.UntilPrice).SendKeys(priceAmount);
            driver.FindElement(Page.OkButton).Click();

            driver.FindElement(Page.MemoryAmount).Click();
            driver.FindElement(Page.LowestPrice).Click();

            var prices = driver.FindElements(Page.ProductPrice);

            List<string> productPrices = prices.Select(p => p.GetAttribute("innerText")).ToList();

            foreach (var productPrice in productPrices)
            {
                int num = Int32.Parse(productPrice);
                //Check weather product price less than 20000
                Assert.LessOrEqual(num, 20000);
            }

            watch.Stop();
            Console.WriteLine($"Test execution time: {watch.ElapsedMilliseconds} ms");
        }
        
        
        [Test, Order(2)]
        public void GetActorName()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            watch.Start();

            var name = GetRequest(Uri, "name");
            Assert.AreEqual("Luke Skywalker", Convert.ToString(name));
            WriteInFile("\n"+name.ToString());

            watch.Stop();
            Console.WriteLine($"Test execution time: {watch.ElapsedMilliseconds} ms");
        }

        [Test, Order(3)]
        public void GetFilmsWithSkywalker()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            watch.Start();

            var client = new RestClient(Uri);
            var response = client.Execute(new RestRequest(), RestSharp.Method.GET);
            JObject jObject = JObject.Parse(response.Content);
            var films = jObject.SelectTokens("$.films");
            
            foreach (var film in films)
            {
                for (int i = 0; i < 4; i++)
                {
                    var title = GetRequest(film[i].ToString(), "title");
                    Assert.AreEqual(filmsWithSkywalker[i], Convert.ToString(title), "film in which Luke Skywalker took a part");
                    WriteInFile("\n" + title.ToString());
                }
            }
            watch.Stop();
            Console.WriteLine($"Test execution time: {watch.ElapsedMilliseconds} ms");
        }

        [Test, Order(4)]
        public void GetPlanetsWithSkywalker()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            watch.Start();

            var planets = GetRequest(Uri, "homeworld");
            var name = GetRequest(planets.ToString(), "name");
            Assert.AreEqual("Tatooine", name.ToString(),"Planet that has relation to Luke Skywalker");
            WriteInFile("\n" + name.ToString());
            watch.Stop();
            Console.WriteLine($"Test execution time: {watch.ElapsedMilliseconds} ms");
        }

        /// <summary>
        /// Write Test result in file
        /// </summary>
        /// <param name="word">Word to write</param>
        public void WriteInFile(string word)
        {
            //Absolute path
            using (StreamWriter writer = new StreamWriter(@"C:/Users/Vladyslav/Desktop/Text.txt"))
            {
                writer.Write(word);
                writer.Close();
            }        
        }

        /// <summary>
        /// Perform Get request
        /// </summary>
        /// <param name="uri">Uri for API call</param>
        /// <param name="jPath">Path for JSON variable selection</param>
        /// <returns>Value from JSON schema</returns>
        public Object GetRequest(string uri, string jPath)
        {
            var client = new RestClient(uri);
            var response = client.Execute(new RestRequest(), RestSharp.Method.GET);
            JObject jObject = JObject.Parse(response.Content);
            var value = jObject.SelectToken($"$.{jPath}");
            return value; 
        }

        public static class Page 
        {
            public static By Apple = By.XPath("//label[@for='Apple']");
            public static By Samsung = By.XPath("//label[@for='Samsung']");
            //Price Area
            public static By UntilPrice = By.XPath("//div['rzdynamicfilterholder']//input[2]");
            public static By OkButton = By.XPath("//div[@data-filter-name='price']//div[@class='slider-filter__inner']/button");
            public static By MemoryAmount = By.XPath("//div//label[@for='128 ГБ']");
            public static By LowestPrice = By.XPath("//select/option[@value='1: cheap']");
            public static By ProductPrice = By.XPath("//div[@class='catalog']//p/span[1]");
        }
    }
}
