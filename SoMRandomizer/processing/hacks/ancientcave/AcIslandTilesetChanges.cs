using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System.Collections.Generic;
using SoMRandomizer.processing.common.structure;

namespace SoMRandomizer.processing.hacks.ancientcave
{
    /// <summary>
    /// Hack to make some adjustments to the look of the island tileset and allow entering the water for ancient cave rando.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class AcIslandTilesetChanges : RandoProcessor
    {
        protected override string getName()
        {
            return "Changes to island tileset for ancient cave";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            int tilesetNum = 5;
            byte[] tilesetRaw = VanillaTilesetUtil.getCompressedVanillaTileset16(origRom, tilesetNum);
            Tileset16 tilesetDecomp = VanillaTilesetUtil.DecodeTileset16(tilesetRaw);

            // all numbers * 4
            // on layer2 (+192) 111, 119-127, 135-143, 159, 174-175, 186-191
            int[] bgChangeIndexesLayer2 = new int[] { 111, 111, 119, 127, 135, 143, 159, 159, 174, 175, 186, 191 };
            for (int i = 0; i < bgChangeIndexesLayer2.Length; i += 2)
            {
                for (int ii = bgChangeIndexesLayer2[i]; ii <= bgChangeIndexesLayer2[i + 1]; ii++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        // FG layer
                        tilesetDecomp[192 + ii][j].AlternateRenderLayer = true;
                    }
                }
            }


            // huts on layer 1
            // palette 4
            // 0x4000 for hflip
            tilesetDecomp[118][0] = new Tile8(tileNum: 863, palette: 4);
            tilesetDecomp[118][1] = new Tile8(tileNum: 48, palette: 4);
            tilesetDecomp[118][2] = new Tile8(tileNum: 863, palette: 4);
            tilesetDecomp[118][3] = new Tile8(tileNum: 64, palette: 4);

            tilesetDecomp[119][0] = new Tile8(tileNum: 863, palette: 4);
            tilesetDecomp[119][1] = new Tile8(tileNum: 863, palette: 4);
            tilesetDecomp[119][2] = new Tile8(tileNum: 863, palette: 4);
            tilesetDecomp[119][3] = new Tile8(tileNum: 2, palette: 4);

            tilesetDecomp[120][0] = new Tile8(tileNum: 863, palette: 4);
            tilesetDecomp[120][1] = new Tile8(tileNum: 863, palette: 4);
            tilesetDecomp[120][2] = new Tile8(tileNum: 3, palette: 4);
            tilesetDecomp[120][3] = new Tile8(tileNum: 3, palette: 4, horizontalFlip: true);

            tilesetDecomp[121][0] = new Tile8(tileNum: 863, palette: 4);
            tilesetDecomp[121][1] = new Tile8(tileNum: 863, palette: 4);
            tilesetDecomp[121][2] = new Tile8(tileNum: 2, palette: 4, horizontalFlip: true);
            tilesetDecomp[121][3] = new Tile8(tileNum: 863, palette: 4);

            tilesetDecomp[122][0] = new Tile8(tileNum: 48, palette: 4, horizontalFlip: true);
            tilesetDecomp[122][1] = new Tile8(tileNum: 863, palette: 4);
            tilesetDecomp[122][2] = new Tile8(tileNum: 64, palette: 4, horizontalFlip: true);
            tilesetDecomp[122][3] = new Tile8(tileNum: 863, palette: 4);


            tilesetDecomp[134][0] = new Tile8(tileNum: 863, palette: 4);
            tilesetDecomp[134][1] = new Tile8(tileNum: 863, palette: 4);
            tilesetDecomp[134][2] = new Tile8(tileNum: 863, palette: 4);
            tilesetDecomp[134][3] = new Tile8(tileNum: 32, palette: 4);

            tilesetDecomp[135][0] = new Tile8(tileNum: 17, palette: 4);
            tilesetDecomp[135][1] = new Tile8(tileNum: 18, palette: 4);
            tilesetDecomp[135][2] = new Tile8(tileNum: 33, palette: 4);
            tilesetDecomp[135][3] = new Tile8(tileNum: 34, palette: 4);

            tilesetDecomp[136][0] = new Tile8(tileNum: 19, palette: 4);
            tilesetDecomp[136][1] = new Tile8(tileNum: 19, palette: 4, horizontalFlip: true);
            tilesetDecomp[136][2] = new Tile8(tileNum: 35, palette: 4);
            tilesetDecomp[136][3] = new Tile8(tileNum: 35, palette: 4, horizontalFlip: true);

            tilesetDecomp[137][0] = new Tile8(tileNum: 18, palette: 4, horizontalFlip: true);
            tilesetDecomp[137][1] = new Tile8(tileNum: 17, palette: 4, horizontalFlip: true);
            tilesetDecomp[137][2] = new Tile8(tileNum: 34, palette: 4, horizontalFlip: true);
            tilesetDecomp[137][3] = new Tile8(tileNum: 33, palette: 4, horizontalFlip: true);

            tilesetDecomp[138][0] = new Tile8(tileNum: 863, palette: 4);
            tilesetDecomp[138][1] = new Tile8(tileNum: 863, palette: 4);
            tilesetDecomp[138][2] = new Tile8(tileNum: 32, palette: 4, horizontalFlip: true);
            tilesetDecomp[138][3] = new Tile8(tileNum: 863, palette: 4);

            // grass on layer 2
            tilesetDecomp[192 + 77][0] = new Tile8(tileNum: 512, palette: 2);
            tilesetDecomp[192 + 77][1] = new Tile8(tileNum: 513, palette: 2);
            tilesetDecomp[192 + 77][2] = new Tile8(tileNum: 528, palette: 2);
            tilesetDecomp[192 + 77][3] = new Tile8(tileNum: 529, palette: 2);

            // fire on layer1
            tilesetDecomp[143][0] = new Tile8(tileNum: 874, palette: 6);
            tilesetDecomp[143][1] = new Tile8(tileNum: 875, palette: 6);
            tilesetDecomp[143][2] = new Tile8(tileNum: 876, palette: 6);
            tilesetDecomp[143][3] = new Tile8(tileNum: 877, palette: 6);

            // write tileset back to a new spot
            List<byte> tilesetCompressed = VanillaTilesetUtil.EncodeTileset16(tilesetDecomp);
            int tilesetLocation = context.workingOffset;
            for (int i = 0; i < tilesetCompressed.Count; i++)
            {
                outRom[context.workingOffset++] = tilesetCompressed[i];
            }

            // no Cx for some reason here
            DataUtil.int24ToBytes(outRom, VanillaRomOffsets.TILESET16_OFFSETS + tilesetNum * 3, tilesetLocation);

            // now modify some collisions

            // tileset 5 - make 171 passable
            // b0400, x180 bytes apiece (layer1, layer2)

            // MOPPLE: these collision values should be documented or put into a structure, this is nonsense to try to understand again

            // no collide + shallow water on top
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 171] = 0x4a;
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 144] = 0x4a;

            // no collide on hut number indicators
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 39] = 0;

            // steps instead of solid island walls
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 118] = 0;
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 119] = 0;
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 120] = 0;
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 121] = 0;
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 122] = 0;
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 134] = 0;
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 135] = 0x21;
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 137] = 0x1E;
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 138] = 0;

            // vertical steps
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 153] = 0x19;
            outRom[VanillaRomOffsets.COLLISION_TABLE_START + tilesetNum * 0x180 + 156] = 0x19;

            return true;
        }
    }
}
