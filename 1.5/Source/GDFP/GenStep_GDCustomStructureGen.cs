using System.Linq;
using KCSG;
using Verse;

namespace GDFP;

public class GenStep_GDCustomStructureGen: GenStep_CustomStructureGen
{
    public override void Generate(Map map, GenStepParams parms)
    {
        foreach (StructureLayoutDef structureLayoutDef in DefDatabase<StructureLayoutDef>.AllDefs.Where(d=>d.HasModExtension<StructureDefModExtension>()))
        {
            structureLayoutDefs.Add(structureLayoutDef);
        }

        base.Generate(map, parms);
    }

}
