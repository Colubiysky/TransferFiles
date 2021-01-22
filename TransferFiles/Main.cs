using System;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Timers;
using System.Collections.Immutable;

namespace TransferFiles
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
            GetOnline();

        }
        Client client = new Client();

        private List<string> Online;

        private System.Timers.Timer aTimer;
        Pinger p;

        System.Diagnostics.Stopwatch sw = new Stopwatch();

        bool firstTime = false;

        private void CheckOnline_btn_Click(object sender, EventArgs e)
        {
            string EndpointFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "\\";
            GetOnline();
        }

        private void GetOnline() //Fills listbox addresses of online devices
        {
            p = new Pinger();
            lst_Computers.Items.Clear();
            sw.Restart();

            if (Online != null)
                Online.Clear();
            
            p.DoPing();
            SetTimer();
        }

        private  void SetTimer() //for pereodical checking of online list
        {
            aTimer = new System.Timers.Timer(100);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private  void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                Invoke(new System.Action(() =>
                {
                    Online = p.Online;
                    var idk = p.Pinged.ToArray();
                    Array.Sort(idk);

                    if (p.IsFinished) //If all threads finished, it will fill listbox with addresses
                    {
                        
                        lst_Computers.Items.Clear();
                        lst_Computers.Items.AddRange(Online.ToArray());
                        sw.Stop();
                        if (!firstTime) //after first update, it will reduce interval of timer updates
                        {
                            firstTime = true;
                            //MessageBox.Show((sw.ElapsedMilliseconds / 100.0).ToString());
                            aTimer.Stop();
                            //aTimer.Interval = 1000000;
                        }
                    }

                    #if DEBUG
                    
                    lst_Computers.Items.Clear();
                    lst_Computers.Items.AddRange(Online.ToArray());
                    aTimer.Stop();
                    
                    #endif
                }));

            }
            catch(InvalidOperationException ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void DropFile_btn_Click(object sender, EventArgs e)
        {
            string SaveFileName = string.Empty;
            SaveFileDialog DialogSave = new SaveFileDialog();
            DialogSave.Filter = "All files (*.*)|*.*";
            DialogSave.RestoreDirectory = true;
            DialogSave.Title = "Select file...";
            DialogSave.InitialDirectory = @"C:/";
            if (DialogSave.ShowDialog() == DialogResult.OK)
                SaveFileName = DialogSave.FileName;
            client.SendTCP(SaveFileName, lst_Computers.SelectedItem.ToString(), 1572);
        }
    }
}
