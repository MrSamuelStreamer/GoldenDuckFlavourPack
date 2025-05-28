using HarmonyLib;
using UnityEngine;
using Verse;

namespace GDFP.Hospitality;

public class GDFPHospitalityMod : Mod
{
    public static Settings settings;

    public GDFPHospitalityMod(ModContentPack content) : base(content)
    {
        Log.Message("Hello world from GDFP.Hospitality");

        // initialize settings
        settings = GetSettings<Settings>();
#if DEBUG
        Harmony.DEBUG = true;
#endif
        Harmony harmony = new Harmony("MSSG.FlavourPack.GDFP.Hospitality.main");
        harmony.PatchAll();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        base.DoSettingsWindowContents(inRect);
        settings.DoWindowContents(inRect);
    }

    public override string SettingsCategory()
    {
        return "GDFP_Settings".Translate();
    }
}
