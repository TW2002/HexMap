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
    public bool Explored { get; set; }
    public bool Hostile  { get; set; }

    public string? Nebula { get; set; }
    public string? Beacon { get; set; }
    public PortData Port { get; set; }

    public int Density { get; set; }
    public bool Anom { get; set; }
    public int NavHaz { get; set; }
    public int PlanetCount { get; set; }
    public int ShipCount { get; set; }
    public int WarpCount { get; set; }
    public int Backdoors { get; set; }
    public string? FirstPlanet { get; set; }
    public string? FirstShip { get; set; }
    public List<int> WarpsOut { get; set; }
    public List<int> WarpsIn { get; set; }

    public Sector(int sector)
    {
        SectorId = sector;

        LastSeen = DateTime.UtcNow;
        //SeenBy = seenBy;

        Port = new(sector);
        WarpsOut = new();
        WarpsIn = new();
    }
    public class PortData
    {
        public int SectorId { get; set; }
        public string? Name { get; set; }
        public int Class { get; set; }

        public DateTime LastSeen { get; set; }
        public string? SeenBy { get; set; }
        public DateTime LastBust { get; set; }
        public string? BustedBy { get; set; }


        public int OreOnHand { get; set; }
        public int OrgOnHand { get; set; }
        public int EquOnHand { get; set; }
        public int OrePerc { get; set; }
        public int OrgPerc { get; set; }
        public int EquPerc { get; set; }
        public int OreMinMIC { get; set; }
        public int OreMaxMCIC { get; set; }
        public int OrgMinMCIC { get; set; }
        public int OrgMaxMCIC { get; set; }
        public int EquMinMCIC { get; set; }
        public int EquMaxMCIC { get; set; }

        public PortData(int sector)
        {
            SectorId = sector;

            //LastSeen = DateTime.UtcNow;
            //SeenBy = seenBy;
        }
    }
}

