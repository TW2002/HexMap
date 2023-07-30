using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using TradeWarsData;
using System.Net.NetworkInformation;

namespace TerminalLib;

public static class Extractor
{
    private static int cs = -1;   // Current sector being worked on.
    private static int ls = -1;   // last  sector from previous map line.
    private static int ship = -1;
    private static int planet = -1;
    private static string lastCase = "";
    private static Sector? sector = null;

    private static readonly Random rnd = new();


    public static void Parse(string data, Game game)
    {
        // Remove ANSI codes before parsing data. 
        string pattern = @"\x1B(?:[@-Z\\-_]|\[[0-?]*[ -/]*[@-~])";
        data = Regex.Replace(data, pattern, "");

        // Split into individual lines and enumerate. 
        var lines = data.Replace("\r", "").Split("\n");
        foreach (var line in lines)
        {
            try {
                if (line.Contains(" > ")) ParseCouse(line, game);

                //  remove parentheses from line
                string cl = line.Replace("(","")
                             .Replace(")", "");

                // Add last case to the lines without one. 
                cl = cl.Replace("        ", lastCase);

                // Remove extra whitespace and split into words for parsing. 
                var words = Regex.Replace(cl, @"\s\s+", " ").Split(" ");
                switch (words[0])
                {
                    // TWX Proxy Server v2.6.31a(Beta)
                    case "TWX":
                        game.TwxVersion = words[3];
                        break;

                    // Using Database data\NoverduF.xdb w/ 20000 sectors and 56430 warps
                    case "Using":
                        game.TwxDatabase = words[2];
                        game.TwxSectors = ParseInt(words, 4);
                        game.TwxWarps = ParseInt(words, 7);
                        game.MaxSectors = game.TwxSectors;
                        game.TwxDetected(); 
                        break;

                    case "::Stardock:": // Location from TWX.
                        int value = ParseInt(words, 1);
                        if (value > 0) game.Stardock = value;
                        break;

                    case "::Alpha:": // Location from TWX.
                        value = ParseInt(words, 1);
                        if (value > 0)
                        {
                            game.Alpha = value;
                            game.Nav1 = value;
                        }
                        else
                        {
                            game.Nav1 = rnd.Next(game.MaxSectors); 
                        }
                        break;

                    case "::Rylos:": // Location from TWX.
                        value = ParseInt(words, 1);
                        if (value > 0)
                        { 
                            game.Rylos = value;
                            game.Nav2 = value;
                        }
                        else
                        {
                            game.Nav2 = rnd.Next(game.MaxSectors);
                        }

                        break;



                    case "Command":  // Main Menu - Command Prompt
                    case "Computer": // Computer Menu
                    case "Corporate": // Corporate Menu
                        ParsePrompt(words, game);
                        break;


                    case "Sector":
                        // Sector number from display or holo scan. 
                        cs = ParseInt(words, 2);
                        if (cs > 0)
                        {
                            sector = game.Sectors.FirstOrDefault(s => s.SectorId == cs);
                            if (sector == null)
                            {
                                sector = new(cs);
                                game.Sectors.Add(sector);
                            }
                            //Sector  : 1335 in uncharted space.
                            sector.LastSeen = DateTime.UtcNow;
                            sector.Explored = true;
                            cl = cl.Substring(0, cl.IndexOf("."))
                                .Replace(words[2], "")
                                .Replace(" unexplored", "");
                            sector.Nebula = cl[14..];
                            break;
                        }

                        // Sector number from dens scan. 
                        cs = ParseInt(words, 1);
                        if (cs > 0)
                        {
                            sector = game.Sectors.FirstOrDefault(s => s.SectorId == cs);
                            if (sector == null)
                            {
                                sector = new(cs);
                                game.Sectors.Add(sector);
                            }


                            //Sector ( 4194) ==> 600  Warps : 2 NavHaz : 0% Anom : Yes

                            sector.Density = ParseInt(words, 3);
                            sector.WarpCount = ParseInt(words, 6);
                            sector.NavHaz = ParseInt(words, 9);
                            sector.Anom = ParseBool(words, 12);
                        }
                        break;


                    // Beacon  : FedSpace, FedLaw Enforced
                    case "Beacon":
                        if (sector != null) 
                            sector.Beacon = cl[10..];
                        break;

                    //Ports: Marcini Minor, Class 3(SBB)
                    case "Ports":
                        if (sector != null)
                        {
                            sector.Port.Name = cl[10..].Split(",")[0];
                            sector.Port.Class = ParseInt(cl.Split(",")[1].Split(" "), 2);
                        }
                        break;

                    //Planets: <<<< (M)Ferrengal >>>> (Shielded)
                    case "Planets":
                        //sector = ParseInt(words, 1);
                        if (sector != null)
                        {
                            if (lastCase != "Planets ")
                            {
                                lastCase = "Planets ";
                                sector.PlanetCount = 1;
                                sector.FirstPlanet = cl[10..];
                            }
                            else
                            {
                                sector.PlanetCount++;
                            }
                        }
                        break;

                    //Ships   : OtherFishes Star[Owned by] Sharkbait[1], w / 400,000 ftrs,
                    case "Ships":
                        if (sector != null)
                        {
                            if (lastCase != "Ships   ")
                            {
                                lastCase = "Ships   ";
                                sector.ShipCount = 1;
                                sector.FirstShip = cl[10..].Split(",")[0];
                            }
                            else
                            {
                                sector.ShipCount++;
                            }
                        }
                        break;

                    //Fighters: 1,000,000(belong to your Corp)[Defensive]
                    case "Fighters":
                        //sector = ParseInt(words, 1);
                        break;

                    //Mines: 250(Type 1 Armid)(belong to your Corp)
                    //     : 250(Type 2 Limpet)(belong to your Corp)
                    case "Mines":
                        //sector = ParseInt(words, 1);
                        lastCase = "Mines";
                        break;

                    //Warps to Sector(s) :  (4194) - 18590
                    case "Warps":
                        if (sector == null) break;
                        sector.WarpsOut.Clear();
                        for (int w = 4; w < 15; w++)
                        {
                            int warp = ParseInt(words, w);
                            if (warp > 0) sector.WarpsOut.Add(warp);
                        }
                        sector.WarpCount = sector.WarpsOut.Count;
                        // Todo add warps in to destination secures.
                        break;



                    // Planet #9 in sector 1335: <Name>
                    case "Planet":  // Used several times on planet display. 
                        int p = ParseInt(words, 1);
                        if (p > 0)
                        {
                            planet = p;
                            cs = ParseInt(words, 4);
                            game.MovedTo(cs);
                        }
                        else
                        { //todo
                        }
                        break;

                }
                // 
                // "\u001b[1;32mSector"
                //string s = e.Data.Split(" ")[1];
            }
            catch { }
        }
    }

