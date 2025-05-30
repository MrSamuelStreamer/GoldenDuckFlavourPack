using RimWorld;
using Verse;

namespace GDFP;

public class SpecialThingFilterWorker_NonGoldApparel : SpecialThingFilterWorker
{
    public override bool Matches(Thing t) => t is Apparel apparel && (apparel.Stuff == null || apparel.Stuff != ThingDefOf.Gold);

    public override bool CanEverMatch(ThingDef def)
    {
        return def.IsApparel && def.MadeFromStuff;
    }

}
