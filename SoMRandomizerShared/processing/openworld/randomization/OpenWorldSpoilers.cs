using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using System.Collections.Generic;
using static SoMRandomizer.processing.openworld.randomization.OpenWorldSimulator;

namespace SoMRandomizer.processing.openworld.randomization
{
    /// <summary>
    /// Logging for open world spoilers.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldSpoilers
    {
        public static void logFailedSimulation(Dictionary<PrizeLocation, PrizeItem> prizePlacements, SimResult simulationResult)
        {
            Logging.log("Failed simulation attempt log.", "spoiler");
            List<PrizeLocation> allLocationsLeft = new List<PrizeLocation>();
            allLocationsLeft.AddRange(prizePlacements.Keys);
            // prize locations
            for (int cycleNum = 0; cycleNum < simulationResult.collectionCycles.Count; cycleNum++)
            {
                Logging.log("  Collection cycle:" + cycleNum, "spoiler");
                foreach (PrizeLocation prizeLocation in prizePlacements.Keys)
                {
                    if (simulationResult.collectionCycles[cycleNum].Contains(prizeLocation.locationName))
                    {
                        PrizeItem thisPrize = prizePlacements[prizeLocation];
                        Logging.log("    " + prizeLocation.locationName + " -> " + prizePlacements[prizeLocation].prizeName
                            + " [event 0x" + prizeLocation.eventNum.ToString("X")
                            + "] [flag 0x" + thisPrize.gotItemEventFlag.ToString("X") + "]", "spoiler");
                        allLocationsLeft.Remove(prizeLocation);
                    }
                }
            }
            // unreachable locatiosn if any
            Logging.log("Unreachable locations:", "spoiler");
            int numUnreachable = 0;
            foreach(PrizeLocation location in allLocationsLeft)
            {
                numUnreachable++;
                Logging.log(location.locationName + " [" + prizePlacements[location].prizeName + "]", "spoiler");
                Logging.log("  needs: ", "spoiler");
                foreach (string dependency in location.lockedByPrizes)
                {
                    Logging.log("  " + dependency, "spoiler");
                }
            }
            if (numUnreachable == 0)
            {
                Logging.log("(none)", "spoiler");
            }
        }

        public static void logOpenWorldSpoilers(Dictionary<PrizeLocation, PrizeItem> prizePlacements, SimResult simulationResult, RandoContext context)
        {
            Dictionary<int, byte> crystalOrbColorMap = ElementSwaps.getCrystalOrbElementMap(context);
            Logging.log("Open world spoilers:", "spoiler");
            Logging.log("[Checks]", "spoiler");
            List<PrizeLocation> allLocationsLeft = new List<PrizeLocation>();
            allLocationsLeft.AddRange(prizePlacements.Keys);
            // prize locations
            for (int cycleNum = 0; cycleNum < simulationResult.collectionCycles.Count; cycleNum++)
            {
                Logging.log("  Collection cycle:" + cycleNum, "spoiler");
                foreach (PrizeLocation prizeLocation in prizePlacements.Keys)
                {
                    if (simulationResult.collectionCycles[cycleNum].Contains(prizeLocation.locationName))
                    {
                        PrizeItem thisPrize = prizePlacements[prizeLocation];
                        Logging.log("    " + prizeLocation.locationName + " -> " + prizePlacements[prizeLocation].prizeName 
                            + " [event 0x" + prizeLocation.eventNum.ToString("X") 
                            + "] [flag 0x" + thisPrize.gotItemEventFlag.ToString("X") + "]", "spoiler");
                        allLocationsLeft.Remove(prizeLocation);
                    }
                }
            }
            // unreachable locatiosn if any
            Logging.log("Unreachable locations:", "spoiler");
            int numUnreachable = 0;
            foreach (PrizeLocation location in allLocationsLeft)
            {
                numUnreachable++;
                Logging.log(location.locationName + " [" + prizePlacements[location].prizeName + "]", "spoiler");
                Logging.log("  needs: ", "spoiler");
                foreach (string dependency in location.lockedByPrizes)
                {
                    Logging.log("  " + dependency, "spoiler");
                }
            }
            if (numUnreachable == 0)
            {
                Logging.log("(none)", "spoiler");
            }

            // starting weapons
            Logging.log("[Weapons]", "spoiler");
            int boyStarterWeapon = context.workingData.getInt(StartingWeaponRandomizer.BOY_START_WEAPON_INDEX);
            int girlStarterWeapon = context.workingData.getInt(StartingWeaponRandomizer.GIRL_START_WEAPON_INDEX);
            int spriteStarterWeapon = context.workingData.getInt(StartingWeaponRandomizer.SPRITE_START_WEAPON_INDEX);
            string boyStartWeaponName = SomVanillaValues.weaponByteToName(boyStarterWeapon);
            string girlStartWeaponName = SomVanillaValues.weaponByteToName(girlStarterWeapon);
            string spriteStartWeaponName = SomVanillaValues.weaponByteToName(spriteStarterWeapon);
            if (boyStartWeaponName != "")
            {
                Logging.log("Boy starting weapon = " + boyStartWeaponName, "spoiler");
            }
            if (girlStartWeaponName != "")
            {
                Logging.log("Girl starting weapon = " + SomVanillaValues.weaponByteToName(girlStarterWeapon), "spoiler");
            }
            if (spriteStartWeaponName != "")
            {
                Logging.log("Sprite starting weapon = " + SomVanillaValues.weaponByteToName(spriteStarterWeapon), "spoiler");
            }

            // orb elements
            Logging.log("[Orb elements]", "spoiler");
            Logging.log("Element to open earth palace = " + (crystalOrbColorMap[ElementSwaps.ORBMAP_EARTHPALACE] != 0xFF ? SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_EARTHPALACE], false) : "none"), "spoiler");
            Logging.log("Element to open fire palace 1 = " + (crystalOrbColorMap[ElementSwaps.ORBMAP_FIREPALACE1] != 0xFF ? SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_FIREPALACE1], false) : "none"), "spoiler");
            Logging.log("Element to open fire palace 2 = " + (crystalOrbColorMap[ElementSwaps.ORBMAP_FIREPALACE2] != 0xFF ? SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_FIREPALACE2], false) : "none"), "spoiler");
            Logging.log("Element to open fire palace 3 = " + (crystalOrbColorMap[ElementSwaps.ORBMAP_FIREPALACE3] != 0xFF ? SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_FIREPALACE3], false) : "none"), "spoiler");
            Logging.log("Element to open luna palace = " + (crystalOrbColorMap[ElementSwaps.ORBMAP_LUNAPALACE] != 0xFF ? SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_LUNAPALACE], false) : "none"), "spoiler");
            Logging.log("Element to open matango cave = " + (crystalOrbColorMap[ElementSwaps.ORBMAP_MATANGO] != 0xFF ? SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_MATANGO], false) : "none"), "spoiler");
            if (crystalOrbColorMap.ContainsKey(41))
            {
                Logging.log("Element to cross upper land = " + (crystalOrbColorMap[ElementSwaps.ORBMAP_UPPERLAND] != 0xFF ? SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_UPPERLAND], false) : "none"), "spoiler");
            }
            Logging.log("Element to open lost continent gnome room = " + (crystalOrbColorMap[ElementSwaps.ORBMAP_GRANDPALACE1] != 0xFF ? SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_GRANDPALACE1], false) : "none"), "spoiler");
            Logging.log("Element to open lost continent undine room = " + (crystalOrbColorMap[ElementSwaps.ORBMAP_GRANDPALACE2] != 0xFF ? SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_GRANDPALACE2], false) : "none"), "spoiler");
            Logging.log("Element to open lost continent sylphid room = " + (crystalOrbColorMap[ElementSwaps.ORBMAP_GRANDPALACE3] != 0xFF ? SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_GRANDPALACE3], false) : "none"), "spoiler");
            Logging.log("Element to open lost continent salamando room = " + (crystalOrbColorMap[ElementSwaps.ORBMAP_GRANDPALACE4] != 0xFF ? SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_GRANDPALACE4], false) : "none"), "spoiler");
            Logging.log("Element to open lost continent lumina room = " + (crystalOrbColorMap[ElementSwaps.ORBMAP_GRANDPALACE5] != 0xFF ? SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_GRANDPALACE5], false) : "none"), "spoiler");
            Logging.log("Element to open lost continent shade room = " + (crystalOrbColorMap[ElementSwaps.ORBMAP_GRANDPALACE6] != 0xFF ? SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_GRANDPALACE6], false) : "none"), "spoiler");
            Logging.log("Element to open lost continent luna room = " + (crystalOrbColorMap[ElementSwaps.ORBMAP_GRANDPALACE7] != 0xFF ? SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[ElementSwaps.ORBMAP_GRANDPALACE7], false) : "none"), "spoiler");

            Logging.log("End open world spoilers.", "spoiler");
        }
    }
}
