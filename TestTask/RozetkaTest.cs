using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace TestTask
{
    public class Tests
    {
        static IWebDriver driver;
        private string priceAmount = "20000";

        [SetUp]
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

            var price = driver.FindElement(Page.ProductPrice).GetAttribute("innerText");

            var prices = driver.FindElements(Page.ProductPrice);

            List<string> productPrices = prices.Select(p => p.GetAttribute("innerText")).ToList();

            foreach (var item in productPrices)
            {
                int num = Int32.Parse(item);
                Assert.LessOrEqual(num, 20000);
            }

            watch.Stop();
            Console.WriteLine($"Execution Time: {watch.ElapsedMilliseconds} ms");
        }
        
        /*
        [Test, Order(2)]
        public async System.Threading.Tasks.Task GetFilmsByNameAsync()
        {
            var url = "https://swapi.dev/api/people/1/";
            using var client = new HttpClient();

            var response = await client.GetAsync(url);
            string responseBody = await response.Content.ReadAsStringAsync();   
        }
        */

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
