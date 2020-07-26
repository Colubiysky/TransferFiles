using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TransferFiles
{
    class Client
    {
        public Client()
        {

        }

        int port = 1488; // порт сервера
        public string Address { get => address; set => address = value; }
        string address = String.Empty; // адрес сервера

        public string FilePath { get => filePath; set => filePath = value; }
        string filePath = String.Empty;

        public void Send(string endpoint)
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(endpoint), 1337);

                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                socket.Connect(endPoint);
                Console.WriteLine("Client connected");

                Console.WriteLine("Sending file...");
                using (FileStream stream = new FileStream(@"C:\Users\cola\Desktop\test.txt", FileMode.Open, FileAccess.Read))
                {
                    byte[] data = new byte[stream.Length];
                    int length = stream.Read(data, 0, data.Length);
                    NetFile.NetFile file = new NetFile.NetFile();
                    file.FileName = Path.GetFileName(stream.Name);
                    file.Data = data;

                    byte[] to = file.ToArray();
                    socket.Send(to);
                }
                Console.WriteLine("File sended");

                socket.Shutdown(SocketShutdown.Both);
                socket.Close();
                //Console.ReadKey();
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
        }
    }
}
