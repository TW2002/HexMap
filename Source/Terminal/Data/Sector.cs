using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TradeWarsData;

public class Sector
{
     public int SectorId { get; set; }
    public DateTime LastSeen { get; set; }
    public string? SeenBy { get; set; }
    public string? Name { get; set; }

    public Sector(int sector)
    {
        SectorId = sector;

        LastSeen = DateTime.UtcNow;
        //SeenBy = seenBy;
    }
}
