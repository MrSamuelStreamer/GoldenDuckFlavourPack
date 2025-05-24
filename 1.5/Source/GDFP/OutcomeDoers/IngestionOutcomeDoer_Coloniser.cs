using RimWorld;
using Verse;

namespace GDFP.OutcomeDoers;

public class IngestionOutcomeDoer_Coloniser : IngestionOutcomeDoer
{
    public XenotypeDef forcedXenotypeDef;

    protected override void DoIngestionOutcomeSpecial(Pawn pawn, Thing ingested, int ingestedCount)
    {
        RecruitUtility.Recruit(pawn, Faction.OfPlayer);
        pawn.genes?.SetXenotype(forcedXenotypeDef);
    }
}
