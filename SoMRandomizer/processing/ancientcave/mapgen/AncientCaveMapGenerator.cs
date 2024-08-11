using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.hacks.common.other;
using SoMRandomizer.util;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.common.GeneratedMapUtil;
using static SoMRandomizer.processing.common.VanillaBossMaps;

namespace SoMRandomizer.processing.ancientcave.mapgen
{
    /// <summary>
    /// Abstract ancient cave floor generator.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public abstract class AncientCaveMapGenerator
    {
        public const string PROPERTY_FLOOR_NUMBER = "FloorNumber";

        protected static int dialogueEventStart = 0x280;

        // random NPCs to use in AC maps
        protected byte[] npcIds = new byte[]
        {
                // 8A, 8B, 8C = boy/girl/sprite
                0x99, // neko
                0x9C, // watts
                0x9E, // truffle
                0x9F, // walrus guy
                0xA0, // jema
                0xA1, // luka
                0xA4, // kilroy guy
                0xA5, // kilroy guy groupie
                0xA6, // phanna
                0xA7, // krissie
                0xA8, // pirate chick
                0xA9, // ruins guard
                0xAC, // dyluck
                0xAD, // rudolph
                0xAE, // santa
                0xAF, // mushroom dude
                0xB1, // green haired empire dude
                0xB6, // red soldier
                0xB7, // blue soldier
                0xBB, // old dude
                0xBF, // thanatos
                0xC0, // elliot
                0xC1, // timothy
                0xC2, // krissie groupie A
                0xC3, // krissie groupie B
                0xC4, // potos gatekeeper
                0xC5, // pink woman
                0xC6, // green hair woman
                0xC7, // kid with bear hat
                0xC8, // little girl
                0xC9, // old man in hat/potos elder
                0xCA, // old woman/tame elinee
                0xCB, // mara
        };

        public AncientCaveFloor generate(RandoContext context)
        {
            int floorNumber = context.workingData.getInt(PROPERTY_FLOOR_NUMBER);
            AncientCaveFloor ancientCaveFloor = new AncientCaveFloor();
            AncientCaveGenerationContext generationContext = getGenerationContext(context);
            ancientCaveFloor.outsideMap = generateOutdoorMap(context, generationContext);
            // floor number is determined in-game by dividing the 8-bit LSB of the map number by two (see Layer3Changes and EnemyChanges)
            // so all three of these map numbers end up having the same effective floor number.
            // MOPPLE: these map ids are set up in such a way that dividing the 8-bit map id by 2 gives you the floor number
            // it might be better to set an event flag or two to determine the floor number instead
            ancientCaveFloor.outsideMap.mapId = (ushort)(floorNumber * 2);
            ancientCaveFloor.outsideMap.mapName = "Floor " + floorNumber + " outside map";
            ancientCaveFloor.insideMap = generateIndoorMap(context, generationContext);
            ancientCaveFloor.insideMap.mapId = (ushort)(floorNumber * 2 + 1);
            ancientCaveFloor.insideMap.mapName = "Floor " + floorNumber + " inside map";
            ancientCaveFloor.bossMap = makeBossMap(context, generationContext);
            ancientCaveFloor.bossMap.mapId = (ushort)(floorNumber * 2 + 256);
            ancientCaveFloor.bossMap.mapName = "Floor " + floorNumber + " boss map";
            ancientCaveFloor.songEventData = getSongEventData();

            // check that everything that should have been defined on the generated maps actually was
            checkMapDataPopulated(ancientCaveFloor, floorNumber);

            Logging.log("\n\n----------------------------------------------------------------------------------------------------", "spoiler");
            Logging.log("Floor " + (floorNumber + 1) + " (" + getFloorType() + ") map:\n" + getMapLogString(ancientCaveFloor, generationContext), "spoiler");

            context.generatedMaps[ancientCaveFloor.outsideMap.mapId] = ancientCaveFloor.outsideMap.mapData;
            context.generatedMaps[ancientCaveFloor.insideMap.mapId] = ancientCaveFloor.insideMap.mapData;
            context.generatedMaps[ancientCaveFloor.bossMap.mapId] = ancientCaveFloor.bossMap.mapData;

            // create map pieces (floor) * 6, 2 for outdoor, 2 for indoor, 2 for boss
            ancientCaveFloor.outsideMap.mapData.mapPieces = new FullMapMapPieces();
            ancientCaveFloor.outsideMap.mapData.mapPieces.bgPieces.Add(getPieceRef(floorNumber * 6, false));
            ancientCaveFloor.outsideMap.mapData.mapPieces.fgPieces.Add(getPieceRef(floorNumber * 6 + 1, true));
            context.replacementMapPieces[floorNumber * 6] = SomMapCompressor.EncodeMap(ancientCaveFloor.outsideMap.layer1Data).ToList();
            context.replacementMapPieces[floorNumber * 6 + 1] = SomMapCompressor.EncodeMap(ancientCaveFloor.outsideMap.layer2Data).ToList();

            ancientCaveFloor.insideMap.mapData.mapPieces = new FullMapMapPieces();
            ancientCaveFloor.insideMap.mapData.mapPieces.bgPieces.Add(getPieceRef(floorNumber * 6 + 2, false));
            ancientCaveFloor.insideMap.mapData.mapPieces.fgPieces.Add(getPieceRef(floorNumber * 6 + 3, true));
            context.replacementMapPieces[floorNumber * 6 + 2] = SomMapCompressor.EncodeMap(ancientCaveFloor.insideMap.layer1Data).ToList();
            context.replacementMapPieces[floorNumber * 6 + 3] = SomMapCompressor.EncodeMap(ancientCaveFloor.insideMap.layer2Data).ToList();

            ancientCaveFloor.bossMap.mapData.mapPieces = new FullMapMapPieces();
            ancientCaveFloor.bossMap.mapData.mapPieces.bgPieces.Add(getPieceRef(floorNumber * 6 + 4, false));
            ancientCaveFloor.bossMap.mapData.mapPieces.fgPieces.Add(getPieceRef(floorNumber * 6 + 5, true));
            context.replacementMapPieces[floorNumber * 6 + 4] = SomMapCompressor.EncodeMap(ancientCaveFloor.bossMap.layer1Data).ToList();
            context.replacementMapPieces[floorNumber * 6 + 5] = SomMapCompressor.EncodeMap(ancientCaveFloor.bossMap.layer2Data).ToList();

            return ancientCaveFloor;
        }

        private void checkMapDataPopulated(AncientCaveFloor floorData, int floorNumber)
        {
            // (outside map)
            // map exists at all
            ErrorUtil.checkNotNull(floorData.outsideMap, "Outside map not defined for floor " + floorNumber + " with floor type " + getFloorType());
            // able to warp to this map
            ErrorUtil.checkNotNull(floorData.outsideMap.entryPos, "Outside map entry position not defined for floor " + floorNumber + " with floor type " + getFloorType());
            // actual map tile data defined
            ErrorUtil.checkNotNull(floorData.outsideMap.layer1Data, "Outside map layer 1 data not defined for floor " + floorNumber + " with floor type " + getFloorType());
            ErrorUtil.checkNotNull(floorData.outsideMap.layer2Data, "Outside map layer 2 data not defined for floor " + floorNumber + " with floor type " + getFloorType());
            ErrorUtil.checkAnyEntries(floorData.outsideMap.exitLocations, "Outside map exit position not defined for floor " + floorNumber + " with floor type " + getFloorType());
            // map header/misc data defined
            ErrorUtil.checkNotNull(floorData.outsideMap.mapData, "Outside map misc data block not defined for floor " + floorNumber + " with floor type " + getFloorType());
            ErrorUtil.checkNotNull(floorData.outsideMap.mapData.mapHeader, "Outside map header not defined for floor " + floorNumber + " with floor type " + getFloorType());

            // (inside map)
            // map exists at all
            ErrorUtil.checkNotNull(floorData.insideMap, "Inside map not defined for floor " + floorNumber + " with floor type " + getFloorType());
            // actual map tile data defined
            ErrorUtil.checkNotNull(floorData.insideMap.layer1Data, "Inside map layer 1 data not defined for floor " + floorNumber + " with floor type " + getFloorType());
            ErrorUtil.checkNotNull(floorData.insideMap.layer2Data, "Inside map layer 2 data not defined for floor " + floorNumber + " with floor type " + getFloorType());
            // map header/misc data defined
            ErrorUtil.checkNotNull(floorData.insideMap.mapData, "Inside map misc data block not defined for floor " + floorNumber + " with floor type " + getFloorType());
            ErrorUtil.checkNotNull(floorData.insideMap.mapData.mapHeader, "Inside map header not defined for floor " + floorNumber + " with floor type " + getFloorType());

            // (boss map)
            ErrorUtil.checkNotNull(floorData.bossMap, "Boss map not defined for floor " + floorNumber + " with floor type " + getFloorType());
            // able to warp to boss map
            ErrorUtil.checkNotNull(floorData.bossMap.entryPos, "Boss map entry position not defined for floor " + floorNumber + " with floor type " + getFloorType());
            // actual map tile data defined
            ErrorUtil.checkNotNull(floorData.bossMap.layer1Data, "Boss map layer 1 data not defined for floor " + floorNumber + " with floor type " + getFloorType());
            ErrorUtil.checkNotNull(floorData.bossMap.layer2Data, "Boss map layer 2 data not defined for floor " + floorNumber + " with floor type " + getFloorType());
            // map header/misc data defined
            ErrorUtil.checkNotNull(floorData.bossMap.mapData, "Boss map misc data block not defined for floor " + floorNumber + " with floor type " + getFloorType());
            ErrorUtil.checkNotNull(floorData.bossMap.mapData.mapHeader, "Boss map header not defined for floor " + floorNumber + " with floor type " + getFloorType());
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

        public GeneratedMap makeBossMap(RandoContext context, AncientCaveGenerationContext generationContext)
        {
            if ((context.randomFunctional.Next() % 2) == 0)
            {
                // use floor-themed boss map
                return generateBossMap(context, generationContext);
            }
            else
            {
                // use vanilla boss map
                List<byte> possibleBossIds = new List<byte>();
                possibleBossIds.AddRange(VanillaBossMaps.BY_VANILLA_BOSS_ID.Keys);
                // for AC, don't use triple tonpole.
                possibleBossIds.Remove(VanillaBossMap.TRIPLE_TONPOLE_OBJECT_INDICATOR);
                byte bossId = possibleBossIds[context.randomFunctional.Next() % possibleBossIds.Count];
                GeneratedMap vanillaBossMap = GeneratedMapUtil.importVanillaBossArena(context, bossId);
                // clear any walkon flag from vanilla so not to execute random events that aren't present anymore
                vanillaBossMap.mapData.mapHeader.setWalkonEventEnabled(false);
                return vanillaBossMap;
            }
        }

        protected abstract AncientCaveGenerationContext getGenerationContext(RandoContext context);
        protected abstract AncientCaveMap generateOutdoorMap(RandoContext context, AncientCaveGenerationContext generationContext);
        protected abstract AncientCaveMap generateIndoorMap(RandoContext context, AncientCaveGenerationContext generationContext);
        protected abstract AncientCaveMap generateBossMap(RandoContext context, AncientCaveGenerationContext generationContext);
        protected abstract byte[] getSongEventData();
        protected abstract string getMapLogString(AncientCaveFloor floorData, AncientCaveGenerationContext generationContext);
        protected abstract string getFloorType();

        protected static void applyPaletteSource(RandoContext context, AncientCaveMap map, PaletteSetSource palSource)
        {
            MapPaletteSet paletteSet = palSource.getPaletteData(context);
            int palIndex = context.workingData.getIntAndIncrement(GeneratedMapUtil.PROPERTY_GENERATED_PALETTE_INDEX);
            AnimatedPaletteSimplification.copyPaletteAnimationSettings(context, palSource.getVanillaPaletteSet(), palIndex);
            context.replacementMapPalettes[palIndex] = paletteSet;
            map.mapData.mapHeader.setPaletteSet((byte)palIndex);
        }
    }

    public class AncientCaveGenerationContext
    {
        // map sizes, in 16x16 tiles
        public int outdoorMapWidth;
        public int outdoorMapHeight;
        public int indoorMapWidth;
        public int indoorMapHeight;
        public int bossMapWidth;
        public int bossMapHeight;
        // nodes representing connections between the indoor and outdoor map that need to be generated before either map
        public List<ProcGenMapNode> sharedNodes = new List<ProcGenMapNode>();
        public ProcGenMapNode getSharedNode(string name)
        {
            return sharedNodes.Find(node => node.name == name);
        }
        public Dictionary<string, ProcGenMapNode> getSharedNodesByName()
        {
            Dictionary<string, ProcGenMapNode> nodesByName = new Dictionary<string, ProcGenMapNode>();
            sharedNodes.ForEach(node => { if (node.name != null) nodesByName[node.name] = node; });
            return nodesByName;
        }
        // other named nodes used for map processing
        public Dictionary<string, ProcGenMapNode> specialNodes = new Dictionary<string, ProcGenMapNode>();
        // other settings shared between map generators
        public StringValueSettings sharedSettings = new StringValueSettings();
    }

    // indoor or outdoor floor map of an ancient cave floor
    public class AncientCaveMap : GeneratedMap
    {
        // this map's door location -> same map's door ids
        public Dictionary<XyPos, int> sameMapExitLocations = new Dictionary<XyPos, int>();
        // this map's door ids -> place to warp to in this map from the same map
        public Dictionary<int, XyPos> sameMapEntryLocations = new Dictionary<int, XyPos>();
        // this map's door location -> other map's door ids
        public Dictionary<XyPos, int> altMapExitLocations = new Dictionary<XyPos, int>();
        // this map's door ids -> place to warp to in this map from the other map
        public Dictionary<int, XyPos> altMapEntryLocations = new Dictionary<int, XyPos>();
        // this map's door ids that don't have an assigned door because they use SoM's return door tiles
        public List<int> returnDoors = new List<int>();
    }

    // all the info needed to construct one floor of ancient cave
    public class AncientCaveFloor
    {
        public AncientCaveMap outsideMap;
        public AncientCaveMap insideMap;
        public GeneratedMap bossMap;
        // from previous floor
        public XyPos floorEntryLocation;
        // to next floor
        public XyPos nextFloorDoorLocation;
        // to play when entering floor
        public byte[] songEventData;
    }
}
