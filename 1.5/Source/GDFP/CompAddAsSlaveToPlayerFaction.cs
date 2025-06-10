using RimWorld;
using Verse;

namespace GDFP;

public class CompAddAsSlaveToPlayerFaction: ThingComp
{
    public override void PostSpawnSetup(bool respawningAfterLoad)
    {
        if (!parent.def.CanHaveFaction || parent is not Pawn pawn) return;

        pawn.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Slave);
    }

}
