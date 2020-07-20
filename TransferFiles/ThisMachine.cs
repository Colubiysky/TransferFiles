using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace TransferFiles
{
    class ThisMachine
    {
        public static IPAddress DefaultGateway { get; set; }
        public static IPAddress LocalIP { get; set; }
    }
}
