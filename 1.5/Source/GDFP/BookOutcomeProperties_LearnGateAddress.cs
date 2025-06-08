using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using KCSG;
using RimWorld;
using Verse;

namespace GDFP;

public class BookOutcomeProperties_LearnGateAddress: BookOutcomeProperties
{
    public class GenStepWithChance: IExposable
    {
        public GenStepDef step;
        public List<GenStepDef> conflictsWith;
        public List<GenStepDef> requires;
        public float chance;

        public void ExposeData()
        {
            Scribe_Defs.Look(ref step, "step");
            Scribe_Collections.Look(ref conflictsWith, "conflictsWith", LookMode.Def);
            Scribe_Collections.Look(ref requires, "requires", LookMode.Def);
            Scribe_Values.Look(ref chance, "chance");
        }
    }

    [CanBeNull] public BiomeDef planetBiome;
    public float? planetTemperature;
    [CanBeNull] public IncidentDef planetIncident;
    public float? planetIncidentChance;
    [CanBeNull] public FactionDef planetFaction;
    [CanBeNull] public string planetAddress;
    [CanBeNull] public string planetName;
    [CanBeNull] public string planetDescription;
    [CanBeNull] public List<GenStepWithChance> extraGenSteps;

    public List<GenStepDef> ExtraGenSteps()
    {
        List<GenStepDef> list = [];
        if (extraGenSteps != null)
        {
            foreach (GenStepWithChance genStepWithChance in extraGenSteps)
            {
                if (Rand.Chance(genStepWithChance.chance))
                {
                    if (!list.Any(s => genStepWithChance.conflictsWith.Contains(s)))
                    {
                        list.Add(genStepWithChance.step);
                        if(!genStepWithChance.requires.NullOrEmpty())
                            list.AddRange(genStepWithChance.requires);
                    }
                }
            }
        }

        return list;
    }

    public List<StructureLayoutDef> StructureLayouts()
    {
        List<StructureLayoutDef> layouts = DefDatabase<StructureLayoutDef>.AllDefsListForReading.Where(sld => sld.HasModExtension<StructureDefModExtension>()).ToList();

        StructureLayoutDef firstChoice = layouts.RandomElementWithFallback();
        if(firstChoice == null) return [];
        List<StructureLayoutDef> choices = [firstChoice];

        if(firstChoice.GetModExtension<StructureDefModExtension>().standalone) return choices;

        List<StructureLayoutDef> standaloneLayouts = layouts.Where(sld => sld.GetModExtension<StructureDefModExtension>().standalone).Except(firstChoice).ToList();

        choices.AddRange(standaloneLayouts.TakeRandomDistinct(new IntRange(1, 3).RandomInRange));

        return choices;
    }

    public override Type DoerClass => typeof(BookOutcomeDoerLearnGateAddress);

}
