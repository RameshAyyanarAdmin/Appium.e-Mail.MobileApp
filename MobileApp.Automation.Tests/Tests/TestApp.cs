using MobileApp.Automation.POM.PageObjects;
using MobileApp.Automation.Utilities.CommonMethods;
using OpenQA.Selenium;
using System.Security.Cryptography.X509Certificates;

namespace MobileApp.Automation.Tests.Tests
{
    public class Tests : TestBase
    {
        
        [Test]
        [Category("Andriod"), Author("Ramesh")]
        public void VerifyOutlookEmail() {
            try
            {
                extentTest = extent.CreateTest(TestContext.CurrentContext.Test.Name);
                CommonMethods.extentTest = extentTest;
                Page.EmailHomePage.HomePage_AddEmail();
                Page.EmailHomePage.Access_OutlookEmail();               
            }
            catch (Exception ex)
            {
                Console.WriteLine("An exception occured: " + ex.Message);
                Assert.Fail(ex.Message);
            }
        }      
    }
}
