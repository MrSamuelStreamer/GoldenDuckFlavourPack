using RimWorld;
using Verse;

namespace GDFP;

public class BookOutcomeDoerLearnGateAddress: BookOutcomeDoer
{
    public BookOutcomeProperties_LearnGateAddress GateBookProps
    {
        get => (BookOutcomeProperties_LearnGateAddress) props;
    }

    public bool HasProvidedAddress = false;
    public static GDFP_WorldComponent worldComponent => Find.World.GetComponent<GDFP_WorldComponent>();

    public override bool DoesProvidesOutcome(Pawn reader) => true;

    public override void PostExposeData()
    {
        base.PostExposeData();
        Scribe_Values.Look(ref HasProvidedAddress, "HasProvidedAddress", false);
    }

    public override void OnReadingTick(Pawn reader, float factor)
    {
        if(HasProvidedAddress) return;
        HasProvidedAddress = true;

        if(GateBookProps == null)
        {
            LearnGateAddress(GateAddress.RandomGateAddress());
            return;
        }

        GateAddress address = new()
        {
            address = string.IsNullOrEmpty(GateBookProps.planetAddress) ? GateAddress.RandomGateAddressString() : GateBookProps.planetAddress,
            biome = GateBookProps.planetBiome ?? GateAddress.GetBiome(),
            temperature = GateBookProps.planetTemperature ?? Rand.Range(-45, 45),
            name = string.IsNullOrEmpty(GateBookProps.planetName) ? GateAddress.RandomGateName() : GateBookProps.planetName,
            faction = GateBookProps.planetFaction,
            extraGenSteps = GateBookProps.ExtraGenSteps()
        };

        if (GateBookProps.planetIncident != null)
        {
            if (GateBookProps.planetIncidentChance.HasValue)
            {
                if (Rand.Chance(GateBookProps.planetIncidentChance.Value))
                {
                    address.incidentToTrigger = GateBookProps.planetIncident;
                }
            }
            else
            {
                address.incidentToTrigger = GateBookProps.planetIncident;
            }
        }

        address.description = string.IsNullOrEmpty(GateBookProps.planetDescription) ? $"Biome = {address.biome.label}; Avg Temperature: {address.temperature}" : GateBookProps.planetDescription;


        LearnGateAddress(address);
    }

    public bool LearnGateAddress(GateAddress address)
    {
        if (worldComponent.LearnedAddresses.Any(ga => ga.address == address.address))
        {
            ModLog.Debug("Gate address already learned.");
            return false;
        }

        worldComponent.LearnedAddresses.Add(address);
        Messages.Message("GDFP_LearnedAddress".Translate(address.address), MessageTypeDefOf.PositiveEvent);
        return true;
    }
}
