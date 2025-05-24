using RimWorld;
using Verse;

namespace GDFP.Hediffs;

public class HediffCompProperties_SlaveryWhileActive : HediffCompProperties
{
    public XenotypeDef forcedXenotypeDef;
    public HediffCompProperties_SlaveryWhileActive() => compClass = typeof(HediffComp_SlaveryWhileActive);
}
