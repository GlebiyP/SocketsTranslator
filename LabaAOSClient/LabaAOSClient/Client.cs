using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Text;

namespace LabaAOSClient
{
    class Client
    {
        const int _port = 1213;

        const string _serverIp = "127.0.0.1";
        const string _logPath = "C:\\Users\\Gleb\\source\\repos\\LabaAOSClient\\LabaAOSClient\\client_log.txt";

        private IPEndPoint endPoint;
        private Socket server;
        private string lastMessage;
        private StringBuilder builder;
        private bool is_working;

        public Client()
        {
            is_working = true;
            builder = new StringBuilder();
            endPoint = new IPEndPoint(IPAddress.Parse(_serverIp), _port);
            server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            server.Connect(endPoint);
            Log("Connected to server at" + DateTime.Now.ToString());
        }

        private void Log(string data)
        {
            StreamWriter clientStreamWriter = new StreamWriter(_logPath, true);
            clientStreamWriter.WriteLine(data);
            Console.WriteLine(data);
            clientStreamWriter.Close();
        }

        private void Send(string messege)
        {
            byte[] data = Encoding.Unicode.GetBytes(messege);
            server.Send(data);
            Log("Client sent: \"" + messege + "\"");
        }

        private void Receive()
        {
            byte[] data = new byte[256];
            int bytes = 0;
            do
            {
                bytes = server.Receive(data, data.Length, 0);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (server.Available > 0);

            lastMessage = builder.ToString();
            builder.Clear();
            Log("Received from server: \"" + lastMessage + "\"");
        }

        public void Work()
        {
            while(is_working)
            {
                int count = 0;
                string msg;
                Console.WriteLine("Enter the command");
                do
                {
                    msg = Console.ReadLine();

                    if (msg == "WHO")
                    {
                        Send(msg);
                        Receive();
                    }
                    else if (msg == "TRANSLATE")
                    {
                        Send(msg);
                        goto Receive;
                    }
                    else if (msg == "TIME")
                    {
                        Send(msg);
                        Receive();
                    }
                    else if(msg=="CLOSE")
                    {
                        Send(msg);
                        is_working = false;
                        Console.WriteLine("Client shutted down.");
                        return;
                    }
                    else
                    {
                        Send(msg);
                        if (msg != String.Empty)
                            count++;
                    }
                }
                while (msg != "EOS");

            Receive:
                for (int i = 0; i < count; i++)
                {
                    Receive();
                }
            }
        }
    }
}
