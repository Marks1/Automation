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
    class UIMailConnetion : WebOperation
    {

        public UIMailConnetion(string DDEI_IP, string username, string passwrod)
            : base(DDEI_IP, username, passwrod)
        {
        }



        public void SetPort(int port)
        {

            try
            {
                IWebElement portElement = driver.FindElement(By.XPath("//input[@name='port']"));
                portElement.Clear();
                portElement.SendKeys(port.ToString());

                //click save
                //driver.FindElement(By.XPath("//li/span")).Click();
            }
            catch (Exception e)
            {
                Console.Write("error {0}", e.Message);
            }
        }

        public void SetTimeout(int timeout)
        {
            try
            {
                IWebElement timeoutElement = driver.FindElement(By.Name("timeout"));
                timeoutElement.Clear();
                timeoutElement.SendKeys(timeout.ToString());
            }
            catch (Exception e)
            {
                Console.Write("error {0}", e.Message);
            }
        }

        //connectionNum = -1 means no limitation on UI
        public void SetMacConnection(int connectionNum)
        {
            try
            {
                IWebElement connectionRadioElement = driver.FindElement(By.Name("maxConnType"));
                IWebElement connectionElement = driver.FindElement(By.Name("maxConnNum"));
                //no limit
                if (connectionNum == -1)
                {
                    connectionRadioElement.Click();
                }
                else
                {
                    connectionElement.Clear();
                    connectionElement.SendKeys(connectionNum.ToString());
                }
            }
            catch (Exception e)
            {
                Console.Write("error {0}", e.Message);
            }
        }


        public void AddAllowedSingleUser(string userIdentity)
        {
            try
            {

                //check the first radio box to set allow list
                driver.FindElement(By.XPath("//tr[9]/td/input")).Click();

                driver.FindElement(By.Name("denySingleAddr")).SendKeys(userIdentity);
                driver.FindElement(By.Name("Apply22222")).Click();
            }
            catch (Exception e)
            {
                Console.Write("error {0}", e.Message);
            }

        }

        public void AddAllowedGroup(groupIdentity group, protocolType IpVersion)
        {
            //check the first radio box to set allow list
            driver.FindElement(By.XPath("//tr[9]/td/input")).Click();

            //check the group radio
            driver.FindElement(By.XPath("(//input[@name='denyAddrType'])[2]")).Click();

            //find list element
            SelectElement pro = new SelectElement(driver.FindElement(By.Name("ipTypeofDeny")));
            if (IpVersion == protocolType.IPV4)
            {
                pro.SelectByIndex(0);
                driver.FindElement(By.Name("denySubnetMask")).SendKeys(group.netMask);
            }
            else
            {
                //IPv6
                pro.SelectByIndex(1);
            }

            //set value
            driver.FindElement(By.Name("denySubnetAddr")).SendKeys(group.IPrange);


            //apply 
            driver.FindElement(By.Name("Apply22222")).Click();

            Thread.Sleep(1000);

        }

        public void AddDenyedSingleUser(string userIdentity)
        {
            //check the second radio box to set deny list
            driver.FindElement(By.XPath("//tr[11]/td/input")).Click();

            driver.FindElement(By.Name("acceptSingleAddr")).SendKeys(userIdentity);
            driver.FindElement(By.Name("Apply22222")).Click();
        }

        public void AddDenyedGroup(groupIdentity group, protocolType IpVersion)
        {
            //check the first radio box to set allow list
            driver.FindElement(By.XPath("//tr[11]/td/input")).Click();

            //check the group radio
            driver.FindElement(By.XPath("(//input[@name='acceptAddrType'])[2]")).Click();

            //find list element
            SelectElement pro = new SelectElement(driver.FindElement(By.Name("ipTypeofAccept")));
            if (IpVersion == protocolType.IPV4)
            {
                pro.SelectByIndex(0);
                driver.FindElement(By.Name("acceptSubnetMask")).SendKeys(group.netMask);
            }
            else
            {
                //IPv6
                pro.SelectByIndex(1);
            }

            //set value
            driver.FindElement(By.Name("acceptSubnetAddr")).SendKeys(group.IPrange);

            //apply 
            driver.FindElement(By.Name("Apply22222")).Click();

        }

        public void EnableIncomingTLS(bool enable)
        {
            IWebElement checkboxEnableIncomingTLS = driver.FindElement(By.XPath("//input[@name='useTLS']"));

            if (enable == true && !checkboxEnableIncomingTLS.Selected)
            {
                checkboxEnableIncomingTLS.Click();
            }
            if (enable == false && checkboxEnableIncomingTLS.Selected)
            {
                checkboxEnableIncomingTLS.Click();
            }
        }

        public void EnableOutgoingTLS(bool enable)
        {
            IWebElement checkboxEnableOutgoingTLS = driver.FindElement(By.XPath("//input[@name='enforceTLS']"));

            if (enable == true && !checkboxEnableOutgoingTLS.Selected)
            {
                checkboxEnableOutgoingTLS.Click();
            }
            if (enable == false && checkboxEnableOutgoingTLS.Selected)
            {
                checkboxEnableOutgoingTLS.Click();
            }
        }

        public void EnableForceTLS(bool enable)
        {
            EnableIncomingTLS(true);

            IWebElement checkboxEnableForceTLS = driver.FindElement(By.XPath("//input[@name='useOutgoingTLS']"));

            if (enable == true && !checkboxEnableForceTLS.Selected)
            {
                checkboxEnableForceTLS.Click();
            }
            if (enable == false && checkboxEnableForceTLS.Selected)
            {
                checkboxEnableForceTLS.Click();
            }
        }

        public void Save()
        {
            
            wait.Until((d) => { return d.FindElement(By.XPath("//li[@id='smtp_conn_save']/span")); });

            IWebElement save_btn = driver.FindElement(By.XPath("//li[@id='smtp_conn_save']/span"));
            while (!save_btn.Displayed)
            {
                Console.WriteLine("wait displayed - save btn");
                Thread.Sleep(500);
            }
            save_btn.Click();
            Console.WriteLine("Save");
            Thread.Sleep(500);
            //while (!driver.PageSource.Contains("Saved successfully"))
            //{
            //    Console.WriteLine("wait result - save btn");
            //    Thread.Sleep(500);
            //}
        }
    }
}
