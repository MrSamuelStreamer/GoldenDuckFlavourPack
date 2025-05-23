using RimWorld;
using Verse;
using Verse.Sound;

namespace GDFP;

public class Building_QuackaaiExit: Building_Quackaai
{
    public override bool IsMainGate => false;

    public Building_Quackaai entryGate;

    public override Map GetOtherMap() => entryGate.Map;

    public override IntVec3 GetDestinationLocation() => entryGate.Position;
    public override void OnEntered(Pawn pawn)
    {
        base.OnEntered(pawn);
        if (Find.CurrentMap == Map)
        {
            SoundDefOf.TraversePitGate.PlayOneShot((SoundInfo) (Thing) this);
        }
        else
        {
            if (Find.CurrentMap != entryGate.Map)
                return;
            SoundDefOf.TraversePitGate.PlayOneShot((SoundInfo) (Thing) entryGate);
        }
    }
    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_References.Look(ref entryGate, "entryGate");
    }

}
