using System;
using System.Reflection;
using JetBrains.Annotations;
using RimWorld;
using UnityEngine;
using Verse;

namespace GDFP.HarmonyPatches;

using System.Collections.Generic;
using System.Reflection.Emit;
using HarmonyLib;

[HarmonyPatch]
public static class EquipmentPrefixPatch
{
    [HarmonyTargetMethods]
    public static IEnumerable<MethodBase> TargetMethods()
    {
        Type HarmonyPrivateNormalPatch = AccessTools.TypeByName("EquipmentPrefix.HarmonyPrivateNormalPatch");
        yield return AccessTools.Method(HarmonyPrivateNormalPatch, "ApplyticksBetweenBurstShots");
    }

    public static bool Prefix(int originalValue, [CanBeNull] CompEquippable comp, ref int __result)
    {
        if (comp == null)
        {
            __result = originalValue;
            return false;
        }

        return true;
    }
}
