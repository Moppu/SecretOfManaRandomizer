using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.openworld.events;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.hacks.openworld.XmasRandoData;

namespace SoMRandomizer.processing.openworld.randomization
{
    /// <summary>
    /// Create events associated with open world christmas gift giving mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class GiftModeProcessing
    {
        public static Dictionary<string, string> possibleXmasGifts = new Dictionary<string, string>();
        // some breadcrumbs to tell the randomizer where to inject prizes and hints later
        private static byte[] OPENWORLD_GIFT_INJECTION_PATTERN = VanillaEventUtil.getBytes("%GIFT").ToArray();

        static GiftModeProcessing()
        {
            possibleXmasGifts["midge mallet"] = "The Midge Mallet";
            possibleXmasGifts["moogle belt"] = "The Moogle Belt";
            possibleXmasGifts["dryad spells"] = "Dryad Spells";
            possibleXmasGifts["luna spells"] = "Luna Spells";
            possibleXmasGifts["lumina spells"] = "Lumina Spells";
            possibleXmasGifts["shade spells"] = "Shade Spells";
            possibleXmasGifts["undine spells"] = "Undine Spells";
            possibleXmasGifts["salamando spells"] = "Salamando Spells";
            possibleXmasGifts["gnome spells"] = "Gnome Spells";
            possibleXmasGifts["sylphid spells"] = "Sylphid Spells";
            possibleXmasGifts["1000 gold"] = "1000 Gold";
            possibleXmasGifts["5000 gold"] = "5000 Gold";
            possibleXmasGifts["10000 gold"] = "10000 Gold";
            possibleXmasGifts["hat"] = "A new hat";
            // these ones are populated randomly in a copy of this dictionary on every generate
            possibleXmasGifts["hat#"] = "";
            possibleXmasGifts["accessory"] = "A new accessory";
            possibleXmasGifts["accessory#"] = "";
            possibleXmasGifts["armor"] = "A new armor";
            possibleXmasGifts["armor#"] = "";
            possibleXmasGifts["candy"] = "A piece of candy";
            possibleXmasGifts["chocolate"] = "A chocolate";
            possibleXmasGifts["royaljam"] = "A royal jam";
            possibleXmasGifts["walnut"] = "A faerie walnut";
            possibleXmasGifts["barrel"] = "A barrel";
        }

        public static List<PrizeLocation> processLocations(RandoSettings settings, RandoContext context, List<PrizeLocation> allLocations, List<PrizeItem> allPrizes)
        {
            string goal = context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME);
            if (goal != OpenWorldGoalProcessor.GOAL_GIFTMODE)
            {
                return allLocations;
            }
            List<PrizeLocation> filteredLocations = new List<PrizeLocation>(allLocations);
            int numGifts = settings.getInt(OpenWorldSettings.PROPERTYNAME_NUM_XMAS_GIFTS);
            List<NpcLocationData> npcs = new List<NpcLocationData>();
            for (int gift = 0; gift < numGifts; gift++)
            {
                int npcId = context.workingData.getInt(GiftDeliveryIntroEvent.GIFT_DELIVERY_INDEX_PREFIX + gift);
                npcs.Add(GiftDeliveryIntroEvent.DELIVERY_LOCATIONS[npcId]);
            }

            // don't allow rando items at the gift giving locations
            HashSet<PrizeLocation> removalLocations = new HashSet<PrizeLocation>();
            foreach (PrizeLocation loc in allLocations)
            {
                foreach (NpcLocationData npc in npcs)
                {
                    if (npc.eventId == loc.eventNum)
                    {
                        filteredLocations.Remove(loc);
                    }
                }
            }

            // add santa prize locations
            for (int i = 0; i < 7; i++)
            {
                PrizeLocation santaLoc = new PrizeLocation("santa gift " + i, -1, -1, GiftDeliveryIntroEvent.SANTA_DIALOGUE_EVENTS[i * 2 + 1], 0, new string[] { }, new string[] { "from Santa" }, new string[] { }, 1.0 - i * 0.1);
                filteredLocations.Add(santaLoc);
            }

            // dependency is .. dependency of the santa prizes before it, + dependency of the gift's location, + any dependency of getting to the location itself
            // some of this we don't know and have to fill in below post-process
            return filteredLocations;
        }

