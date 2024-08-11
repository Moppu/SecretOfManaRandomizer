using SoMRandomizer.processing.common.structure;
using System.Linq;

namespace SoMRandomizer.processing.common.mapgen
{
    /// <summary>
    /// Utility used to copy the vanilla mana beast fight map.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ManaBeastMap
    {
        public const string MANABEAST_DEATH_EVENT = "ManaBeastDeathEvent";
        public const string MANABEAST_WALKON_EVENT = "ManaBeastWalkonEvent";

        // create a copy of the mana beast map (vanilla 0xFD) for generated-map modes
        public static void makeManaBeastMap(byte[] origRom, RandoContext context)
        {
            int manaBeastDeathEvent = context.workingData.getInt(MANABEAST_DEATH_EVENT);
            int manaBeastEntryEvent = context.workingData.getInt(MANABEAST_WALKON_EVENT);
            MapHeader manaBeastHeader = VanillaMapUtil.getHeader(origRom, 0xFD);
            FullMap manaBeastMap = new FullMap();
            manaBeastMap.mapHeader = manaBeastHeader;
            MapObject manaBeastObj = new MapObject();
            manaBeastObj.setDirection(MapObject.DIR_DOWN);
            manaBeastObj.setXpos(18);
            manaBeastObj.setYpos(18);
            manaBeastObj.setSpecies(0x7F);
            manaBeastObj.setEvent((ushort)manaBeastDeathEvent);
            manaBeastObj.setEventVisFlag(0x00);
            manaBeastObj.setEventVisMinimum(0x00);
            manaBeastObj.setEventVisMaximum(0x0F);

            manaBeastMap.mapObjects.Add(manaBeastObj);
            // a single layer 1 piece for collision, with no graphics - vanilla 753 (755 in editor)
            byte[,] manaBeastMapPiece = VanillaMapUtil.loadVanillaMapPiece(origRom, 753);
            context.replacementMapPieces[753] = SomMapCompressor.EncodeMap(manaBeastMapPiece).ToList();
            manaBeastMap.mapPieces = new FullMapMapPieces();
            FullMapMapPieceReference manaBeastLayer1 = new FullMapMapPieceReference();
            manaBeastLayer1.eventVisibilityFlagId = 0x00;
            manaBeastLayer1.eventVisibilityValueLow = 0x00;
            manaBeastLayer1.eventVisibilityValueHigh = 0x0F;
            manaBeastLayer1.pieceIndex = 753;
            manaBeastMap.mapPieces.bgPieces.Add(manaBeastLayer1);
            if (manaBeastEntryEvent >= 0)
            {
                // header is flagged for entry event; add this to run when map is loaded
                manaBeastHeader.setWalkonEventEnabled(true);
                manaBeastMap.mapTriggers.Add((ushort)manaBeastEntryEvent);
            }
            context.generatedMaps[0xFD] = manaBeastMap;

        }
    }
}
