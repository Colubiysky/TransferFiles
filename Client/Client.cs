using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using NetFile;

namespace Client
{
    public class Client
    {
        // адрес и порт сервера, к которому будем подключаться
        int port = 1337; // порт сервера
        public string Address { get => address; set => address = value; }
        string address = String.Empty; // адрес сервера

        public string FilePath { get => filePath; set => filePath = value; }
        string filePath = String.Empty;

        public void Main(string endpoint)
        {
            try
            {
                IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse(endpoint), 8080);

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
                Console.ReadKey();
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
                Console.ReadKey();
            }
        }
    }
}
