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
    class UIMailLimitation:WebOperation
    {
        

        public UIMailLimitation(string DDEI_IP, string username, string passwrod) : base(DDEI_IP,username,passwrod)
        {
        }

        public void SetMaxMsgSize(int size)
        {
            IWebElement edit = driver.FindElement(By.XPath("//input[@name='maxMsgSize']"));
            edit.Clear();
            edit.SendKeys(size.ToString());
        }

        public void SetMaxRecipient(int number)
        {
            IWebElement edit = driver.FindElement(By.XPath("//input[@name='maxMsgRecipient']"));
            edit.Clear();
            edit.SendKeys(number.ToString());
        }

        public void SetPermitedSender(PermittedSenderType type)
        {
            if (type == PermittedSenderType.Subnet)
            {
                driver.FindElement(By.XPath("(//input[@name='myNetworkStyle'])[2]")).Click();

            }
            if (type == PermittedSenderType.Class)
            {
                try
                {
                    IWebElement Class_radioBox = wait.Until<IWebElement>((d) =>
                    {
                       return d.FindElement(By.XPath("(//input[@name='myNetworkStyle'])[3]")); 
                    });

                    Class_radioBox.Click();

                    //Thread.Sleep(1000);
                }
                catch (Exception e)
                {
                    Console.WriteLine("exception: {0}", e.Message);
                }
            }
        }

        public void SetPermitedSender(PermittedSenderType type, string IP)
        {
            if (type == PermittedSenderType.Specify)
            {
                driver.FindElement(By.XPath("(//input[@name='myNetworkStyle'])[4]")).Click();

            }
            driver.FindElement(By.XPath("//input[@name='addrType']")).Click();

            //input IP address
            IWebElement edit = driver.FindElement(By.XPath("//input[@name='singleAddr']"));
            edit.Clear();
            edit.SendKeys(IP);
            //click add
            driver.FindElement(By.XPath("//input[@name='addButton2']")).Click();
        }

        public void SetPermitedSender(PermittedSenderType type, groupIdentity group, protocolType IPProtocol)
        {
            if (type == PermittedSenderType.Specify)
            {
                driver.FindElement(By.XPath("(//input[@name='myNetworkStyle'])[4]")).Click();

            }
            driver.FindElement(By.XPath("(//input[@name='addrType'])[2]")).Click();

            //input IP address
            IWebElement edit = driver.FindElement(By.XPath("//input[@name='subnetAddr']"));
            IWebElement edit2 = driver.FindElement(By.XPath("//input[@name='subnetMask']"));


            //set IP protocol
            SelectElement pro = new SelectElement(driver.FindElement(By.XPath("//select[@name='ipType']")));
            if (IPProtocol == protocolType.IPV4)
            {
                pro.SelectByIndex(0);
                
                edit.Clear();
                edit.SendKeys(group.IPrange);
                edit2.Clear();
                edit2.SendKeys(group.netMask);
            }
            else
            {
                //IPv6
                pro.SelectByIndex(1);
                edit.Clear();
                edit.SendKeys(group.IPrange+group.netMask);
            }
            //click add
            driver.FindElement(By.XPath("//input[@name='addButton2']")).Click();
        }

        public void Save()
        {
            WebDriverWait wait = new WebDriverWait(driver, TimeSpan.FromSeconds(10));
            wait.Until((d) => { return d.FindElement(By.XPath("//li[@id='smtp_rule_save']/span")); });

            IWebElement save_btn = driver.FindElement(By.XPath("//li[@id='smtp_rule_save']/span"));
            save_btn.Click();
            while (!driver.PageSource.Contains("Saved successfully"))
            {
                Thread.Sleep(500);
            }
            
        }

    }
}
