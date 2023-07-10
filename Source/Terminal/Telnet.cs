using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace TerminalLib;

public class TelnetClient : TcpClient
{
    public TelnetClient()
    {
    }

    public TelnetClient(IPEndPoint localEP) : base(localEP)
    {
    }

    public TelnetClient(AddressFamily family) : base(family)
    {
    }

    public TelnetClient(string hostname, int port) : base(hostname, port)
    {
    }
}