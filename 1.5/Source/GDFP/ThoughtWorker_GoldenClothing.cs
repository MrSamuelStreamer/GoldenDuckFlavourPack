using RimWorld;

namespace GDFP;

public class ThoughtWorker_GoldenClothing: ThoughtWorker_ApparelThought
{
    protected override bool ApparelCounts(Apparel apparel) => apparel.Stuff == ThingDefOf.Gold;
}
