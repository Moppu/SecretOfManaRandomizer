using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.mapgen;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.hacks.common.procgen;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.common.GeneratedMapUtil;
using static SoMRandomizer.processing.common.VanillaBossMaps;

namespace SoMRandomizer.processing.bossrush
{
    /// <summary>
    /// Main randomization for boss rush mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class BossRushRandomizer : RandoProcessor
    {
        public const string PROPERTY_FLOOR_NUMBER = "FloorNumber";
        public const int BASE_DOOR_NUM = 10;

        protected override string getName()
        {
            return "Randomizer for boss rush mode";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Random r = context.randomFunctional;

            List<int> bossesInOrder = new List<int>();
            int numMaps = BossOrderRandomizer.allPossibleBosses.Count;
            for(int i=0; i <= numMaps; i++)
            {
                bossesInOrder.Add(context.workingData.getInt(BossOrderRandomizer.BOSS_INDEX_PREFIX + i));
            }
            
            int doorNum = BASE_DOOR_NUM;
            List<int> entryDoors = new List<int>();
            entryDoors.Add(doorNum);
            
            // make rest map pieces and palette shared by every floor
            RestMap.makeRestMapCommon(context);
            context.workingData.setInt(GeneratedMapUtil.PROPERTY_GENERATED_PALETTE_INDEX, 16);
            for (int i = 0; i < bossesInOrder.Count; i++)
            {
                int mapNum = i;
                context.workingData.setInt(PROPERTY_FLOOR_NUMBER, i);

                // make this floor's rest map
                RestMap.makeRestMap(context);

                // make door into map
                entryDoors.Add(doorNum);
                Door doorToRestMap = new Door();
                doorToRestMap.setTargetMap((ushort)(i * 2));
                doorToRestMap.setXpos(34);
                doorToRestMap.setYpos(13);
                doorToRestMap.setTransitionType(0);
                context.replacementDoors[doorNum++] = doorToRestMap;

                // pull the vanilla boss map as map floornum * 2 + 256
                byte bossNum = (byte)bossesInOrder[i];
                // mana beast map is handled separately
                if (bossNum != 0x7F)
                {
                    Logging.log("Boss " + (i + 1) + ": " + BossRushEventGenerator.bossNames[bossNum], "spoiler");
                    VanillaBossMap vanillaBossMapInfo = VanillaBossMaps.BY_VANILLA_BOSS_ID[bossNum];
                    GeneratedMap vanillaBossMap = GeneratedMapUtil.importVanillaBossArena(context, bossNum);
                    // clear any walkon flag from vanilla so not to execute random events that aren't present anymore
                    vanillaBossMap.mapData.mapHeader.setWalkonEventEnabled(false);
                    foreach (MapObject bossObject in vanillaBossMap.mapData.mapObjects)
                    {
                        int bossDeathEvent = context.workingData.getInt(BossRushEventGenerator.BOSS_DEATH_EVENT_ID_PREFIX + i);
                        bossObject.setEvent((ushort)bossDeathEvent);
                    }
                    context.generatedMaps[i * 2 + 256] = vanillaBossMap.mapData;

                    vanillaBossMap.mapData.mapPieces = new FullMapMapPieces();
                    vanillaBossMap.mapData.mapPieces.bgPieces.Add(getPieceRef(i * 2, false));
                    vanillaBossMap.mapData.mapPieces.fgPieces.Add(getPieceRef(i * 2 + 1, true));
                    context.replacementMapPieces[i * 2] = SomMapCompressor.EncodeMap(vanillaBossMap.layer1Data).ToList();
                    context.replacementMapPieces[i * 2 + 1] = SomMapCompressor.EncodeMap(vanillaBossMap.layer2Data).ToList();

                    Door doorToBossMap = new Door();
                    doorToBossMap.setTargetMap((ushort)(i * 2 + 256));
                    doorToBossMap.setXpos((byte)vanillaBossMapInfo.entryPos.xpos);
                    doorToBossMap.setYpos((byte)vanillaBossMapInfo.entryPos.ypos);
                    doorToBossMap.setTransitionType(0);
                    doorToBossMap.setLayer2Collision(vanillaBossMap.layer2Collision);
                    context.replacementDoors[doorNum++] = doorToBossMap;
                }
                else
                {
                    // mana beast entry
                    Door doorToBossMap = new Door();
                    doorToBossMap.setTargetMap(0xFD);
                    doorToBossMap.setXpos(18);
                    doorToBossMap.setYpos(22);
                    doorToBossMap.setTransitionType(0x81);
                    context.replacementDoors[0xBC] = doorToBossMap;
                }
            }

            ManaBeastMap.makeManaBeastMap(origRom, context);
            CreditsMap.makeCreditsMap(origRom, context);

            int startDoorOffset = context.workingData.getInt(DoorExpansion.START_DOOR_OFFSET);
            // fill in the doors array for starting each map - this is used for magic rope and continues
            for (int i = 0; i < numMaps; i++)
            {
                outRom[startDoorOffset + i * 2] = (byte)entryDoors[i];
                outRom[startDoorOffset + i * 2 + 1] = (byte)(entryDoors[i] >> 8);
            }
            return true;
        }

        private static FullMapMapPieceReference getPieceRef(int pieceIndex, bool isForeground)
        {
            FullMapMapPieceReference pieceRef = new FullMapMapPieceReference();
            pieceRef.xPos = 0;
            pieceRef.yPos = 0;
            pieceRef.eventVisibilityFlagId = 0x00;
            pieceRef.eventVisibilityValueLow = 0x00;
            pieceRef.eventVisibilityValueHigh = 0x0F;
            pieceRef.pieceIndex = pieceIndex;
            pieceRef.isForegroundPiece = isForeground;
            return pieceRef;
        }
    }
}
