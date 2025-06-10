using RimWorld;
using UnityEngine;
using Verse;

namespace GDFP;

[DefOf]
[StaticConstructorOnStartup]
public static class GDFPDefOf
{
    public static SoundDef GDFP_Travel;
    public static SoundDef GDFP_Activate;

    public static JobDef GDFP_OpenGate;
    public static JobDef GDFP_CloseGate;
    public static JobDef GDFP_Replicate;

    public static ThingDef GDFP_Quakkaai;
    public static ThingDef GDFP_QuakkaaiExit;
    public static ThingDef GDFP_StrangeLetter;
    public static ThingDef GDFP_GateAddressBookSGC;

    public static MapGeneratorDef GDFP_Planet;
    public static MapGeneratorDef GDFP_PlanetStandalone;

    public static LetterDef GDFP_DeathQuestionMark;
    public static LetterDef GDFP_MapAuthor;
    public static LetterDef GDFP_PostieLetter;

    public static JobDef GDFP_GetGene;

    static GDFPDefOf()
    {
        DefOfHelper.EnsureInitializedInCtor(typeof(GDFPDefOf));
    }
}
