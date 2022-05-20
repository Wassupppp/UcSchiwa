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
    public class Common
    {
        public static void printHelp()
        {
            Console.ForegroundColor = ConsoleColor.Red;
            string executable = System.AppDomain.CurrentDomain.FriendlyName;
            Console.WriteLine("Options:");
            Console.WriteLine("Listener mode: {0} <port> <certificate.pfx> <certificate.passwd>", executable);
            Console.WriteLine("Reverse shell mode: {0} <IP> <port>", executable);
            Console.WriteLine("Specific reverseShell cmd:");
            Console.WriteLine("exit: close this connection and wait the next one");
            Console.WriteLine("bg: run next cmd in background (close connection before cmd result)");
            Console.ResetColor();
        }

        public static string readMsg(SslStream sslStream, TcpClient client)
        {
            byte[] buffer = new byte[client.ReceiveBufferSize];
            int bytesRead = sslStream.Read(buffer, 0, client.ReceiveBufferSize);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }
        public static void sendMsg(string message, SslStream sslStream)
        {
            sslStream.Write(Encoding.UTF8.GetBytes(message), 0, Encoding.UTF8.GetBytes(message).Length);
        }
    }
}
