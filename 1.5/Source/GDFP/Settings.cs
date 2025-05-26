using UnityEngine;
using Verse;

namespace GDFP;

public class Settings : ModSettings
{
    public int planetWidth = 150;
    public int planetHeight = 150;

    public void DoWindowContents(Rect wrect)
    {
        Listing_Standard options = new();
        options.Begin(wrect);

        options.Label("GDFP_Settings_DefaultGatePlanetWidth".Translate(planetWidth));
        options.IntAdjuster(ref planetWidth, 10, 100);
        options.Gap();

        options.Label("GDFP_Settings_DefaultGatePlanetHeight".Translate(planetHeight));
        options.IntAdjuster(ref planetHeight, 10, 100);
        options.Gap();

        options.End();
    }

    public override void ExposeData()
    {
        Scribe_Values.Look(ref planetWidth, "planetWidth", 150);
        Scribe_Values.Look(ref planetHeight, "planetHeight", 150);
    }
}
