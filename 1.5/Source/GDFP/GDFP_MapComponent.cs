using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml;
using HarmonyLib;
using LudeonTK;
using RimWorld;
using UnityEngine;
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

    public static Lazy<FieldInfo> SaveStream = new Lazy<FieldInfo>(() => AccessTools.Field(typeof(ScribeSaver), "saveStream"));
    public static Lazy<FieldInfo> Writer = new Lazy<FieldInfo>(() => AccessTools.Field(typeof(ScribeSaver), "writer"));
    public static Lazy<MethodInfo> EnterNode = new Lazy<MethodInfo>(() => AccessTools.Method(typeof(ScribeSaver), "EnterNode"));
    public static Lazy<MethodInfo> ForceStop = new Lazy<MethodInfo>(() => AccessTools.Method(typeof(ScribeSaver), "ForceStop"));

    public static void InitMemorySaving(ScribeSaver saver, string documentElementName)
    {
        if (Scribe.mode != LoadSaveMode.Inactive)
        {
            Log.Error("Called InitSaving() but current mode is " + Scribe.mode);
            Scribe.ForceStop();
        }

        try
        {
            Scribe.mode = LoadSaveMode.Saving;
            Stream saveStream = new MemoryStream();
            SaveStream.Value.SetValue(saver, saveStream);
            XmlWriter writer = XmlWriter.Create(saveStream, new XmlWriterSettings { Indent = true, IndentChars = "\t" });
            Writer.Value.SetValue(saver, writer);

            writer.WriteStartDocument();
            EnterNode.Value.Invoke(saver, [documentElementName]);
        }
        catch (Exception ex)
        {
            Log.Error("Exception while saving: \n" + ex);
            ForceStop.Value.Invoke(saver, []);
            throw;
        }
    }

    public static Lazy<FieldInfo> anyInternalException = new Lazy<FieldInfo>(() => AccessTools.Field(typeof(ScribeSaver), "anyInternalException"));
    public static Lazy<FieldInfo> savingForDebug = new Lazy<FieldInfo>(() => AccessTools.Field(typeof(ScribeSaver), "savingForDebug"));
    public static Lazy<FieldInfo> loadIDsErrorsChecker = new Lazy<FieldInfo>(() => AccessTools.Field(typeof(ScribeSaver), "loadIDsErrorsChecker"));
    public static Lazy<FieldInfo> curPath = new Lazy<FieldInfo>(() => AccessTools.Field(typeof(ScribeSaver), "curPath"));
    public static Lazy<FieldInfo> savedNodes = new Lazy<FieldInfo>(() => AccessTools.Field(typeof(ScribeSaver), "savedNodes"));
    public static Lazy<FieldInfo> nextListElementTemporaryId = new Lazy<FieldInfo>(() => AccessTools.Field(typeof(ScribeSaver), "nextListElementTemporaryId"));
    public static Lazy<MethodInfo> ExitNode = new Lazy<MethodInfo>(() => AccessTools.Method(typeof(ScribeSaver), "ExitNode"));

    public static void FinalizeSaving(ScribeSaver saver)
    {
        if (Scribe.mode != LoadSaveMode.Saving)
        {
            Log.Error("Called FinalizeSaving() but current mode is " + Scribe.mode);
        }
        else
        {
            if (anyInternalException.Value.GetValue(saver) as bool? == true)
            {
                ForceStop.Value.Invoke(saver, []);
                ;
                throw new Exception("Can't finalize saving due to internal exception. The whole file would be most likely corrupted anyway.");
            }

            try
            {
                XmlWriter writer = Writer.Value.GetValue(saver) as XmlWriter;

                if (writer != null)
                {
                    ExitNode.Value.Invoke(saver, []);
                    writer.WriteEndDocument();
                    writer.Flush();
                    writer.Close();
                }

                Stream saveStream = SaveStream.Value.GetValue(saver) as Stream;

                if (saveStream != null)
                {
                    saveStream.Seek(0, SeekOrigin.Begin);
                    StreamReader sr = new(saveStream, Encoding.UTF8);
                    string xml = sr.ReadToEnd();

                    // Hack to put the xml into the clipboard
                    TextEditor te = new() { text = xml };
                    te.SelectAll();
                    te.Copy();
                    saveStream.Flush();
                    saveStream.Close();
                }

                Scribe.mode = LoadSaveMode.Inactive;
                savingForDebug.Value.SetValue(saver, false);
                loadIDsErrorsChecker.Value.SetValue(saver, null);
                curPath.Value.SetValue(saver, null);
                savedNodes.Value.SetValue(saver, null);
                nextListElementTemporaryId.Value.SetValue(saver, 0);
                anyInternalException.Value.SetValue(saver, false);
            }
            catch (Exception ex)
            {
                Log.Error("Exception in FinalizeLoading(): " + ex);
                ForceStop.Value.Invoke(saver, []);
                throw;
            }
        }
    }

    [DebugAction(
        "Map",
        "Export Colonists",
        false,
        false,
        false,
        false,
        0,
        false,
        actionType = DebugActionType.Action,
        allowedGameStates = AllowedGameStates.PlayingOnMap,
        requiresIdeology = false)]
    public static void ExportColonists()
    {
        InitMemorySaving(Scribe.saver, "spawnedPawns");

        foreach (Pawn pawn in Find.CurrentMap.mapPawns.FreeColonists)
        {
            PawnRepr repr = PawnRepr.FromPawn(pawn);
            Scribe_Deep.Look(ref repr, "li");
        }

        FinalizeSaving(Scribe.saver);
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
