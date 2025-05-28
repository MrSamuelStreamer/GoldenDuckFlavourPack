using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using Hospitality.Patches;
using RimWorld;

namespace GDFP.Hospitality.HarmonyPatches;

[HarmonyPatch(typeof(JobGiver_Work_Patch))]
public static class JobGiver_Work_Patch_Patch
{

    [HarmonyPatch(typeof(JobGiver_Work_Patch.PawnCanUseWorkGiver), nameof(JobGiver_Work_Patch.PawnCanUseWorkGiver.Prefix))]
    [HarmonyTranspiler]
    static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        MethodInfo goodwillWithMethod = AccessTools.Method(typeof(Faction), nameof(Faction.GoodwillWith));
        bool found = false;

        foreach (CodeInstruction instruction in instructions)
        {
            if (!found && instruction.Calls(goodwillWithMethod))
            {
                found = true;
                // Pop the Faction.OfPlayer argument
                yield return new CodeInstruction(OpCodes.Pop);
                // Pop the this (Faction) reference
                yield return new CodeInstruction(OpCodes.Pop);
                // Push constant 100 onto the stack
                yield return new CodeInstruction(OpCodes.Ldc_R4, 100);
            }
            else
            {
                yield return instruction;
            }
        }

        if (!found)
        {
            ModLog.Error("Failed to patch Hospitality's JobGiver_Work_Patch - couldn't find GoodwillWith call");
        }
    }

}
