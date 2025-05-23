using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace GDFP;

public class Building_Quackaai: MapPortal
{
    // Handle alt texture for door closed
    private GDFPModExtension defExtension;

    public Graphic gateClosedGraphic;
    protected bool HasExtension => defExtension != null;
    public virtual bool IsMainGate => false;


    public bool IsOpen = false;

    public void OpenGate()
    {
        IsOpen = true;
        if (Find.CurrentMap == Map)
        {
            GDFPDefOf.GDFP_Activate.PlayOneShot((SoundInfo) (Thing) this);
        }
        else
        {
            if (Find.CurrentMap != exitGate.Map)
                return;
            GDFPDefOf.GDFP_Activate.PlayOneShot((SoundInfo) (Thing) exitGate);
        }
    }


    public override Graphic Graphic => !IsOpen ? AlternateGraphic : base.Graphic;

    protected Graphic AlternateGraphic
    {
        get
        {
            if (gateClosedGraphic != null)
            {
                return gateClosedGraphic;
            }

            if (!HasExtension || defExtension.extraGraphicData == null)
            {
                return BaseContent.BadGraphic;
            }

            gateClosedGraphic = defExtension.extraGraphicData.GraphicColoredFor(this);

            return gateClosedGraphic;
        }
    }


    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        defExtension = def.GetModExtension<GDFPModExtension>();
    }

    public virtual string OpenCommandString => "GDFP_OpenPortal".Translate(Label);


    public Map planetMap;
    public Building_QuackaaiExit exitGate;

    public override Map GetOtherMap()
    {
        if (planetMap == null)
            GenerateNewPlanetMap();
        return planetMap;
    }

    public override void ExposeData()
    {
        base.ExposeData();
        Scribe_Values.Look(ref IsOpen, "IsOpen");
        Scribe_References.Look(ref planetMap, "planetMap");
        Scribe_References.Look(ref exitGate, "exitGate");
    }

    public override IntVec3 GetDestinationLocation()
    {
        return exitGate?.Position ?? IntVec3.Invalid;
    }

    public override void OnEntered(Pawn pawn)
    {
        base.OnEntered(pawn);

        if (Find.CurrentMap == Map)
        {
            GDFPDefOf.GDFP_Travel.PlayOneShot((SoundInfo) (Thing) this);
        }
        else
        {
            if (Find.CurrentMap != exitGate.Map)
                return;
            GDFPDefOf.GDFP_Travel.PlayOneShot((SoundInfo) (Thing) exitGate);
        }
    }

    public virtual void GenerateNewPlanetMap()
    {
        if(planetMap != null) return;
        planetMap = PocketMapUtility.GeneratePocketMap(new IntVec3(100, 1, 100), GDFPDefOf.GDFP_Planet, null, Map);

        foreach (IntVec3 allCell in planetMap.AllCells)
            planetMap.roofGrid.SetRoof(allCell, null);

        exitGate = planetMap.listerThings.ThingsOfDef(GDFPDefOf.GDFP_QuakkaaiExit).First() as Building_QuackaaiExit;
        if(exitGate != null)
            exitGate.entryGate = this;
    }

}
