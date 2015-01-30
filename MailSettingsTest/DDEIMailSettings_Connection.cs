using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;


namespace MailSettingsTest
{
    using OpenQA.Selenium;
    using OpenQA.Selenium.Firefox;
    using OpenQA.Selenium.Support.UI;

    [TestClass]
    public class DDEIMailSettings_Connection
    {
        private static UIMailConnetion ddeiWebdriver;
        private static SSHOperation ddeiSSH;

        //[TestInitialize]
        //public DDEIMailSettings_Connection()
        // above initilization function will be called at every case execution cycle, whick make unnecesry 
        //browser startup
        [ClassInitialize]
        public static void BeforeTest(TestContext test)
        {
            ddeiWebdriver = new UIMailConnetion("10.64.70.250", "admin", "ddei");
            ddeiSSH = new SSHOperation("10.64.70.250", "root", "111111");

            if (ddeiWebdriver == null || ddeiSSH == null)
            {
                Assert.Fail();
            }

            ddeiWebdriver.Switch2ConnectionPage();
        }
        [ClassCleanup]
        public static void AfterTest()
        {
            ddeiWebdriver.Delete();
        }

        [TestMethod]
        public void ChangeSmtpPort()
        {
            //set new SMTP port to 2025
            int newPort = 2025;
            ddeiWebdriver.SetPort(newPort);
            ddeiWebdriver.Save();
            Assert.AreEqual(newPort, Int32.Parse(ddeiSSH.GetRuntimePostfixPort()));
        }

        [TestMethod]
        public void ChangeTimeout()
        {
            //set new timeout value to 2 minutes
            int timeout = 2;
            ddeiWebdriver.SetTimeout(timeout);
            ddeiWebdriver.Save();

            timeout = timeout * 60;
            Assert.AreEqual(timeout.ToString() + "s", ddeiSSH.GetRuntimePostfixConf("smtpd_timeout"));
        }


        [TestMethod]
        public void ChangeUnlimitedSMTPConnetions()
        {
            ddeiWebdriver.SetMacConnection(-1);
            ddeiWebdriver.Save();

            Assert.AreEqual(0, Int32.Parse(ddeiSSH.getRuntimePostfixConn()));
        }

        [TestMethod]
        public void ChangeSMTPConnections()
        {
            ddeiWebdriver.SetMacConnection(100);
            ddeiWebdriver.Save();

            Assert.AreEqual(100, Int32.Parse(ddeiSSH.getRuntimePostfixConn()));
        }

        [TestMethod]
        public void AddSingleAllowedComputer()
        {
            string IP = "10.64.1.1";
            ddeiWebdriver.AddAllowedSingleUser(IP);
            ddeiWebdriver.Save();

            string denyRule = ddeiSSH.ExecuteCMD(@"cat /opt/trend/ddei/postfix/etc/postfix/denyAccessList | grep " + IP);

            Assert.AreNotEqual(denyRule.Trim().Length, 0);
        }

        [TestMethod]
        public void AddGroupAllowedGroupV4()
        {
            groupIdentity group;
            group.IPrange = "192.168.1.0";
            group.netMask = "255.255.255.0";

            ddeiWebdriver.AddAllowedGroup(group, protocolType.IPV4);

            ddeiWebdriver.Save();

            string denyRule = ddeiSSH.ExecuteCMD(@"cat /opt/trend/ddei/postfix/etc/postfix/denyAccessList | grep " + group.IPrange + @"/24");

            Assert.AreNotEqual(denyRule.Trim().Length, 0);

        }

        [TestMethod]
        public void AddGroupAllowedGroupV6()
        {
            groupIdentity group;
            group.IPrange = "2001:db8::/32";
            group.netMask = "255.255.255.0";

            ddeiWebdriver.AddAllowedGroup(group, protocolType.IPV6);
            ddeiWebdriver.Save();

            string denyRule = ddeiSSH.ExecuteCMD(@"cat /opt/trend/ddei/postfix/etc/postfix/denyAccessList | grep " + group.IPrange);

            Assert.AreNotEqual(denyRule.Trim().Length, 0);

        }

        [TestMethod]
        public void IncomingTLS_enable()
        {
            ddeiWebdriver.EnableIncomingTLS(true);
            ddeiWebdriver.EnableForceTLS(false);
            ddeiWebdriver.EnableOutgoingTLS(false);

            ddeiWebdriver.Save();

            string level = ddeiSSH.GetRuntimePostfixConf("smtpd_tls_security_level");
            Assert.AreEqual("may", level);
        }

        [TestMethod]
        public void IncomingTLS_disable()
        {
            ddeiWebdriver.EnableIncomingTLS(false);
            ddeiWebdriver.EnableForceTLS(false);
            ddeiWebdriver.EnableOutgoingTLS(false);

            ddeiWebdriver.Save();

            string level = ddeiSSH.GetRuntimePostfixConf("smtpd_tls_security_level");
            Assert.AreEqual("none", level);
        }

        [TestMethod]
        public void IncomingTLS_force()
        {
            ddeiWebdriver.EnableIncomingTLS(true);
            ddeiWebdriver.EnableForceTLS(true);
            ddeiWebdriver.EnableOutgoingTLS(false);

            ddeiWebdriver.Save();

            string level = ddeiSSH.GetRuntimePostfixConf("smtpd_tls_security_level");
            Assert.AreEqual("encrypt", level);
        }

        [TestMethod]
        public void OutgoingTLS_enable()
        {
            ddeiWebdriver.EnableIncomingTLS(false);
            ddeiWebdriver.EnableForceTLS(false);
            ddeiWebdriver.EnableOutgoingTLS(true);

            ddeiWebdriver.Save();

            string level = ddeiSSH.GetRuntimePostfixConf("smtpd_tls_security_level");
            Assert.AreEqual("may", level);
        }

        [TestMethod]
        public void OutgoingTLS_disable()
        {
            ddeiWebdriver.EnableIncomingTLS(false);
            ddeiWebdriver.EnableForceTLS(false);
            ddeiWebdriver.EnableOutgoingTLS(true);

            ddeiWebdriver.Save();

            string level = ddeiSSH.GetRuntimePostfixConf("smtpd_tls_security_level");
            Assert.AreEqual("none", level);
        }


    }
}
