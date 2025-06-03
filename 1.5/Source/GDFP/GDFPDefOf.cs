using RimWorld;
using Verse;

namespace GDFP;

[DefOf]
public static class GDFPDefOf
{
    public static SoundDef GDFP_Travel;
    public static SoundDef GDFP_Activate;

    public static JobDef GDFP_OpenGate;
    public static JobDef GDFP_CloseGate;

    public static ThingDef GDFP_QuakkaaiExit;

    public static MapGeneratorDef GDFP_Planet;

    public static LetterDef GDFP_DeathQuestionMark;

    public static JobDef GDFP_GetGene;

    static GDFPDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(GDFPDefOf));
}
