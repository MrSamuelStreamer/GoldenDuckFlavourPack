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
    [CanBeNull] public string planetFactionSearch;
    [CanBeNull] public FactionDef planetFaction;
    [CanBeNull] public string planetAddress;
    [CanBeNull] public string planetName;
    [CanBeNull] public string planetDescription;
    [CanBeNull] public List<GenStepWithChance> extraGenSteps;
    [CanBeNull] public StructureLayoutDef structure;

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
        if(structure != null) return [structure];

        return GateAddress.GetRandomStructureLayouts();
    }

    public override Type DoerClass => typeof(BookOutcomeDoerLearnGateAddress);

}
