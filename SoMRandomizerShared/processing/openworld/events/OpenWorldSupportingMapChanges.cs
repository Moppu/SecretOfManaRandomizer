using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using static SoMRandomizer.processing.hacks.openworld.XmasRandoData;
using static SoMRandomizer.processing.common.SomVanillaValues;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Map event/object visibility/etc changes necessary to make open world work.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OpenWorldSupportingMapChanges : RandoProcessor
    {
        protected override string getName()
        {
            return "Changes to map object visibility etc. for open world";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            bool flammieDrumInLogic = settings.getBool(OpenWorldSettings.PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC);
            string goal = context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME);

            if(goal == OpenWorldGoalProcessor.GOAL_GIFTMODE || goal == OpenWorldGoalProcessor.GOAL_REINDEER)
            {
                // repurpose door 82 (falling down waterfall) to put us in santa house
                // map 333 15,13
                // 83xxx
                // note: don't use door 00 because it makes that log
                int waterfallDoorNum = 0x82;
                int santaHouseMapNum = MAPNUM_SANTAHOUSE_INTERIOR;
                outRom[0x83000 + waterfallDoorNum * 4] = (byte)(santaHouseMapNum);
                outRom[0x83000 + waterfallDoorNum * 4 + 1] = (byte)(santaHouseMapNum >> 8);
                outRom[0x83000 + waterfallDoorNum * 4 + 1] |= (byte)(17 << 1); // x
                outRom[0x83000 + waterfallDoorNum * 4 + 2] = (byte)(11 << 1); // y
                outRom[0x83000 + waterfallDoorNum * 4 + 2] |= 0x01; // collision layer, i think
                outRom[0x83000 + waterfallDoorNum * 4 + 3] = 0;

                // santa always visible and in a new spot
                setObjectAlwaysVisible(outRom, santaHouseMapNum, 0);
                setObjectData(outRom, santaHouseMapNum, 0, 2, 0x80 + 17); // x pos
                setObjectData(outRom, santaHouseMapNum, 0, 3, 0x80 + 10); // y pos
            }
            else if (flammieDrumInLogic)
            {
                // repurpose door 82 (falling down waterfall) to put us in potos elder house
                // don't do this if we're doing christmas modes - still start at ice country.  logic accounts for this.
                int waterfallDoorNum = 0x82;
                int potosHouseMapNum = MAPNUM_POTOS_INTERIOR_B;
                outRom[0x83000 + waterfallDoorNum * 4] = (byte)(potosHouseMapNum);
                outRom[0x83000 + waterfallDoorNum * 4 + 1] = (byte)(potosHouseMapNum >> 8);
                outRom[0x83000 + waterfallDoorNum * 4 + 1] |= (byte)(56 << 1);
                outRom[0x83000 + waterfallDoorNum * 4 + 2] = (byte)(13 << 1);
                outRom[0x83000 + waterfallDoorNum * 4 + 2] |= 0x01;
                outRom[0x83000 + waterfallDoorNum * 4 + 3] = 0;

                // change sword event and visibility for flammie in logic mode
                setObjectAlwaysVisible(outRom, MAPNUM_RUSTYSWORD, 0);
                setObjectEvent(outRom, MAPNUM_RUSTYSWORD, 0, 0x532);
            }

            // potos guard - repurpose as mantis ant fight npc. adjust his position to near the hole
            setObjectXY(outRom, MAPNUM_POTOS, 9, 0x80 + 28, 0x80 + 29);

            // remove dyluck and his buddies south of water palace
            for (int i = 0; i < 5; i++)
            {
                setObjectNeverVisible(outRom, MAPNUM_WATERPALACE_WITCHCASTLE_CROSSROADS, i);
            }

            for (int i = 0; i < 8; i++)
            {
                if(i > 0)
                {
                    // remove the iffish directly outside of water palace
                    setObjectNeverVisible(outRom, MAPNUM_WATERPALACE_EXTERIOR, i);
                }
                else
                {
                    // but keep jema and use him as the jabberwocky entry
                    setObjectVisibility(outRom, MAPNUM_WATERPALACE_EXTERIOR, i, EventFlags.OPENWORLD_JABBERWOCKY_FLAG, 0, 0);
                }
            }

            // water palace interior - hide both seeds. i do not know why there are two
            setObjectNeverVisible(outRom, MAPNUM_WATERPALACE_LUKA_DIALOGUE, 0);
            setObjectNeverVisible(outRom, MAPNUM_WATERPALACE_LUKA_DIALOGUE, 1);
            // move luka down 4 tiles
            setObjectData(outRom, MAPNUM_WATERPALACE_LUKA_DIALOGUE, 2, 3, (byte)(getObjectData(outRom, 139, 2, 3) + 4));
            // hide jema, multiple copies of him
            setObjectNeverVisible(outRom, MAPNUM_WATERPALACE_LUKA_DIALOGUE, 4);
            setObjectNeverVisible(outRom, MAPNUM_WATERPALACE_LUKA_DIALOGUE, 5);
            setObjectNeverVisible(outRom, MAPNUM_WATERPALACE_LUKA_DIALOGUE, 6);

            // water palace jabberwocky map - hide the triggers to make the bridges
            int jabberwockyMapNum = MAPNUM_JABBERWOCKY_ARENA;
            int jabberwockyMapTriggerOffset = VanillaMapUtil.getTriggerOffset(outRom, jabberwockyMapNum);
            // w/ walkon trigger present
            jabberwockyMapTriggerOffset += 2;
            // wipe out triggers [2] and [3] 
            outRom[jabberwockyMapTriggerOffset + 2 * 2 + 0] = 0x00;
            outRom[jabberwockyMapTriggerOffset + 2 * 2 + 1] = 0x00;
            outRom[jabberwockyMapTriggerOffset + 2 * 3 + 0] = 0x00;
            outRom[jabberwockyMapTriggerOffset + 2 * 3 + 1] = 0x00;

            // remove the "special event" in the pandora castle girl room that triggers her to run in when you step on a certain tile
            int pandoraGirlMapNum = MAPNUM_PANDORA_CASTLE_INTERIOR_B;
            int pandoraGirlMapTriggerOffset = VanillaMapUtil.getTriggerOffset(outRom, pandoraGirlMapNum);
            // set it to event 0
            outRom[pandoraGirlMapTriggerOffset] = 0x00;
            outRom[pandoraGirlMapTriggerOffset + 1] = 0x00;

            // repurpose the 0x188 special event as the girl's gift event
            setObjectAlwaysVisible(outRom, pandoraGirlMapNum, 0);
            setObjectData(outRom, pandoraGirlMapNum, 0, 3, (byte)(getObjectData(outRom, pandoraGirlMapNum, 0, 3) + 3)); // girl y += 2
            setObjectData(outRom, pandoraGirlMapNum, 0, 6, 0x88); // event lsb
            setObjectData(outRom, pandoraGirlMapNum, 0, 7, 0x41); // event msb (unsure what the 0x40 is for)

            // remove elinee from map 306 because she can despawn if the chairs become summoner enemies;
            // we modify event 0x1e7 to account for this
            setObjectNeverVisible(outRom, MAPNUM_WITCHCASTLE_NESOBERI, 0);

            // in dwarf town, move jema, and make him always visible. we use him for the tropicallo fight entry
            setObjectAlwaysVisible(outRom, MAPNUM_DWARFTOWN, 0);
            setObjectXY(outRom, MAPNUM_DWARFTOWN, 0, 0x80 + 29, 0x80 + 33);

            // make tropicallo always visible; normally he uses x14 which we change
            // you can't go back to his arena so there's no reason to hide him
            setObjectAlwaysVisible(outRom, MAPNUM_TROPICALLO_ARENA, 0);

            // door 0x52 out of tropicallo originally returns you to where the kilroy hole opens up
            // this adjusts it to stick you somewhere better, since that hole is always open now
            outRom[0x83000 + 0x52 * 4 + 1] = (byte)(((29 << 1)) | (outRom[0x83000 + 0x52 * 4 + 1] & 0x01));
            outRom[0x83000 + 0x52 * 4 + 2] = (byte)(((40 << 1)) | (outRom[0x83000 + 0x52 * 4 + 2] & 0x01));

            // wind palace - hide seeds
            setObjectNeverVisible(outRom, MAPNUM_WINDSEED, 0);
            setObjectNeverVisible(outRom, MAPNUM_WINDSEED, 1);
            // change sylphid into a rabite that is made indestructible by a separate hack (InvincibleRabite)
            setObjectAlwaysVisible(outRom, MAPNUM_WINDSEED, 2);
            setObjectData(outRom, MAPNUM_WINDSEED, 2, 5, 0); // field 5 is object species; change to 0 (rabite)
            setObjectData(outRom, MAPNUM_WINDSEED, 2, 2, (byte)(getObjectData(outRom, 273, 2, 2) + 2)); // sylphid/rabite x += 2
            setObjectData(outRom, MAPNUM_WINDSEED, 2, 3, (byte)(getObjectData(outRom, 273, 2, 3) - 2)); // sylphid/rabite y -= 2
            // change a couple flags to make the sylphid palace elder easier to interact with
            setObjectData(outRom, MAPNUM_WINDSEED, 3, 7, (byte)(getObjectData(outRom, 273, 3, 7) & 0x0f));
            setObjectData(outRom, MAPNUM_WINDSEED, 3, 7, (byte)(getObjectData(outRom, 273, 3, 7) | 0x40));

            // fire palace - hide seeds
            setObjectNeverVisible(outRom, MAPNUM_FIRESEED, 0);
            // actually change this one into salamando so it can give a prize (event 0x589)
            setObjectAlwaysVisible(outRom, MAPNUM_FIRESEED, 1);
            setObjectData(outRom, MAPNUM_FIRESEED, 1, 3, (byte)(getObjectData(outRom, 349, 1, 3) + 3)); // salamando y += 3
            setObjectData(outRom, MAPNUM_FIRESEED, 1, 5, 0x92); // field 5 is object species; change to 0x92 (salamando)

            // lumina palace - hide seeds
            setObjectNeverVisible(outRom, MAPNUM_LIGHTSEED, 0);
            setObjectNeverVisible(outRom, MAPNUM_LIGHTSEED, 1);

            // shade palace - hide seeds
            setObjectNeverVisible(outRom, MAPNUM_DARKSEED, 0);
            setObjectNeverVisible(outRom, MAPNUM_DARKSEED, 1);

            // luna palace - hide seeds
            setObjectNeverVisible(outRom, MAPNUM_MOONSEED, 0);
            setObjectNeverVisible(outRom, MAPNUM_MOONSEED, 1);

            // dryad palace - hide seeds
            setObjectNeverVisible(outRom, MAPNUM_DRYADSEED, 0);
            setObjectNeverVisible(outRom, MAPNUM_DRYADSEED, 1);
            // use object 2 as the dryad gift npc; run event 0x4b6
            setObjectAlwaysVisible(outRom, MAPNUM_DRYADSEED, 2);
            setObjectData(outRom, MAPNUM_DRYADSEED, 2, 6, 0xB6);
            setObjectData(outRom, MAPNUM_DRYADSEED, 2, 7, 0x44);

            // neko visibilities
            setObjectAlwaysVisible(outRom, MAPNUM_GAIASNAVEL_INTERIOR_A_NEKO, 0);
            setObjectAlwaysVisible(outRom, MAPNUM_WITCHFOREST_A, 0);
            setObjectAlwaysVisible(outRom, MAPNUM_WITCHCASTLE_JAIL, 4);
            setObjectAlwaysVisible(outRom, MAPNUM_UPPERLAND_SPRING, 0);
            setObjectAlwaysVisible(outRom, MAPNUM_UPPERLAND_MOOGLE_VILLAGE, 1);
            setObjectAlwaysVisible(outRom, MAPNUM_JEHKCAVE_EXTERIOR, 0);

            // adjust red dragon position to better fit other bosses
            setObjectXY(outRom, MAPNUM_REDDRAGON_ARENA, 0, 0x80 + 18, 10);

            // triple tonpole map - make only one of them visible, with flag x25
            setObjectVisibility(outRom, MAPNUM_TRIPLETONPOLE_ARENA, 0, 0x25, 0x00, 0x00);
            setObjectNeverVisible(outRom, MAPNUM_TRIPLETONPOLE_ARENA, 1);
            setObjectNeverVisible(outRom, MAPNUM_TRIPLETONPOLE_ARENA, 2);

            // iffish outside undine cave always visible
            setObjectAlwaysVisible(outRom, MAPNUM_UNDINECAVE_EXTERIOR, 0);
            setObjectAlwaysVisible(outRom, MAPNUM_UNDINECAVE_EXTERIOR, 1);
            setObjectAlwaysVisible(outRom, MAPNUM_UNDINECAVE_EXTERIOR, 2);

            // remove scorpion guys on salamando town map
            setObjectNeverVisible(outRom, MAPNUM_ICECOUNTRY_PARADISETOWN_COLD, 2);
            setObjectNeverVisible(outRom, MAPNUM_ICECOUNTRY_PARADISETOWN_COLD, 3);
            // copy object 0 to 1; use 0 for unopened stove, 1 for opened
            for(int i=0; i < 8; i++)
            {
                setObjectData(outRom, MAPNUM_ICECOUNTRY_PARADISETOWN_COLD, 1, i, getObjectData(outRom, MAPNUM_ICECOUNTRY_PARADISETOWN_COLD, 0, i));
            }
            setObjectVisibility(outRom, MAPNUM_ICECOUNTRY_PARADISETOWN_COLD, 0, EventFlags.SALAMANDO_STOVE_FLAG, 0x0, 0x4);
            setObjectVisibility(outRom, MAPNUM_ICECOUNTRY_PARADISETOWN_COLD, 1, EventFlags.SALAMANDO_STOVE_FLAG, 0x4, 0xf);
            setObjectData(outRom, MAPNUM_ICECOUNTRY_PARADISETOWN_COLD, 0, 6, 0x8C); // run event 0x38c; unsure what the 0x20 bit is for here
            setObjectData(outRom, MAPNUM_ICECOUNTRY_PARADISETOWN_COLD, 0, 7, 0x23);
            setObjectData(outRom, MAPNUM_ICECOUNTRY_PARADISETOWN_COLD, 1, 6, 0x00); // empty stove has no event
            setObjectData(outRom, MAPNUM_ICECOUNTRY_PARADISETOWN_COLD, 1, 7, 0x00);
            setObjectData(outRom, MAPNUM_ICECOUNTRY_PARADISETOWN_COLD, 0, 4, (byte)((getObjectData(outRom, 197, 0, 4) & 0x3F) | 0x40)); // direction on the stove indicates full/empty sprite; face down for full one
            setObjectData(outRom, MAPNUM_ICECOUNTRY_PARADISETOWN_COLD, 1, 4, (byte)((getObjectData(outRom, 197, 1, 4) & 0x3F) | 0xC0)); // direction on the stove indicates full/empty sprite; face right for empty one

            // make flammie not land on the ice country vacation town map, otherwise we can skip boreal face
            // x67780, 3 bytes apiece - mapnum,x,y;16 x 16 of them, this is [24]
            outRom[0x67780 + 24 * 3] = 77;
            outRom[0x67780 + 24 * 3 + 1] = 16;
            outRom[0x67780 + 24 * 3 + 2] = 22;

            // adjust door destination coming out of frost gigas fight so we can see santa
            outRom[0x83B2E] = 0x1A;

            // move the mountain neko down nearer to the entrance
            setObjectXY(outRom, MAPNUM_JEHKCAVE_EXTERIOR, 0, 19, 44 + 0x80);

            // cannon travel fellas
            setObjectEvent(outRom, MAPNUM_POTOS_CANNON, 8, 0xD1); // potos cannon's event - always go to upper land
            setObjectAlwaysVisible(outRom, MAPNUM_UPPERLAND_NORTH_CANNON, 8); // matango cannon
            setObjectAlwaysVisible(outRom, MAPNUM_KAKKARA_CANNON, 8); // kakkara cannon
            setObjectAlwaysVisible(outRom, MAPNUM_ICECOUNTRY_CANNON_SOUTH, 8); // ice country cannon
            setObjectAlwaysVisible(outRom, MAPNUM_SOUTHTOWN_CANNON, 8); // south town cannon

            // changes to matango cannon travel, from two different locations
            outRom[0x67D33] = 0x97; // change target to outside so you can't get stuck with no axe
            outRom[0x67D3B] = 0x97; // change target to outside so you can't get stuck with no axe
            // it still plays the matango music but idc

            if (flammieDrumInLogic)
            {
                // move southtown password guy, who blocks you in normal open world
                // don't remove him, because gift mode can sometimes require him.
                setObjectXY(outRom, MAPNUM_SOUTHTOWN, 2, (byte)(getObjectX(outRom, MAPNUM_SOUTHTOWN, 2) - 1), getObjectY(outRom, MAPNUM_SOUTHTOWN, 2));
            }

            // enable flammie on a couple extra maps 
            int[] mapsToEnableFlammie = new int[]
            {
                MAPNUM_UPPERLAND_MOOGLE_VILLAGE, // moogle village
                MAPNUM_KARONFERRY_ENDPOINTS, // karon ferry
            };

            foreach (int mapToEnableFlammie in mapsToEnableFlammie)
            {
                int mapObjOffset = 0x80000 + outRom[0x87000 + mapToEnableFlammie * 2] + (outRom[0x87000 + mapToEnableFlammie * 2 + 1] << 8);
                outRom[mapObjOffset + 4] |= 0x80;
            }

            // 7e003e, 40 are our x/y position when on flammie
            // $01/E811 BF 80 7A C6 LDA $C67A80,x[$C6:7C0C] A:018C X:018C Y:2E50 P:envmxdIzc (for map c6 * 2)
            // copy map 96 data to map 32 - 0x67b40 -> 0x67ac0 .. sorta
            // use 0x3858 for good luna palace position.
            outRom[0x67ac0] = 0x38;
            outRom[0x67ac1] = 0x58;

            // move the kakkara town landing spot
            // 0x67780;
            // this is 0x52
            // 3 bytes each
            // 67853, 67854
            outRom[0x67853] = 0x36;
            outRom[0x67854] = 0x32;

            // don't allow landing at salamando map
            outRom[0x677ec] = 0x4D;
            outRom[0x677ed] = 16 * 2;
            outRom[0x677ee] = 21 * 2;
            // or witch's castle
            outRom[0x6795d] = 0x1B;
            outRom[0x6795e] = 10 * 2;
            outRom[0x6795f] = 23 * 2;

            if (flammieDrumInLogic)
            {
                // hide the hoppy thing south of potos since you can get stuck without a cutting weapon
                setObjectNeverVisible(outRom, MAPNUM_POTOS_SOUTH_FIELD, 3);
            }

            if (goal == OpenWorldGoalProcessor.GOAL_REINDEER)
            {
                // make npcs visible for reindeer thing if we removed them for lag reduction etc above
                for (int i = 0; i < 8; i++)
                {
                    // expects ReindeerIntroEvent to have run before this, to determine these ids
                    ReplacementObject thisReplacement = ReindeerIntroEvent.getByGlobalId(context.workingData.getInt(ReindeerIntroEvent.SELECTED_LOCATION_PREFIX + i));
                    int reindeerMapNum = thisReplacement.mapNum;
                    int reindeerMapObjOffset = 0x80000 + outRom[0x87000 + reindeerMapNum * 2] + (outRom[0x87000 + reindeerMapNum * 2 + 1] << 8);
                    reindeerMapObjOffset += 8; // skip header
                    int objNum = thisReplacement.index;
                    outRom[reindeerMapObjOffset + 8 * objNum + 0] = EventFlags.XMAS_EXTRA_FLAGS[i]; // adjust event data of obj i
                    outRom[reindeerMapObjOffset + 8 * objNum + 1] = 0x00; // adjust event data of obj i
                    Logging.log("Reindeer " + i + " at map " + reindeerMapNum + " object " + objNum, "spoiler");
                }
            }

            // -- npc removal for lag reduction --

            // mandala - most of the npcs
            for (int i = 0; i < 6; i++)
            {
                setObjectNeverVisible(outRom, MAPNUM_MANDALA_SOUTH, i);
            }

            // south town
            setObjectNeverVisible(outRom, MAPNUM_SOUTHTOWN, 1);
            // 2 is password guy
            setObjectAlwaysVisible(outRom, MAPNUM_SOUTHTOWN, 2);
            setObjectNeverVisible(outRom, MAPNUM_SOUTHTOWN, 3);
            setObjectNeverVisible(outRom, MAPNUM_SOUTHTOWN, 4);
            setObjectNeverVisible(outRom, MAPNUM_SOUTHTOWN, 5);
            setObjectNeverVisible(outRom, MAPNUM_SOUTHTOWN, 6);
            setObjectNeverVisible(outRom, MAPNUM_SOUTHTOWN, 7);
            setObjectNeverVisible(outRom, MAPNUM_SOUTHTOWN, 8);

            // north town
            setObjectNeverVisible(outRom, MAPNUM_NORTHTOWN, 0);
            setObjectNeverVisible(outRom, MAPNUM_NORTHTOWN, 1);
            setObjectNeverVisible(outRom, MAPNUM_NORTHTOWN, 3);
            setObjectNeverVisible(outRom, MAPNUM_NORTHTOWN, 4);
            setObjectNeverVisible(outRom, MAPNUM_NORTHTOWN, 5);
            setObjectNeverVisible(outRom, MAPNUM_NORTHTOWN, 6);
            setObjectNeverVisible(outRom, MAPNUM_NORTHTOWN, 8);

            // pandora
            setObjectNeverVisible(outRom, MAPNUM_PANDORA, 1);
            setObjectNeverVisible(outRom, MAPNUM_PANDORA, 2);
            setObjectNeverVisible(outRom, MAPNUM_PANDORA, 4);

            // walruses and such
            setObjectNeverVisible(outRom, MAPNUM_TODO, 2);
            setObjectNeverVisible(outRom, MAPNUM_TODO, 4);

            // mushrooms
            setObjectNeverVisible(outRom, MAPNUM_MATANGO_FRONTYARD, 3);
            setObjectNeverVisible(outRom, MAPNUM_MATANGO_FRONTYARD, 4);
            setObjectNeverVisible(outRom, MAPNUM_MATANGO_FRONTYARD, 6);
            setObjectNeverVisible(outRom, MAPNUM_MATANGO_FRONTYARD, 7);

            // kakkara
            setObjectNeverVisible(outRom, MAPNUM_KAKKARA, 3);
            setObjectNeverVisible(outRom, MAPNUM_KAKKARA, 5);
            setObjectNeverVisible(outRom, MAPNUM_KAKKARA, 6);
            setObjectNeverVisible(outRom, MAPNUM_KAKKARA, 7);

            // moogle town
            setObjectNeverVisible(outRom, MAPNUM_UPPERLAND_MOOGLE_VILLAGE, 3);
            setObjectNeverVisible(outRom, MAPNUM_UPPERLAND_MOOGLE_VILLAGE, 4);
            setObjectNeverVisible(outRom, MAPNUM_UPPERLAND_MOOGLE_VILLAGE, 5);
            setObjectNeverVisible(outRom, MAPNUM_UPPERLAND_MOOGLE_VILLAGE, 6);

            // inside pandora
            setObjectNeverVisible(outRom, MAPNUM_PANDORA_CASTLE_INTERIOR_A, 0);
            setObjectNeverVisible(outRom, MAPNUM_PANDORA_CASTLE_INTERIOR_A, 1);
            setObjectNeverVisible(outRom, MAPNUM_PANDORA_CASTLE_INTERIOR_A, 5);
            setObjectNeverVisible(outRom, MAPNUM_PANDORA_CASTLE_INTERIOR_A, 6);

            setObjectNeverVisible(outRom, MAPNUM_PANDORA_CASTLE_INTERIOR_B, 3);
            setObjectNeverVisible(outRom, MAPNUM_PANDORA_CASTLE_INTERIOR_B, 4);
            setObjectNeverVisible(outRom, MAPNUM_PANDORA_CASTLE_INTERIOR_B, 5);
            setObjectNeverVisible(outRom, MAPNUM_PANDORA_CASTLE_INTERIOR_B, 6);

            // mandala north
            setObjectNeverVisible(outRom, MAPNUM_MANDALA_NORTH, 1);
            setObjectNeverVisible(outRom, MAPNUM_MANDALA_NORTH, 2);
            setObjectNeverVisible(outRom, MAPNUM_MANDALA_NORTH, 4);
            setObjectNeverVisible(outRom, MAPNUM_MANDALA_NORTH, 5);

            // gold town
            setObjectNeverVisible(outRom, MAPNUM_GOLDCITY, 0);
            setObjectNeverVisible(outRom, MAPNUM_GOLDCITY, 2);
            setObjectNeverVisible(outRom, MAPNUM_GOLDCITY, 5);

            // turtle island interior
            setObjectNeverVisible(outRom, MAPNUM_TURTLEISLAND_INTERIOR, 0);
            setObjectNeverVisible(outRom, MAPNUM_TURTLEISLAND_INTERIOR, 1);
            setObjectNeverVisible(outRom, MAPNUM_TURTLEISLAND_INTERIOR, 2);
            setObjectNeverVisible(outRom, MAPNUM_TURTLEISLAND_INTERIOR, 3);
            setObjectNeverVisible(outRom, MAPNUM_TURTLEISLAND_INTERIOR, 4);
            setObjectNeverVisible(outRom, MAPNUM_TURTLEISLAND_INTERIOR, 5);
            setObjectNeverVisible(outRom, MAPNUM_TURTLEISLAND_INTERIOR, 6);
            setObjectNeverVisible(outRom, MAPNUM_TURTLEISLAND_INTERIOR, 7);
            setObjectNeverVisible(outRom, MAPNUM_TURTLEISLAND_INTERIOR, 8);

            // ---

            // clear the npcs on the map before kilroy to get them out of the way
            setObjectNeverVisible(outRom, MAPNUM_KILROYSHIP_INTERIOR, 0);
            setObjectNeverVisible(outRom, MAPNUM_KILROYSHIP_INTERIOR, 1);
            setObjectNeverVisible(outRom, MAPNUM_KILROYSHIP_INTERIOR, 2);
            setObjectNeverVisible(outRom, MAPNUM_KILROYSHIP_INTERIOR, 3);

            // remove all the npcs in northtown right when you come out of the sewer
            setObjectNeverVisible(outRom, MAPNUM_NORTHTOWN_INTERIOR_A, 0);
            setObjectNeverVisible(outRom, MAPNUM_NORTHTOWN_INTERIOR_A, 1);
            setObjectNeverVisible(outRom, MAPNUM_NORTHTOWN_INTERIOR_A, 2);
            setObjectNeverVisible(outRom, MAPNUM_NORTHTOWN_INTERIOR_A, 3);
            setObjectNeverVisible(outRom, MAPNUM_NORTHTOWN_INTERIOR_A, 4);
            setObjectNeverVisible(outRom, MAPNUM_NORTHTOWN_INTERIOR_A, 5);

            // remove girl npc on vampire map
            setObjectNeverVisible(outRom, MAPNUM_VAMPIRE_ARENA, 0);

            // northtown castle gate, take out that thing that blocks you from entering
            int northtownCastleMapNum = MAPNUM_NTC_ENTRANCE;
            int northtownCastleMapObjOffset = 0x80000 + outRom[0x85000 + northtownCastleMapNum * 2] + (outRom[0x85000 + northtownCastleMapNum * 2 + 1] << 8); // 85b64->7a
            // FE 00 0F 3B 00 01 34 2E D2 26 43 FF 00 0F 3C 00 01 34 2E D3 26 43
            outRom[northtownCastleMapObjOffset + 6] = 0;
            outRom[northtownCastleMapObjOffset + 7] = 0xFF;
            outRom[northtownCastleMapObjOffset + 17] = 0;
            outRom[northtownCastleMapObjOffset + 18] = 0xFF;

            // snap dragon map; hide some npcs, and show the watermelon entry guy after the snap dragon is gone
            setObjectNeverVisible(outRom, MAPNUM_SNAPDRAGON_ARENA, 1);
            setObjectNeverVisible(outRom, MAPNUM_SNAPDRAGON_ARENA, 2);
            setObjectVisibility(outRom, MAPNUM_SNAPDRAGON_ARENA, 3, EventFlags.LOST_CONTINENT_HEXAS_FLAG, 0x5, 0xF);
            setObjectData(outRom, MAPNUM_SNAPDRAGON_ARENA, 3, 6, 0xB5); // run event 0x4b5 to go to watermelon; unsure what the 0x40 bit is for here
            setObjectData(outRom, MAPNUM_SNAPDRAGON_ARENA, 3, 7, 0x44);

            // modify the door y values from outside dryad palace to the snap dragon map to stick you on the snap dragon
            // platform, instead of down at the entrance
            outRom[0x83f5a] = 0x38;
            outRom[0x83f5e] = 0x38;

            // make basement luka visible, she says a silly thing
            setObjectAlwaysVisible(outRom, MAPNUM_WATERPALACE_BASEMENT, 0);

            // make jema invisible on the entryway map of water palace
            setObjectNeverVisible(outRom, MAPNUM_WATERPALACE_HALLWAY_NPC, 0);

            int tonpoleMapNum = MAPNUM_TONPOLE_ARENA;
            setObjectVisibility(outRom, tonpoleMapNum, 0, 0xA0, 0x0, 0x0); // tonpole visibility - A0 is unused in vanilla
            // piece visibility for the exits
            int tonpoleMapObjOffset = 0x80000 + outRom[0x85000 + tonpoleMapNum * 2] + (outRom[0x85000 + tonpoleMapNum * 2 + 1] << 8); // 878be
            // FE 00 0F 32 00 00 A9 1F 30 2C 0A A9 1F F2 1C 24 FF 00 0F 33 00 00
            outRom[tonpoleMapObjOffset + 6] = 0xA0; // tonpole visibility - A0 is unused in vanilla
            outRom[tonpoleMapObjOffset + 11] = 0xA0;

            setObjectAlwaysVisible(outRom, MAPNUM_NTC_INTERIOR_J, 2); // lets you access metal mantis - make visible always

            // map 377 - metal mantis .. use event flag x58 for visibility instead of x34
            int metalMantisMapNum = MAPNUM_METALMANTIS_ARENA;
            setObjectVisibility(outRom, metalMantisMapNum, 0, 0x58, 0x0, 0x0);
            // piece visibility for the exit
            int metalMantisMapObjOffset = 0x80000 + outRom[0x85000 + metalMantisMapNum * 2] + (outRom[0x85000 + metalMantisMapNum * 2 + 1] << 8); // 8815b
            // FE 00 0F 5E 00 01 34 4F 5C 1E 21 FF 00 0F 5F 00 01
            outRom[metalMantisMapObjOffset + 6] = 0x58;
            outRom[metalMantisMapObjOffset + 7] = 0x1F;

            // door 3fa - go past the one switchy spot in 430 in case you do the whole lower section first and now have to fight the snap dragon
            outRom[0x83000 + 0x3fa * 4 + 1] = (byte)(((32 << 1)) | (outRom[0x83000 + 0x3fa * 4 + 1] & 0x01));
            outRom[0x83000 + 0x3fa * 4 + 2] = (byte)(((27 << 1)) | (outRom[0x83000 + 0x3fa * 4 + 2] & 0x01));

            // jehk and johk always visible
            setObjectAlwaysVisible(outRom, MAPNUM_JEHK_LOBBY, 0);
            setObjectAlwaysVisible(outRom, MAPNUM_JEHK_LOBBY, 1);

            // unblock tasnica; these guys normally use the luna flag
            setObjectNeverVisible(outRom, MAPNUM_TASNICA_ENTRANCE, 0);
            setObjectNeverVisible(outRom, MAPNUM_TASNICA_ENTRANCE, 1);
            setObjectNeverVisible(outRom, MAPNUM_TASNICA_ENTRANCE, 2);

            // watts visible always on his original map
            setObjectAlwaysVisible(outRom, MAPNUM_WATTS, 0);

            // neko always visible outside on the purple continent
            int finalNekoMapNum = MAPNUM_SUNKENCONTINENT_EXTERIOR;
            setObjectAlwaysVisible(outRom, finalNekoMapNum, 5);
            // a couple changes to the purple continent exterior's map piece visibility to stop you from going backward through the undersea area
            // FE 00 0F 57 01 01 02 01 55 01 01 02 22 5D 4D 77 02 22 5D 3B 85 02 22 5D 29 77 02 22 5D 4D 93 02 22 5D 29 93 02 
            // 22 5D 3B A1 02 22 5E 4D AF 02 22 5E 29 AF 02 22 59 35 77 02 22 59 45 77 02 22 59 4D 85 02 22 59 2D 85 02 22 59 
            // 35 93 02 22 59 45 93 02 22 59 4D A1 02 22 59 2D A1 02 22 59 35 AF 02 22 59 45 AF 02 22 5A 3D 77 02 22 5A 25 85 
            // 02 22 5A 35 85 02 22 5A 45 85 02 22 5A 55 85 02 22 5A 3D 93 02 22 5A 25 A1 02 22 5A 35 A1 02 22 5A 45 A1 02 22 
            // 5A 55 A1 02 22 5B 25 77 02 22 5B 21 81 02 22 5B 21 8F 02 22 5B 25 93 02 22 5B 21 9D 02 22 5B 25 AF 02 22 5B 21 
            // AB 02 22 5C 59 AF 02 22 5C 5D AB 02 22 5C 5D 9D 02 22 5C 59 93 02 22 5C 5D 8F 02 22 5C 5D 81 02 22 5C 59 77 47 
            // 3F D6 48 BB FF 00 0F 58 01 01 02 01 56 01 01 47 3F D7 48 BB
            outRom[0x86b78] = 0x00;
            outRom[0x86b79] = 0x0F;
            outRom[0x86b88] = 0x00;
            outRom[0x86b89] = 0x0F;
            // also the triggers list
            int finalNekoMapObjOffset = 0x80000 + outRom[0x84000 + finalNekoMapNum * 2] + (outRom[0x84000 + finalNekoMapNum * 2 + 1] << 8);
            finalNekoMapObjOffset += 2; // walkon event
            outRom[finalNekoMapObjOffset + 2 * 4 + 0] = 0x79; // change from direct door to event 0x579, which we repurpose to kick you out of the kettlekin door
            outRom[finalNekoMapObjOffset + 2 * 4 + 1] = 0x05;
            outRom[finalNekoMapObjOffset + 2 * 5 + 0] = 0x79;
            outRom[finalNekoMapObjOffset + 2 * 5 + 1] = 0x05;

            // remove a pointless northtown castle gate, which frees to event flag 0xDD to use
            int northtownCastleDDGateMapNum = MAPNUM_NTC_INTERIOR_E;
            int northTownCastleMapObjOffset = 0x80000 + outRom[0x85000 + northtownCastleDDGateMapNum * 2] + (outRom[0x85000 + northtownCastleDDGateMapNum * 2 + 1] << 8); // 881A6->c6
            // FE 00 0F A7 00 00 DD 01 A9 10 1E DD 01 A9 12 1E FF 00 0F A8 00 00 DD 01 AA 10 1E DD 01 AA 12 1E
            outRom[northTownCastleMapObjOffset + 6] = 0;
            outRom[northTownCastleMapObjOffset + 7] = 0xFF;
            outRom[northTownCastleMapObjOffset + 11] = 0;
            outRom[northTownCastleMapObjOffset + 12] = 0xFF;
            outRom[northTownCastleMapObjOffset + 22] = 0;
            outRom[northTownCastleMapObjOffset + 23] = 0xFF;
            outRom[northTownCastleMapObjOffset + 27] = 0;
            outRom[northTownCastleMapObjOffset + 28] = 0xFF;

            // modify event flag used for lime slime's display, which i think freed up another flag to use
            setObjectVisibility(outRom, MAPNUM_LIMESLIME_ARENA, 0, EventFlags.SHADE_PALACE_PROGRESS_FLAG_1, 0x0, 0x5);

            // make gnome always visible
            setObjectAlwaysVisible(outRom, MAPNUM_EARTHSEED, 2);

            // modify visibility of a map piece in the room directly under snap dragon to ensure the whip post is always visible
            int sunkenContinentLastWhipPostMapNum = MAPNUM_GRANDPALACE_INTERIOR_F;
            int sunkenContinentLastWhipPostMapObjOffset = 0x80000 + outRom[0x85000 + sunkenContinentLastWhipPostMapNum * 2] + (outRom[0x85000 + sunkenContinentLastWhipPostMapNum * 2 + 1] << 8); // 88666->b8
            // FE 00 0F AA 00 01 00 0F EE 40 3C 00 0F EE 1C 3C 00 0F 87 14 44 EE 00 CC 3A 45 48 2F AC 1A 27 48 2F 28 1C 19 48 3F AD 24 2D 48 3F 28 24 29 49 2F AC 40 27 49 2F 28 42 19 49 3F AD 38 2D 49 3F 28 3A 29 FF 00 0F AB 00 01 48 3F AE 24 2D 49 3F AE 38 2D
            //                                                                ^^ ^^
            outRom[sunkenContinentLastWhipPostMapObjOffset + 21] = 0;
            outRom[sunkenContinentLastWhipPostMapObjOffset + 22] = 0xFF;

            return true;
        }

        // utility methods

        private static void setObjectData(byte[] outRom, int mapNum, int objNum, int index, byte value)
        {
            int mapObjOffset = VanillaMapUtil.getObjectOffset(outRom, mapNum);
            mapObjOffset += 8; // skip header
            outRom[mapObjOffset + 8 * objNum + index] = value;
        }

        private static byte getObjectData(byte[] outRom, int mapNum, int objNum, int index)
        {
            int mapObjOffset = VanillaMapUtil.getObjectOffset(outRom, mapNum);
            mapObjOffset += 8; // skip header
            return outRom[mapObjOffset + 8 * objNum + index];
        }

        private static byte getObjectX(byte[] outRom, int mapNum, int objNum)
        {
            // note this includes the msb, which isn't part of the position
            return getObjectData(outRom, mapNum, objNum, 2);
        }

        private static byte getObjectY(byte[] outRom, int mapNum, int objNum)
        {
            // note this includes the msb, which isn't part of the position
            return getObjectData(outRom, mapNum, objNum, 3);
        }

        private static void setObjectXY(byte[] outRom, int mapNum, int objNum, byte x, byte y)
        {
            setObjectData(outRom, mapNum, objNum, 2, x); // x position; i forget what the MSB is for but it's not part of the position value
            setObjectData(outRom, mapNum, objNum, 3, y); // y position; i forget what the MSB is for but it's not part of the position value
        }

        private static void setObjectAlwaysVisible(byte[] outRom, int mapNum, int objNum)
        {
            // visibility on event flag 0x00; visible if flag value is 0 -> F (always)
            setObjectVisibility(outRom, mapNum, objNum, 0x00, 0x00, 0x0F);
        }

        private static void setObjectNeverVisible(byte[] outRom, int mapNum, int objNum)
        {
            // visibility on event flag 0x00; visible if flag value is F -> F (never)
            setObjectVisibility(outRom, mapNum, objNum, 0x00, 0x0F, 0x0F);
        }

        private static void setObjectVisibility(byte[] outRom, int mapNum, int objNum, byte eventFlag, int minValue, int maxValue)
        {
            setObjectData(outRom, mapNum, objNum, 0, eventFlag);
            // min/max values the object is visible for; 4 bits each; min in MSN so it "reads" left to right
            setObjectData(outRom, mapNum, objNum, 1, (byte)(maxValue | (minValue << 4)));
        }

        private static void setObjectEvent(byte[] outRom, int mapNum, int objNum, int eventNum)
        {
            // set the event executed by an npc when spoken to, or by an enemy when they die
            // obj [6] is the LSB of this
            // obj [7] & 0x07 is the MSB (max event ID 0x7ff)
            setObjectData(outRom, mapNum, objNum, 6, (byte)eventNum); // lsb
            byte oldMsb = (byte)(getObjectData(outRom, mapNum, objNum, 7) & 0xF8);
            byte newByte7 = (byte)(((eventNum >> 8) & 0x07) | oldMsb);
            setObjectData(outRom, mapNum, objNum, 7, newByte7); // msb
        }
    }
}
