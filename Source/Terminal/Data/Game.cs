using System.Xml;
using System.Linq;


namespace TradeWarsData;

public class Game
{
    public string Name { get; set; }
    public string? Hostname { get; set; }
    public int Port { get; set; }

    public string? Login { get; set; }
    public string? Password { get; set; }
    public string? Letter { get; set; }
    public string? Trader { get; set; }
    public string? Ship { get; set; }
    public string? Planet { get; set; }
    public List<Sector> Sectors { get; set; }
    public Status Status { get; set; }


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

            Sector s = Sectors.Single(se => se.SectorId == sector);
            if (s == null) Sectors.Add(new(sector));
        }
 
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
