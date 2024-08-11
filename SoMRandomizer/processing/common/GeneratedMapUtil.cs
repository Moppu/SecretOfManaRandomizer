using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.hacks.common.other;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.common.VanillaBossMaps;

namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// Utilities for generating maps for rando roms.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class GeneratedMapUtil
    {
        public const string PROPERTY_GENERATED_PALETTE_INDEX = "GeneratedPaletteIndex";

        public static GeneratedMap importVanillaBossArena(RandoContext context, byte bossId)
        {
            VanillaBossMap vanillaBossMap = VanillaBossMaps.BY_VANILLA_BOSS_ID[bossId];
            MapHeader bossMapHeader = VanillaMapUtil.getHeader(context.originalRom, vanillaBossMap.originalMapNum);
            // if making new palettes for each map (AC, boss rush) otherwise just keep the pal it already has (chaos)
            if (context.workingData.hasSetting(PROPERTY_GENERATED_PALETTE_INDEX))
            {
                // pull whole pal from OG and write to runningGeneratedPalIndex; use that index here
                int palIndex = context.workingData.getIntAndIncrement(PROPERTY_GENERATED_PALETTE_INDEX);
                AnimatedPaletteSimplification.copyPaletteAnimationSettings(context, bossMapHeader.getPaletteSet(), palIndex);
                context.replacementMapPalettes[palIndex] = VanillaPaletteUtil.getMapPaletteSet(context.originalRom, bossMapHeader.getPaletteSet());
                bossMapHeader.setPaletteSet((byte)palIndex);
            }

            FullMapMapPieces ogMapPieces = VanillaMapUtil.getMapPieceReference(context.originalRom, vanillaBossMap.originalMapNum);
            // keep these separate (not a dictionary) to preserve insertion order for layering onto the final map data after
            List<FullMapMapPieceReference> ogFilteredPieces = new List<FullMapMapPieceReference>();
            List<byte[,]> ogFilteredData = new List<byte[,]>();
            foreach (FullMapMapPieceReference pieceRef in ogMapPieces.bgPieces)
            {
                if (!vanillaBossMap.bgPieceExclusions.Contains(pieceRef.indexOnMap))
                {
                    ogFilteredPieces.Add(pieceRef);
                    ogFilteredData.Add(VanillaMapUtil.loadVanillaMapPieceAndRemoveAllDoors(context.originalRom, pieceRef.pieceIndex, vanillaBossMap.originalMapNum));
                }
            }
            foreach (FullMapMapPieceReference pieceRef in ogMapPieces.fgPieces)
            {
                if (!vanillaBossMap.fgPieceExclusions.Contains(pieceRef.indexOnMap))
                {
                    ogFilteredPieces.Add(pieceRef);
                    ogFilteredData.Add(VanillaMapUtil.loadVanillaMapPieceAndRemoveAllDoors(context.originalRom, pieceRef.pieceIndex, vanillaBossMap.originalMapNum));
                }
            }
            int maxWidth = 0;
            int maxHeight = 0;
            // determine size of consolidated map piece for boss arena
            for (int i = 0; i < ogFilteredPieces.Count; i++)
            {
                FullMapMapPieceReference pieceRef = ogFilteredPieces[i];
                byte[,] mapPieceData = ogFilteredData[i];
                int widthNeeded = pieceRef.xPos + mapPieceData.GetLength(0);
                int heightNeeded = pieceRef.yPos + mapPieceData.GetLength(1);
                maxWidth = Math.Max(maxWidth, widthNeeded);
                maxHeight = Math.Max(maxHeight, heightNeeded);
            }
            byte[,] consolidatedBg = new byte[maxWidth, maxHeight];
            byte[,] consolidatedFg = new byte[maxWidth, maxHeight];
            for (int i = 0; i < ogFilteredPieces.Count; i++)
            {
                FullMapMapPieceReference pieceRef = ogFilteredPieces[i];
                byte[,] mapPieceData = ogFilteredData[i];
                if (!pieceRef.isForegroundPiece)
                {
                    VanillaMapUtil.copyMapPieceData(mapPieceData, consolidatedBg, pieceRef.xPos, pieceRef.yPos);
                }
                else
                {
                    VanillaMapUtil.copyMapPieceData(mapPieceData, consolidatedFg, pieceRef.xPos, pieceRef.yPos);
                }
            }

            GeneratedMap bossMap = new GeneratedMap();
            bossMap.mapData = new FullMap();
            bossMap.mapData.mapHeader = bossMapHeader;
            bossMap.layer1Data = consolidatedBg;
            bossMap.layer2Data = consolidatedFg;
            bossMap.entryPos = vanillaBossMap.entryPos;

            if (bossId == VanillaBossMap.TRIPLE_TONPOLE_OBJECT_INDICATOR)
            {
                // put the other two tonpoles in
                bossId = TONPOLE.getOriginalBossId(context.originalRom);

                MapObject extraTonpole1 = new MapObject();
                extraTonpole1.setSpecies(bossId);
                extraTonpole1.setXpos((byte)(vanillaBossMap.bossPos.xpos - 5));
                extraTonpole1.setYpos((byte)(vanillaBossMap.bossPos.ypos + 4));
                extraTonpole1.setEventVisFlag(0x00);
                extraTonpole1.setEventVisMinimum(0x00);
                extraTonpole1.setEventVisMaximum(0x0F);
                bossMap.mapData.mapObjects.Add(extraTonpole1);

                MapObject extraTonpole2 = new MapObject();
                extraTonpole2.setSpecies(bossId);
                extraTonpole2.setXpos((byte)(vanillaBossMap.bossPos.xpos + 6));
                extraTonpole2.setYpos((byte)(vanillaBossMap.bossPos.ypos + 4));
                extraTonpole2.setEventVisFlag(0x00);
                extraTonpole2.setEventVisMinimum(0x00);
                extraTonpole2.setEventVisMaximum(0x0F);
                bossMap.mapData.mapObjects.Add(extraTonpole2);
            }

            MapObject bossObject = new MapObject();
            bossObject.setSpecies(bossId);
            bossObject.setXpos((byte)vanillaBossMap.bossPos.xpos);
            bossObject.setYpos((byte)vanillaBossMap.bossPos.ypos);
            bossObject.setEventVisFlag(0x00);
            bossObject.setEventVisMinimum(0x00);
            bossObject.setEventVisMaximum(0x0F);
            bossMap.mapData.mapObjects.Add(bossObject);
            return bossMap;
        }

        public class GeneratedMap
        {
            public string mapName = "?";
            public ushort mapId;
            public FullMap mapData;
            public byte[,] layer1Data;
            public byte[,] layer2Data;
            public XyPos entryPos;
            public List<XyPos> exitLocations = new List<XyPos>();
            public bool layer2Collision;
        }
    }
}
