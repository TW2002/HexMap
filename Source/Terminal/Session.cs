using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using System.Text.RegularExpressions;
using static System.Runtime.InteropServices.JavaScript.JSType;
using TradeWarsData;

namespace TerminalLib;

public class Session
{
    public string Name { get; set; }
    public string? Hostname { get; set; }
    public int Port { get; set; }
    public TelnetClient GameServer { get; set; }

    //public Session(string name, string? hostname = null, int port = 0)
    public Session(Game game)
    {
        Name = game.Name;
        Hostname = game.Hostname ;
        Port = game.Port;

        GameServer = new();
        GameServer.GameData = game;

        GameServer.DataReceived += OnReceive;
    }

    private void OnReceive(object sender, TelnetClient.DataEventArgs e)
    {
        Extractor.Parse(e.Data, e.GameData);
    }

    public void Connect()
    {
        if (Hostname == null) return;

        GameServer.Connect(Hostname, Port);
  
    }

}

