using System;
using System.Collections.Generic;
using System.Linq;
using KCSG;
using RimWorld;
using Verse;

namespace GDFP;

public class GenStep_GDCustomGen : GenStep
{
    public IntRange extraStructureCount = new(0, 3);

    public override void Generate(Map map, GenStepParams parms)
    {
        if (GateAddress.CurrentGateAddress.chosenStructures.NullOrEmpty())
        {
            List<GenStepDef> validDefs = [];
            List<GenStepDef> allDefs = DefDatabase<GenStepDef>.AllDefsListForReading;
            foreach (GenStepDef genStepDef in allDefs)
            {
                GenStep_GDCustomStructureGen genstep = genStepDef.genStep as GenStep_GDCustomStructureGen;
                if (genstep == null || genstep.structureLayoutDefs.NullOrEmpty() ||
                    !genstep.structureLayoutDefs.Any(sd => sd.HasModExtension<StructureDefModExtension>())) continue;

                validDefs.Add(genStepDef);
            }

            GenStepDef firstStep = validDefs.RandomElementWithFallback();

            if (firstStep == null) return;

            List<GenStepDef> otherSteps = [];

            if (!(firstStep.genStep is GenStep_GDCustomStructureGen genStep && genStep.structureLayoutDefs.Any(sd =>
                    sd.HasModExtension<StructureDefModExtension>() && sd.GetModExtension<StructureDefModExtension>().standalone)))
            {
                otherSteps = validDefs.Except(firstStep).Where(d =>
                    d.genStep is GenStep_GDCustomStructureGen gs &&
                    !gs.structureLayoutDefs.Any(sd => sd.HasModExtension<StructureDefModExtension>() && sd.GetModExtension<StructureDefModExtension>().standalone)
                ).ToList();

                if (!otherSteps.NullOrEmpty())
                {
                    int extra = extraStructureCount.RandomInRange;
                    extra = Math.Min(extra, otherSteps.Count);

                    for (int i = 0; i < extra; i++)
                    {
                        GenStepDef genStepDef = otherSteps.RandomElementWithFallback();
                        if (genStepDef == null) continue;
                        otherSteps.Remove(genStepDef);
                    }
                }
            }

            GateAddress.CurrentGateAddress.chosenStructures = [firstStep];
            GateAddress.CurrentGateAddress.chosenStructures.AddRange(otherSteps);
        }

        List<string> authors = [];
        foreach (GenStepDef genStepDef in GateAddress.CurrentGateAddress.chosenStructures)
        {
            List<string> authorsLocal = [];
            ((GenStep_GDCustomStructureGen) genStepDef.genStep).structureLayoutDefs.Where(sd => sd.HasModExtension<StructureDefModExtension>())
                .Select(sd => sd.GetModExtension<StructureDefModExtension>()).Where(mde => !string.IsNullOrEmpty(mde.author)).ToList()
                .ForEach(mde => authorsLocal.Add(mde.author));

            LongEventHandler.SetCurrentEventText("GDFP_LoadingBy".Translate(string.Join(", ", authorsLocal)));
            try
            {
                genStepDef.genStep.Generate(map, parms);
                // if (genStepDef.genStep is GenStep_GDCustomStructureGen genStep)
                // {
                //     foreach (StructureLayoutDef structureLayoutDef in genStep.structureLayoutDefs.Where(sd => sd.HasModExtension<StructureDefModExtension>()))
                //     {
                //         StructureDefModExtension mde = structureLayoutDef.GetModExtension<StructureDefModExtension>();
                //         if(mde.spawnedPawns.NullOrEmpty()) continue;
                //         foreach (Pawn mdeSpawnedPawn in mde.spawnedPawns)
                //         {
                //             ModLog.Debug($"Attempring to spawn pawn {mdeSpawnedPawn}");
                //             GenSpawn.Spawn(mdeSpawnedPawn, mdeSpawnedPawn.Position, map);
                //         }
                //     }
                // }
            }
            finally
            {
                authorsLocal.ForEach(authors.Add);
            }
        }

        if (authors.NullOrEmpty()) return;

        AuthorLetter(map, authors);
    }

    public static void AuthorLetter(Map map, List<string> authors)
    {
        TaggedString label = "GDFP_Author_Title".Translate();

        string authorStr = string.Join("\n", authors.Distinct().InRandomOrder());

        TaggedString text = "GDFP_Author_More".Translate(authorStr);

        AuthorLetter authorLetter = (AuthorLetter) LetterMaker.MakeLetter(GDFPDefOf.GDFP_MapAuthor);
        authorLetter.Label = label;
        authorLetter.Text = text;

        Find.LetterStack.ReceiveLetter(authorLetter);
    }

    public override int SeedPart => 1241521352;
}
