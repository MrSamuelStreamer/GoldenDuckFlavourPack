using System.Linq;
using Verse;

namespace GDFP;

public class GenStep_FindGateExit : GenStep
{
    public const float ClearRadius = 4.5f;

    public override int SeedPart => 12412314;

    public override void Generate(Map map, GenStepParams parms)
    {
        Building_QuackaaiExit bld = map.listerBuildings.AllBuildingsColonistOfClass<Building_QuackaaiExit>().FirstOrDefault();
        if (bld != null)
        {
            MapGenerator.PlayerStartSpot = bld.InteractionCell;
            return;
        }

        IntVec3 result;
        CellFinder.TryFindRandomCell(map, cell => cell.Standable(map) && cell.DistanceToEdge(map) > 5.5, out result);
        foreach (IntVec3 c in GenRadial.RadialCellsAround(result, 4.5f, true))
        {
            foreach (Thing thing in c.GetThingList(map).ToList().Where(t => t.def.destroyable))
                thing.Destroy();
        }
        GenSpawn.Spawn(ThingMaker.MakeThing(GDFPDefOf.GDFP_QuakkaaiExit), result, map);
        MapGenerator.PlayerStartSpot = result;
    }
}
