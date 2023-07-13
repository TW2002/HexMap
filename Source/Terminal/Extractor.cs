using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TradeWarsData;

namespace TerminalLib;

public static class Extractor
{
    public static void Parse(string data, Game game)
    {
        int sector = -1;
        int ship = -1;
        int planet = -1;

        // Remove ANSI codes before parsing data. 
        string pattern = @"\x1B(?:[@-Z\\-_]|\[[0-?]*[ -/]*[@-~])";
        data = Regex.Replace(data, pattern, "");

        // Split into individual lines and enumerate. 
        var lines = data.Replace("\r", "").Split("\n");
        foreach (var line in lines)
        {
            try
            {
                // Remove extra whitespace and split into words for parsing. 
                var words = Regex.Replace(line, @"\s\s+", " ").Split(" ");
                       switch (words[0])
                {
                    // Main Prompts with [TL=]:[Sector]
                    case "Command":  // Main Menu - Command Prompt
                    case "Computer": // Computer Menu
                    case "Corporate": // Corporate Menu
                        ParsePrompt(words, game);
                        break;




                    case "Sector":  // Sector number from dens scan. 
                        sector = ParseInt(words, 1);
                        break;
                    case "Sector:":  // Sector number from display or holo scan. 
                        sector = ParseInt(words, 1);
                        break;

                        // Planet #9 in sector 1335: <Name>
                        case "Planet":  // 
                        int p = ParseInt(words, 1);
                        if (p > 0)
                        {
                            planet = p;
                            sector = ParseInt(words, 4);
                            game.MovedTo(sector);
                        }
                        break;

                }
                // "\u001b[1;32mSector"
                //string s = e.Data.Split(" ")[1];
            }
            catch { }
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
            value = Int32.Parse(words[word].Replace(":", "").Replace("#", ""));
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
    }
}


