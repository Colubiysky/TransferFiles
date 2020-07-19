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

        public string PingReply { get => PingRes; set => PingRes = value; }
        private string PingRes = "";

        private static System.Timers.Timer aTimer;
        Pinger p;

        private void button2_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Stopwatch sw = new Stopwatch();
            sw.Start();

            p = new Pinger(this);
            p.DoPing();
            SetTimer();

            sw.Stop();
            MessageBox.Show((sw.ElapsedMilliseconds / 100.0).ToString());
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
            Invoke(new System.Action(()=> textBox1.Text = PingRes));
        }
    }
}
