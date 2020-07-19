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

namespace TransferFiles
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        private List<string> Online;

        private System.Timers.Timer aTimer;
        Pinger p;

        System.Diagnostics.Stopwatch sw = new Stopwatch();

        private void button2_Click(object sender, EventArgs e)
        {
            p = new Pinger();
            lst_Computers.Items.Clear();
            sw.Restart();
            if (Online != null) 
                Online.Clear();
            p.DoPing();
            SetTimer();            
        }

        private  void SetTimer()
        {
            aTimer = new System.Timers.Timer(100);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private  void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            Online = p.Online;

            if (p.IsFinished)
            {

                Invoke(new System.Action(() =>
                {
                    foreach (var address in Online)
                        lst_Computers.Items.Add(address);
                }));

                sw.Stop();
                aTimer.Stop();
                MessageBox.Show((sw.ElapsedMilliseconds / 100.0).ToString());
            }
        }
    }
}
