using System;
using System.Reflection;
using Verse;

namespace GDFP.HarmonyPatches;

using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

[HarmonyPatch]
public static class Patch_HasFreeWill_RemoveSlaveCheck
{
    [HarmonyTargetMethods]
    public static IEnumerable<MethodBase> TargetMethods()
    {
        Type worldComponentType = AccessTools.TypeByName("FreeWill.FreeWill_WorldComponent");
        Type mapComponentType = AccessTools.TypeByName("FreeWill.FreeWill_MapComponent");
        Type itabType = AccessTools.TypeByName("FreeWill.ITab_Pawn_FreeWill");
        yield return AccessTools.Method(worldComponentType, "HasFreeWill");
        yield return AccessTools.Method(worldComponentType, "FreeWillCanChange");
        yield return AccessTools.Method(worldComponentType, "TryRemoveFreeWill");
        yield return AccessTools.Method(mapComponentType, "SetPriorityAction");
        yield return AccessTools.Method(itabType, "DrawPawnFreeWillCheckbox");
    }

    public static MethodInfo IsSlaveProp = AccessTools.Property(typeof(Pawn), "IsSlaveOfColony").GetGetMethod();

    public static CodeInstruction TransferLabels(CodeInstruction orig, CodeInstruction replacement)
    {
        replacement.labels.AddRange(orig.labels);
        return replacement;
    }

    public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
    {
        bool foundSlaveCheck = false;
        CodeInstruction lastInstruction = null;
        foreach (CodeInstruction instruction in instructions)
        {
            // If we find a call to the IsSlaveOfColony property, we will start changing the instructions
            if (instruction.opcode == OpCodes.Callvirt && instruction.operand as MethodInfo == IsSlaveProp) foundSlaveCheck = true;

            // Swap out the last instruction which will be a load of the pawn with a nop if we are at the slave check
            if (foundSlaveCheck) lastInstruction = TransferLabels(lastInstruction, new CodeInstruction(OpCodes.Nop));

            if (lastInstruction != null) yield return lastInstruction;

            // Replace the current instruction with a false on the stack if we found the slave check to emulate it returning false
            lastInstruction = foundSlaveCheck ? TransferLabels(instruction,new CodeInstruction(OpCodes.Ldc_I4_0)) : instruction;
            foundSlaveCheck = false;
        }
        if (lastInstruction != null) yield return lastInstruction;
    }
}
