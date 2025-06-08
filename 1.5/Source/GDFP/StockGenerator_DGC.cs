using System.Collections.Generic;
using RimWorld;
using Verse;

namespace GDFP;

public class StockGenerator_DGC : StockGenerator
{
    public float chance;

    public GDFP_WorldComponent worldComponent => Find.World.GetComponent<GDFP_WorldComponent>();

    public override IEnumerable<Thing> GenerateThings(int forTile, Faction faction = null)
    {
        if (worldComponent.LearnedAddresses.Any(ga => ga.name == "DGC")) yield break;

        if(Rand.Value < chance) yield break;

        Thing book = ThingMaker.MakeThing(GDFPDefOf.GDFP_GateAddressBookSGC);
        book.stackCount = 1;
        yield return book;
    }

    public override bool HandlesThingDef(ThingDef thingDef)
    {
        return thingDef == GDFPDefOf.GDFP_GateAddressBookSGC;
    }

}
