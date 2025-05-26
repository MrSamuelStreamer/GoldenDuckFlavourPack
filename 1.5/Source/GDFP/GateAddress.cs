using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace GDFP;

public class GateAddress: IExposable, ILoadReferenceable
{
    public static GateAddress CurrentGateAddress;
    public static Faction SelectedFaction;
    public static GDFP_WorldComponent WorldComponent => Find.World.GetComponent<GDFP_WorldComponent>();
    public static readonly char[] GateAddressSymbols = "öøÿĂƋƍƎƔƛƱƼǂƿƾȣȸɁɄɣɞɸɷʘʭʢʞ".ToCharArray();

    public string address;
    public BiomeDef biome;
    public float temperature;
    public int tile;
    public string name;
    public string description;
    public IncidentDef incidentToTrigger;
    public FactionDef faction;
    public Map map;
    public Map mapReference;

    public List<GenStepDef> extraGenSteps;

    public Map Map => map ?? mapReference;
    public static string RandomGateAddressString()
    {
        return new string(Enumerable.Repeat(GateAddressSymbols, 7)
            .Select(s => s.RandomElement()).ToArray());
    }

    public static GateAddress RandomGateAddress()
    {
        GateAddress address = new();
        address.address = RandomGateAddressString();
        address.biome = GetBiome();
        address.temperature = Rand.Range(-45, 45);
        address.tile = Find.World.grid.tiles.IndexOf(GetWorldTileMatching(address.temperature));
        return address;
    }

    public static string RandomGateName()
    {
        return $"P{Rand.Range(1, 9)}{Rand.Element(["A", "P", "Z", "X"])}-{Rand.Range(100, 999)}";
    }

    public static Lazy<List<BiomeDef>> PotentialBiomes = new(()=>[
        BiomeDefOf.BorealForest,
        BiomeDefOf.SeaIce,
        BiomeDefOf.Desert,
        BiomeDefOf.TemperateForest,
        BiomeDefOf.Tundra
    ]);

    public static BiomeDef GetBiome()
    {

        //return PotentialBiomes.Value.RandomElement();

        return DefDatabase<BiomeDef>.AllDefs.Where(b => b.generatesNaturally).RandomElement();
    }

    public static Tile GetWorldTileMatching(float temperature)
    {
        // expand range up to +/- 20.5 as needed to find the closes match, otherwise fall back to random
        for (int i = 0; i < 20; i++)
        {
            float tempRange = temperature + (0.5f + i);

            Tile tile =  Find.World.grid.tiles.Where(t => !t.WaterCovered && t.temperature > (temperature - tempRange) && t.temperature < (temperature + tempRange)).RandomElement();
            if (tile != null) return tile;
        }
        return Find.World.grid.tiles.Where(t => !t.WaterCovered).RandomElement();
    }

    public string Description
    {
        get {
            if (!string.IsNullOrEmpty(description)) return description;
            return "GDFP_AddressInspectorDescription".Translate(biome.label, temperature);
        }
    }

    public string Name => !string.IsNullOrEmpty(name) ? name : address;

    public void ExposeData()
    {
        Scribe_Values.Look(ref address, "address");
        Scribe_Defs.Look(ref biome, "biome");
        Scribe_Values.Look(ref temperature, "temperature");
        Scribe_Values.Look(ref tile, "tile");
        Scribe_Values.Look(ref name, "name");
        Scribe_Values.Look(ref description, "description");
        Scribe_Deep.Look(ref map, "map");
        Scribe_References.Look(ref mapReference, "mapReference");
        Scribe_Defs.Look(ref incidentToTrigger, "incidentToTrigger");
        Scribe_Defs.Look(ref faction, "faction");
    }

    public static void GenerateNewPlanetMap(Building_Quackaai entryGate, out Map planetMap, out Building_QuackaaiExit exitGate, GateAddress address = null)
    {
        address ??= RandomGateAddress();

        CurrentGateAddress = address;

        List<GenStepWithParams> gtwp = address.extraGenSteps == null ? [] : address.extraGenSteps.Select(gs => new GenStepWithParams(gs, new GenStepParams())).ToList();

        PocketMapParent mapParent = WorldObjectMaker.MakeWorldObject(WorldObjectDefOf.PocketMap) as PocketMapParent;
        mapParent.sourceMap = entryGate.Map;
        planetMap = MapGenerator.GenerateMap(new IntVec3(GDFPMod.settings.planetWidth, 1, GDFPMod.settings.planetHeight), mapParent, GDFPDefOf.GDFP_Planet, gtwp, isPocketMap: true, extraInitBeforeContentGen: (Map map) =>
        {
            if (gtwp.Any(s => s.def.defName.ToLower().Contains("settlement")))
            {
                SelectedFaction = Find.FactionManager.RandomNonHostileFaction();
            }
            map.TileInfo.biome = address.biome;
            map.TileInfo.temperature = address.temperature;
        });
        Find.World.pocketMaps.Add(mapParent);

        // foreach (IntVec3 allCell in planetMap.AllCells)
            // planetMap.roofGrid.SetRoof(allCell, null);

        exitGate = planetMap.listerThings.ThingsOfDef(GDFPDefOf.GDFP_QuakkaaiExit).First() as Building_QuackaaiExit;
        if(exitGate != null)
            exitGate.entryGate = entryGate;

        WorldComponent.AddNewAddressAndGate(CurrentGateAddress, planetMap);

        if (address.incidentToTrigger != null)
        {
            IncidentParms parms = StorytellerUtility.DefaultParmsNow(address.incidentToTrigger.category, planetMap);
            Find.Storyteller.incidentQueue.Add(new QueuedIncident(new FiringIncident(address.incidentToTrigger, null, parms), Find.TickManager.TicksGame + 300));
        }

        CurrentGateAddress = null;
        SelectedFaction = null;
    }

    public string GetUniqueLoadID()
    {
        return "GDFP_GateAddress_" + name;
    }
}
