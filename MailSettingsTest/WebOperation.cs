using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;

namespace MailSettingsTest
{

    public enum protocolType { IPV4, IPV6 };
    public enum PermittedSenderType { Subnet, Class, Specify };
    public struct groupIdentity
    {
        public string IPrange;
        public string netMask;
    };

    
    //base class
    public class WebOperation 
    {

        public IWebDriver driver;
        public WebDriverWait wait;

        public IWebDriver WebInstance
        {
            get
            {
                return driver;
            }
        }

        public WebOperation(string DDEI_IP, string username, string passwrod)
        {
            
            driver = new FirefoxDriver();

            //set global timeout to 10 seconds
            driver.Manage().Timeouts().ImplicitlyWait(TimeSpan.FromSeconds(10));
            
            wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));

            try
            {
                string URL = string.Format(@"https://{0}", DDEI_IP);
                driver.Navigate().GoToUrl(URL);

                wait.Until((d) => { return d.Title.StartsWith("Deep Discovery Email Inspector Login"); });
                
                //login
                IWebElement usernameElement = driver.FindElement(By.Id("userid"));
                IWebElement passwordElement = driver.FindElement(By.Id("password"));
                IWebElement login_btnElement = driver.FindElement(By.Id("login_btn"));

                usernameElement.SendKeys(username);
                passwordElement.SendKeys(passwrod);
                login_btnElement.Click();
            }
            catch (Exception e)
            {

                Console.Write("error {0}", e.Message);
            }

        }

        ~WebOperation()
        {
            //driver.Quit();
        }


        public void Delete()
        {
            driver.Quit();
        }

        public void Switch2ConnectionPage()
        {
            try
            {
                IWebElement adminNavigation = driver.FindElement(By.Id("Admin"));
                Actions mover = new Actions(driver);
                mover.MoveToElement(adminNavigation).Perform();
                IJavaScriptExecutor js = driver as IJavaScriptExecutor;
                js.ExecuteScript("document.getElementById('Navigation__SMTPRouting').click()");

                driver.SwitchTo().DefaultContent();
                driver.SwitchTo().Frame(driver.FindElement(By.Id("middle_page")));


                WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
                IWebElement login = wait.Until<IWebElement>((d) =>
                {
                    return d.FindElement(By.XPath("//li/span"));
                });
            }
            catch (Exception e)
            {
                Console.Write("error {0}", e.Message);
                driver.Quit();
            }
            Thread.Sleep(1000);
        }

        public void Switch2LimitationPage()
        {
            Switch2ConnectionPage();

            driver.FindElement(By.Id("a_tab_rule")).Click();
            driver.SwitchTo().Frame(driver.FindElement(By.Id("tab3")));

            Thread.Sleep(1000);
        }

        public void Switch2GreetingPage()
        {
            Switch2ConnectionPage();
            
            driver.FindElement(By.Id("a_tab_smtp")).Click();
            driver.SwitchTo().Frame(driver.FindElement(By.Id("tab4")));
            Thread.Sleep(1000);
        }


        public void Switch2DeliveryPage()
        {
            Switch2ConnectionPage();

            driver.FindElement(By.Id("a_tab_delivery")).Click();
            driver.SwitchTo().Frame(driver.FindElement(By.Id("tab2")));

            Thread.Sleep(1000);
        }

    }
}
