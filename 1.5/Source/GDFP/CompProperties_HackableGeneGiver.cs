using System.Collections.Generic;
using RimWorld;
using Verse;

namespace GDFP;

public class CompProperties_HackableGeneGiver: CompProperties_Hackable
{
    public string Signal = "Hackend";
    public List<GeneDef> GenePool;

    public CompProperties_HackableGeneGiver()
    {
        compClass = typeof(CompHackableGeneGiver);
    }
}
