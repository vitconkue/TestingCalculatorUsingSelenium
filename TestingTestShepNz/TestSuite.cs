using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Edge;
using OpenQA.Selenium.Firefox;
using Xunit;
using Xunit.Abstractions;

namespace TestingTestShepNz
{
    public class TestSuite : IDisposable
    {
        private readonly ITestOutputHelper _testOutputHelper;
        private readonly string _webLink = "https://testsheepnz.github.io/BasicCalculator.html";
        private IWebDriver Driver { get; set; }
        private Dictionary<string, string> Vars { get; set; }

        private IWebDriver WebDriverFactory(string browserName)
        {
            return browserName switch
            {
                "firefox" => new FirefoxDriver(),
                "chrome" => new ChromeDriver(),
                "edge" => new EdgeDriver(),
                _ => new ChromeDriver()
            };
        }

        public TestSuite(ITestOutputHelper testOutputHelper)
        {
            _testOutputHelper = testOutputHelper;
            Vars = new Dictionary<string, string>()
            {
                {"numberAnswerField", ""},
                {"errorMsgField", ""}
            };
        }

        public void Dispose()
        {
            Driver.Quit();
        }

        private async Task RunInput(string number1,
            string number2,
            string buildVersion,
            string operation,
            bool checkIntegerOnly,
            bool willCalculate,
            bool willClear)
        {
            Driver.Navigate().GoToUrl(_webLink);
            Driver.FindElement(By.Id("selectBuild")).Click();
            {
                var dropdown = Driver.FindElement(By.Id("selectBuild"));
                dropdown.FindElement(By.XPath($"//option[. = '{buildVersion}']")).Click();
            }
            Driver.FindElement(By.Id("number1Field")).Click();
            Driver.FindElement(By.Id("number1Field")).SendKeys(number1);
            Driver.FindElement(By.Id("number2Field")).Click();
            Driver.FindElement(By.Id("number2Field")).SendKeys(number2);
            Driver.FindElement(By.Id("selectOperationDropdown")).Click();
            {
                var dropdown = Driver.FindElement(By.Id("selectOperationDropdown"));
                dropdown.FindElement(By.XPath($"//option[. = '{operation}']")).Click();
            }
            Driver.FindElement(By.Id("calculateButton")).Click();
            //await Task.Delay(3000);
            string text = "";
            while (text == "")
            {
                text = Driver.FindElement(By.Id("numberAnswerField")).GetAttribute("value");
            }
            Vars["numberAnswerField"] = text;
            Driver.FindElement(By.Id("clearButton")).Click();
        }


        [Theory]
        [MemberData(nameof(GetAllEnvironment))]
        public async Task ValidMultiplyInput(string browserName, string buildVersion)
        {
            Driver = WebDriverFactory(browserName);
            await
                RunInput("3", "5", buildVersion, "Multiply", false, true, false);
            Assert.Equal("15", Vars["numberAnswerField"]);
        }


        // for reusing the test for all browser and build version 
        public static List<object[]> GetAllEnvironment()
        {
            var allEnvi = new List<object[]>();
            var browsers = new List<string>
            {
                "firefox", "chrome", "edge"
            };

            var buildVersions = new List<string>
            {
                //"Prototype", "1", "2", "3", "4", "5", "6", "7", "8", "9"
                 "8" 
            };

            foreach (var browser in browsers)
            foreach (var buildVersion in buildVersions)
                allEnvi.Add(new object[] {browser, buildVersion});


            return allEnvi;
        }
    }
}