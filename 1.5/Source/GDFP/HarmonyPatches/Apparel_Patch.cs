using HarmonyLib;
using RimWorld;
using Verse;

namespace GDFP.HarmonyPatches;

[HarmonyPatch(typeof(Apparel))]
public static class Apparel_Patch
{
    [HarmonyPatch(nameof(Apparel.PawnCanWear))]
    [HarmonyPostfix]
    public static void PawnCanWear_Patch(Apparel __instance, Pawn pawn, ref bool __result)
    {
        if(!__result) return;

        if(!__instance.def.HasModExtension<GDFPModExtension>()) return;

        GDFPModExtension extension = __instance.def.GetModExtension<GDFPModExtension>();

        if(extension.apparelAllowOnlyXenotypes.NullOrEmpty()) return;

        __result = __result && extension.apparelAllowOnlyXenotypes!.Contains(pawn.genes.Xenotype);
    }
}
