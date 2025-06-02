using System.Collections.Generic;
using Verse;

namespace GDFP;

public class CompProperties_HackableGeneGiver: CompProperties
{
    public string Signal = "Hackend";
    public List<GeneDef> GenePool;

    public CompProperties_HackableGeneGiver()
    {
        compClass = typeof(CompHackableGeneGiver);
    }
}
