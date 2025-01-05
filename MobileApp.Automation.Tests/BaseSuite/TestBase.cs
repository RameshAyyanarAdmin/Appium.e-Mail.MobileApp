using OpenQA.Selenium.Appium;
using MobileApp.Automation.Utilities.CommonMethods;
using MobileApp.Automation.Utilities;

namespace MobileApp.Automation.Tests.Tests
{
    public class TestBase
    {
        public static AppiumDriver<AppiumWebElement>? driver { get; set; }
        public static ProxyServer? proxy;

        [OneTimeSetUp]
        public void BeforeAll()
        {
            Assert.That(
                Config.KobitonApiKey, Is.Not.EqualTo("your_kobiton_api_key"),
                "Please update value for the KobitonApiKey constant first. See more at README.md file."
            );
            AppiumOptions capabilities = Config.GetGalaxyS22Android14DesiredCapabilities();
            CommonMethods.FindOnlineDevice(capabilities);
            CommonMethods.AppiumDriverInitialization(capabilities, 1);
            CommonMethods.UpdateSettings();
            CommonMethods.SwitchToNativeContext();
            CommonMethods.SetImplicitWaitInMiliSecond(Config.ImplicitWaitInMs);
            driver = CommonMethods.driver;
            proxy = CommonMethods.proxy;
        }

        [SetUp]
        public void BeforeTest()
        {
            driver.LaunchApp();
        }

        [TearDown]
        public void AfterTest()
        {
            driver.CloseApp();
        }

        [OneTimeTearDown]
        public void AfterAll()
        {
            Cleanup();
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
