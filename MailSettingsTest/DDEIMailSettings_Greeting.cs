using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MailSettingsTest
{
    [TestClass]
    public class DDEIMailSettings_Greeting
    {
        private static UIMailGreeting ddeiWebdriver;
        private static SSHOperation ddeiSSH;

        [ClassInitialize]
        public static void BeforeTest(TestContext test)
        {
            ddeiWebdriver = new UIMailGreeting("10.64.70.250", "admin", "ddei");
            ddeiSSH = new SSHOperation("10.64.70.250", "root", "Tm00@1#No.1#");

            if (ddeiWebdriver == null || ddeiSSH == null)
            {
                Assert.Fail();
            }

            ddeiWebdriver.Switch2GreetingPage();
        }

        [ClassCleanup]
        public static void AfterTest()
        {
            ddeiWebdriver.Delete();
        }
        [TestMethod]
        public void SetGreetingMessage()
        {
            string banner = "DDEI";
            ddeiWebdriver.SetGreetingString(banner);
            ddeiWebdriver.Save();

            Assert.AreEqual(banner, ddeiSSH.GetRuntimePostfixConf("smtpd_banner"));

        }
    }
}
