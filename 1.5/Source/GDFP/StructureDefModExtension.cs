using System.Collections.Generic;
using RimWorld;
using Verse;

namespace GDFP;

public class StructureDefModExtension: DefModExtension
{
    public string author;
    public bool standalone = true;
    public bool doLoot = true;
    public bool anyHostile = false;
    public BiomeDef biome;
    public IntVec2 size;
    public FactionDef pawnFaction;
    public List<PawnRepr> spawnedPawns;
    public List<GenStepDef> extraGenSteps;
}
