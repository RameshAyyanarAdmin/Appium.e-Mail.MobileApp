using MobileApp.Automation.Utilities.CommonMethods;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileApp.Automation.POM.PageFunctions
{
    public class EmailHomePage
    {

        private AppiumDriver<AppiumWebElement>? driver = CommonMethods.driver;

        IWebElement gotIT_button => driver.FindElement(By.XPath("//android.widget.TextView[@text='GOT IT']"));
        IWebElement Add_EmailAddress_link => driver.FindElement(By.XPath("//android.widget.TextView[@text='Add an email address']"));
        IWebElement Add_Outlook_link => driver.FindElement(By.XPath("//android.widget.TextView[@text='Outlook, Hotmail, and Live']"));
        IWebElement Outlook_userName => driver.FindElement(By.XPath("//android.widget.EditText[@resource-id='i0116']"));
        IWebElement Outlook_password => driver.FindElement(By.XPath("//android.view.View[@resource-id='i0281']/android.view.View[2]"));


        public void HomePage_AddEmail()
        {
            try
            {
                Thread.Sleep(5000);                
                CommonMethods.Click("gotIT_button", gotIT_button);
                Thread.Sleep(5000);                
                CommonMethods.Click("Add_EmailAddress_link", Add_EmailAddress_link);
            }
            catch(Exception ex){
                Console.WriteLine("An exception occured: " + ex.Message);
                Assert.Fail(ex.Message);
            }
        }

        public bool Access_OutlookEmail()
        {
            bool result = false;  
            try
            {
                Thread.Sleep(5000);
                CommonMethods.Click("Add_Outlook_link", Add_Outlook_link);
                Thread.Sleep(5000);
                CommonMethods.sendKeys("Outlook_userName", Outlook_userName, "DummyUser1@outlook.com");
                Thread.Sleep(5000);
                CommonMethods.sendKeys("Outlook_password", Outlook_password, "DummyPass1");
                result = true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occured: " + ex.Message);
                Assert.Fail(ex.Message);
            }
            return result;
        }
    }
}
