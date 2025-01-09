using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using System.Drawing;
using Castle.Core.Internal;
using HtmlAgilityPack;
using Newtonsoft.Json;
using System.Net;
using MobileApp.Automation.Tests.Tests;
using OpenQA.Selenium;
using Microsoft.CodeAnalysis;
using NUnit.Framework;
using AventStack.ExtentReports;

namespace MobileApp.Automation.Utilities.CommonMethods
{
    public class CommonMethods
    {
        public static AppiumDriver<AppiumWebElement>? driver { get; set;}
        public static AppiumOptions? options;
        public static ProxyServer? proxy;
        public static bool isIos;
        public static Point? screenSize;
        public static double retinaScale;
        public static string deviceName, platformVersion;
        public static HttpClient httpClient = new HttpClient();
        private static string currentContext;
        public static string IosXpathRedundantPrefix = "/AppiumAUT";
        public static string NativeContext = "NATIVE_APP";
        private const int SleepAfterAction = 200;
        public static ExtentTest extentTest { get; set; }

        public static void Click(string elementName, IWebElement element)
        {
            try
            {
                element.Click();
                extentTest.Log(Status.Pass, $"Clicked on {elementName}");
            }
            catch(Exception ex)
            {
                extentTest.Log(Status.Fail, $"Failed to click on {elementName}");
                Assert.Fail(ex.Message);
            }
        }

        public static void sendKeys(string elementName, IWebElement element, String testInput)
        {
            try
            {
                element.SendKeys(testInput);
                extentTest.Log(Status.Pass, $"Entered {testInput} in {elementName}");
            }
            catch (Exception ex)
            {
                extentTest.Log(Status.Fail, $"Failed to enter {testInput} in {elementName}");
                Assert.Fail(ex.Message);
            }
        }

        public static bool IsDisplayed(IWebElement element)
        {
            bool isDisplayed = false;
            try
            {
                Thread.Sleep(1000);
                isDisplayed = element.Displayed;
            }
            catch (Exception ex)
            {
                Assert.Fail(ex.Message);
            }
            return isDisplayed;
        }

        public static Device FindOnlineDevice(AppiumOptions capabilities)
        {
            if (Config.DeviceSource != Config.DeviceSourceEnums.Kobiton)
            {
                return null;
            }

            int tryTime = 1;
            Device device = null;
            string deviceName = (string)capabilities.ToCapabilities().GetCapability(MobileCapabilityType.DeviceName);
            string deviceGroup = (string)capabilities.ToCapabilities().GetCapability("deviceGroup");
            string platformVersion =
                (string)capabilities.ToCapabilities().GetCapability(MobileCapabilityType.PlatformVersion);
            string platformName =
                (string)capabilities.ToCapabilities().GetCapability(MobileCapabilityType.PlatformName);
            while (tryTime <= Config.DeviceWaitingMaxTryTimes)
            {
                Console.WriteLine(
                    $"Is device with capabilities: (deviceName: {deviceName}, deviceGroup: {deviceGroup}, platformName: {platformName}, platformVersion: {platformVersion}) online? Retrying at {Utils.ConvertToOrdinal(tryTime)} time");
                device = GetAvailableDevice(capabilities).Result;
                if (device != null)
                {
                    Console.WriteLine(
                        $"Device is found with capabilities: (deviceName: {deviceName}, deviceGroup: {deviceGroup}, platformName: {platformName}, platformVersion: {platformVersion})");
                    break;
                }
                tryTime++;
                sleep(Config.DeviceWaitingInternalInMs);
            }
            if (device == null)
            {
                throw new Exception(
                    $"Cannot find any online devices with capabilites: (deviceName: {deviceName}, deviceGroup: {deviceGroup}, platformName: {platformName}, platformVersion: {platformVersion})");
            }
            return device;
        }

        public static void sleep(int durationInMs)
        {
            Console.WriteLine($"Sleep for {durationInMs} ms");
            Thread.Sleep(durationInMs);
        }

