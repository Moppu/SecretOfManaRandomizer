using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.hacks.common.util;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.common.VanillaBossMaps;
using static SoMRandomizer.processing.openworld.PlandoProperties;
using static SoMRandomizer.processing.common.SomVanillaValues;
using SoMRandomizer.processing.common.structure;

namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// Swap bosses for vanilla rando or open world.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class BossSwaps : RandoProcessor
    {
        protected override string getName()
        {
            return "Boss randomizer";
        }

        // 70 c0, d0 e0
        // spikey pos:
        // C2/DCB7:	11 2B00 D000			[Store $00D0 at CharMem $002B]
        // C2/DCBC:	11 3200 E000            [Store $00E0 at CharMem $0032]
        // C2/DCCA:	11 2B00 7000			[Store $0070 at CharMem $002B]
        // C2/DCCF:	11 3200 C000            [Store $00C0 at CharMem $0032]
        // C2/DCDD:	11 2B00 3001			[Store $0130 at CharMem $002B]
        // C2/DCE2:	11 3200 C000            [Store $00C0 at CharMem $0032]
        // + blue spike below at 2DCFB?
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            bool preserveEarlyBosses = false;
            bool swapOnly = false;
            bool bypass = false;
            bool modifyTripleTonpole = false;

            if (randoMode == VanillaRandoSettings.MODE_KEY)
            {
                bypass = !settings.getBool(VanillaRandoSettings.PROPERTYNAME_RANDOMIZE_BOSSES);
                preserveEarlyBosses = settings.getBool(VanillaRandoSettings.PROPERTYNAME_PRESERVE_EARLY_BOSSES);
                swapOnly = true; // always swap for vanilla rando
                modifyTripleTonpole = true;
            }
            else if (randoMode == OpenWorldSettings.MODE_KEY)
            {
                string bossType = settings.get(OpenWorldSettings.PROPERTYNAME_RANDOMIZE_BOSSES);
                swapOnly = bossType == "swap";
                bypass = bossType == "vanilla";
            }
            else
            {
                Logging.log("Unsupported mode for boss randomizer");
                return false;
            }

            context.namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 96, "Mech Rider I");
            context.namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 100, "Mech Rider II");
            context.namesOfThings.setName(NamesOfThings.INDEX_ENEMIES_START + 114, "Mech Rider III");

            if(bypass)
            {
                Logging.log("Bypassing boss rando - configured not to randomize");
                return false;
            }

            Random r = context.randomFunctional;
            
            // MOPPLE: it would be nice to eventually set up a table for these per-boss-arena so these bosses can exist on
            // multiple maps per randomized rom, instead of modifying them statically based on the one map they appear on.

            // /////////////////////////////////////
            // hardcoded positions:
            // /////////////////////////////////////

            // tropicallo
            int[] tropicalloXLocations = new int[] { 0x249F3, 0x249F7, 0x249FB, 0x249FF, 0x24A03, 0x24A07, 0x24A0B, 0x24A0F, 0x24A13, 0x24A17, 0x24A1B, 0x24A1F, };
            int[] tropicalloYLocations = new int[] { 0x249F5, 0x249F9, 0x249FD, 0x24A01, 0x24A05, 0x24A09, 0x24A0D, 0x24A11, 0x24A15, 0x24A19, 0x24A1D, 0x24A21, };
            int[] tropicalloBXLocations = new int[] { 0x24643, 0x24647, };
            int[] tropicalloBYLocations = new int[] { 0x24645, 0x24649, };

            // frozen tropicallo thing wtf was its name
            // B0 00 A0 00 40 01 A0 00 B0 00 50 01 50 01 50 01
            // original values [4]: xb, x14, xb, x15, 
            int[] borealXLocations = new int[] { 0x24A33, 0x24A37, 0x24A3B, 0x24A3F, 0x24A43, 0x24A47, 0x24A4B, 0x24A4F, 0x24A53, 0x24A57, 0x24A5B, 0x24A5F, };
            // original values [4]: xa, xa, x15, x15
            int[] borealYLocations = new int[] { 0x24A35, 0x24A39, 0x24A3D, 0x24A41, 0x24A45, 0x24A49, 0x24A4D, 0x24A51, 0x24A55, 0x24A59, 0x24A5D, 0x24A61, };
            // original values: xb.8, x14.8 
            int[] borealBXLocations = new int[] { 0x2464B, 0x2464D, };
            // original values: x10, x10
            int[] borealBYLocations = new int[] { 0x2464F, 0x24651, };

            // spikey: middle, left platform, right platform
            int spikeyMiddleXLoc = 0x2DCBA; // original value 13 * 16
            int spikeyLeftXLoc = 0x2DCCD; // original value 7 * 16
            int spikeyRightXLoc = 0x2DCE0; // original value 19 * 16
            int spikeyMiddleYLoc = 0x2DCBF; // original value 14 * 16
            int spikeyLeftYLoc = 0x2DCD2; // original value 12 * 16
            int spikeyRightYLoc = 0x2DCE5; // original value 12 * 16

            // blue spike
            int blueSpikeLeftXLoc = 0x2DCFE; // original value x8
            int blueSpikeRightXLoc = 0x2DD11; // original value x16
            int blueSpikeMiddleXLoc = 0x2DD24; // original value xf
            int blueSpikeLeftYLoc = 0x2DD03; // original value x13
            int blueSpikeRightYLoc = 0x2DD16; // original value xf
            int blueSpikeMiddleYLoc = 0x2DD29; // original value x12

            // original value: 0xf
            int springBeakCenterXLocation = 0x1CE398;
            // original value: 0xe
            int springBeakCenterYLocation = 0x1CE39A;

            // 0x130
            int axeBeakCenterXLocation = 0x1CE39C;
            // 0x1B0
            int axeBeakCenterYLocation = 0x1CE39E;

            // original values: 0x8, 0x15
            int[] springBeakPodiumXLocations = new int[] { 0x1CE3D0, 0x1CE3D4, };
            // original values: 0xd, 0xe
            int[] springBeakPodiumYLocations = new int[] { 0x1CE3D2, 0x1CE3D6, };

            // 0xE8, 0x198
            int[] axeBeakPodiumXLocations = new int[] { 0x1CE3D8, 0x1CE3DC, };
            // 0x120, 0x120
            int[] axeBeakPodiumYLocations = new int[] { 0x1CE3DA, 0x1CE3DE, };

            // original values: 0x9, 0xf, 0x14, 0x9, 0xf, 0x15, 0x8, x15
            int[] springBeakJumpXLocations = new int[] { 0x1CE3A0, 0x1CE3A4, 0x1CE3A8, 0x1CE3AC, 0x1CE3B0, 0x1CE3B4, 0x2E4F5, 0x2E51E, };
            // original values: 0x9, 0xb, 0xa, 0x13, 0x12, 0x14, 0xd, 0xe
            int[] springBeakJumpYLocations = new int[] { 0x1CE3A2, 0x1CE3A6, 0x1CE3AA, 0x1CE3AE, 0x1CE3B2, 0x1CE3B6, 0x2E4FA, 0x2E523, };

            // 0xB0, 0x1E0, 0xC0, 0xD0, 0x190, 0x1B0, 0xF0, 0x170, 
            int[] axeBeakJumpXLocations = new int[] { 0x1CE3B8, 0x1CE3BC, 0x1CE3C0, 0x1CE3C4, 0x1CE3C8, 0x1CE3CC, 0x2E553, 0x2E579, };
            // 0x140, 0x140, 0x1A0, 0x180, 0x190, 0x1A0, 0x120, 0x120, 
            int[] axeBeakJumpYLocations = new int[] { 0x1CE3BA, 0x1CE3BE, 0x1CE3C2, 0x1CE3C6, 0x1CE3CA, 0x1CE3CE, 0x2E558, 0x2E57E, };
            
            List<VanillaBossMap> swapMaps = new List<VanillaBossMap>();
            swapMaps.Add(VanillaBossMaps.MANTISANT);
            if (!preserveEarlyBosses)
            {
                swapMaps.Add(VanillaBossMaps.TROPICALLO);
                swapMaps.Add(VanillaBossMaps.SPIKEY);
                swapMaps.Add(VanillaBossMaps.TONPOLE);
            }
            swapMaps.Add(VanillaBossMaps.MINOTAUR);
            swapMaps.Add(VanillaBossMaps.JABBER);
            swapMaps.Add(VanillaBossMaps.SPRINGBEAK);
            swapMaps.Add(VanillaBossMaps.FROSTGIGAS);
            swapMaps.Add(VanillaBossMaps.SNAPDRAGON);
            swapMaps.Add(VanillaBossMaps.MECHRIDER1);
            swapMaps.Add(VanillaBossMaps.VAMPIRE);
            swapMaps.Add(VanillaBossMaps.METALMANTIS);
            swapMaps.Add(VanillaBossMaps.MECHRIDER2);
            swapMaps.Add(VanillaBossMaps.KILROY);
            swapMaps.Add(VanillaBossMaps.GORGON);
            swapMaps.Add(VanillaBossMaps.BOREAL);
            swapMaps.Add(VanillaBossMaps.GREATVIPER);
            swapMaps.Add(VanillaBossMaps.BLUESPIKE);
            swapMaps.Add(VanillaBossMaps.HYDRA);
            swapMaps.Add(VanillaBossMaps.HEXAS);
            swapMaps.Add(VanillaBossMaps.KETTLEKIN);
            swapMaps.Add(VanillaBossMaps.MECHRIDER3);
            swapMaps.Add(VanillaBossMaps.FIREGIGAS);
            swapMaps.Add(VanillaBossMaps.AXEBEAK);
            swapMaps.Add(VanillaBossMaps.BUFFY);
            swapMaps.Add(VanillaBossMaps.DRAGONWORM);
            swapMaps.Add(VanillaBossMaps.THUNDERGIGAS);
            // layer 2 ones
            swapMaps.Add(VanillaBossMaps.AEGRTOJNFSDGHOPALNIAPLEIAN);
            swapMaps.Add(VanillaBossMaps.WALLFACE);
            swapMaps.Add(VanillaBossMaps.DOOMSWALL);
            swapMaps.Add(VanillaBossMaps.SNOWDRAGON);
            swapMaps.Add(VanillaBossMaps.REDDRAGON);
            swapMaps.Add(VanillaBossMaps.BLUEDRAGON);
            swapMaps.Add(VanillaBossMaps.DARKLICH);

            Dictionary<VanillaBossMap, byte> newBossNums = new Dictionary<VanillaBossMap, byte>();
            foreach(VanillaBossMap swapMap in swapMaps)
            {
                newBossNums[swapMap] = swapMap.getOriginalBossId(origRom);
            }

            List<byte> restrictedPositionBosses = new List<byte>();
            restrictedPositionBosses.Add(88); // wall face
            restrictedPositionBosses.Add(89); // tropicallo
            restrictedPositionBosses.Add(91); // spikey
            restrictedPositionBosses.Add(93); // spring beak
            restrictedPositionBosses.Add(104); // boreal face
            restrictedPositionBosses.Add(107); // blue spike
            restrictedPositionBosses.Add(118); // axe beak

            // make vampire behave like buffy collision-wise
            outRom[0x26b24] = 0xEA;
            outRom[0x26b25] = 0xEA;

            // also fix hitting enemies with ranged weapons on vampire map
            // $02/C5C7 30 F4       BMI $F4    [$C5BD]      A:0082 X:0600 Y:0000 P:eNvMxdIzc
            outRom[0x2c5c7] = 0xEA;
            outRom[0x2c5c8] = 0xEA;

            // make a new display settings
            // 0C = 00 15 00 00 15 10 7D 46 11; 4611 -> 0000
            outRom[0x801E1] = 0x00;
            outRom[0x801E2] = 0x15;
            outRom[0x801E3] = 0x00;
            outRom[0x801E4] = 0x00;
            outRom[0x801E5] = 0x15;
            outRom[0x801E6] = 0x10;
            outRom[0x801E7] = 0x7D;
            outRom[0x801E8] = 0x00;
            outRom[0x801E9] = 0x00;
            List<byte> restrictedPositionBossesRemaining = new List<byte>(restrictedPositionBosses);
            List<byte> layer2Bosses = new byte[] { 88, 97, 110, 115, 117, 119, 121, }.ToList();
            // OG map num -> plando boss location key
            Dictionary<int, string> plandoBosses = new Dictionary<int, string> {
                {MAPNUM_MANTISANT_ARENA, KEY_BOSS_MANTISANT},
                {MAPNUM_WALLFACE_ARENA, KEY_BOSS_WALLFACE},
                {MAPNUM_TROPICALLO_ARENA, KEY_BOSS_TROPICALLO},
                {MAPNUM_MINOTAUR_ARENA, KEY_BOSS_MINOTAUR},
                {MAPNUM_SPIKEY_ARENA, KEY_BOSS_SPIKEY},
                {MAPNUM_JABBERWOCKY_ARENA, KEY_BOSS_JABBERWOCKY},
                {MAPNUM_SPRINGBEAK_ARENA, KEY_BOSS_SPRINGBEAK},
                {MAPNUM_FROSTGIGAS_ARENA, KEY_BOSS_FROSTGIGAS},
                {MAPNUM_SNAPDRAGON_ARENA, KEY_BOSS_SNAPDRAGON},
                // mech rider 1
                {MAPNUM_DOOMSWALL_ARENA, KEY_BOSS_DOOMSWALL},
                {MAPNUM_VAMPIRE_ARENA, KEY_BOSS_VAMPIRE},
                {MAPNUM_METALMANTIS_ARENA, KEY_BOSS_METALMANTIS},
                {MAPNUM_MECHRIDER2_ARENA, KEY_BOSS_MECHRIDER2},
                {MAPNUM_KILROY_ARENA, KEY_BOSS_KILROY},
                {MAPNUM_GORGONBULL_ARENA, KEY_BOSS_GORGONBULL},
                // brambler
                {MAPNUM_BOREALFACE_ARENA, KEY_BOSS_BOREALFACE},
                {MAPNUM_GREATVIPER_ARENA, KEY_BOSS_GREATVIPER},
                // lime slime
                {MAPNUM_BLUESPIKE_ARENA, KEY_BOSS_BLUESPIKE},
                // chamber's eye
                {MAPNUM_HYDRA_ARENA, KEY_BOSS_HYDRA},
                {MAPNUM_WATERMELON_ARENA, KEY_BOSS_WATERMELON},
                {MAPNUM_HEXAS_ARENA, KEY_BOSS_HEXAS},
                {MAPNUM_KETTLEKIN_ARENA, KEY_BOSS_KETTLEKIN},
                {MAPNUM_TONPOLE_ARENA, KEY_BOSS_TONPOLE},
                {MAPNUM_MECHRIDER3_ARENA, KEY_BOSS_MECHRIDER3},
                {MAPNUM_SNOWDRAGON_ARENA, KEY_BOSS_SNOWDRAGON},
                {MAPNUM_FIREGIGAS_ARENA, KEY_BOSS_FIREGIGAS},
                {MAPNUM_REDDRAGON_ARENA, KEY_BOSS_REDDRAGON},
                {MAPNUM_AXEBEAK_ARENA, KEY_BOSS_AXEBEAK},
                {MAPNUM_BLUEDRAGON_ARENA, KEY_BOSS_BLUEDRAGON},
                {MAPNUM_BUFFY_ARENA, KEY_BOSS_BUFFY},
                {MAPNUM_DARKLICH_ARENA_B, KEY_BOSS_DARKLICH}, 
                // biting lizard
                {MAPNUM_DRAGONWORM_ARENA, KEY_BOSS_DRAGONWORM},
                // dread slime
                {MAPNUM_THUNDERGIGAS_ARENA, KEY_BOSS_THUNDERGIGAS},
                // doom's eye
                // mana beast
            };

            List<VanillaBossMap> plandoMaps = new List<VanillaBossMap>();
            foreach (VanillaBossMap swapMap1 in swapMaps)
            {
                // one plando pass first
                bool foundPlando = false;
                byte boss = 0x57;
                if (plandoBosses.ContainsKey(swapMap1.originalMapNum) && context.plandoSettings.ContainsKey(plandoBosses[swapMap1.originalMapNum]))
                {
                    string plandoValue = context.plandoSettings[plandoBosses[swapMap1.originalMapNum]][0];
                    // look up boss value based on name from the UI
                    if (BOSS_IDS_BY_PLANDO_VALUE.ContainsKey(plandoValue))
                    {
                        boss = BOSS_IDS_BY_PLANDO_VALUE[plandoValue];
                        foundPlando = true;
                        Logging.log("Processing boss plando for map: " + swapMap1.name + " -> " + plandoValue, "debug");
                    }
                    else
                    {
                        Logging.log("Warning: couldn't process boss plando for: " + swapMap1.name + "; unknown boss name: " + plandoValue);
                    }
                }
                if (foundPlando)
                {
                    restrictedPositionBossesRemaining.Remove(boss);
                    newBossNums[swapMap1] = boss;
                    plandoMaps.Add(swapMap1);
                }
            }

            foreach (VanillaBossMap swapMap1 in swapMaps)
            {
                // check if already plando'ed
                if (!plandoMaps.Contains(swapMap1))
                {
                    if (swapOnly)
                    {
                        for (int i = 0; i < 100; i++)
                        {
                            // pick a random one to try to swap with
                            VanillaBossMap swapMap2 = swapMaps[r.Next() % swapMaps.Count];
                            // not having its own boss in this list should mean it never gets swapped back
                            if (swapMap1.supportedSwapBosses.Contains(newBossNums[swapMap2]) &&
                                swapMap2.supportedSwapBosses.Contains(newBossNums[swapMap1]))
                            {
                                byte boss1 = newBossNums[swapMap1];
                                byte boss2 = newBossNums[swapMap2];
                                newBossNums[swapMap1] = boss2;
                                newBossNums[swapMap2] = boss1;
                                // stop searching
                                break;
                            }
                        }
                    }
                    else
                    {
                        byte[] supportedBosses = swapMap1.allSupportedBosses;
                        byte boss = supportedBosses[r.Next() % supportedBosses.Length];
                        // don't allow duplicates of ones with hardcoded positions
                        while (restrictedPositionBosses.Contains(boss) && !restrictedPositionBossesRemaining.Contains(boss))
                        {
                            boss = supportedBosses[r.Next() % supportedBosses.Length];
                        }
                        restrictedPositionBossesRemaining.Remove(boss);
                        newBossNums[swapMap1] = boss;

                        // change display settings to disable layer 2 so it doesn't look all fucky for normal bosses
                        if (!layer2Bosses.Contains(newBossNums[swapMap1]))
                        {
                            if (swapMap1.originalMapNum == MAPNUM_WALLFACE_ARENA)
                            {
                                // wallface
                                outRom[0x8B66D] = 25;
                            }
                            if (swapMap1.originalMapNum == MAPNUM_WATERMELON_ARENA)
                            {
                                // watermelon
                                outRom[0x8BF05] = 25;
                            }
                            if (swapMap1.originalMapNum == MAPNUM_BLUEDRAGON_ARENA)
                            {
                                // blue d
                                outRom[0x8BE9D] = 25;
                            }
                            if (swapMap1.originalMapNum == MAPNUM_REDDRAGON_ARENA)
                            {
                                // red d
                                outRom[0x8BE8D] = 25;
                            }
                            if (swapMap1.originalMapNum == MAPNUM_SNOWDRAGON_ARENA)
                            {
                                // snow d
                                outRom[0x8BE7D] = 25;
                            }
                            if (swapMap1.originalMapNum == MAPNUM_DARKLICH_ARENA_B)
                            {
                                // lich
                                outRom[0x8BF5D] = 25;
                            }
                        }
                    }
                }
            }

            foreach (VanillaBossMap testMap in swapMaps)
            {
                byte bossNum = newBossNums[testMap];
                string bossName = context.namesOfThings.getOriginalName(NamesOfThings.INDEX_ENEMIES_START + bossNum);
                Logging.log("Boss map " + testMap + " has boss " + bossName + " (" + bossNum + ")", "spoiler");
                List<XyPos> pos = null;
                List<XyPos> bpos = null;
                testMap.process(outRom, bossNum);
                // set positions for jumps and stuff for bosses that need them
                switch (bossNum)
                {
                    case BOSSID_WALLFACE:
                        // wall
                        int wallYValue = testMap.wallBossDeathYValue;
                        // 27df5
                        outRom[0x27DF5] = (byte)wallYValue;
                        outRom[0x27DF6] = (byte)(wallYValue>>8);
                        break;
                    case BOSSID_TROPICALLO:
                        // tropicallo
                        pos = testMap.tropicalloPositions;
                        bpos = testMap.bramblerPositions;
                        for (int i = 0; i < tropicalloXLocations.Length; i++)
                        {
                            short xpos = pos[i % pos.Count].xpos;
                            short ypos = pos[i % pos.Count].ypos;
                            outRom[tropicalloXLocations[i]] = (byte)xpos;
                            outRom[tropicalloXLocations[i] + 1] = (byte)(xpos >> 8);
                            outRom[tropicalloYLocations[i]] = (byte)ypos;
                            outRom[tropicalloYLocations[i] + 1] = (byte)(ypos >> 8);
                        }
                        for (int i = 0; i < tropicalloBXLocations.Length; i++)
                        {
                            short xpos = bpos[i % pos.Count].xpos;
                            short ypos = bpos[i % pos.Count].ypos;
                            outRom[tropicalloBXLocations[i]] = (byte)xpos;
                            outRom[tropicalloBXLocations[i] + 1] = (byte)(xpos >> 8);
                            outRom[tropicalloBYLocations[i]] = (byte)ypos;
                            outRom[tropicalloBYLocations[i] + 1] = (byte)(ypos >> 8);
                        }
                        break;
                    case BOSSID_SPIKEY:
                        {
                            // spikey
                            XyPos posC = testMap.spikeyCenterPos;
                            XyPos posL = testMap.spikeyLeftPos;
                            XyPos posR = testMap.spikeyRightPos;
                            outRom[spikeyMiddleXLoc] = (byte)posC.xpos;
                            outRom[spikeyMiddleXLoc + 1] = (byte)(posC.xpos >> 8);
                            outRom[spikeyMiddleYLoc] = (byte)posC.ypos;
                            outRom[spikeyMiddleYLoc + 1] = (byte)(posC.ypos >> 8);
                            outRom[spikeyLeftXLoc] = (byte)posL.xpos;
                            outRom[spikeyLeftXLoc + 1] = (byte)(posL.xpos >> 8);
                            outRom[spikeyLeftYLoc] = (byte)posL.ypos;
                            outRom[spikeyLeftYLoc + 1] = (byte)(posL.ypos >> 8);
                            outRom[spikeyRightXLoc] = (byte)posR.xpos;
                            outRom[spikeyRightXLoc + 1] = (byte)(posR.xpos >> 8);
                            outRom[spikeyRightYLoc] = (byte)posR.ypos;
                            outRom[spikeyRightYLoc + 1] = (byte)(posR.ypos >> 8);
                        }
                        break;
                    case BOSSID_SPRINGBEAK:
                        // spring beak
                        {
                            XyPos posC = testMap.springBeakCenterPos;
                            outRom[springBeakCenterXLocation] = (byte)posC.xpos;
                            outRom[springBeakCenterXLocation + 1] = (byte)(posC.xpos >> 8);
                            outRom[springBeakCenterYLocation] = (byte)posC.ypos;
                            outRom[springBeakCenterYLocation + 1] = (byte)(posC.ypos >> 8);

                            List<XyPos> posP = testMap.springBeakPodiumPositions;
                            for (int i = 0; i < springBeakPodiumXLocations.Length; i++)
                            {
                                short xpos = posP[i % posP.Count].xpos;
                                short ypos = posP[i % posP.Count].ypos;
                                outRom[springBeakPodiumXLocations[i]] = (byte)xpos;
                                outRom[springBeakPodiumXLocations[i] + 1] = (byte)(xpos >> 8);
                                outRom[springBeakPodiumYLocations[i]] = (byte)ypos;
                                outRom[springBeakPodiumYLocations[i] + 1] = (byte)(ypos >> 8);
                            }

                            List<XyPos> posJ = testMap.springBeakJumpPositions;
                            for (int i = 0; i < springBeakJumpXLocations.Length; i++)
                            {
                                short xpos = posJ[i % posJ.Count].xpos;
                                short ypos = posJ[i % posJ.Count].ypos;
                                outRom[springBeakJumpXLocations[i]] = (byte)xpos;
                                outRom[springBeakJumpXLocations[i] + 1] = (byte)(xpos >> 8);
                                outRom[springBeakJumpYLocations[i]] = (byte)ypos;
                                outRom[springBeakJumpYLocations[i] + 1] = (byte)(ypos >> 8);
                            }
                        }
                        break;
                    case BOSSID_BOREAL:
                        // boreal face
                        pos = testMap.tropicalloPositions;
                        bpos = testMap.bramblerPositions;
                        for (int i = 0; i < borealXLocations.Length; i++)
                        {
                            short xpos = pos[i % pos.Count].xpos;
                            short ypos = pos[i % pos.Count].ypos;
                            outRom[borealXLocations[i]] = (byte)xpos;
                            outRom[borealXLocations[i] + 1] = (byte)(xpos >> 8);
                            outRom[borealYLocations[i]] = (byte)ypos;
                            outRom[borealYLocations[i] + 1] = (byte)(ypos >> 8);
                        }
                        for (int i = 0; i < borealBXLocations.Length; i++)
                        {
                            short xpos = bpos[i % pos.Count].xpos;
                            short ypos = bpos[i % pos.Count].ypos;
                            outRom[borealBXLocations[i]] = (byte)xpos;
                            outRom[borealBXLocations[i] + 1] = (byte)(xpos >> 8);
                            outRom[borealBYLocations[i]] = (byte)ypos;
                            outRom[borealBYLocations[i] + 1] = (byte)(ypos >> 8);
                        }
                        break;
                    case BOSSID_BLUESPIKE:
                        // blue spike
                        {
                            XyPos posC = testMap.spikeyCenterPos;
                            XyPos posL = testMap.spikeyLeftPos;
                            XyPos posR = testMap.spikeyRightPos;
                            outRom[blueSpikeMiddleXLoc] = (byte)posC.xpos;
                            outRom[blueSpikeMiddleXLoc + 1] = (byte)(posC.xpos >> 8);
                            outRom[blueSpikeMiddleYLoc] = (byte)posC.ypos;
                            outRom[blueSpikeMiddleYLoc + 1] = (byte)(posC.ypos >> 8);
                            outRom[blueSpikeLeftXLoc] = (byte)posL.xpos;
                            outRom[blueSpikeLeftXLoc + 1] = (byte)(posL.xpos >> 8);
                            outRom[blueSpikeLeftYLoc] = (byte)posL.ypos;
                            outRom[blueSpikeLeftYLoc + 1] = (byte)(posL.ypos >> 8);
                            outRom[blueSpikeRightXLoc] = (byte)posR.xpos;
                            outRom[blueSpikeRightXLoc + 1] = (byte)(posR.xpos >> 8);
                            outRom[blueSpikeRightYLoc] = (byte)posR.ypos;
                            outRom[blueSpikeRightYLoc + 1] = (byte)(posR.ypos >> 8);
                        }
                        break;
                    case BOSSID_AXEBEAK:
                        // axe beak
                        {
                            XyPos posC = testMap.springBeakCenterPos;
                            outRom[axeBeakCenterXLocation] = (byte)posC.xpos;
                            outRom[axeBeakCenterXLocation + 1] = (byte)(posC.xpos >> 8);
                            outRom[axeBeakCenterYLocation] = (byte)posC.ypos;
                            outRom[axeBeakCenterYLocation + 1] = (byte)(posC.ypos >> 8);

                            List<XyPos> posP = testMap.springBeakPodiumPositions;
                            for (int i = 0; i < axeBeakPodiumXLocations.Length; i++)
                            {
                                short xpos = posP[i % posP.Count].xpos;
                                short ypos = posP[i % posP.Count].ypos;
                                outRom[axeBeakPodiumXLocations[i]] = (byte)xpos;
                                outRom[axeBeakPodiumXLocations[i] + 1] = (byte)(xpos >> 8);
                                outRom[axeBeakPodiumYLocations[i]] = (byte)ypos;
                                outRom[axeBeakPodiumYLocations[i] + 1] = (byte)(ypos >> 8);
                            }

                            List<XyPos> posJ = testMap.springBeakJumpPositions;
                            for (int i = 0; i < axeBeakJumpXLocations.Length; i++)
                            {
                                short xpos = posJ[i % posJ.Count].xpos;
                                short ypos = posJ[i % posJ.Count].ypos;
                                outRom[axeBeakJumpXLocations[i]] = (byte)xpos;
                                outRom[axeBeakJumpXLocations[i] + 1] = (byte)(xpos >> 8);
                                outRom[axeBeakJumpYLocations[i]] = (byte)ypos;
                                outRom[axeBeakJumpYLocations[i] + 1] = (byte)(ypos >> 8);
                            }
                        }
                        break;
                }

                // mantis ant
                // get offset using params from testMap
                int mapNum = testMap.originalMapNum;
                int objsOffset = 0x80000 + outRom[0x87000 + mapNum * 2] + (outRom[0x87000 + mapNum * 2 + 1] << 8);
                objsOffset += 8; // skip map header
                objsOffset += testMap.originalObjNum * 8;
                objsOffset += 5; // index for id
                byte oldBossNum = outRom[objsOffset];
                outRom[objsOffset] = bossNum;

                // 101C00, 29 bytes apiece
                // [0]     level 
                // [1,2]   hp    
                // [3]     mp    
                // [4]     str   
                // [5]     agi   
                // [6]     int   
                // [7]     wis   
                // [8]     eva   
                // [9,10]  def   
                // [11]    mev   
                // [12,13] mdef  
                // [14]    species
                // [15]    element
                // [16,17] exp   
                // [18]    black magic power
                // [19]    white magic power
                // [20]    death anim
                // [26]    weapon/magic levels
                // [27,28] gold
                int[] eightBitSwapStats = new int[] { 0, 11, 26, 3, 4, 5, 6, 7, 8, 18, 19, };
                int[] sixteenBitSwapStats = new int[] { 1, 16, 27, 9, 12, };
                foreach (int eightSwapIndex in eightBitSwapStats)
                {
                    byte newValue = origRom[0x101C00 + oldBossNum * 29 + eightSwapIndex];

                    if (eightSwapIndex == 3 && (bossNum == BOSSID_TONPOLE || bossNum == BOSSID_SNAPDRAGON || bossNum == BOSSID_BITINGLIZARD))
                    {
                        // tonpole mp
                        newValue = 6;
                    }
                    outRom[0x101C00 + bossNum * 29 + eightSwapIndex] = newValue;
                    if (bossNum == BOSSID_TONPOLE)
                    {
                        // tonpole?  set biting lizard (122) too
                        outRom[0x101C00 + BOSSID_BITINGLIZARD * 29 + eightSwapIndex] = newValue;
                    }
                    // 108 chamber eye 126 doom eye 88 wall face 97 doom's wall
                    // note that this depends on walls being swapped to themselves
                    if(BOSSID_WALLFACE == 88 && oldBossNum == BOSSID_DOOMSWALL)
                    {
                        // set chamber eye
                        newValue = origRom[0x101C00 + BOSSID_DOOMSEYE * 29 + eightSwapIndex];
                        outRom[0x101C00 + BOSSID_CHAMBERSEYE * 29 + eightSwapIndex] = newValue;
                    }
                    if (bossNum == BOSSID_DOOMSWALL && oldBossNum == BOSSID_WALLFACE)
                    {
                        // set doom eye
                        newValue = origRom[0x101C00 + BOSSID_CHAMBERSEYE * 29 + eightSwapIndex];
                        outRom[0x101C00 + BOSSID_DOOMSEYE * 29 + eightSwapIndex] = newValue;
                    }
                }
                foreach (int sixteenSwapIndex in sixteenBitSwapStats)
                {
                    outRom[0x101C00 + bossNum * 29 + sixteenSwapIndex] = origRom[0x101C00 + oldBossNum * 29 + sixteenSwapIndex];
                    outRom[0x101C00 + bossNum * 29 + sixteenSwapIndex + 1] = origRom[0x101C00 + oldBossNum * 29 + sixteenSwapIndex + 1];
                    if (bossNum == BOSSID_TONPOLE)
                    {
                        // tonpole?  set biting lizard (122) too
                        outRom[0x101C00 + BOSSID_BITINGLIZARD * 29 + sixteenSwapIndex] = origRom[0x101C00 + oldBossNum * 29 + sixteenSwapIndex];
                        outRom[0x101C00 + BOSSID_BITINGLIZARD * 29 + sixteenSwapIndex + 1] = origRom[0x101C00 + oldBossNum * 29 + sixteenSwapIndex + 1];
                    }
                    // 108 chamber eye 126 doom eye 88 wall face 97 doom's wall
                    // note that this depends on walls being swapped to themselves
                    if (bossNum == BOSSID_WALLFACE && oldBossNum == BOSSID_DOOMSWALL)
                    {
                        // set chamber eye
                        outRom[0x101C00 + BOSSID_CHAMBERSEYE * 29 + sixteenSwapIndex] = origRom[0x101C00 + BOSSID_DOOMSEYE * 29 + sixteenSwapIndex];
                        outRom[0x101C00 + BOSSID_CHAMBERSEYE * 29 + sixteenSwapIndex + 1] = origRom[0x101C00 + BOSSID_DOOMSEYE * 29 + sixteenSwapIndex + 1];
                    }
                    if (bossNum == BOSSID_DOOMSWALL && oldBossNum == BOSSID_WALLFACE)
                    {
                        // set doom eye
                        outRom[0x101C00 + BOSSID_DOOMSEYE * 29 + sixteenSwapIndex] = origRom[0x101C00 + BOSSID_CHAMBERSEYE * 29 + sixteenSwapIndex];
                        outRom[0x101C00 + BOSSID_DOOMSEYE * 29 + sixteenSwapIndex + 1] = origRom[0x101C00 + BOSSID_CHAMBERSEYE * 29 + sixteenSwapIndex + 1];
                    }
                }
            }

            // make triple tonpole spawn one at a time for vanilla rando mode
            if (modifyTripleTonpole)
            {
                // ////////////////////////////////////////////////////
                // triple tonpole experiment
                // ////////////////////////////////////////////////////
                EventScript event6 = new EventScript();
                context.replacementEvents[0x6] = event6;
                event6.Add(EventCommandEnum.REFRESH_OBJECTS.Value);
                event6.End();

                int tripleTonpoleMapNum = MAPNUM_TRIPLETONPOLE_ARENA;
                int mapObjOffset = 0x80000 + outRom[0x87000 + tripleTonpoleMapNum * 2] + (outRom[0x87000 + tripleTonpoleMapNum * 2 + 1] << 8);
                mapObjOffset += 8; // skip header

                outRom[mapObjOffset + 8 * 0 + 0] = EventFlags.MULTI_TONPOLE_FLAG; // adjust event data of obj 0
                outRom[mapObjOffset + 8 * 0 + 1] = 0x00; // adjust event data of obj 0
                outRom[mapObjOffset + 8 * 1 + 0] = EventFlags.MULTI_TONPOLE_FLAG; // adjust event data of obj 1
                outRom[mapObjOffset + 8 * 1 + 1] = 0x11; // adjust event data of obj 1
                outRom[mapObjOffset + 8 * 2 + 0] = EventFlags.MULTI_TONPOLE_FLAG; // adjust event data of obj 2
                outRom[mapObjOffset + 8 * 2 + 1] = 0x22; // adjust event data of obj 2
                
                EventScript event66a = new EventScript();
                context.replacementEvents[0x66a] = event66a;
                event66a.IncrFlag(EventFlags.MULTI_TONPOLE_FLAG);
                event66a.Logic(EventFlags.MULTI_TONPOLE_FLAG, 0x0, 0x2, EventScript.GetJumpCmd(6));
                event66a.SetFlag(EventFlags.ICE_CASTLE_SWITCHES_FLAG_2, 5);
                event66a.SetFlag(EventFlags.ICE_PALACE_FLAG, 3);
                event66a.Door(0x2c3); // exit thingy
                event66a.Jump(0x715); // music probably
                event66a.End();
            }

            // modify weapon loader to load str instead of weapon power

            // B9 88 E1     LDA $E188,y
            outRom[0x462C] = 0xB9;
            outRom[0x462D] = 0x88;
            outRom[0x462E] = 0xE1;
            outRom[0x462F] = 0xEA;
            
            // i don't remember what all this does but it had something to do with making triple tonpole work

            // replace whole subroutine [0B8B-0BB7] with:
            // 22 xx xx xx    JSL
            outRom[0x20B8B] = 0x22;
            outRom[0x20B8C] = (byte)context.workingOffset;
            outRom[0x20B8D] = (byte)(context.workingOffset >> 8);
            outRom[0x20B8E] = (byte)((context.workingOffset >> 16) + 0xC0);

            // 60             RTS
            outRom[0x20B8F] = 0x60;
            // use the rest to expose C2 subroutines to JSLs .. 0C11, 0C33, 0C89
            // 0C11
            int jsl_0c11_offset = 0x20B90;
            outRom[0x20B90] = 0x20;
            outRom[0x20B91] = 0x11;
            outRom[0x20B92] = 0x0C;
            outRom[0x20B93] = 0x6B;

            int jsl_0c33_offset = 0x20B94;
            outRom[0x20B94] = 0x20;
            outRom[0x20B95] = 0x33;
            outRom[0x20B96] = 0x0C;
            outRom[0x20B97] = 0x6B;

            int jsl_0c89_offset = 0x20B98;
            outRom[0x20B98] = 0x20;
            outRom[0x20B99] = 0x89;
            outRom[0x20B9A] = 0x0C;
            outRom[0x20B9B] = 0x6B;

            int jsl_3963_offset = 0x20B9C;
            outRom[0x20B9C] = 0x20;
            outRom[0x20B9D] = 0x63;
            outRom[0x20B9E] = 0x39;
            outRom[0x20B9F] = 0x6B;

            // now mostly the same code for the new subr
            outRom[context.workingOffset++] = 0xA6;
            outRom[context.workingOffset++] = 0x87;

            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x96;
            outRom[context.workingOffset++] = 0x00;

            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x18;
            outRom[context.workingOffset++] = 0x00;

            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x12 + 9;

            // new code here
            // AF DC 00 7E         LDA 7E00DC
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xDC;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;
            // C9 69 01            CMP #0169
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x01;
            // F0 07               BEQ over
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x07;
            // LDA #07F8
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0xF8;
            outRom[context.workingOffset++] = 0x07;
            // JSL jsl_0c11_offset
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)jsl_0c11_offset;
            outRom[context.workingOffset++] = (byte)(jsl_0c11_offset >> 8);
            outRom[context.workingOffset++] = (byte)((jsl_0c11_offset >> 16) + 0xC0);
            // LDX $87
            outRom[context.workingOffset++] = 0xA6;
            outRom[context.workingOffset++] = 0x87;
            // JSL $C0006F
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xC0;
            // LDX $87
            outRom[context.workingOffset++] = 0xA6;
            outRom[context.workingOffset++] = 0x87;
            // LDA $0096,X
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x96;
            outRom[context.workingOffset++] = 0x00;
            // CMP #0034
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x34;
            outRom[context.workingOffset++] = 0x00;
            // BCS over
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x0F;
            // AND #0003
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x03;
            outRom[context.workingOffset++] = 0x00;
            // BNE 04
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x04;
            // JSL jsl_0c33_offset
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)jsl_0c33_offset;
            outRom[context.workingOffset++] = (byte)(jsl_0c33_offset >> 8);
            outRom[context.workingOffset++] = (byte)((jsl_0c33_offset >> 16) + 0xC0);
            // JSL jsl_0c89_offset
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)jsl_0c89_offset;
            outRom[context.workingOffset++] = (byte)(jsl_0c89_offset >> 8);
            outRom[context.workingOffset++] = (byte)((jsl_0c89_offset >> 16) + 0xC0);
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // over:
            // STZ $00A7,X
            outRom[context.workingOffset++] = 0x9E;
            outRom[context.workingOffset++] = 0xA7;
            outRom[context.workingOffset++] = 0x00;
            // INC $0094,X
            outRom[context.workingOffset++] = 0xFE;
            outRom[context.workingOffset++] = 0x94;
            outRom[context.workingOffset++] = 0x00;
            // STA $0096,X
            outRom[context.workingOffset++] = 0x9E;
            outRom[context.workingOffset++] = 0x96;
            outRom[context.workingOffset++] = 0x00;
            // LDA #8A00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x8A;
            // JSL jsl_3963_offset
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)jsl_3963_offset;
            outRom[context.workingOffset++] = (byte)(jsl_3963_offset >> 8);
            outRom[context.workingOffset++] = (byte)((jsl_3963_offset >> 16) + 0xC0);
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
