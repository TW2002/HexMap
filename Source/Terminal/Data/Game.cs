using System.Xml;
using System.Linq;


namespace TradeWarsData;

public class Game
{
    public StatusEventHandler? Moved;
    public StatusEventHandler? Updated;
    public delegate void StatusEventHandler(object sender, StatusEventArgs e);

    public string Name { get; set; }
    public string? Hostname { get; set; }
    public int Port { get; set; }

    public string? TwxVersion { get; set; }
    public string? TwxDatabase { get; set; }
    public int TwxSectors { get; set; }
    public int TwxWarps { get; set; }

    public int MaxSectors { get; set; }


    public string? Login { get; set; }

    public string? Password { get; set; }
    public string? Letter { get; set; }
    public string? Trader { get; set; }
    public string? Ship { get; set; }
    public string? Planet { get; set; }
    public List<Sector> Sectors { get; set; }
    public Status Status { get; set; }

    public int Stardock { get; set; }

    public int Alpha { get; set; }

    public int Rylos { get; set; }
    public int Nav1 { get; set; }
    public int Nav2 { get; set; }



    public Game(string name, string? hostname = null, int port = 0)
    {
        Name = name;
        if (hostname != null)
        {

            if (hostname.Contains(":"))
            {
                try
                {
                    Hostname = hostname.Split(":")[0];
                    Port = Int32.Parse(hostname.Split(":")[1]);
                }
                catch { Port = 0; }
            }
            else
            {
                Hostname = hostname;
                Port = port;
            }
        }

        Sectors = new List<Sector>();
        Status = new Status();


    }

    public void MovedTo(int sector, int ship = 0, int planet = 0)
    {
        if (sector > 0)
        {
            Status.Sector = sector;

            Sector? s = Sectors.FirstOrDefault(se => se.SectorId == sector);
            if (s == null) Sectors.Add(new(sector));
            if (Moved != null)
            {
                StatusEventArgs eArgs = new(sector);
                Moved(this, eArgs);
            }
        }
    }

    public void TwxDetected()
    {
        if (Updated == null) return;
        StatusEventArgs eArgs = new(0);
        Updated(this, eArgs);
    }

    public void AddWarps(int cs, int w1, int w2 = 0, int w3 = 0, int w4 = 0, int w5 = 0, int w6 = 0)
    {
        AddWarp(cs,w1);
        AddWarp(cs,w2);
        AddWarp(cs,w3);
        AddWarp(cs,w4);
        AddWarp(cs,w5);
        AddWarp(cs,w6);
    }
    public void AddWarp(int cs, int dest)
    {
        if (dest ==  0) return;

        Sector? sector = Sectors.FirstOrDefault(s => s.SectorId == cs);
        if (sector == null)
        {
            sector = new(cs);
            Sectors.Add(sector);
        }

        int wc = sector.WarpsOut.Count(w => w == dest);
        if (wc > 0) return;

        sector.WarpsOut.Add(dest);


    }
}
    


public class Games : LinkedList<Game>
{
    public void Save()
    {
        const int DEFAULT_PORT = 2002;
        XmlDocument doc = new();
        doc.AppendChild(doc.CreateXmlDeclaration("1.0", "UTF-8", null));
        XmlElement root = doc.CreateElement("Games");
        doc.AppendChild(root);

        foreach (Game game in this)
        {
            if (game.Port == 0) game.Port = DEFAULT_PORT;
            XmlElement g = doc.CreateElement("Game");
            g.SetAttribute("Name", game.Name);
            if (!string.IsNullOrEmpty(game.Hostname))
            {
                g.SetAttribute("Host", $"{game.Hostname}:{game.Port}");
            }
            AddElement(doc, g, "Login", game.Login);
            AddElement(doc, g, "Password", game.Password);
            AddElement(doc, g, "Letter", game.Letter);

            AddElement(doc, g, "Trader", game.Trader);
            AddElement(doc, g, "Ship", game.Ship);
            AddElement(doc, g, "Planet", game.Planet);

            root.AppendChild(g);
        }
        doc.Save("Games.xml");
    }

    private void AddElement(XmlDocument d, XmlElement e, string name, string? value)
    {
        if (!string.IsNullOrEmpty(value))
        {
            XmlElement x = d.CreateElement(name);
            x.InnerText = value;
            e.AppendChild(x);
        }

    }

    public void Load()
    {
        //var x = XElement.ReadFrom(XmlReader.Create("Games.xml"));
        XmlDocument doc = new();
        doc.Load("Games.xml");
        XmlNodeList nodes = doc.GetElementsByTagName("Game");
        //XmlNodeList list = node.ChildNodes;
        foreach (XmlNode node in nodes)
        {
            string name = "";
            string host = "";

            if (node.Attributes != null)
            {
                foreach (XmlAttribute a in node.Attributes)
                {
                    switch (a.Name)
                    {
                        case "Name":
                            name = a.Value;
                            break;
                        case "Host":
                            host = a.Value;
                            break;

                    }
                }
            }
            AddLast(new Game(name, host));
        }
    }
}

public class StatusEventArgs : EventArgs
{
    public int Sector { get; set; }
    public StatusEventArgs(int sector)
    {
        Sector = sector;
    }
}
