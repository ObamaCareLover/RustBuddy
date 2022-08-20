using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RustBuddy.API
{
    public struct RustSocketConfiguration
    {
        public string Server { get; set; }
        public uint Port { get; set; }
        public ulong Steam { get; set; }
        public int Token { get; set; }

        public RustSocketConfiguration(string server, uint port, ulong steam, int token)
        {
            Server = server;
            Port = port;
            Steam = steam;
            Token = token;
        }
    }
}
