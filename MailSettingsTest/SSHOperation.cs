using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Tamir.SharpSsh;
using Tamir.SharpSsh.jsch;

namespace MailSettingsTest
{
    public class SSHOperation
    {
        private SshExec shell;

        public SSHOperation(string DDEI_IP, string SSH_ROOT, string SSH_PASS)
        {
            shell = new SshExec(DDEI_IP, SSH_ROOT, SSH_PASS);
            try
            {
                shell.Connect();
            }
            catch (Exception e)
            {
                Console.WriteLine("Error happened when SSH connect to DDEI! [{0}]", e.Message);
            }
        }

        ~SSHOperation()
        {
            shell.Close();
        }

        public SshExec SSHInstance
        {
            get
            {
                return shell;
            }
        }

        public string ExecuteCMD(string cmd)
        {
            if (!shell.Connected)
            {
                return "";
            }

            string output = "";

            try
            {
                output = shell.RunCommand(cmd);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error when execute command by SSH. [{0}]", e.Message);
            }

            return output;           
        }

        public string GetRuntimePostfixConf(string option)
        {
            WaitPostfixReload();
            string value = "";
            string cmd = @"/opt/trend/ddei/postfix/usr/sbin/postconf | grep ";
            option = "'" + option + " ='";
            cmd = cmd + option + @" | awk -F= '{print $2}'";
            value = ExecuteCMD(cmd);
            return value.Trim();
        }

        public string GetRuntimePostfixPort()
        {
            string cmd = @"cat  /opt/trend/ddei/postfix/etc/postfix/master.cf |grep smtpd |  wc -l";
            int lines = Int32.Parse(ExecuteCMD(cmd));

            string line = "";
            string[] arr = { "" };
            for (int i = 1; i <= lines; i++)
            {
                cmd = @"cat  /opt/trend/ddei/postfix/etc/postfix/master.cf |grep smtpd |  grep inet | head -n " + i.ToString() + " | tail -n 1";
                line = ExecuteCMD(cmd);

                if (line.Length != 0)
                {

                    if (line.StartsWith("#"))
                    {
                        continue;
                    }
                    else
                    {
                        char[] separator = { ' ' };
                        arr = line.Split(separator);
                        break;
                    }
                }
            }
            return arr[0];

        }

        public string getRuntimePostfixConn()
        {
            string cmd = @"cat  /opt/trend/ddei/postfix/etc/postfix/master.cf |grep smtpd |  wc -l";
            int lines = Int32.Parse(ExecuteCMD(cmd));

            string line = "";
            string[] arr = { "" };
            for (int i = 1; i <= lines; i++)
            {
                cmd = @"cat  /opt/trend/ddei/postfix/etc/postfix/master.cf |grep smtpd |  grep inet | head -n " + i.ToString() + " | tail -n 1";
                line = ExecuteCMD(cmd);

                if (line.Length != 0)
                {

                    if (line.StartsWith("#"))
                    {
                        continue;
                    }
                    else
                    {
                        char[] separator = { ' ' };
                        arr = line.Split(separator);
                        break;
                    }
                }
            }
            return arr[36];

        }

        public void WaitPostfixReload()
        {
            //TBC
        }

    }
}
