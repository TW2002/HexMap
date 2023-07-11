using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace TerminalLib;

public class Session
{
    public string Name { get; set; }
    public string? Hostname { get; set; }
    public int Port { get; set; }
    public TelnetClient? GameServer { get; set; }

    public Session(string name, string? hostname = null, int port = 0)
    {
        Name = name;
        Hostname = hostname;
        Port = port;
    }

    public void Connect()
    {
        if (GameServer == null && Hostname != null)
        {
            GameServer = new();
        }
        else
        {
            return;
        }

        GameServer.Connect(Hostname, Port);
        if (GameServer.Connected == false) 
        { 

        }

    }

}