        public static AppiumDriver<AppiumWebElement>? AppiumDriverInitialization(AppiumOptions desiredCaps, double retinaScale)
        {
            options = desiredCaps;
            retinaScale = retinaScale;
            isIos = MobilePlatform.IOS.Equals(desiredCaps.ToCapabilities()
                .GetCapability(MobileCapabilityType.PlatformName).ToString());
            deviceName = desiredCaps.ToCapabilities().GetCapability(MobileCapabilityType.DeviceName).ToString();
            platformVersion = desiredCaps.ToCapabilities().GetCapability(MobileCapabilityType.PlatformVersion)
            .ToString();
            if (Config.DeviceSource == Config.DeviceSourceEnums.Kobiton)
            {
                proxy = new ProxyServer();
                proxy.StartProxy();
            }
            Uri appiumServerUrl = GetAppiumServerUrl();
            if (isIos)
            {
                driver = new IOSDriver<AppiumWebElement>(appiumServerUrl, desiredCaps);
            }
            else
            {
                driver = new AndroidDriver<AppiumWebElement>(appiumServerUrl, desiredCaps);
            }
            Console.WriteLine($"View session at: https://portal.kobiton.com/sessions/{GetKobitonSessionId()}");
            return driver;
        }

        public static Uri GetAppiumServerUrl()
        {
            if (proxy != null)
            {
                return new Uri(proxy.GetServerUrl());
            }
            else
            {
                return new Uri($"{Config.AppiumServerUrl}/");
            }
        }

        public static void SwitchContext(string context)
        {
            if (currentContext == context) return;
            Console.WriteLine($"Switch to {context} context");
            driver.Context = context;
            currentContext = context;
        }

        public static void SwitchToNativeContext()
        {
            string currentContext = driver.Context;
            if (NativeContext.Equals(currentContext))
            {
                return;
            }

            SwitchContext(NativeContext);
        }

        protected HtmlDocument LoadXMLFromString(string xml)
        {
            HtmlDocument HtmlDocument = new HtmlDocument();
            HtmlDocument.LoadHtml(xml);
            return HtmlDocument;
        }

        public string SwitchToWebContext()
        {
            for (int tryTime = 1; tryTime <= 3; tryTime++)
            {
                Console.WriteLine($"Find a web context, {Utils.ConvertToOrdinal(tryTime)} time");
                List<ContextInfo> contextInfos = new List<ContextInfo>();

                SwitchToNativeContext();
                HtmlDocument nativeDocument = new HtmlDocument();
                nativeDocument = LoadXMLFromString(driver.PageSource);
                string textNodeSelector = isIos ? "//XCUIElementTypeStaticText" : "//android.widget.TextView";
                List<string> nativeTexts = new List<string>();
                var textNodes = nativeDocument.DocumentNode.SelectNodes(textNodeSelector);

                if (textNodes != null)
                {
                    foreach (HtmlNode element in nativeDocument.DocumentNode.SelectNodes(textNodeSelector))
                    {
                        if (element.NodeType != HtmlNodeType.Element) continue;
                        string textAttr = element.Attributes[isIos ? "value" : "text"].Value;
                        if (textAttr == null)
                            textAttr = "";
                        textAttr = textAttr.Trim().ToLower();
                        if (!string.IsNullOrEmpty(textAttr))
                            nativeTexts.Add(textAttr);
                    }
                }

                HashSet<string> contexts = new HashSet<string>(driver.Contexts);
                foreach (string context in contexts)
                {
                    if (context.StartsWith("WEBVIEW") || context.Equals("CHROMIUM"))
                    {
                        string source = null;
                        try
                        {
                            SwitchContext(context);
                            source = driver.PageSource;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Bad context {context}, error \"{ex.Message}\", skipping...");
                            continue;
                        }

                        if (source == null) continue;
                        ContextInfo contextInfo = contextInfos.FirstOrDefault(e => e.context.Equals(context));
                        if (contextInfo == null)
                        {
                            contextInfo = new ContextInfo(context);
                            contextInfos.Add(contextInfo);
                        }

                        contextInfo.sourceLength = source.Length;
                        if (nativeTexts.IsNullOrEmpty()) continue;

                        HtmlDocument htmlDoc = LoadXMLFromString(source);
                        HtmlNode bodyElements = htmlDoc.DocumentNode.SelectSingleNode("/html/body");
                        if (bodyElements == null) continue;

                        HtmlNode bodyElement = bodyElements.FirstChild;

                        string bodyString = bodyElement.InnerText.ToLower();
                        long matchTexts = 0;
                        foreach (string nativeText in nativeTexts)
                        {
                            if (bodyString.Contains(nativeText)) matchTexts++;
                        }

                        contextInfo.matchTexts = matchTexts;
                        contextInfo.matchTextsPercent = matchTexts * 100 / nativeTexts.Count();
                        if (contextInfo.matchTextsPercent >= 80)
                        {
                            break;
                        }
                    }
                }

                if (!contextInfos.IsNullOrEmpty())
                {
                    string bestWebContext;
                    contextInfos.Sort((ContextInfo c1, ContextInfo c2) =>
                        (int)(c2.matchTextsPercent - c1.matchTextsPercent));
                    if (contextInfos[0].matchTextsPercent > 40)
                    {
                        bestWebContext = contextInfos[0].context;
                    }
                    else
                    {
                        contextInfos.Sort((ContextInfo c1, ContextInfo c2) => (int)(c2.sourceLength - c1.sourceLength));
                        bestWebContext = contextInfos[0].context;
                    }

                    SwitchContext(bestWebContext);
                    SetImplicitWaitInMiliSecond(Config.ImplicitWaitInMs);
                    Console.WriteLine($"Switched to {bestWebContext} web context successfully");
                    return bestWebContext;
                }

                Thread.Sleep(10000);
            }

