using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace GDFP;

public class CompHackableGeneGiver: CompHackable
{
    public CompProperties_HackableGeneGiver HackProps => (CompProperties_HackableGeneGiver)this.props;
    public override void ReceiveCompSignal(string signal)
    {
        if(HackProps.GenePool.NullOrEmpty()) return;
        if (signal != HackProps.Signal) return;

        Pawn user = parent.Map.thingGrid.ThingAt<Pawn>(parent.InteractionCell);
        if(user?.genes == null) return;

        List<GeneDef> pool = HackProps.GenePool.ToList();
        user.genes.GenesListForReading.ForEach(gene=>pool.Remove(gene.def));

        if(pool.NullOrEmpty()) return;
        GeneDef gene = pool.RandomElement();

        user.genes.AddGene(gene, false);

        Messages.Message("GDFP_GotNewGene".Translate(user.Named("PAWN"), gene.LabelCap), new LookTargets(user), MessageTypeDefOf.PositiveEvent);
    }

}
