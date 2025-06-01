using System.Collections.Generic;
using JetBrains.Annotations;
using RimWorld;
using Verse;

namespace GDFP;

public class GDFPModExtension : DefModExtension
{
    [CanBeNull] public GraphicData openingGraphicData;
    [CanBeNull] public List<XenotypeDef> apparelAllowOnlyXenotypes;
}
