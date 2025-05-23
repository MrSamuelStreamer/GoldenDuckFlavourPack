using RimWorld;
using Verse;

namespace GDFP;

[DefOf]
public static class GDFPDefOf
{
    public static SoundDef GDFP_Travel;
    public static SoundDef GDFP_Activate;

    public static JobDef GDFP_OpenGate;

    public static ThingDef GDFP_QuakkaaiExit;

    public static MapGeneratorDef GDFP_Planet;

    static GDFPDefOf() => DefOfHelper.EnsureInitializedInCtor(typeof(GDFPDefOf));
}
