using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TradeWarsData;

namespace TerminalLib;

public class TelnetClient
{
    public EventHandler? xConnected;
    public EventHandler? Disconnected;
    public delegate void EventHandler(object sender, EventArgs e);
    public DataEventHandler? DataReceived;
    public delegate void DataEventHandler(object sender, DataEventArgs e);




    public Game GameData { get; set; }

    public Socket Client { get; private set; }
    public bool isConnected { get; private set; }
    public bool isConnecting { get; private set; }

    private byte[] buffer = new byte[8192];

    public TelnetClient()
    {
        Client = new(AddressFamily.InterNetwork,
            SocketType.Stream, ProtocolType.Tcp);
        isConnected = false;
        isConnecting = false;
    }

    public void Connect(string host, int port)
    {
        //IPAddress[] IPs = Dns.GetHostAddresses(host);
        var ipAddress = Dns.GetHostEntry(host)
            .AddressList
            .First(ip => ip.AddressFamily == AddressFamily.InterNetwork);

        Client.BeginConnect(ipAddress, port,
            new AsyncCallback(OnConnect), Client);
    }

    private void OnConnect(IAsyncResult ar)
    {
        Socket? s = (Socket?)ar.AsyncState;
        if (s == null) return;
        
        try
        {
            s.EndConnect(ar);
            s.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, s);

            isConnected = s.Connected;
        } 
        catch (Exception ex) 
        { }
    }

    private void OnReceive(IAsyncResult ar)
    {
        // This is the client that sent you
        // data. AsyncState is exactly what you passed into
        // the state parameter in BeginReceive
        Socket? s = (Socket?)ar.AsyncState;
        if (s == null) return;

        int bytes = s.EndReceive(ar);

        // if no bytes Received connection was closed.
        if (bytes == 0) return;

        // Handle received data in buffer, send reply to client etc...
        if (DataReceived != null)
        {
            DataEventArgs eArgs = new(Encoding.ASCII.GetString(buffer, 0, bytes), GameData);
            DataReceived(this, eArgs);
        }

        // Start a new async receive on the client to receive more data.
        s.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, OnReceive, s);

    }

    public class DataEventArgs : EventArgs
    {
        public string Data { get; set; }
        public Game GameData { get; set; }
        public DataEventArgs(string data, Game game)
        {
            Data = data;
            GameData = game;
        }
    }

    public void Send(string text)
    {
        if (!Client.Connected) return;
        //Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        
        byte[] data = Encoding.ASCII.GetBytes(text);
        Client.BeginSend(data, 0, data.Length, SocketFlags.None, OnSent, null);

        static void OnSent(IAsyncResult ar)
        {
            // Handle completion of the send operation
        }
    }


}

