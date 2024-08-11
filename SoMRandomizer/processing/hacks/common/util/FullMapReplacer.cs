using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using static SoMRandomizer.util.DataUtil;

namespace SoMRandomizer.processing.hacks.common.util
{
    /// <summary>
    /// Utility for replacing a full set of game maps with another one, for procgen randomization modes.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FullMapReplacer
    {
        const int triggerOffsetsOffset = 0x84000;
        const int piecePlacementOffsetsOffset = 0x85000;
        const int objectOffsetsOffset = 0x87000;

        private static TupleList<int, int> freeSpaceRanges = new TupleList<int, int>
            {
                //{ 0x84400, 0x84FFF }, // vanilla triggers set one
                //{ 0x85400, 0x86FFF }, // vanilla piece refs
                { 0x87400, 0x8E068 }, // vanilla triggers set two, and objects
            };

        public void process(byte[] rom, RandoContext context)
        {
            Dictionary<int, FullMap> replacementMaps = context.generatedMaps;
            // MOPPLE: this assumes the addresses/blocks involved have enough space, and that
            // the set of maps provided are the only maps in use. this should be improved later by expanding
            // all of these blocks to a new location and using 24 bit addressing, similar to what EventExpander
            // or MapPieceExpander already does
            int offset = freeSpaceRanges[0].Item1;

            SortedSet<int> mapNumsInOrder = new SortedSet<int>(replacementMaps.Keys);
            // header + objects; each 8 bytes
            foreach (int mapNum in mapNumsInOrder)
            {
                Logging.log("Generated map " + mapNum.ToString("X") + " objects", "debug");
                try
                {
                    FullMap mapData = replacementMaps[mapNum];
                    int objectsSize = 8 + mapData.mapObjects.Count * 8;
                    checkWriteOffset(ref offset, objectsSize);
                    write16BitOffset(rom, objectOffsetsOffset + mapNum * 2, offset);
                    Logging.log("  Header: " + mapData.mapHeader, "debug");
                    mapData.mapHeader.put(rom, offset);
                    for (int i = 0; i < mapData.mapObjects.Count; i++)
                    {
                        Logging.log("  Object " + i + " @" + (offset + 8 + 8 * i).ToString("X6") + ": " + mapData.mapObjects[i], "debug");
                        mapData.mapObjects[i].put(rom, offset + 8 + 8 * i);
                    }
                    offset += objectsSize;
                    // start of next map's block = end of this one's
                    write16BitOffset(rom, objectOffsetsOffset + (mapNum + 1) * 2, offset);
                }
                catch(Exception e)
                {
                    throw ErrorUtil.appendExceptionInfo("Unable to write header/objects for generated map " + mapNum, e);
                }
            }

            // piece refs
            foreach (int mapNum in mapNumsInOrder)
            {
                try
                {
                    FullMap mapData = replacementMaps[mapNum];
                    byte[] mapPieceRef = VanillaMapUtil.makeMapPieceReference(mapData.mapPieces);
                    checkWriteOffset(ref offset, mapPieceRef.Length);
                    Logging.log("Generated map " + mapNum.ToString("X") + " piece refs @" + offset.ToString("X6") + ": " + mapData.mapPieces, "debug");
                    write16BitOffset(rom, piecePlacementOffsetsOffset + mapNum * 2, offset);
                    for (int i = 0; i < mapPieceRef.Length; i++)
                    {
                        rom[offset + i] = mapPieceRef[i];
                    }
                    offset += mapPieceRef.Length;
                    // start of next map's block = end of this one's
                    write16BitOffset(rom, piecePlacementOffsetsOffset + (mapNum + 1) * 2, offset);
                }
                catch(Exception e)
                {
                    throw ErrorUtil.appendExceptionInfo("Unable to write pieces for generated map " + mapNum, e);
                }
            }

            // triggers
            foreach (int mapNum in mapNumsInOrder)
            {
                Logging.log("Generated map " + mapNum.ToString("X") + " triggers", "debug");
                try
                {
                    FullMap mapData = replacementMaps[mapNum];
                    byte[] triggerData = new byte[mapData.mapTriggers.Count * 2];
                    checkWriteOffset(ref offset, triggerData.Length);
                    for (int i = 0; i < mapData.mapTriggers.Count; i++)
                    {
                        Logging.log("  Trigger " + i + " @" + (offset + i * 2).ToString("X6") + ": " + mapData.mapTriggers[i].ToString("X4"), "debug");
                        triggerData[i * 2] = (byte)mapData.mapTriggers[i];
                        triggerData[i * 2 + 1] = (byte)(mapData.mapTriggers[i] >> 8);
                    }
                    write16BitOffset(rom, triggerOffsetsOffset + mapNum * 2, offset);
                    for (int i = 0; i < triggerData.Length; i++)
                    {
                        rom[offset + i] = triggerData[i];
                    }
                    offset += triggerData.Length;
                    // start of next map's block = end of this one's
                    write16BitOffset(rom, triggerOffsetsOffset + (mapNum + 1) * 2, offset);
                }
                catch(Exception e)
                {
                    throw ErrorUtil.appendExceptionInfo("Unable to write triggers for generated map " + mapNum, e);
                }
            }
        }

        private static void write16BitOffset(byte[] rom, int offset, int offsetValue)
        {
            rom[offset] = (byte)offsetValue;
            rom[offset + 1] = (byte)(offsetValue >> 8);
        }

        private static void checkWriteOffset(ref int offset, int sizeNeeded)
        {
            for(int i=0; i < freeSpaceRanges.Count - 1; i++)
            {
                Tuple<int, int> freeSpaceRange = freeSpaceRanges[i];
                if(offset >= freeSpaceRange.Item1 && offset <= freeSpaceRange.Item2)
                {
                    if(offset + sizeNeeded > freeSpaceRange.Item2)
                    {
                        // inside range, but the next location isn't - use the next one
                        offset = freeSpaceRanges[i + 1].Item1;
                    }
                }
            }
        }
    }
}
