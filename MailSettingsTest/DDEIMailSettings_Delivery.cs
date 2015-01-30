using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MailSettingsTest
{
    [TestClass]
    public class DDEIMailSettings_Delivery
    {
        private static UIMailDeliverySetting ddeiWebdriver;
        private static SSHOperation ddeiSSH;

        [ClassInitialize]
        public static void BeforeTest(TestContext test)
        {
            ddeiWebdriver = new UIMailDeliverySetting("10.64.70.250", "admin", "ddei");
            ddeiSSH = new SSHOperation("10.64.70.250", "root", "Tm00@1#No.1#");

            if (ddeiWebdriver == null || ddeiSSH == null)
            {
                Assert.Fail();
            }

            ddeiWebdriver.Switch2DeliveryPage();
        }
        [ClassCleanup]
        public static void AfterTest()
        {
            ddeiWebdriver.Delete();
        }

        [TestMethod]
        public void AddMailRoute()
        {
            MailRouting rule;
            rule.destDomain = "DDEIUAT.com";
            rule.deliveryIP = "1.2.3.4";
            rule.port = "1025";

            ddeiWebdriver.AddDeliveryRule(rule);
            ddeiWebdriver.Save();

            //assert
            string transportList = @"cat /opt/trend/ddei/postfix/etc/postfix/transportList | grep " + rule.destDomain;
            string output = ddeiSSH.ExecuteCMD(transportList);

            //test.com        smtp:[1.2.4.3]:1025
            output = output.Trim();
            if (output.Length != 0)
            {
                char[] seperator = { ' ' };
                string[] arr = output.Split(seperator);

                Assert.AreEqual(rule.destDomain, arr[0].Trim());

                char[] seperator2 = { ':' };
                string[] arr2 = arr[1].Trim().Split(seperator2);

                Assert.AreEqual("[" + rule.deliveryIP + "]", arr2[1]);
                Assert.AreEqual(rule.port, arr2[2].Trim());
            }
            else
            {
                Assert.AreNotEqual(0, output.Length);
            }

        }
    }
}
