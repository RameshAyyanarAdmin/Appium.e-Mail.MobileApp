using OpenQA.Selenium.Appium;

namespace MobileApp.Automation.Utilities
{
    public class Config
    {
        public enum DeviceSourceEnums { Kobiton, SauceLabs }

        public const string KobitonUserName = "Ramesh.sep89";
        public const string KobitonApiKey = "7c554b1b-27a0-4c20-9e4e-479095db50b6";
        public const string AppiumServerUrl = "https://" + KobitonUserName + ":" + KobitonApiKey + "@api-prod-blue.kobiton.com/wd/hub";
        public const DeviceSourceEnums DeviceSource = DeviceSourceEnums.Kobiton;
        public const int ImplicitWaitInMs = 30000;
        public const int DeviceWaitingMaxTryTimes = 5;
        public const int DeviceWaitingInternalInMs = 30000;
        public const int VisibilityTimeoutInMs = 60000;
        public const int SleepTimeBeforeSendKeysInMs = 3000;
        public const string KobitonApiUrl = "https://api-prod-blue.kobiton.com";
        public const string AndroidEmailApp = "com.google.android.gm";
        public static string ExtentReportPath = Directory.GetCurrentDirectory().Split("\\bin\\Debug\\")[0] + "\\Test-Results\\ExtentReport.html";


        public static string GetBasicAuthString()
        {
            string authString = KobitonUserName + ":" + KobitonApiKey;
            byte[] authEncBytes = System.Text.Encoding.UTF8.GetBytes(authString);
            string authEncString = Convert.ToBase64String(authEncBytes);
            return "Basic " + authEncString;
        }

        public static AppiumOptions GetGalaxyS22Android14DesiredCapabilities()
        {
            AppiumOptions capabilities = new AppiumOptions();
            capabilities.AddAdditionalCapability("sessionName", "Automation on Galaxy S22");
            capabilities.AddAdditionalCapability("sessionDescription", "");
            capabilities.AddAdditionalCapability("deviceOrientation", "portrait");
            capabilities.AddAdditionalCapability("noReset", false);
            capabilities.AddAdditionalCapability("fullReset", true);
            capabilities.AddAdditionalCapability("captureScreenshots", true);
            capabilities.AddAdditionalCapability("newCommandTimeout", 900);
            capabilities.AddAdditionalCapability("keepScreenOn", true);
            capabilities.AddAdditionalCapability("ensureWebviewsHavePages", true);
            capabilities.AddAdditionalCapability("kobiton:flexCorrect", false);
            capabilities.AddAdditionalCapability("kobiton:baselineSessionId", 7545037);
            capabilities.AddAdditionalCapability("kobiton:includeSystemWindows", true);
            capabilities.AddAdditionalCapability("appPackage", AndroidEmailApp);
            capabilities.AddAdditionalCapability("deviceGroup", "KOBITON");
            capabilities.AddAdditionalCapability("deviceName", "Galaxy S22");
            capabilities.AddAdditionalCapability("platformVersion", "14");
            capabilities.AddAdditionalCapability("platformName", "Android");
            capabilities.AddAdditionalCapability("autoLaunch", false);
            return capabilities;
        }
    }
}
