using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using NetFile;
using System.IO;

namespace Server
{
    public class Server
    {
        public int Port { get => port; }
        int port = 1337;

        string EndpointFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";

        public void Run()
        {
            IPEndPoint ipPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);

            Socket listenSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            try
            {
                // связываем сокет с локальной точкой, по которой будем принимать данные
                listenSocket.Bind(ipPoint);

                // начинаем прослушивание
                listenSocket.Listen(10);

                Console.WriteLine("Server is running...");

                int bytes = 0;
                const int bufferSize = 8192;
                while (true)
                {
                    Socket handler = listenSocket.Accept();

                    NetFile.NetFile file;
                    using (MemoryStream memStream = new MemoryStream())
                    {
                        byte[] buffer = new byte[bufferSize];
                        do
                        {
                            int received = handler.Receive(buffer);
                            //File.AppendAllText("Log.log", string.Format("Received={0}\r\n", received));
                            memStream.Write(buffer, 0, received);
                            bytes += received;
                        }
                        while (handler.Available > 0);

                        file = new NetFile.NetFile(memStream.ToArray());
                    }
                    Console.WriteLine("Size of received data: " + bytes.ToString() + " bytes");

                    using (FileStream stream = new FileStream(file.FileName, FileMode.Create, FileAccess.Write))
                    {
                        stream.Write(file.Data, 0, file.Data.Length);
                    }

                    handler.Shutdown(SocketShutdown.Both);
                    handler.Close();

                    bytes = 0;
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
                Console.ReadKey();
            }

        }
    }
}
