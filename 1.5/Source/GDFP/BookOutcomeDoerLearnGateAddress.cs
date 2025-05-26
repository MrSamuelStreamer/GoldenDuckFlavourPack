using RimWorld;
using Verse;

namespace GDFP;

public class BookOutcomeDoerLearnGateAddress: BookOutcomeDoer
{
    public BookOutcomeProperties_LearnGateAddress Props
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

        if(Props == null)
        {
            LearnGateAddress(GateAddress.RandomGateAddress());
            return;
        }

        GateAddress address = new()
        {
            address = string.IsNullOrEmpty(Props.planetAddress) ? GateAddress.RandomGateAddressString() : Props.planetAddress,
            biome = Props.planetBiome ?? GateAddress.GetBiome(),
            temperature = Props.planetTemperature ?? Rand.Range(-45, 45),
            name = string.IsNullOrEmpty(Props.planetName) ? GateAddress.RandomGateName() : Props.planetName,
            faction = Props.planetFaction,
            extraGenSteps = Props.ExtraGenSteps()
        };

        if (Props.planetIncident != null)
        {
            if (Props.planetIncidentChance.HasValue)
            {
                if (Rand.Chance(Props.planetIncidentChance.Value))
                {
                    address.incidentToTrigger = Props.planetIncident;
                }
            }
            else
            {
                address.incidentToTrigger = Props.planetIncident;
            }
        }

        address.description = string.IsNullOrEmpty(Props.planetDescription) ? $"Biome = {address.biome.label}; Avg Temperature: {address.temperature}" : Props.planetDescription;


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
        return true;
    }
}
