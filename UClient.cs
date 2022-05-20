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

namespace uCShiwa
{
    public static class UClient
    {
        public static bool CertificateValidationCallback(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            //Be aware that there is no PKI, the authentication method is manual
            X509Certificate2 cert2 = new X509Certificate2(certificate);

            string cn = cert2.Issuer;
            string cedate = cert2.GetExpirationDateString();
            string cpub = cert2.GetPublicKeyString();
            string thumb = cert2.Thumbprint;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("CN:{0}\nExpirDate:{1}\nPubKey:{2}\nThumprint:{3}\n", cn, cedate, cpub, thumb);
            Console.WriteLine("YOU HAVE FEW SECONDS TO KILL THE PROCESS AND REJECT THE RISK...");
            Thread.Sleep(3000);
            Console.WriteLine("(accepted)");
            Console.ResetColor();

            return true;
        }

        public static string executePwsh(string message)
        {
            String output = String.Empty;
            try
            {
                using (PowerShell ps = PowerShell.Create())
                {
                    Collection<PSObject> results = ps.AddScript(message).Invoke();
                    foreach (PSObject result in results)
                    {
                        output += result.ToString() + "\n";
                    }
                    ps.Commands.Clear();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("{0} Exception caught.", e);
            }

            if (output == String.Empty)
            {
                output = "(Empty)";
            }
            return output;
        }
    }
}
