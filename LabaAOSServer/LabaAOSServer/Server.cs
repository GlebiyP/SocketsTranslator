using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace LabaAOSServer
{
    class Server
    {
        static readonly int _port = 1213;
        static readonly string _serverIp = "127.0.0.1";
        static readonly string _logPath = "C:\\Users\\Gleb\\source\\repos\\LabaAOSServer\\LabaAOSServer\\server_log.txt";
        const string info = "Petruk Gleb, K-25, v.8, Translator";
        private List<string> receivedWords;
        private IPEndPoint endPoint;
        private Socket listenSocket;
        private Socket receiver;
        private string lastMessage;
        private StringBuilder builder;
        private bool is_working;
        private string time;
        private Dictionary<string, string> dictionary = new Dictionary<string, string>
        {
            { "яблоко", "apple" },
            { "машина", "car" },
            { "компьютер", "computer" },
            { "рабочий стол", "desktop" },
            { "скорость", "speed" },
            { "переводчик", "translator" },
            { "апельсин", "orange" },
            { "клавиатура", "keyboard" },
            { "стена", "wall, side" },
            { "поле", "field, square" }
        };

        static void Send(string messege, Socket handler)
        {
            byte[] data = Encoding.Unicode.GetBytes(messege);
            handler.Send(data);
            Log("Server sent: \"" + messege + "\"");
        }

        static void Log(string data)
        {
            StreamWriter serverStreamWriter = new StreamWriter(_logPath, true);
            string time = DateTime.Now.ToString();
            serverStreamWriter.WriteLine(time + ": " + data);
            Console.WriteLine(data);
            serverStreamWriter.Close();
        }

        public Server()
        {
            time = "The words have not been translated yet.";
            builder = new StringBuilder();
            is_working = true;
            receivedWords = new List<string>();
            endPoint = new IPEndPoint(IPAddress.Parse(_serverIp), _port);
            listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            listenSocket.Bind(endPoint);
            listenSocket.Listen(10);
            receiver = listenSocket.Accept();
            Log("Client connected");
        }

        private void Who()
        {
            Send(info, receiver);
        }

        private void Receive()
        {
            int bytes = 0;
            byte[] data = new byte[256];

            do
            {
                bytes = receiver.Receive(data);
                builder.Append(Encoding.Unicode.GetString(data, 0, bytes));
            }
            while (receiver.Available > 0);

            lastMessage = builder.ToString().Substring(0);
            builder.Clear();
            Log("Received from client: \"" + lastMessage + "\"");
        }

        private void TranslateWords()
        {
            var start = System.Diagnostics.Stopwatch.StartNew();

            for (int i = 0; i < receivedWords.Count; i++)
            {
                string word = receivedWords[i];
                foreach (var s in dictionary)
                {
                    if (word == s.Key)
                    {
                        receivedWords[i] = s.Value;
                        break;
                    }
                    else
                    {
                        receivedWords[i] = "The word is not in the dictionary.";
                    }
                }
            }

            start.Stop();
            var resTime = start.Elapsed;
            time = resTime.ToString();
        }

        private void Process()
        {
            if (lastMessage == "WHO")
            {
                Who();
            }
            else if (lastMessage == "TRANSLATE")
            {
                TranslateWords();
                for (int i = 0; i < receivedWords.Count; i++)
                {
                    System.Threading.Thread.Sleep(5);
                    Send(receivedWords[i], receiver);
                }
                receivedWords.Clear();
            }
            else if (lastMessage == "TIME")
            {
                Send(time, receiver);
            }
            else if(lastMessage=="CLOSE")
            {
                receiver.Shutdown(SocketShutdown.Both);
                receiver.Close();
                Console.WriteLine("Server shutted down.");
                is_working = false;
            }
            else
            {
                receivedWords.Add(lastMessage);
            }
        }

        public void Work()
        {
            while (is_working)
            {
                Receive();
                Process();
            }
            return;
        }
    }
}


