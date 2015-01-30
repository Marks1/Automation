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
    public struct MailRouting{
      public string destDomain;
      public string deliveryIP;
      public string port;
    };

    class UIMailDeliverySetting:WebOperation
    {
        public UIMailDeliverySetting(string DDEI_IP, string username, string passwrod)
            : base(DDEI_IP, username, passwrod)
        { }

        public void AddDeliveryRule(MailRouting rule)
        {
            //click add button
            driver.FindElement(By.Id("btn_add")).Click();
            //look up the new popup window
            var currentWindow = driver.CurrentWindowHandle;
            if (SwitchWindow("Destination Domain"))
            {
                //in the pop up dialog
                driver.FindElement(By.XPath("//input[@name='domainName']")).SendKeys(rule.destDomain);
                driver.FindElement(By.XPath("//input[@name='server']")).SendKeys(rule.deliveryIP);
                driver.FindElement(By.XPath("//input[@name='port']")).SendKeys(rule.port);
                //save in popup
                driver.FindElement(By.XPath("//input[@value='OK']")).Click();
            }
            //switch back to parent page.
            driver.SwitchTo().Window(currentWindow);
            driver.SwitchTo().Frame(driver.FindElement(By.Id("middle_page")));
            driver.SwitchTo().Frame(driver.FindElement(By.Id("tab2")));

        }

        public void Save()
        {
            driver.FindElement(By.XPath("//li[@id='smtp_delivery_save']/span")).Click();
            while (!driver.PageSource.Contains("Saved successfully"))
            {
                Thread.Sleep(500);
            }
        }


        protected Boolean SwitchWindow(string title)
        {
            var currentWindow = driver.CurrentWindowHandle;
            var availableWindows = new List<string>(driver.WindowHandles);

            foreach (string w in availableWindows)
            {
                if (w != currentWindow)
                {
                    driver.SwitchTo().Window(w);
                    if (driver.Title.Contains(title))
                        return true;
                    else
                    {
                        driver.SwitchTo().Window(currentWindow);
                    }

                }
            }
            return false;
        }
    }
}
