﻿using System;
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
            Console.WriteLine("CN:{0}\nExpirDate:{1}\nPubKey:{2}\nThumprint:{3}\n",
                cert2.Issuer, cert2.GetExpirationDateString(), cert2.GetPublicKeyString(), cert2.Thumbprint);
            Console.WriteLine("YOU HAVE FEW SECONDS TO KILL THE PROCESS AND REJECT THE RISK...");
            Thread.Sleep(3000);
            Console.WriteLine("(accepted)");

            return true;
        }

        public static string executePwsh(string message)
        {
            Console.WriteLine("received cmd: {0}",message);
            String output = String.Empty;
            try
            {
                using (PowerShell ps = PowerShell.Create())
                {
                    Collection<PSObject> results = ps.AddScript(message).Invoke();
                    foreach (PSObject result in results)
                    {
                        output += result.ToString() + Environment.NewLine;
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

            Console.WriteLine(output);
            return output;
        }
    }
}
