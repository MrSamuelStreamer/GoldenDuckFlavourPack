using System;
using System.Collections.Generic;
using LudeonTK;
using RimWorld;
using RimWorld.Planet;
using Verse;

namespace GDFP;

public class GDFP_MapComponent : MapComponent
{
    public IntRange TimeBetweenLettersRange = new(GenDate.TicksPerQuadrum, GenDate.TicksPerSeason);
    public int nextLetterTick = -1;

    public GDFP_MapComponent(Map map) : base(map)
    {
    }

    public override void MapComponentTick()
    {
        if (nextLetterTick < 0)
        {
            nextLetterTick = Find.TickManager.TicksGame + TimeBetweenLettersRange.RandomInRange;
        }

        if (Find.TickManager.TicksGame >= nextLetterTick)
        {
            nextLetterTick = Find.TickManager.TicksGame + TimeBetweenLettersRange.RandomInRange;

            DeathLetter(map);
        }
    }

    [DebugAction("Map", "Send faux death letter", false, false, false, false, 0, false, actionType = DebugActionType.Action, allowedGameStates = AllowedGameStates.PlayingOnMap,
        requiresIdeology = false)]
    public static void SendLetter()
    {
        DeathLetter(Find.CurrentMap);
    }

    public static void DeathLetter(Map map)
    {
        Pawn duck = map.mapPawns.AllPawns.FirstOrDefault(t => t.def.defName == "MSSG_Duck");

        if (duck == null) return;

        TaggedString diedLetterText = "GDFP_Alive".Translate(duck.Named("PAWN"));

        TaggedString letter = diedLetterText.AdjustedFor(duck);

        TaggedString label = "Death".Translate() + ": " + duck.LabelShortCap + "?";

        if (duck.Name is { Numerical: false } && duck.RaceProps.Animal)
            label += " (" + duck.KindLabel + ")";

        Find.LetterStack.ReceiveLetter(label, letter, GDFPDefOf.GDFP_DeathQuestionMark, (Thing) duck);
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref nextLetterTick, "nextLetterTick");
    }
}
