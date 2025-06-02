using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace GDFP;

public class CompHackableGeneGiver: ThingComp
{
    public CompProperties_HackableGeneGiver Props => (CompProperties_HackableGeneGiver)this.props;
    public override void ReceiveCompSignal(string signal)
    {
        if(Props.GenePool.NullOrEmpty()) return;
        if (signal != Props.Signal) return;

        Pawn user = parent.Map.thingGrid.ThingAt<Pawn>(parent.InteractionCell);
        if(user?.genes == null) return;

        List<GeneDef> pool = Props.GenePool.ToList();
        user.genes.GenesListForReading.ForEach(gene=>pool.Remove(gene.def));

        if(pool.NullOrEmpty()) return;
        GeneDef gene = pool.RandomElement();

        user.genes.AddGene(gene, true);

        Messages.Message("GDFP_GotNewGene".Translate(user.Named("PAWN"), gene.LabelCap), new LookTargets(user), MessageTypeDefOf.PositiveEvent);
    }

}