        public static void makeGiftModeEvents(RandoSettings settings, RandoContext context)
        {
            Random r = context.randomFunctional;
            Dictionary<int, byte> crystalOrbColorMap = ElementSwaps.getCrystalOrbElementMap(context);

            int flagValue = 0;
            int index = 0;
            // populate a copy with random selections for armor pieces
            Dictionary<string, string> possibleXmasGiftsProcessed = new Dictionary<string, string>(possibleXmasGifts);
            int numGifts = settings.getInt(OpenWorldSettings.PROPERTYNAME_NUM_XMAS_GIFTS);
            VanillaEventUtil.replaceEventData(OPENWORLD_GIFT_INJECTION_PATTERN.ToList(), context.replacementEvents[0x106], VanillaEventUtil.getBytes(possibleXmasGiftsProcessed[context.workingData.get(OpenWorldLocationPrizeMatcher.GIFT_SELECTION_PREFIX + "0")]));
            for (int i = 0; i < numGifts; i++)
            {
                int npcId = context.workingData.getInt(GiftDeliveryIntroEvent.GIFT_DELIVERY_INDEX_PREFIX + i);
                NpcLocationData npc = GiftDeliveryIntroEvent.DELIVERY_LOCATIONS[npcId];

                // this one should try to jump to one of the two below, then fall through and be like oh thanks for the gift
                int eventId = npc.eventId;

                // for each we need two? events?  maybe three
                // before/event/after
                int beforeEventId = npc.newEventBeforeGift;
                int giftEventId = npc.newEventGiveGift;

                // main talk events:
                // if 6F is < the desired value
                //    run beforeEventId
                // if 6F is = the desired value
                //    run giftEventId
                // say thanks
                EventScript npc_mainEvent = new EventScript();
                context.replacementEvents[eventId] = npc_mainEvent;
                if (flagValue > 0)
                {
                    npc_mainEvent.Logic(EventFlags.OPEN_WORLD_CHRISTMAS_PROGRESS, 0x0, (byte)(flagValue - 1), EventScript.GetJumpCmd(beforeEventId));
                }
                npc_mainEvent.Logic(EventFlags.OPEN_WORLD_CHRISTMAS_PROGRESS, (byte)flagValue, (byte)flagValue, EventScript.GetJumpCmd(giftEventId));

                npc_mainEvent.AddDialogueBox("Thanks for the thing");
                npc_mainEvent.End();


                // giftEvent:
                // (for event driven gifts)
                // if you don't have it yet based on item state
                //   run beforeEventId
                // (for inventory gifts)
                // run the removal command
                // if flag 0F is 01
                //   run beforeEventId
                // 
                // (for event driven gifts)
                // remove the item
                //
                // increment 6F
                EventScript npc_giftEvent = new EventScript();
                context.replacementEvents[giftEventId] = npc_giftEvent;
                string thisGift = context.workingData.get(OpenWorldLocationPrizeMatcher.GIFT_SELECTION_PREFIX + i);
                string giftDescription = possibleXmasGiftsProcessed[thisGift];

                if (thisGift.Contains("spells"))
                {
                    // crystalOrbColorMap (map num->byte) + elementByteToName
                    string eleName = thisGift.Split(new char[] { ' ' })[0].ToLower();
                    // set trigger orbs opened if applicable
                    setOrbsOpen(eleName, npc_giftEvent, crystalOrbColorMap);
                    switch (eleName)
                    {
                        // note these have to set the spells gone for every character since role rando is a thing
                        case "undine":
                            npc_giftEvent.Logic(EventFlags.ELEMENT_UNDINE_FLAG, 0x0, 0x0, EventScript.GetJumpCmd(beforeEventId));
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x01); // boy
                            npc_giftEvent.Add(0xC9); // undine
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x02); // girl
                            npc_giftEvent.Add(0xC9); // undine
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x03); // sprite
                            npc_giftEvent.Add(0xC9); // undine
                            npc_giftEvent.Add(0x00); // no spells
                            break;
                        case "gnome":
                            npc_giftEvent.Logic(EventFlags.ELEMENT_GNOME_FLAG, 0x0, 0x0, EventScript.GetJumpCmd(beforeEventId));
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x01); // boy
                            npc_giftEvent.Add(0xC8); // gnome
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x02); // girl
                            npc_giftEvent.Add(0xC8); // gnome
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x03); // sprite
                            npc_giftEvent.Add(0xC8); // gnome
                            npc_giftEvent.Add(0x00); // no spells
                            break;
                        case "sylphid":
                            npc_giftEvent.Logic(EventFlags.ELEMENT_SYLPHID_FLAG, 0x0, 0x0, EventScript.GetJumpCmd(beforeEventId));
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x01); // boy
                            npc_giftEvent.Add(0xCB); // sylphid
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x02); // girl
                            npc_giftEvent.Add(0xCB); // sylphid
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x03); // sprite
                            npc_giftEvent.Add(0xCB); // sylphid
                            npc_giftEvent.Add(0x00); // no spells
                            break;
                        case "salamando":
                            npc_giftEvent.Logic(EventFlags.ELEMENT_SALAMANDO_FLAG, 0x0, 0x0, EventScript.GetJumpCmd(beforeEventId));
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x01); // boy
                            npc_giftEvent.Add(0xCA); // sala
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x02); // girl
                            npc_giftEvent.Add(0xCA); // sala
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x03); // sprite
                            npc_giftEvent.Add(0xCA); // sala
                            npc_giftEvent.Add(0x00); // no spells
                            break;
                        case "lumina":
                            npc_giftEvent.Logic(EventFlags.ELEMENT_LUMINA_FLAG, 0x0, 0x0, EventScript.GetJumpCmd(beforeEventId));
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x01); // boy
                            npc_giftEvent.Add(0xCF); // lum
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x02); // girl
                            npc_giftEvent.Add(0xCF); // lum
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x03); // sprite
                            npc_giftEvent.Add(0xCF); // lum
                            npc_giftEvent.Add(0x00); // no spells
                            break;
                        case "shade":
                            npc_giftEvent.Logic(EventFlags.ELEMENT_SHADE_FLAG, 0x0, 0x0, EventScript.GetJumpCmd(beforeEventId));
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x01); // boy
                            npc_giftEvent.Add(0xCE); // shade
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x02); // girl
                            npc_giftEvent.Add(0xCE); // shade
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x03); // sprite
                            npc_giftEvent.Add(0xCE); // shade
                            npc_giftEvent.Add(0x00); // no spells
                            break;
                        case "luna":
                            npc_giftEvent.Logic(EventFlags.ELEMENT_LUNA_FLAG, 0x0, 0x0, EventScript.GetJumpCmd(beforeEventId));
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x01); // boy
                            npc_giftEvent.Add(0xCC); // luna
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x02); // girl
                            npc_giftEvent.Add(0xCC); // luna
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x03); // sprite
                            npc_giftEvent.Add(0xCC); // luna
                            npc_giftEvent.Add(0x00); // no spells
                            break;
                        case "dryad":
                            npc_giftEvent.Logic(EventFlags.ELEMENT_DRYAD_FLAG, 0x0, 0x0, EventScript.GetJumpCmd(beforeEventId));
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x01); // boy
                            npc_giftEvent.Add(0xCD); // dryad
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x02); // girl
                            npc_giftEvent.Add(0xCD); // dryad
                            npc_giftEvent.Add(0x00); // no spells
                            npc_giftEvent.Add(EventCommandEnum.SET_CHARACTER_ATTRIBUTE.Value);
                            npc_giftEvent.Add(0x03); // sprite
                            npc_giftEvent.Add(0xCD); // dryad
                            npc_giftEvent.Add(0x00); // no spells
                            break;
                    }
                }
                else if (thisGift.Contains("gold"))
                {
                    int goldAmount = Int32.Parse(thisGift.Split(new char[] { ' ' })[0]);
                    // clear removal flag
                    npc_giftEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
                    // remove gold
                    npc_giftEvent.Add(EventCommandEnum.REMOVE_GOLD.Value);
                    npc_giftEvent.Add((byte)goldAmount);
                    npc_giftEvent.Add((byte)(goldAmount >> 8));
                    npc_giftEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(beforeEventId)); // 0F set if removal failed
                }
                else if (thisGift == "midge mallet")
                {
                    npc_giftEvent.Logic(EventFlags.MIDGE_MALLET_FLAG, 0x0, 0x0, EventScript.GetJumpCmd(beforeEventId));
                    npc_giftEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
                    npc_giftEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    npc_giftEvent.Add(0xFF); // remove (see RemoveItemEventCommand hack)
                    npc_giftEvent.Add(0x49); // remove midge mallet
                    npc_giftEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(beforeEventId)); // 0F set if removal failed
                }
                else if (thisGift == "moogle belt")
                {
                    npc_giftEvent.Logic(EventFlags.OPENWORLD_MOOGLE_BELT_FLAG, 0x0, 0x0, EventScript.GetJumpCmd(beforeEventId));
                    npc_giftEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
                    npc_giftEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    npc_giftEvent.Add(0xFF); // remove (see RemoveItemEventCommand hack)
                    npc_giftEvent.Add(0x48); // remove moogle belt
                    npc_giftEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(beforeEventId)); // 0F set if removal failed
                }
                else if (thisGift == "hat")
                {
                    npc_giftEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
                    npc_giftEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    npc_giftEvent.Add(0xFF);
                    npc_giftEvent.Add(0xFF); // remove a hat (FE is armor, FD is accessory - see RemoveItemEventCommand)
                    npc_giftEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(beforeEventId)); // 0F set if removal failed
                }
                else if (thisGift == "hat#")
                {
                    // random hat number; pull name from somewhere
                    Dictionary<int, string> hatNames = new Dictionary<int, string>();
                    // 1-20
                    hatNames[1] = "A new [bandanna]";
                    hatNames[2] = "A new [hair ribbon]";
                    hatNames[3] = "A new [rabite cap]";
                    hatNames[4] = "A new [head gear]";
                    hatNames[5] = "A new [quill cap]";
                    hatNames[6] = "A new [steel cap]";
                    hatNames[7] = "A new [golden tiara]";
                    hatNames[8] = "A new [raccoon cap]";
                    hatNames[9] = "A new [quilted hood]";
                    hatNames[10] = "A new [tiger cap]";
                    hatNames[11] = "A new [circlet]";
                    hatNames[12] = "A new [ruby armet]";
                    hatNames[13] = "A new [unicorn helm]";
                    hatNames[14] = "A new [dragon helm]";
                    hatNames[15] = "A new [duck helm]";
                    hatNames[16] = "A new [needle helm]";
                    hatNames[17] = "A new [cockatrice cap]";
                    hatNames[18] = "A new [amulet helm]";
                    hatNames[19] = "A new [griffin helm]";
                    hatNames[20] = "A new [faerie crown]";
                    int hatId = 1 + (r.Next() % hatNames.Count);
                    // fill in possibleXmasGifts["hat#"] with what we picked
                    possibleXmasGiftsProcessed["hat#"] = hatNames[hatId];
                    giftDescription = hatNames[hatId];
                    npc_giftEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
                    npc_giftEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    npc_giftEvent.Add(0xFF);
                    npc_giftEvent.Add((byte)hatId); // remove hat
                    npc_giftEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(beforeEventId)); // 0F set if removal failed
                }
                else if (thisGift == "armor")
                {
                    npc_giftEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
                    npc_giftEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    npc_giftEvent.Add(0xFF);
                    npc_giftEvent.Add(0xFE); // remove a hat (FE is armor, FD is accessory - see RemoveItemEventCommand)
                    npc_giftEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(beforeEventId)); // 0F set if removal failed
                }
                else if (thisGift == "armor#")
                {
                    // random armor number; pull name from somewhere
                    Dictionary<int, string> armorNames = new Dictionary<int, string>();
                    // 22-41
                    armorNames[22] = "A new [overalls]";
                    armorNames[23] = "A new [kung fu suit]";
                    armorNames[24] = "A new [midge robe]";
                    armorNames[25] = "A new [chain vest]";
                    armorNames[26] = "A new [spiky suit]";
                    armorNames[27] = "A new [kung fu dress]";
                    armorNames[28] = "A new [fancy overalls]";
                    armorNames[29] = "A new [chest guard]";
                    armorNames[30] = "A new [golden vest]";
                    armorNames[31] = "A new [ruby vest]";
                    armorNames[32] = "A new [tiger suit]";
                    armorNames[33] = "A new [tiger bikini]";
                    armorNames[34] = "A new [magical armor]";
                    armorNames[35] = "A new [tortoise mail]";
                    armorNames[36] = "A new [flower suit]";
                    armorNames[37] = "A new [battle suit]";
                    armorNames[38] = "A new [vestguard]";
                    armorNames[39] = "A new [vampire cape]";
                    armorNames[40] = "A new [power suit]";
                    armorNames[41] = "A new [faerie cloak]";
                    int armorId = 22 + (r.Next() % armorNames.Count);
                    // fill in possibleXmasGifts["armor#"] with what we picked
                    possibleXmasGiftsProcessed["armor#"] = armorNames[armorId];
                    giftDescription = armorNames[armorId];
                    npc_giftEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
                    npc_giftEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    npc_giftEvent.Add(0xFF);
                    npc_giftEvent.Add((byte)armorId); // remove armor
                    npc_giftEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(beforeEventId)); // 0F set if removal failed
                }
                else if (thisGift == "accessory")
                {
                    npc_giftEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
                    npc_giftEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    npc_giftEvent.Add(0xFF);
                    npc_giftEvent.Add(0xFD); // remove a hat (FE is armor, FD is accessory - see RemoveItemEventCommand)
                    npc_giftEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(beforeEventId)); // 0F set if removal failed
                }
                else if (thisGift == "accessory#")
                {
                    // random accessory number; pull name from somewhere
                    Dictionary<int, string> accessoryNames = new Dictionary<int, string>();
                    // 43-62
                    accessoryNames[43] = "A new [faerie's ring]";
                    accessoryNames[44] = "A new [elbow pad]";
                    accessoryNames[45] = "A new [power wrist]";
                    accessoryNames[46] = "A new [cobra bracelet]";
                    accessoryNames[47] = "A new [wolf's band]";
                    accessoryNames[48] = "A new [silver band]";
                    accessoryNames[49] = "A new [golem ring]";
                    accessoryNames[50] = "A new [frosty ring]";
                    accessoryNames[51] = "A new [ivy amulet]";
                    accessoryNames[52] = "A new [gold bracelet]";
                    accessoryNames[53] = "A new [shield ring]";
                    accessoryNames[54] = "A new [lazuri ring]";
                    accessoryNames[55] = "A new [guardian ring]";
                    accessoryNames[56] = "A new [gauntlet]";
                    accessoryNames[57] = "A new [ninja gloves]";
                    accessoryNames[58] = "A new [dragon ring]";
                    accessoryNames[59] = "A new [watcher ring]";
                    accessoryNames[60] = "A new [imp's ring]";
                    accessoryNames[61] = "A new [amulet ring]";
                    accessoryNames[62] = "A new [wristband]";
                    int accessoryId = 43 + (r.Next() % accessoryNames.Count);
                    // fill in possibleXmasGifts["accessory#"] with what we picked
                    possibleXmasGiftsProcessed["accessory#"] = accessoryNames[accessoryId];
                    giftDescription = accessoryNames[accessoryId];
                    npc_giftEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
                    npc_giftEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    npc_giftEvent.Add(0xFF);
                    npc_giftEvent.Add((byte)accessoryId); // remove accessory
                    npc_giftEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(beforeEventId)); // 0F set if removal failed
                }
                else if (thisGift == "candy")
                {
                    // BuyMultipleItems lists the ids here
                    npc_giftEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
                    npc_giftEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    npc_giftEvent.Add(0xFF);
                    npc_giftEvent.Add(0x40); // remove a candy
                    npc_giftEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(beforeEventId)); // 0F set if removal failed
                }
                else if (thisGift == "chocolate")
                {
                    npc_giftEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
                    npc_giftEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    npc_giftEvent.Add(0xFF);
                    npc_giftEvent.Add(0x41); // remove a chocolate
                    npc_giftEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(beforeEventId)); // 0F set if removal failed
                }
                else if (thisGift == "royaljam")
                {
                    npc_giftEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
                    npc_giftEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    npc_giftEvent.Add(0xFF);
                    npc_giftEvent.Add(0x42); // remove a royal jam
                    npc_giftEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(beforeEventId)); // 0F set if removal failed
                }
                else if (thisGift == "walnut")
                {
                    npc_giftEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
                    npc_giftEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    npc_giftEvent.Add(0xFF);
                    npc_giftEvent.Add(0x43); // remove a walnut
                    npc_giftEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(beforeEventId)); // 0F set if removal failed
                }
                else if (thisGift == "barrel")
                {
                    npc_giftEvent.SetFlag(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0);
                    npc_giftEvent.Add(EventCommandEnum.ADD_INVENTORY_ITEM.Value);
                    npc_giftEvent.Add(0xFF);
                    npc_giftEvent.Add(0x4A); // remove a barrel
                    npc_giftEvent.Logic(EventFlags.NOT_ENOUGH_GOLD_FLAG, 0x1, 0x1, EventScript.GetJumpCmd(beforeEventId)); // 0F set if removal failed
                }

                // giftDescription has now been populated, so inject it into the events
                // even values = trying to do delivery
                // odd values = going back
                VanillaEventUtil.replaceEventData(OPENWORLD_GIFT_INJECTION_PATTERN.ToList(), context.replacementEvents[GiftDeliveryIntroEvent.SANTA_DIALOGUE_EVENTS[i * 2]], VanillaEventUtil.getBytes(giftDescription));

                string[] giftDialogues = new string[] {
                        "Oh sweet, is that " + giftDescription + "? Thanks!",
                        "Oh, this is just what I wanted! " + giftDescription + "!",
                        "You brought me " + giftDescription + "? I've been wanting this!",
                    };
                // successful gift
                npc_giftEvent.AddDialogueBox(VanillaEventUtil.wordWrapText(giftDialogues[r.Next() % giftDialogues.Length]));
                npc_giftEvent.IncrFlag(EventFlags.OPEN_WORLD_CHRISTMAS_PROGRESS);
                npc_giftEvent.End();

                // beforeEvent:
                // just dialogue, plz give the thing
                EventScript npc_beforeEvent = new EventScript();
                context.replacementEvents[beforeEventId] = npc_beforeEvent;
                string[] beforeDialogues = new string[] {
                        "Have you got a gift for me?",
                        "I didn't get a gift this year!",
                        "I think Santa forgot about me.",
                        "Hmm, I didn't want anything for Christmas anyway.",
                    };
                // unsuccessful gift
                npc_beforeEvent.AddDialogueBox(VanillaEventUtil.wordWrapText(beforeDialogues[r.Next() % beforeDialogues.Length]));
                npc_beforeEvent.End();

                Logging.log("Xmas gift -- " + npc.description + ": " + thisGift + " (" + possibleXmasGiftsProcessed[thisGift] + ")", "spoiler");
                index++;
                flagValue += 2;
            }
        }

        private static void setOrbsOpen(string elementName, EventScript eventData, Dictionary<int, byte> crystalOrbColorMap)
        {
            // at sunken continent (set to 01):
            // e8 gnome
            // e9 undine
            // ea sylphid
            // eb sala
            // ec lumina
            // ed shade
            // ee luna

            // at other spots:
            // 24->04 matango cave (gnome vanilla)
            // 18->02 earth palace (undine vanilla)
            // 2b->04 fire palace A-also B? (sala vanilla)
            // 2c->01 fire palace A (sala vanilla - entrance)
            // 3b->02 luna palace (lumina vanilla)
            // 2b 00->01 = bridge room (sala)
            // 2b 03->04 = flames room (undine) .. this is a minor issue, but oh well.. just set it to 4 either way
            // 2c 00->01 = intro room (sala)
            Dictionary<int, byte> flagNum = new Dictionary<int, byte>();
            Dictionary<int, byte> valueNum = new Dictionary<int, byte>();
            flagNum[ElementSwaps.ORBMAP_GRANDPALACE1] = EventFlags.GRANDPALACE_GNOME_ORB_FLAG;
            valueNum[ElementSwaps.ORBMAP_GRANDPALACE1] = 0x01;
            flagNum[ElementSwaps.ORBMAP_GRANDPALACE2] = EventFlags.GRANDPALACE_UNDINE_ORB_FLAG;
            valueNum[ElementSwaps.ORBMAP_GRANDPALACE2] = 0x01;
            flagNum[ElementSwaps.ORBMAP_GRANDPALACE3] = EventFlags.GRANDPALACE_SYLPHID_ORB_FLAG;
            valueNum[ElementSwaps.ORBMAP_GRANDPALACE3] = 0x01;
            flagNum[ElementSwaps.ORBMAP_GRANDPALACE4] = EventFlags.GRANDPALACE_SALAMANDO_ORB_FLAG;
            valueNum[ElementSwaps.ORBMAP_GRANDPALACE4] = 0x01;
            flagNum[ElementSwaps.ORBMAP_GRANDPALACE5] = EventFlags.GRANDPALACE_LUMINA_ORB_FLAG;
            valueNum[ElementSwaps.ORBMAP_GRANDPALACE5] = 0x01;
            flagNum[ElementSwaps.ORBMAP_GRANDPALACE6] = EventFlags.GRANDPALACE_SHADE_ORB_FLAG;
            valueNum[ElementSwaps.ORBMAP_GRANDPALACE6] = 0x01;
            flagNum[ElementSwaps.ORBMAP_GRANDPALACE7] = EventFlags.GRANDPALACE_LUNA_ORB_FLAG;
            valueNum[ElementSwaps.ORBMAP_GRANDPALACE7] = 0x01;
            flagNum[ElementSwaps.ORBMAP_MATANGO] = EventFlags.MATANGO_PROGRESS_FLAG;
            valueNum[ElementSwaps.ORBMAP_MATANGO] = 0x04;
            flagNum[ElementSwaps.ORBMAP_EARTHPALACE] = EventFlags.EARTHPALACE_FLAG;
            valueNum[ElementSwaps.ORBMAP_EARTHPALACE] = 0x02;
            flagNum[ElementSwaps.ORBMAP_FIREPALACE3] = EventFlags.FIRE_PALACE_SWITCHES_FLAG;
            valueNum[ElementSwaps.ORBMAP_FIREPALACE3] = 0x04;
            flagNum[ElementSwaps.ORBMAP_FIREPALACE2] = EventFlags.FIRE_PALACE_SWITCHES_FLAG;
            valueNum[ElementSwaps.ORBMAP_FIREPALACE2] = 0x04;
            flagNum[ElementSwaps.ORBMAP_FIREPALACE1] = EventFlags.FIRE_PALACE_COMPLETION_FLAG;
            valueNum[ElementSwaps.ORBMAP_FIREPALACE1] = 0x01;
            flagNum[ElementSwaps.ORBMAP_LUNAPALACE] = EventFlags.LUNA_PALACE_FLAG;
            valueNum[ElementSwaps.ORBMAP_LUNAPALACE] = 0x02;
            flagNum[ElementSwaps.ORBMAP_UPPERLAND] = EventFlags.UPPERLAND_MOOGLES_FLAG;
            valueNum[ElementSwaps.ORBMAP_UPPERLAND] = 0x04;
            foreach (int mapNum in crystalOrbColorMap.Keys)
            {
                string orbElem = SomVanillaValues.elementOrbByteToName(crystalOrbColorMap[mapNum], false);
                if (orbElem == elementName && flagNum.ContainsKey(mapNum))
                {
                    eventData.SetFlag(flagNum[mapNum], valueNum[mapNum]);
                }
            }
        }
    }
}