            throw new Exception("Cannot find any web context");
        }

        public static IOSDriver<AppiumWebElement> GetIosDriver()
        {
            return (IOSDriver<AppiumWebElement>)driver;
        }

        public static AndroidDriver<AppiumWebElement> GetAndroidDriver()
        {
            return (AndroidDriver<AppiumWebElement>)driver;
        }

        public static void SetImplicitWaitInMiliSecond(int value)
        {
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromMilliseconds(value);
        }

        public static void UpdateSettings()
        {
            if (!isIos)
            {
                GetAndroidDriver().IgnoreUnimportantViews(true);
            }
        }

        public static long GetKobitonSessionId()
        {
            return proxy != null ? proxy.kobitonSessionId : 0;
        }

        public static async Task<Device> GetAvailableDevice(AppiumOptions capabilities)
        {
            var deviceListUriBuilder = new UriBuilder(Config.KobitonApiUrl + "/v1/devices");
            var query = new Dictionary<string, string>
            {
                { "isOnline", "true" },
                { "isBooked", "false" },
                {
                    "deviceName",
                    capabilities.ToCapabilities().GetCapability(MobileCapabilityType.DeviceName).ToString()
                },
                {
                    "platformVersion",
                    capabilities.ToCapabilities().GetCapability(MobileCapabilityType.PlatformVersion).ToString()
                },
                {
                    "platformName",
                    capabilities.ToCapabilities().GetCapability(MobileCapabilityType.PlatformName).ToString()
                },
                { "deviceGroup", capabilities.ToCapabilities().GetCapability("deviceGroup").ToString() }
            };
            deviceListUriBuilder.Query = new FormUrlEncodedContent(query).ReadAsStringAsync().Result;

            using (var httpClient = new HttpClient())
            {
                httpClient.DefaultRequestHeaders.Add(HttpRequestHeader.Authorization.ToString(),
                    Config.GetBasicAuthString());

                var response = await httpClient.GetAsync(deviceListUriBuilder.Uri);

                if (!response.IsSuccessStatusCode)
                {
                    throw new Exception(await response.Content.ReadAsStringAsync());
                }

                var responseContent = await response.Content.ReadAsStringAsync();

                var deviceListResponse = JsonConvert.DeserializeObject<DeviceListResponse>(responseContent);

                var deviceList = new List<Device>();
                deviceList.AddRange(deviceListResponse.cloudDevices);
                deviceList.AddRange(deviceListResponse.privateDevices);

                if (deviceList.Count == 0)
                {
                    return null;
                }

                return deviceList[0];
            }
        }




        public class Device
        {
            public long id;
            public bool isBooked, isOnline, isFavorite, isCloud;
            public string deviceName, platformName, platformVersion, udid;
        }

        public class DeviceListResponse
        {
            public List<Device> privateDevices;
            public List<Device> favoriteDevices;
            public List<Device> cloudDevices;
        }

        public class GenericLocator
        {
            public string type, value;

            public GenericLocator(string type, string value)
            {
                this.type = type;
                this.value = value;
            }
        }

        public class ContextInfo
        {
            public string context;
            public long sourceLength, matchTexts, matchTextsPercent;

            public ContextInfo(string context)
            {
                this.context = context;
            }
        }

    }
}
