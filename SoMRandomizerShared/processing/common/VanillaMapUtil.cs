using SoMRandomizer.processing.common.structure;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using static SoMRandomizer.processing.common.VanillaRomOffsets;
using static SoMRandomizer.util.DataUtil;

namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// Utilities for loading and manipulating vanilla composite/full maps.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class VanillaMapUtil
    {
        // get all objects on a vanilla map
        public static List<MapObject> getObjects(byte[] rom, int fullMapId)
        {
            int startOffset = getObjectOffset(rom, fullMapId) + 8; // skip header, which is always 8-byte object 0
            int endOffset = getObjectOffset(rom, fullMapId + 1);
            List<MapObject> objects = new List<MapObject>();
            for (int i = startOffset; i < endOffset; i += 8)
            {
                byte[] objectData = new byte[8];
                Array.Copy(rom, i, objectData, 0, 8);
                objects.Add(new MapObject(objectData));
            }
            return objects;
        }

        // overwrite an existing object
        public static void putObject(byte[] rom, MapObject obj, int fullMapId, int objIndex)
        {
            int startOffset = getObjectOffset(rom, fullMapId) + 8; // + 8 to skip header
            int objOffset = startOffset + objIndex * 8;
            obj.put(rom, objOffset);
        }

        // get all triggers on a vanilla map, ordered left to right, top to bottom, and sometimes preceded by a couple special triggers
        // as indicated by the map header
        public static List<ushort> getTriggers(byte[] rom, int fullMapId)
        {
            // get the list of triggers for the map in order:
            // - walk-on trigger (if enabled in map header)
            // - special event trigger (if enabled in map header)
            // - all trigger tile values for the map, left to right, top to bottom, as assigned by collision values of tiles on the map
            // values are 0x000-0x7FF to indicate an event, 0x800+ to indicate a door (0x800 == door 0x000)
            int startOffset = getTriggerOffset(rom, fullMapId);
            int endOffset = getTriggerOffset(rom, fullMapId + 1);
            List<ushort> values = new List<ushort>();
            for (int i = startOffset; i < endOffset; i += 2)
            {
                values.Add(DataUtil.ushortFromBytes(rom, i));
            }
            return values;
        }

        // get all map pieces used to form the map
        public static FullMapMapPieces getMapPieceReference(byte[] rom, int fullMapId)
        {
            // map 16 example 0x85468 
            // FE             - BG indicator
            // 00 0F 16 00 00 - BG piece
            // ^^               event flag for visibility
            //    ^^            event flag values for visibility
            //       ^^         map piece id LSB (8 bits)
            //          ^^      x pos (first 7 bits; LSB is the 0x100 bit of the map piece id)
            //             ^^   y pos (first 7 bits; LSB is the 0x200 bit of the map piece id)
            // FF             - FG indicator
            // 00 0F 17 00 00 - FG piece
            // vanilla maps are expected to have all FE pieces followed by all FF pieces
            int startOffset = getPiecePlacementOffset(rom, fullMapId);
            int endOffset = getPiecePlacementOffset(rom, fullMapId + 1);
            bool isFg = false;
            int offset = startOffset;
            int pieceCounter = 0;
            FullMapMapPieces pieces = new FullMapMapPieces();
            while (offset < endOffset)
            {
                if (rom[offset] == 0xFE)
                {
                    // start of background block
                    offset++;
                    pieceCounter = 0;
                    isFg = false;
                }
                else if (rom[offset] == 0xFF)
                {
                    offset++;
                    pieceCounter = 0;
                    isFg = true;
                }
                else
                {
                    FullMapMapPieceReference piece = new FullMapMapPieceReference();
                    piece.eventVisibilityFlagId = rom[offset + 0];
                    piece.eventVisibilityValueLow = (byte)(rom[offset + 1] >> 4);
                    piece.eventVisibilityValueHigh = (byte)(rom[offset + 1] & 0x0F);
                    piece.isForegroundPiece = isFg;
                    piece.indexOnMap = pieceCounter;
                    piece.xPos = (byte)(rom[offset + 3] >> 1);
                    piece.yPos = (byte)(rom[offset + 4] >> 1);
                    piece.pieceIndex = rom[offset + 2];
                    if((rom[offset + 3] & 0x01) > 0)
                    {
                        // bit 0x100 for map index is the LSB of the x position byte
                        piece.pieceIndex += 256;
                    }
                    if ((rom[offset + 4] & 0x01) > 0)
                    {
                        // bit 0x200 for map index is the LSB of the x position byte
                        piece.pieceIndex += 512;
                    }
                    pieceCounter++;
                    if(isFg)
                    {
                        pieces.fgPieces.Add(piece);
                    }
                    else
                    {
                        pieces.bgPieces.Add(piece);
                    }
                    offset += 5;
                }
            }
            return pieces;
        }

        // FullMapMapPieces -> byte array representation for rom
        public static byte[] makeMapPieceReference(FullMapMapPieces mapPieces)
        {
            List<byte> outputBytes = new List<byte>();
            bool anyBg = false;
            // process BG blocks
            foreach (FullMapMapPieceReference pieceRef in mapPieces.bgPieces)
            {
                if (!anyBg)
                {
                    outputBytes.Add(0xFE);
                    anyBg = true;
                }
                outputBytes.Add(pieceRef.eventVisibilityFlagId);
                outputBytes.Add((byte)(pieceRef.eventVisibilityValueLow << 4 | pieceRef.eventVisibilityValueHigh));
                outputBytes.Add((byte)pieceRef.pieceIndex);
                outputBytes.Add((byte)((pieceRef.xPos << 1) | (((pieceRef.pieceIndex & 0x100) > 0) ? 1 : 0)));
                outputBytes.Add((byte)((pieceRef.yPos << 1) | (((pieceRef.pieceIndex & 0x200) > 0) ? 1 : 0)));
            }
            // process FG blocks
            bool anyFg = false;
            foreach (FullMapMapPieceReference pieceRef in mapPieces.fgPieces)
            {
                if (!anyFg)
                {
                    outputBytes.Add(0xFF);
                    anyFg = true;
                }
                outputBytes.Add(pieceRef.eventVisibilityFlagId);
                outputBytes.Add((byte)(pieceRef.eventVisibilityValueLow << 4 | pieceRef.eventVisibilityValueHigh));
                outputBytes.Add((byte)pieceRef.pieceIndex);
                outputBytes.Add((byte)((pieceRef.xPos << 1) | (((pieceRef.pieceIndex & 0x100) > 0) ? 1 : 0)));
                outputBytes.Add((byte)((pieceRef.yPos << 1) | (((pieceRef.pieceIndex & 0x200) > 0) ? 1 : 0)));
            }
            return outputBytes.ToArray();
        }

        // inject source map piece into dest map piece at specified position
        public static void copyMapPieceData(byte[,] source, byte[,] dest, int destX, int destY)
        {
            for(int y=0; y <  source.GetLength(1); y++)
            {
                for (int x = 0; x < source.GetLength(0); x++)
                {
                    byte srcData = source[x, y];
                    int dataDestX = x + destX;
                    int dataDestY = y + destY;
                    if(dataDestX >= 0 && dataDestX < dest.GetLength(0) && dataDestY >= 0 && dataDestY < dest.GetLength(1))
                    {
                        dest[dataDestX, dataDestY] = srcData;
                    }
                }
            }
        }

        // get the map header for the given vanilla map, with various flags about how the game renders / handles the map
        // note that this is stored as just the first "object" in the map's object list
        public static MapHeader getHeader(byte[] rom, int mapNum)
        {
            try
            {
                byte[] header = new byte[8];
                Array.Copy(rom, getObjectOffset(rom, mapNum), header, 0, 8);
                return new MapHeader(header);
            }
            catch(Exception e)
            {
                throw ErrorUtil.appendExceptionInfo("Unable to get header for map " + mapNum, e);
            }
        }

        // write a modified header back
        public static void putHeader(byte[] rom, MapHeader header, int mapNum)
        {
            header.put(rom, getObjectOffset(rom, mapNum));
        }

        // determine rom offset for specified map's trigger collection data
        public static int getTriggerOffset(byte[] rom, int mapNum)
        {
            // 85000 - 16-bit pointers to map piece lists for each composite map
            return getBankStart(MAP_TRIGGER_OFFSETS) + rom[MAP_TRIGGER_OFFSETS + mapNum * 2] + (rom[MAP_TRIGGER_OFFSETS + mapNum * 2 + 1] << 8);
        }

        // determine rom offset for specified map's map piece collection data
        public static int getPiecePlacementOffset(byte[] rom, int mapNum)
        {
            // 85000 - 16-bit pointers to map piece lists for each composite map
            return getBankStart(MAP_PIECE_REFERENCE_OFFSETS) + rom[MAP_PIECE_REFERENCE_OFFSETS + mapNum * 2] + (rom[MAP_PIECE_REFERENCE_OFFSETS + mapNum * 2 + 1] << 8);
        }

        // determine rom offset for specified map's header/object collection data
        public static int getObjectOffset(byte[] rom, int mapNum)
        {
            // 87000 - 16-bit pointers to object lists for each composite map; the first "object" is an 8-byte header
            // with info like tilesets, palettes, and some flags like whether you can flammie
            return getBankStart(MAP_OBJECT_OFFSETS) + rom[MAP_OBJECT_OFFSETS + mapNum * 2] + (rom[MAP_OBJECT_OFFSETS + mapNum * 2 + 1] << 8);
        }

        // for each tileset, the tiles we need to replace to remove all doors to make a boss map inescapable
        private static Dictionary<byte, TupleList<byte,byte>> replacementDoorTileIndexes = new Dictionary<byte, TupleList<byte, byte>>
        {
            // tileset -> door tile, replacement floor tile
            {1, new TupleList<byte, byte> { // map 60; spring beak
                { 0xB2, 0x94 },
            } },

            {9, new TupleList<byte, byte> { // map 407; doom's wall
                { 0xB1, 0xB0 },
                { 0xBD, 0x5E },
            } },

            {10, new TupleList<byte, byte> { // map 277; tonpole
                { 0x77, 0x00 },
                { 0x64, 0x00 },
                { 0x74, 0x00 },
            } },

            {14, new TupleList<byte, byte> { // map 310; spikey, map 377; metal mantis
                { 0x66, 0x0F },
            } },

            {16, new TupleList<byte, byte> { // map 325; snake
                { 0x35, 0x12 },
                { 0x4A, 0x49 },
            } },

            {17, new TupleList<byte, byte> { // map 232; blue dragon
                { 0x18, 0x5D },
                { 0x7C, 0x5D },
            } },

            {19, new TupleList<byte, byte> { // map 317; kilroy
                { 0xAF, 0xAE },
                { 0x89, 0x00 },
            } },

            {21, new TupleList<byte, byte> { // map 237; MR3 (buttons)
                { 0x36, 0x27 },
                { 0x3D, 0x27 },
            } },

            {23, new TupleList<byte, byte> { // map 143; jabber (buttons), map 119; lumina bosses, map 430; snap dragon
                { 0x80, 0x43 },
                { 0x81, 0x43 },
                { 0xBF, 0xBA },
                { 0x97, 0x36 },
                { 0xAA, 0x00 },
            } },

            {26, new TupleList<byte, byte> { // map 46; kettlekin
                { 0x4D, 0x00 },
            } },

            {27, new TupleList<byte, byte> { // map 434; hydra
                { 0xBD, 0x00 },
                { 0xBA, 0xA2 },
            } },

            {30, new TupleList<byte, byte> { // map 230; snow dragon
                { 0x5D, 0x54 },
            } },

        };

        // create a vanilla boss arena intended to teleport you out by boss death event and not have any doors out
        public static byte[,] loadVanillaMapPieceAndRemoveAllDoors(byte[] rom, int pieceIndex, int compositeMapIndex)
        {
            try
            {
                byte[,] decompressedMapData = loadVanillaMapPiece(rom, pieceIndex);
                byte tileset16 = getHeader(rom, compositeMapIndex).getTileset16();
                if (replacementDoorTileIndexes.ContainsKey(tileset16))
                {
                    foreach (Tuple<byte, byte> valuePair in replacementDoorTileIndexes[tileset16])
                    {
                        byte doorValue = valuePair.Item1;
                        byte doorReplacementValue = valuePair.Item2;
                        for (int y = 0; y < decompressedMapData.GetLength(1); y++)
                        {
                            for (int x = 0; x < decompressedMapData.GetLength(0); x++)
                            {
                                if (decompressedMapData[x, y] == doorValue)
                                {
                                    decompressedMapData[x, y] = doorReplacementValue;
                                }
                            }
                        }
                    }
                }
                return decompressedMapData;
            }
            catch (Exception e)
            {
                throw ErrorUtil.appendExceptionInfo("Unable to load filtered vanilla map piece " + pieceIndex + " from composite map " + compositeMapIndex, e);
            }
        }

        // decompress a specified vanilla map piece and return it as a 2d byte array
        public static byte[,] loadVanillaMapPiece(byte[] rom, int pieceIndex)
        {
            // note that numbers displayed by the editor are off by two!
            // if using the editor as a reference, subtract 2 from those numbers when calling this.
            try
            {
                // vanilla map pieces:
                // 0xD0000: id where we start reading from bank 0xE0000 (0xA3)
                // 0xD0002: id where we start reading from bank 0xF0000 (0x206)
                // 0xD0004: id where we start reading from bank 0xC0000 (0x300)
                // 0xD0006: 16-bit offset for map 0 (0xD0804)
                // 0x3FF total 16-bit offsets, except the vanilla loader starts at 0xD0004, so map 0 is just a non-map
                // 0x85000 has 16-bit offsets to map piece lists for each composite map
                // so for example, composite map 310 (0x136) is spikey tiger, and has layer1 map piece 201 (0xC9) (as referenced near 0x87B67)
                // 0xD0004 + 0xC9 * 2 = 0xD0196 => 1C98.  This is in bank 0xE0000 since it's >= 0xA3, so this piece is at 0xE1C98
                // MapPieceExpander also has similar code to calculate this
                int pieceOffset = getMapPieceOffset(rom, pieceIndex);
                int nextPieceOffset = getMapPieceOffset(rom, pieceIndex + 1);
                if ((nextPieceOffset & 0xFF0000) != (pieceOffset & 0xFF0000))
                {
                    // different banks; just use the end of the bank
                    nextPieceOffset = (pieceOffset & 0xFF0000) + 0x10000;
                }
                byte[] compressedMapData = new byte[nextPieceOffset - pieceOffset];
                Array.Copy(rom, pieceOffset, compressedMapData, 0, compressedMapData.Length);
                return SomMapCompressor.DecodeMap(compressedMapData);
            }
            catch(Exception e)
            {
                throw ErrorUtil.appendExceptionInfo("Unable to load vanilla map piece " + pieceIndex, e);
            }
        }

        // get the offset of the specified vanilla map piece, which can come out of one of four different banks based on the index.
        public static int getMapPieceOffset(byte[] rom, int pieceIndex)
        {
            byte o1 = rom[MAP_PIECE_OFFSETS + pieceIndex * 2];
            byte o2 = rom[MAP_PIECE_OFFSETS + pieceIndex * 2 + 1];
            int bank = 0x0D;
            if (pieceIndex >= 0xA3 && pieceIndex < 0x206)
            {
                bank = 0x0E;
            }
            else if (pieceIndex >= 0x206 && pieceIndex < 0x300)
            {
                bank = 0x0F;
            }
            else if(pieceIndex >= 0x300)
            {
                bank = 0x0C;
            }
            return ((bank << 16) + (o2 << 8) + o1);
        }
    }
}
