using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ServerApp
{
    public partial class Main : Form
    {
        private const int BufferSize = 8192;
        public string Status = string.Empty;
        public Thread T = null;

        public Main()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            label1.Text = "Server is Running...";
            ThreadStart Ts = new ThreadStart(StartReceiving);
            T = new Thread(Ts);
            T.SetApartmentState(ApartmentState.STA);
            T.Start();
        }
        private void StartReceiving()
        {
            ReceiveTCP(1572);
        }

        private void FileRecive()
        {
            //Создаем Listener на порт "по умолчанию"
            TcpListener Listen = new TcpListener(1572);
            //Начинаем прослушку
            Listen.Start();
            //и заведем заранее сокет
            Socket ReceiveSocket;
            while (true)
            {
                try
                {
                    string name;
                    //Пришло сообщение
                    ReceiveSocket = Listen.AcceptSocket();
                    Byte[] Receive = new Byte[256];
                    //Читать сообщение будем в поток
                    using (MemoryStream MessageR = new MemoryStream())
                    {

                        //Количество считанных байт
                        Int32 ReceivedBytes;
                        Int32 Firest256Bytes = 0;
                        String FilePath = "";
                        do
                        {//Собственно читаем
                            ReceivedBytes = ReceiveSocket.Receive(Receive, Receive.Length, 0);
                            //Разбираем первые 256 байт
                            if (Firest256Bytes < 256)
                            {
                                Firest256Bytes += ReceivedBytes;
                                Byte[] ToStr = Receive;
                                //Учтем, что может возникнуть ситуация, когда они не могу передаться "сразу" все
                                if (Firest256Bytes > 256)
                                {
                                    Int32 Start = Firest256Bytes - ReceivedBytes;
                                    Int32 CountToGet = 256 - Start;
                                    Firest256Bytes = 256;
                                    //В случае если было принято >256 байт (двумя сообщениями к примеру)
                                    //Остаток (до 256) записываем в "путь файла"
                                    ToStr = Receive.Take(CountToGet).ToArray();
                                    //А остальную часть - в будующий файл
                                    Receive = Receive.Skip(CountToGet).ToArray();
                                    MessageR.Write(Receive, 0, ReceivedBytes);
                                }
                                //Накапливаем имя файла
                                FilePath += Encoding.Default.GetString(ToStr);
                            }
                            else

                                //и записываем в поток
                                MessageR.Write(Receive, 0, ReceivedBytes);
                            //Читаем до тех пор, пока в очереди не останется данных
                        } while (ReceivedBytes == Receive.Length);
                        //Убираем лишние байты
                        String resFilePath = FilePath.Substring(0, FilePath.IndexOf('\0'));
                        MessageBox.Show(resFilePath);


                        using (var File = new FileStream(resFilePath, FileMode.Create))
                        {//Записываем в файл
                            File.Write(MessageR.ToArray(), 0, MessageR.ToArray().Length);
                        }//Уведомим пользователя
                        //ChatBox.BeginInvoke(AcceptDelegate, new object[] { "Получено: " + resFilePath, ChatBox });
                        name = resFilePath;
                    }
                }
                catch (System.Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

            }
        }

        public void ReceiveTCP(int portN)
        {
            TcpListener Listener = null;
            try
            {
                Listener = new TcpListener(IPAddress.Any, portN);
                Listener.Start();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            byte[] RecData = new byte[BufferSize];
            int RecBytes;

            for (; ; )
            {
                TcpClient client = null;
                NetworkStream netstream = null;
                Status = string.Empty;
                try
                {
                    string message = "Accept the Incoming File ";
                    string caption = "Incoming Connection";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result;

                    if (Listener.Pending())
                    {
                        client = Listener.AcceptTcpClient();
                        netstream = client.GetStream();
                        Status = "Connected to a client\n";
                        result = MessageBox.Show(message, caption, buttons);

                        if (result == System.Windows.Forms.DialogResult.Yes)
                        {
                            string SaveFileName = string.Empty /*UTF8Encoding.UTF8.GetString(filenameBuf)*/ /*fname*/;
                            SaveFileDialog DialogSave = new SaveFileDialog();
                            DialogSave.Filter = "All files (*.*)|*.*";
                            DialogSave.RestoreDirectory = true;
                            DialogSave.Title = "Where do you want to save the file?";
                            DialogSave.InitialDirectory = @"C:/";
                            DialogSave.FileName = SaveFileName;
                            if (DialogSave.ShowDialog() == DialogResult.OK)
                                SaveFileName = DialogSave.FileName;
                            if (SaveFileName != string.Empty)
                            {
                                int totalrecbytes = 0;
                                FileStream Fs = new FileStream(SaveFileName, FileMode.OpenOrCreate, FileAccess.Write);
                                while ((RecBytes = netstream.Read(RecData, 0, RecData.Length)) > 0)
                                {
                                    Fs.Write(RecData, 0, RecBytes);
                                    totalrecbytes += RecBytes;
                                }
                                Fs.Close();
                            }
                            netstream.Close();
                            client.Close();
                            MessageBox.Show("Successfully recieved!", "server");
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "server");
                    netstream.Close();
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            T.Abort();
            this.Close();
        }
    }
}
