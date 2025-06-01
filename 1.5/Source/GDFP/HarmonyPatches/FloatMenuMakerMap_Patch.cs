using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.AI;

namespace GDFP.HarmonyPatches;

[HarmonyPatch(typeof(FloatMenuMakerMap))]
public static class FloatMenuMakerMap_Patch
{
    public static bool CanWearApparelLockedToXenotype(Apparel apparel, Pawn pawn, out string reason)
    {
        reason = null;
        if(!apparel.def.HasModExtension<GDFPModExtension>()) return true;

        GDFPModExtension extension = apparel.def.GetModExtension<GDFPModExtension>();
        if(extension.apparelAllowOnlyXenotypes.NullOrEmpty()) return true;
        if (extension.apparelAllowOnlyXenotypes!.Contains(pawn.genes.Xenotype))
        {
            reason = "GDFP_ApparelLockedToXenotype";
            return false;
        }

        return true;
    }

    [HarmonyPatch(nameof(FloatMenuMakerMap.ChoicesAtFor))]
    [HarmonyPostfix]
    public static void ChoicesAtForPatch(ref List<FloatMenuOption> __result, Vector3 clickPos, Pawn pawn)
    {
        if(__result.NullOrEmpty()) return;
        IntVec3 intVec3 = IntVec3.FromVector3(clickPos);

        List<Thing> thingList = intVec3.GetThingList(pawn.Map);
        foreach (Building_Quackaai portal in thingList.OfType<Building_Quackaai>())
        {
            if(portal is Building_QuackaaiExit) continue;

            if (portal.IsOpen && (portal.planetMap == null || !portal.planetMap.mapPawns.FactionsOnMap().Contains(Faction.OfPlayer)))
            {
                __result.Add(FloatMenuUtility.DecoratePrioritizedTask(new FloatMenuOption(portal.CloseCommandString, delegate
                {
                    Job job = JobMaker.MakeJob(GDFPDefOf.GDFP_CloseGate, portal);
                    job.playerForced = true;
                    pawn.jobs.TryTakeOrderedJob(job, JobTag.Misc, false);
                }, MenuOptionPriority.High, null, null, 0f, null, null, true, 0), pawn, portal, "ReservedBy", null));
            }
        }
    }

    [HarmonyPatch("AddHumanlikeOrders")]
    [HarmonyPostfix]
    public static void AddHumanlikeOrders_Patch(Vector3 clickPos, Pawn pawn, List<FloatMenuOption> opts)
    {
        // easier than a transpiler patch 🤣
        IntVec3 clickCell = IntVec3.FromVector3(clickPos);
        foreach (Apparel apparel in pawn.Map.thingGrid.ThingsAt(clickCell).OfType<Apparel>().Where(app=>app.def.HasModExtension<GDFPModExtension>()))
        {
            GDFPModExtension extension = apparel.def.GetModExtension<GDFPModExtension>();
            if(extension.apparelAllowOnlyXenotypes.NullOrEmpty()) continue;

            string menuOptLabel = "ForceWear".Translate((NamedArgument) apparel.LabelShort, (NamedArgument) (Thing) apparel);
            List<FloatMenuOption> matches = opts.Where(fmo=> fmo.Label == menuOptLabel).ToList();
            if(matches.NullOrEmpty()) continue;

            if(extension.apparelAllowOnlyXenotypes!.Contains(pawn.genes.Xenotype)) continue;

            matches.ForEach(match=>opts.Remove(match));

            opts.Add((new FloatMenuOption((string) ("CannotWear".Translate((NamedArgument) apparel.Label, (NamedArgument) (Thing) apparel) + ": " + "GDFP_ApparelLockedToXenotype".Translate().CapitalizeFirst()), (Action) null)));
        }
    }

}
