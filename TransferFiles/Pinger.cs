using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Timers;
using System.Net.Http.Headers;

namespace TransferFiles
{
    class Pinger //IT will checks who is online in local network
    {
        IPAddress DefaultGates; //Default gateway
        IPAddress LocalIP; //IP address of computer in local network

        public bool IsFinished { get => Finished; } //bool that means that all threads finished
        private bool Finished = false;

        public List<string> Online { get => Online_; } //This list contains ip addresses of online devices in network
        private List<string> Online_ = new List<string>();

        int Alive_ = 0; //alive threads
        int Dead_ = 0;  //finished threads

        int countIPs_ = 0; //all ip addresses variants, except local and default gateway
        int pinged_ = 0;   //count of pinged ips


        private System.Timers.Timer aTimer;

        public Pinger()
        {
            DefaultGates = GetDefaultGateway();
            LocalIP = IPAddress.Parse(GetLocalIP());
            SetTimer();
        }

        private  IPAddress GetDefaultGateway()
        {
            return NetworkInterface
               .GetAllNetworkInterfaces()
               .Where(n => n.OperationalStatus == OperationalStatus.Up)
               .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
               .Where(n => (n.Name != "Radmin VPN") && (n.Name != "Hamachi")) //except fucking radmin and hamachi, they are breaking getting 
                                                                              // default gateway, because these shit has their own gateways
               .SelectMany(n => n.GetIPProperties()?.GatewayAddresses)
               .Select(g => g?.Address)
               .Where(a => a != null)
               .FirstOrDefault();
        }

        private  string GetLocalIP()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var item in interfaces)
            {
                if (item.OperationalStatus == OperationalStatus.Up &&
                    item.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    item.Name != "Radmin VPN" && item.Name != "Hamachi") //same love for radmin and hamachi
                {
                    var GatewayAddresses = item.GetIPProperties().GatewayAddresses;

                    foreach (var address in GatewayAddresses)
                    {
                        if (address.Address.ToString() == DefaultGates.ToString())
                        {
                            var IpProperties = item.GetIPProperties();
                            var UnicastAddresses = IpProperties.UnicastAddresses;

                            foreach (var uni_addr in UnicastAddresses)
                            {
                                if (uni_addr.IPv4Mask.ToString() != "0.0.0.0")
                                    return uni_addr.Address.ToString();
                            }
                        }
                    }
                }
            }

            return null;
        }

        public  void DoPing()
        {
            var IPList = CreateIPList();
            countIPs_ = IPList.Count;

            //Configurate ping
            string data = "1";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            PingOptions options = new PingOptions(64, true);

            for (int i =0; i < IPList.Count; i++) //every address will be pinged with different thread
            {
                if (i != IPList.Count - 1)
                    Threads(IPList[i], buffer, options, false); 
                else
                    Threads(IPList[i], buffer, options, true);
            }
        }

        private void SetTimer()
        {
            aTimer = new System.Timers.Timer(10);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            if (Alive_ == Dead_ && pinged_== countIPs_)
            {
                Finished = true;
                aTimer.Stop();
            }
        }

        private  void Threads(IPAddress dest, byte[] buffer, PingOptions opt, bool last)
        {
            Ping ping = new Ping();
            PingReply pingReply;
            Thread t = new Thread(new ThreadStart(() =>
            {
                ping.Send(dest, 100, buffer, opt); //knock knock 
                ping.Send(dest, 100, buffer, opt); //knock knock
                ping.Send(dest, 100, buffer, opt); //knock knock
                pingReply = ping.Send(dest, 100, buffer, opt); //knock knock is someone here?
                if (pingReply.Status != IPStatus.Unknown) //if we don't lost ping packet
                    pinged_++;

                if (pingReply.Status.ToString() == "Success")
                    Online_.Add(pingReply.Address.ToString());

                Dead_++;
            }));

            t.IsBackground = true;
            t.Start();
            Alive_++;
        }

        private  List<IPAddress> CreateIPList() //it creates list with ip addresses
        {
            List<IPAddress> IPList = new List<IPAddress>();
            int[] localAndDefaultIPs = NotIncludedIndeces();

            for (int i = 1; i < 256; i++)
                if (i != localAndDefaultIPs[0] && i != localAndDefaultIPs[1])
                    IPList.Add(IPAddress.Parse(DefaultGates.ToString().Replace(".0.1", ".0." + i.ToString())));

            return IPList;
        }

        private  int[] NotIncludedIndeces() //except local and default gateway addresses
        {
            int[] res = new int[2];
            int count = 0;
            string temp = string.Empty;
            for(int i =0; i < DefaultGates.ToString().Length; i++)
            {
                if (count == 3)
                    temp += DefaultGates.ToString()[i];

                if (DefaultGates.ToString()[i] == '.')
                    count++;
            }

            res[0] = int.Parse(temp);
            temp = string.Empty;
            count = 0;

            for (int i = 0; i < LocalIP.ToString().Length; i++)
            {
                if (count == 3)
                    temp += LocalIP.ToString()[i];

                if (LocalIP.ToString()[i] == '.')
                    count++;
            }

            res[1] = int.Parse(temp);

            return res;
        }
    }
}
