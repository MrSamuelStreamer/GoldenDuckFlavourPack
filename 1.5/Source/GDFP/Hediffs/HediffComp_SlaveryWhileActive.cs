using RimWorld;
using Verse;

namespace GDFP.Hediffs;

public class HediffComp_SlaveryWhileActive : HediffComp
{
    public HediffCompProperties_SlaveryWhileActive Props => (HediffCompProperties_SlaveryWhileActive)props;

    public override void CompPostMake()
    {
        base.CompPostMake();
        if (parent.pawn.IsSlave && parent.pawn.Faction.IsPlayer) return;
        ForceEnslave();
        if (ModsConfig.AnomalyActive && Props.forcedXenotypeDef is { } xeno)
        {
            parent.pawn?.genes?.SetXenotype(xeno);
        }
    }

    public void ForceEnslave()
    {
        if (parent.pawn.Faction != null && parent.pawn.Faction.def.hidden)
            parent.pawn.SetFactionDirect(null);
        parent.pawn.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Slave);
        if (!parent.pawn.guest.EverEnslaved)
            Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.EnslavedPrisonerNotPreviouslyEnslaved));
        parent.pawn.apparel.UnlockAll();
    }

    public override void CompPostPostRemoved()
    {
        base.CompPostPostRemoved();
        if (!parent.pawn.IsSlave) return;
        parent.pawn.guest.SetGuestStatus(Faction.OfPlayer, GuestStatus.Guest);
        parent.pawn.needs.mood?.thoughts.memories.TryGainMemory(ThoughtDefOf.FreedFromSlavery);
    }
}