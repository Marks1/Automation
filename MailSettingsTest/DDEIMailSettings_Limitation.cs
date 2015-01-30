using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MailSettingsTest
{
    [TestClass]
    public class DDEIMailSettings_Limitation
    {
        private static UIMailLimitation ddeiWebdriver;
        private static SSHOperation ddeiSSH;

        [ClassInitialize]
        public static void BeforeTest(TestContext test)
        {
            ddeiWebdriver = new UIMailLimitation("10.64.70.250", "admin", "ddei");
            ddeiSSH = new SSHOperation("10.64.70.250", "root", "Tm00@1#No.1#");

            if (ddeiWebdriver == null || ddeiSSH == null)
            {
                Assert.Fail();
            }

            ddeiWebdriver.Switch2LimitationPage();
        }
        [ClassCleanup]
        public static void AfterTest()
        {
            ddeiWebdriver.Delete();
        }

        [TestMethod]
        public void ChangeMaxMsgSize()
        {
            

            int MaxSize = 20;
            ddeiWebdriver.SetMaxMsgSize(MaxSize);

            //save
            ddeiWebdriver.Save();

            //assert
            Assert.AreEqual(MaxSize * 1048576, Int32.Parse(ddeiSSH.GetRuntimePostfixConf("message_size_limit")));

        }

        [TestMethod]
        public void ChangeMaxRecipientNumber()
        {

            int MaxRecipientNum = 200;
            ddeiWebdriver.SetMaxRecipient(MaxRecipientNum);

            //save
            ddeiWebdriver.Save();

            //assert
            Assert.AreEqual(MaxRecipientNum, Int32.Parse(ddeiSSH.GetRuntimePostfixConf("smtpd_recipient_limit")));

        }

        [TestMethod]
        public void ChangePermittedSender2Subnet()
        {

            ddeiWebdriver.SetPermitedSender(PermittedSenderType.Subnet);

            //save
            ddeiWebdriver.Save();

            //assert
            Assert.AreEqual("subnet", ddeiSSH.GetRuntimePostfixConf("mynetworks_style"));

        }

        [TestMethod]
        public void ChangePermittedSender2Class()
        {

            ddeiWebdriver.SetPermitedSender(PermittedSenderType.Class);

            //save
            ddeiWebdriver.Save();

            //assert
            Assert.AreEqual("class", ddeiSSH.GetRuntimePostfixConf("mynetworks_style"));

        }

        [TestMethod]
        public void ChangePermittedSender2SpecificIP()
        {

            string SpecificIP = "1.2.3.4";
            ddeiWebdriver.SetPermitedSender(PermittedSenderType.Specify, SpecificIP);

            //save
            ddeiWebdriver.Save();

            //assert
            Assert.AreEqual("subnet", ddeiSSH.GetRuntimePostfixConf("mynetworks_style"));
            Assert.AreEqual(true, ddeiSSH.GetRuntimePostfixConf("mynetworks").Contains(SpecificIP));

        }

        [TestMethod]
        public void ChangePermittedSender2SpecificRange()
        {

            groupIdentity group;
            group.IPrange = "1.2.3.0";
            group.netMask = "255.255.255.0";

            ddeiWebdriver.SetPermitedSender(PermittedSenderType.Specify, group, protocolType.IPV4);

            //save
            ddeiWebdriver.Save();

            //assert
            Assert.AreEqual("subnet", ddeiSSH.GetRuntimePostfixConf("mynetworks_style"));
            Assert.AreEqual(true, ddeiSSH.GetRuntimePostfixConf("mynetworks").Contains(group.IPrange + @"/24"));

        }

        [TestMethod]
        public void ChangePermittedSender2SpecificRangeV6()
        {

            groupIdentity group;
            group.IPrange = "2001:db8::";
            group.netMask = @"/32";

            ddeiWebdriver.SetPermitedSender(PermittedSenderType.Specify, group, protocolType.IPV6);

            //save
            ddeiWebdriver.Save();

            //assert
            Assert.AreEqual("subnet", ddeiSSH.GetRuntimePostfixConf("mynetworks_style"));

            string expect_str = "[" + group.IPrange + @"]/32";
            
            Assert.AreEqual(true, ddeiSSH.GetRuntimePostfixConf("mynetworks").Contains(group.IPrange));

        }


    }
}
