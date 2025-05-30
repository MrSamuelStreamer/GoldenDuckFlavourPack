using RimWorld;

namespace GDFP;

public class Hediff_TieredAddiction: Hediff_Addiction
{
    public override int CurStageIndex => 2 - (int)(Need?.CurCategory ?? 0);
}
