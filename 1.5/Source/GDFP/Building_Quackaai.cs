using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using Verse.Sound;

namespace GDFP;

public class Building_Quackaai: MapPortal
{
    public static GDFP_WorldComponent WorldComponent => Find.World.GetComponent<GDFP_WorldComponent>();

    // Handle alt texture for door closed
    private GDFPModExtension defExtension;

    public Graphic gateOpeningGraphic;
    protected bool HasExtension => defExtension != null;
    public virtual bool IsMainGate => true;

    public GateAddress selectedAddress;


    public bool isOpen = false;

    public virtual bool IsOpen
    {
        get => isOpen;
        set=>isOpen = value;
    }

    public void OpenGate()
    {
        if(selectedAddress == null) return;

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

    public virtual void CloseGate()
    {
        IsOpen = false;
        exitGate = null;
        selectedAddress = null;

        PocketMapUtility.DestroyPocketMap(planetMap);
        planetMap = null;
    }


    public override Graphic Graphic => IsOpen ? AlternateGraphic : base.Graphic;

    protected Graphic AlternateGraphic
    {
        get
        {
            if (gateOpeningGraphic != null)
            {
                return gateOpeningGraphic;
            }

            if (!HasExtension || defExtension.openingGraphicData == null)
            {
                return BaseContent.BadGraphic;
            }

            gateOpeningGraphic = defExtension.openingGraphicData.GraphicColoredFor(this);

            return gateOpeningGraphic;
        }
    }


    public override void SpawnSetup(Map map, bool respawningAfterLoad)
    {
        base.SpawnSetup(map, respawningAfterLoad);
        defExtension = def.GetModExtension<GDFPModExtension>();
    }

    public virtual string OpenCommandString => "GDFP_OpenPortal".Translate(Label);
    public virtual string CloseCommandString => "GDFP_ClosePortal".Translate(Label);


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
        Scribe_Values.Look(ref isOpen, "isOpen");
        Scribe_References.Look(ref planetMap, "planetMap");
        Scribe_References.Look(ref exitGate, "exitGate");
        Scribe_References.Look(ref selectedAddress, "selectedAddress");
    }

    public override IntVec3 GetDestinationLocation()
    {
        return exitGate?.Position ?? IntVec3.Invalid;
    }

    public override void OnEntered(Pawn pawn)
    {
        base.OnEntered(pawn);

        selectedAddress.Visited = true;
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

        GateAddress.GenerateNewPlanetMap(this, out planetMap, out exitGate, selectedAddress);
    }

}
