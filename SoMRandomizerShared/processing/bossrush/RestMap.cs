using SoMRandomizer.processing.ancientcave.mapgen;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.hacks.common.other;
using SoMRandomizer.util;
using System.Linq;

namespace SoMRandomizer.processing.bossrush
{
    /// <summary>
    /// Creates the rest maps for each floor in boss rush mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class RestMap
    {
        public static void makeRestMapCommon(RandoContext context)
        {
            // make shared content for rest maps - should only be called once per boss rush generation
            byte[] layer1 = DataUtil.readResource("SoMRandomizer.Resources.customMaps.bossRushRestMapL1.bin");
            byte[] layer2 = DataUtil.readResource("SoMRandomizer.Resources.customMaps.bossRushRestMapL2.bin");

            // generate some of layer1 based off layer2
            for (int i = 0; i < layer1.Length; i++)
            {
                if (layer2[i] == 0x2A || layer2[i] == 0x19 || layer2[i] == 0x1A || layer2[i] == 0x1B
                    || layer2[i] == 0x63)
                {
                    // solid wall
                    layer1[i] = 0x84;
                }
                if (layer2[i] == 0x57 || layer2[i] == 0x2B)
                {
                    // wall in upper left
                    layer1[i] = 0x95;
                }
                if (layer2[i] == 0x29 || layer2[i] == 0x61)
                {
                    // wall in upper right
                    layer1[i] = 0x93;
                }
                if (layer2[i] == 0x0B)
                {
                    // wall in lower left
                    layer1[i] = 0x72;
                }
                if (layer2[i] == 0x09 || layer2[i] == 0x41)
                {
                    // wall in lower right
                    layer1[i] = 0x70;
                }
                if (layer2[i] == 0x5D || layer2[i] == 0x5E || layer2[i] == 0x5F ||
                    layer2[i] == 0x6D || layer2[i] == 0x6E || layer2[i] == 0x6F ||
                    layer2[i] == 0x7D || layer2[i] == 0x7E || layer2[i] == 0x7F ||
                    layer2[i] == 0x9F ||
                    layer2[i] == 0xAC || layer2[i] == 0xAD || layer2[i] == 0xAE || layer2[i] == 0xAF)
                {
                    // waterfall->solid wall
                    layer1[i] = 0x84;
                }
            }

            layer1[46 * 50 + 27] = 0x84;
            layer1[46 * 50 + 28] = 0x84;

            // door
            layer1[8 * 50 + 33] = 182;
            layer2[8 * 50 + 33] = 0x1A;
            layer2[8 * 50 + 32] = 175;

            layer2[7 * 50 + 33] = 0x1A;
            layer2[7 * 50 + 32] = 159;

            layer2[6 * 50 + 32] = 0x6e;
            layer2[6 * 50 + 33] = 174;

            context.replacementMapPieces[128] = SomMapCompressor.EncodeMap(layer1, 50, 70).ToList();
            context.replacementMapPieces[129] = SomMapCompressor.EncodeMap(layer2, 50, 70).ToList();

            // palette index 0 for rest map palette
            ResourcePaletteSetSource paletteSetSrc = new ResourcePaletteSetSource("bossrush.bossRushRestMapPal.bin", 54);
            MapPaletteSet palSet = paletteSetSrc.getPaletteData(context);
            AnimatedPaletteSimplification.copyPaletteAnimationSettings(context, paletteSetSrc.getVanillaPaletteSet(), 0);
            context.replacementMapPalettes[0] = palSet;
        }

        public static void makeRestMap(RandoContext context)
        {
            // generated events to attach to npcs
            int nekoEvent = context.workingData.getInt(BossRushEventGenerator.NEKO_EVENT_NUM);
            int wattsEvent = context.workingData.getInt(BossRushEventGenerator.WATTS_EVENT_NUM);
            int phannaEvent = context.workingData.getInt(BossRushEventGenerator.PHANNA_EVENT_NUM);

            int floorNum = context.workingData.getInt(BossRushRandomizer.PROPERTY_FLOOR_NUMBER);
            FullMap restMap = new FullMap();
            context.generatedMaps[floorNum * 2] = restMap;

            MapObject neko = new MapObject();
            neko.setDirection(MapObject.DIR_DOWN);
            // run generated shop event
            neko.setEvent((ushort)nekoEvent);
            // always visible
            neko.setEventVisFlag(0x00);
            neko.setEventVisMinimum(0x00);
            neko.setEventVisMaximum(0x0F);
            // location
            neko.setXpos(30);
            neko.setYpos(9);
            // neko graphic
            neko.setSpecies(0x99);
            // shrug
            neko.setUnknownB7(0x08);
            neko.setUnknown4A(true);
            restMap.mapObjects.Add(neko);

            MapObject watts = new MapObject();
            watts.setDirection(MapObject.DIR_DOWN);
            // run generated shop event
            watts.setEvent((ushort)wattsEvent);
            // always visible
            watts.setEventVisFlag(0x00);
            watts.setEventVisMinimum(0x00);
            watts.setEventVisMaximum(0x0F);
            // location
            watts.setXpos(29);
            watts.setYpos(9);
            // watts graphic
            watts.setSpecies(0x9C);
            // shrug
            watts.setUnknownB7(0x08);
            watts.setUnknown4A(true);
            restMap.mapObjects.Add(watts);

            MapObject phanna = new MapObject();
            phanna.setDirection(MapObject.DIR_DOWN);
            // run generated shop event
            phanna.setEvent((ushort)phannaEvent);
            // always visible
            phanna.setEventVisFlag(0x00);
            phanna.setEventVisMinimum(0x00);
            phanna.setEventVisMaximum(0x0F);
            // location
            phanna.setXpos(29);
            phanna.setYpos(11);
            // watts graphic
            phanna.setSpecies(0xA6);
            // shrug
            phanna.setUnknownB7(0x08);
            phanna.setUnknown4A(true);
            restMap.mapObjects.Add(phanna);

            FullMapMapPieces restMapPieces = new FullMapMapPieces();
            FullMapMapPieceReference bgLayer = new FullMapMapPieceReference();
            bgLayer.eventVisibilityFlagId = 0x00;
            bgLayer.eventVisibilityValueLow = 0x00;
            bgLayer.eventVisibilityValueHigh = 0x0F;
            bgLayer.pieceIndex = 128; // as generated by makeRestMapCommon
            restMapPieces.bgPieces.Add(bgLayer);
            FullMapMapPieceReference fgLayer = new FullMapMapPieceReference();
            fgLayer.eventVisibilityFlagId = 0x00;
            fgLayer.eventVisibilityValueLow = 0x00;
            fgLayer.eventVisibilityValueHigh = 0x0F;
            fgLayer.pieceIndex = 129; // as generated by makeRestMapCommon
            restMapPieces.fgPieces.Add(fgLayer);
            restMap.mapPieces = restMapPieces;

            // map entry event
            if (floorNum == 0)
            {
                int walkonEvent = context.workingData.getInt(BossRushEventGenerator.RESTMAP_WALKON_EVENT_ID_PREFIX + floorNum);
                restMap.mapTriggers.Add((ushort)walkonEvent);
            }

            // event for yes/no to enter the next boss
            int bossEntryEvent = context.workingData.getInt(BossRushEventGenerator.NEXT_FLOOR_EVENT_ID_PREFIX + floorNum);
            restMap.mapTriggers.Add((ushort)bossEntryEvent);

            MapHeader restMapHeader = new MapHeader();
            restMapHeader.setPaletteSet(0); // as generated by makeRestMapCommon
            restMapHeader.setDisplaySettings(5);
            restMapHeader.setTileset16(30);
            restMapHeader.setTileset8(30);
            restMapHeader.setWalkonEventEnabled(floorNum == 0);
            restMapHeader.setNpcPalette(0xFF); // hostile map
            restMapHeader.setMagicRopeEnabled(false);
            restMapHeader.setFlammieEnabled(true);
            restMapHeader.setLayer1UsesForegroundTiles(false);
            restMapHeader.setLayer2UsesForegroundTiles(true);
            restMapHeader.setAnimatedTiles(true);
            restMapHeader.setPaletteAnimated(true);
            restMap.mapHeader = restMapHeader;
        }
    }
}
