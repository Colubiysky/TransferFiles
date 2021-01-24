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
        private const int BufferSize = 8192;

        int port = 1572; //server port

        public void SendTCP(string Path, string IPA, Int32 PortN)
        {
            byte[] SendingBuffer = null;
            TcpClient client = null;
            NetworkStream netstream = null;
            try
            {
                client = new TcpClient(IPA, PortN);
                netstream = client.GetStream();
                FileStream Fs = new FileStream(Path, FileMode.Open, FileAccess.Read);
                int NoOfPackets = Convert.ToInt32(Math.Ceiling(Convert.ToDouble(Fs.Length) / Convert.ToDouble(BufferSize)));
                int TotalLength = (int)Fs.Length, CurrentPacketLength;

                //Send filename
                string filename = new FileInfo(Path).Name; //gets filename
                byte[] filenameBuf = Encoding.UTF8.GetBytes(filename);
                netstream.Write(filenameBuf, 0, filenameBuf.Length);

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
                }

                Fs.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Client");
            }
            finally
            {
                netstream.Close();
                client.Close();

            }
        }
    }
}
