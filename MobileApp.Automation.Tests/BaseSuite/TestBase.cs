using OpenQA.Selenium.Appium;
using MobileApp.Automation.Utilities.CommonMethods;
using MobileApp.Automation.Utilities;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports;
using AventStack.ExtentReports.Reporter.Config;
using System.Net;

namespace MobileApp.Automation.Tests.Tests
{
    public class TestBase
    {
        public static AppiumDriver<AppiumWebElement>? driver { get; set; }
        public static ProxyServer? proxy;
        public static ExtentSparkReporter ExtentSparkReporter;
        public static ExtentReports extent;
        public static ExtentTest extentTest;

        [OneTimeSetUp]
        public void BeforeAll()
        {
            try
            {
                Assert.That(
                Config.KobitonApiKey, Is.Not.EqualTo("your_kobiton_api_key"),
                "Please update value for the KobitonApiKey constant first. See more at README.md file."
            );
                TestData.TestDataInitialize();
                ExtentSparkReporter = new ExtentSparkReporter(Config.ExtentReportPath);
                ExtentSparkReporter.Config.Theme = Theme.Dark;
                ExtentSparkReporter.Config.Encoding = "utf-8";
                ExtentSparkReporter.Config.Protocol = Protocol.HTTPS;
                ExtentSparkReporter.Config.DocumentTitle = "Email App Automation";
                ExtentSparkReporter.Config.ReportName = "Test Execution Report";
                ExtentSparkReporter.Config.TimeStampFormat = "MMM dd , yyyy HH:mm:ss a";
                extent = new ExtentReports();
                extent.AddSystemInfo("Environment", "QA");
                extent.AddSystemInfo("Host Name", Dns.GetHostName());
                extent.AddSystemInfo("User Name", Environment.UserName);
                extent.AttachReporter(ExtentSparkReporter);
                AppiumOptions capabilities = Config.GetGalaxyS22Android14DesiredCapabilities();
                CommonMethods.FindOnlineDevice(capabilities);
                CommonMethods.AppiumDriverInitialization(capabilities, 1);
                CommonMethods.UpdateSettings();
                CommonMethods.SwitchToNativeContext();
                CommonMethods.SetImplicitWaitInMiliSecond(Config.ImplicitWaitInMs);
                driver = CommonMethods.driver;
                proxy = CommonMethods.proxy;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occured: " + ex.Message);
                extentTest.Log(Status.Error, ex.Message);
            }            
        }

        [SetUp]
        public void BeforeTest()
        {
            try
            {
                CommonMethods.currentTestCaseName = TestContext.CurrentContext.Test.MethodName;
                driver.LaunchApp();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occured: " + ex.Message);
                extentTest.Log(Status.Error, ex.Message);
            }           
        }

        [TearDown]
        public void AfterTest()
        {
            try
            {
                string testOutcome = TestContext.CurrentContext.Result.Outcome.Status.ToString();
                if (testOutcome != "Passed")
                {
                    extentTest.Log(Status.Fail, TestContext.CurrentContext.Test.MethodName + " Failed");
                }
                else
                {
                    extentTest.Log(Status.Pass, TestContext.CurrentContext.Test.MethodName + " Passed");
                }
                driver.CloseApp();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occured: " + ex.Message);
                extentTest.Log(Status.Error, ex.Message);
            }            
        }

        [OneTimeTearDown]
        public void AfterAll()
        {
            try
            {
                Cleanup();
                extent.Flush();
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occured: " + ex.Message);
                extentTest.Log(Status.Error, ex.Message);
            }
        }

        public async void Cleanup()
        {
            if (driver != null)
            {
                driver.Quit();
            }

            if (proxy != null)
            {
                proxy.StopProxy();
            }
        }
    }
}
