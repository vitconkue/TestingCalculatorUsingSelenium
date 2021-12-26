using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
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

        private async Task<Output> RunInput(Input input, string buildVersion)
        {
            Driver.Navigate().GoToUrl(_webLink);
            Driver.FindElement(By.Id("selectBuild")).Click();
            {
                var dropdown = Driver.FindElement(By.Id("selectBuild"));
                dropdown.FindElement(By.XPath($"//option[. = '{buildVersion}']")).Click();
            }
            Driver.FindElement(By.Id("number1Field")).Click();
            Driver.FindElement(By.Id("number1Field")).SendKeys(input.Number1);
            Driver.FindElement(By.Id("number2Field")).Click();
            Driver.FindElement(By.Id("number2Field")).SendKeys(input.Number2);
            Driver.FindElement(By.Id("selectOperationDropdown")).Click();
            {
                var dropdown = Driver.FindElement(By.Id("selectOperationDropdown"));
                dropdown.FindElement(By.XPath($"//option[. = '{input.Operation}']")).Click();
            }
            if (input.WillClear)
            {
                Driver.FindElement(By.Id("clearButton")).Click();
            }

            if (input.CheckIntegerOnly)
            {
                Driver.FindElement(By.Id("integerSelect")).Click();
            }

            if (input.WillCalculate)
            {
                Driver.FindElement(By.Id("calculateButton")).Click();
            }

            await Task.Delay(500);
            return new Output()
            {
                Number1Field = Driver.FindElement(By.Id("number1Field")).GetAttribute("value"),
                Number2Field = Driver.FindElement(By.Id("number2Field")).GetAttribute("value"),
                ResultField = Driver.FindElement(By.Id("numberAnswerField")).GetAttribute("value"),
                ErrorField = Driver.FindElement(By.Id("errorMsgField")).Text
            };
        }


        [Theory]
        [MemberData(nameof(GetAllEnvironmentAndBuildVersion))]
        public async Task ValidMultiplyInput(string browserName, string buildVersion, TestCase testCase)
        {
            Driver = WebDriverFactory(browserName);
            var input = testCase.Input;
            var actualOutput = await
                RunInput(input, buildVersion);
            Assert.Equal(testCase.Output, actualOutput);
        }



        // for reusing the test for all browser and build version 
        public static List<object[]> GetAllEnvironmentAndBuildVersion()
        {
            var browsers = new List<string>
            {
                //"firefox", "chrome", "edge"
                "firefox","edge","chrome"
            };

            var buildVersions = new List<string>
            {
                //"Prototype", "1", "2", "3", "4", "5", "6", "7", "8", "9"
                "8"
            };

            var allTestCases = ReadTestCases();


            return (from browser in browsers 
                    from buildVersion in buildVersions 
                    from testCase in allTestCases 
                    select new object[] {browser, buildVersion, testCase})
                .ToList();
        }
        
        private static List<TestCase> ReadTestCases()
        {
            string fileName = "../../../TestCases.json";
            string jsonString = File.ReadAllText(fileName);
            var testCases = JsonSerializer.Deserialize<List<TestCase>>(jsonString);
            return testCases;
        }
    }
}