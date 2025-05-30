using RimWorld;

namespace GDFP;

public class Hediff_TieredAddiction: Hediff_Addiction
{
    public override int CurStageIndex => (int)(Need?.CurCategory ?? 0);
}
