using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TransferFiles
{
    class Pinger
    {
        IPAddress DefaultGates;
        IPAddress LocalIP;
        Main m;

        public bool IsFinished { get => Finished; }
        private bool Finished = false;

        public Pinger(Main main)
        {
            m = main;
            DefaultGates = GetDefaultGateway();
            LocalIP = IPAddress.Parse(GetLocalIP());
        }

        private IPAddress GetDefaultGateway()
        {
            return NetworkInterface
               .GetAllNetworkInterfaces()
               .Where(n => n.OperationalStatus == OperationalStatus.Up)
               .Where(n => n.NetworkInterfaceType != NetworkInterfaceType.Loopback)
               .Where(n => (n.Name != "Radmin VPN") && (n.Name != "Hamachi"))
               .SelectMany(n => n.GetIPProperties()?.GatewayAddresses)
               .Select(g => g?.Address)
               .Where(a => a != null)
               .FirstOrDefault();
        }

        private string GetLocalIP()
        {
            var interfaces = NetworkInterface.GetAllNetworkInterfaces();
            foreach (var item in interfaces)
            {
                if (item.OperationalStatus == OperationalStatus.Up &&
                    item.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    item.Name != "Radmin VPN" && item.Name != "Hamachi")
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

        public void DoPing()
        {
            var IPList = CreateIPList();
            string data = "1";
            byte[] buffer = Encoding.ASCII.GetBytes(data);
            PingOptions options = new PingOptions(64, true);

            for (int i =0; i < IPList.Count; i++)
            {
                if (i != IPList.Count)
                    Threads(IPList[i], buffer, options, false);
                else
                    Threads(IPList[i], buffer, options, true);
            }
        }

        private void Threads(IPAddress dest, byte[] buffer, PingOptions opt, bool last)
        {
            Ping ping = new Ping();
            PingReply pingReply;
            Thread t = new Thread(new ThreadStart(() =>
            {
                pingReply = ping.Send(dest, 100, buffer, opt);

                if (pingReply.Status.ToString() == "Success")
                    m.PingReply += pingReply.Address.ToString() + " "
                                + pingReply.Status + " " 
                                + Encoding.UTF8.GetString(pingReply.Buffer) + " " 
                                + pingReply.Options.Ttl
                                + "\r\n";
            }));

            t.IsBackground = true;
            t.Start();

            if(last)
                Finished = true;
        }


        private List<IPAddress> CreateIPList()
        {
            List<IPAddress> IPList = new List<IPAddress>();
            int[] localAndDefaultIPs = NotIncludedIndeces();

            for (int i = 1; i < 256; i++)
                if (i != localAndDefaultIPs[0] && i != localAndDefaultIPs[1])
                    IPList.Add(IPAddress.Parse(DefaultGates.ToString().Replace(".0.1", ".0." + i.ToString())));

            return IPList;
        }

        private int[] NotIncludedIndeces()
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
