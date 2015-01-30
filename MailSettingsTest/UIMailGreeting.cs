using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Support.UI;
using OpenQA.Selenium.Interactions;


namespace MailSettingsTest
{
    class UIMailGreeting:WebOperation
    {
        public UIMailGreeting(string DDEI_IP, string username, string passwrod)
            : base(DDEI_IP, username, passwrod)
        { }

        public void SetGreetingString(string greeting)
        {
            IWebElement edit = driver.FindElement(By.Id("greetingMsg_text"));
            edit.Clear();
            edit.SendKeys(greeting);
        }

        public void Save()
        {
            driver.FindElement(By.XPath("//li[@id='greeting_save']/span")).Click();
            while (!driver.PageSource.Contains("Saved successfully"))
            {
                Thread.Sleep(500);
            }
        }
    }
}
