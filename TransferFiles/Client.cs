using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace TransferFiles
{
    class Client
    {
        public Client()
        {

        }

        public string SendingFilePath = string.Empty;
        private const int BufferSize = 1024;

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

        public void SendTCP(string Path, string IPA, Int32 PortN)
        {
            byte[] SendingBuffer = null;
            TcpClient client = null;
            string status = "";
            NetworkStream netstream = null;
            try
            {
                client = new TcpClient(IPA, PortN);
                status = "Connected to the Server...\n";
                netstream = client.GetStream();
                FileStream Fs = new FileStream(Path, FileMode.Open, FileAccess.Read);
                int NoOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Fs.Length) / Convert.ToDouble(BufferSize)));
                //progressBar1.Maximum = NoOfPackets;
                int TotalLength = (int)Fs.Length, CurrentPacketLength, counter = 0;
                for (int i = 0; i < NoOfPackets; i++)
                {
                    if (TotalLength > BufferSize)
                    {
                        CurrentPacketLength = BufferSize;
                        TotalLength = TotalLength - CurrentPacketLength;
                    }
                    else
                        CurrentPacketLength = TotalLength;
                    SendingBuffer = new byte[CurrentPacketLength];
                    Fs.Read(SendingBuffer, 0, CurrentPacketLength);
                    netstream.Write(SendingBuffer, 0, (int)SendingBuffer.Length);
                    //if (progressBar1.Value >= progressBar1.Maximum)
                    //    progressBar1.Value = progressBar1.Minimum;
                    //progressBar1.PerformStep();
                }

                //lblStatus.Text = lblStatus.Text + "Sent " + Fs.Length.ToString() + " bytes to the server";
                Fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                netstream.Close();
                client.Close();

            }
        }
    }
}
