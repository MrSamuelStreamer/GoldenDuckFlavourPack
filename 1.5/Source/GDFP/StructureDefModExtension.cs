using System.Collections.Generic;
using Verse;

namespace GDFP;

public class StructureDefModExtension: DefModExtension
{
    public string author;
    public bool standalone = true;
    public List<PawnRepr> spawnedPawns;
}