    private static void ParseCouse(string line, Game game)
    {
        int cw = -1;
        int lw = -1;

                                                                string[] sectors = line.Trim().Split(" > ");
        // Enumerate each sector 
        foreach(string sector in sectors)
        {
            if ((sector == "FM") || (sector == "TO")) return;

            try
            {
                lw = cw;
                   cw = -1;
                              cw = Int32.Parse(sector);
            } catch { }

            if ((cw > 0) && (lw > 0))
            {
                game.AddWarp(lw, cw);
            }       
        }
    }

    private static int ParseInt(string s)
    {
        int value = -1;
        try
        {
            value = Int32.Parse(s);
        }
        catch { }

        return value;
    }

    private static int ParseInt(string[] words, int word)
    {
        int value = -1;
        try
        {
            value = Int32.Parse(words[word]
                .Replace(",", "")
                .Replace(":", "")
                .Replace("%", "")
                .Replace("#", ""));
        }
        catch { }

        return value;
    }

    private static bool ParseBool(string[] words, int word)
    {
        bool value = false;
        try
        {
            if (words[word].ToLower() == "yes")
            {
                value = true;
            }
        }
        catch { }

        return value;
    }

    private static void ParsePrompt(string[] words, Game game)
    {
        int value = -1;
        string tl= "";
        string[] s;
        try
        {
            if (words[1].Contains("]:["))
            {
                s = words[1].Split("]:[");
            }
            else
            {
                s = words[2].Split("]:[");
            }
            value = Int32.Parse(s[1].Replace("]", ""));
            tl = s[0].Replace("[TL=", "");
        }
        catch { }

        game.Status.Sector = value;
        game.Status.TimeLeft = tl;
        game.MovedTo(value);
    }
}


