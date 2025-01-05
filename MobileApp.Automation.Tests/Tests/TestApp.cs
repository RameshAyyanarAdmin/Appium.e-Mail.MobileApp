using MobileApp.Automation.Utilities.CommonMethods;
using OpenQA.Selenium;

namespace MobileApp.Automation.Tests.Tests
{
    public class Tests : TestBase
    {
        
        [Test]
        public void VerifyOutlookEmailLink() {
            try
            {                
                Thread.Sleep(5000);
                IWebElement gotIT_button = CommonMethods.driver.FindElement(By.XPath("//android.widget.TextView[@text='GOT IT']"));             
                gotIT_button.Click();
                Thread.Sleep(5000);
                IWebElement Add_EmailAddress_link = CommonMethods.driver.FindElement(By.XPath("//android.widget.TextView[@text='Add an email address']"));
                Add_EmailAddress_link.Click();
                Thread.Sleep(5000);
                IWebElement Add_Outlook_link = CommonMethods.driver.FindElement(By.XPath("//android.widget.TextView[@text='Outlook, Hotmail, and Live']"));
                Add_Outlook_link.Click();                
                Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occured: " + ex.Message);
                throw;
            }
        }      
    }
}
