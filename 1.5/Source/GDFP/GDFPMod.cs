using Verse;
using UnityEngine;
using HarmonyLib;

namespace GDFP;

public class GDFPMod : Mod
{
    public static Settings settings;

    public GDFPMod(ModContentPack content) : base(content)
    {
        Log.Message("Hello world from GDFP");

        // initialize settings
        settings = GetSettings<Settings>();
#if DEBUG
        Harmony.DEBUG = true;
#endif
        Harmony harmony = new Harmony("MSSG.FlavourPack.GDFP.main");
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
