using RimWorld;
using Verse;

namespace GDFP;

public class SpecialThingFilterWorker_GoldApparel : SpecialThingFilterWorker
{
    public override bool Matches(Thing t) => t is Apparel apparel && apparel.Stuff == ThingDefOf.Gold;

    public override bool CanEverMatch(ThingDef def)
    {
        return def.IsApparel && def.MadeFromStuff;
    }
}
