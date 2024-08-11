using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System.Collections.Generic;

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
            List<short> tilesetDecomp = VanillaTilesetUtil.DecodeTileset16(tilesetRaw);

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
                        tilesetDecomp[192 * 4 + ii * 4 + j] |= 0x2000;
                    }
                }
            }

            // MOPPLE: it might be nice to have a structure for what's represented by these Tileset16 values, instead
            // of just shoving bits onto it here (16 bit VHAPPPTTTTTTTTT)

            // huts on layer 1
            // palette 4
            // 0x4000 for hflip
            tilesetDecomp[118 * 4] = 863 + (4 << 10);
            tilesetDecomp[118 * 4 + 1] = 48 + (4 << 10);
            tilesetDecomp[118 * 4 + 2] = 863 + (4 << 10);
            tilesetDecomp[118 * 4 + 3] = 64 + (4 << 10);

            tilesetDecomp[119 * 4] = 863 + (4 << 10);
            tilesetDecomp[119 * 4 + 1] = 863 + (4 << 10);
            tilesetDecomp[119 * 4 + 2] = 863 + (4 << 10);
            tilesetDecomp[119 * 4 + 3] = 2 + (4 << 10);

            tilesetDecomp[120 * 4] = 863 + (4 << 10);
            tilesetDecomp[120 * 4 + 1] = 863 + (4 << 10);
            tilesetDecomp[120 * 4 + 2] = 3 + (4 << 10);
            tilesetDecomp[120 * 4 + 3] = (3 + (4 << 10)) | 0x4000;

            tilesetDecomp[121 * 4] = 863 + (4 << 10);
            tilesetDecomp[121 * 4 + 1] = 863 + (4 << 10);
            tilesetDecomp[121 * 4 + 2] = (2 + (4 << 10)) | 0x4000;
            tilesetDecomp[121 * 4 + 3] = 863 + (4 << 10);

            tilesetDecomp[122 * 4] = (48 + (4 << 10)) | 0x4000;
            tilesetDecomp[122 * 4 + 1] = 863 + (4 << 10);
            tilesetDecomp[122 * 4 + 2] = (64 + (4 << 10)) | 0x4000;
            tilesetDecomp[122 * 4 + 3] = 863 + (4 << 10);


            tilesetDecomp[134 * 4] = 863 + (4 << 10);
            tilesetDecomp[134 * 4 + 1] = 863 + (4 << 10);
            tilesetDecomp[134 * 4 + 2] = 863 + (4 << 10);
            tilesetDecomp[134 * 4 + 3] = 32 + (4 << 10);

            tilesetDecomp[135 * 4] = 17 + (4 << 10);
            tilesetDecomp[135 * 4 + 1] = 18 + (4 << 10);
            tilesetDecomp[135 * 4 + 2] = 33 + (4 << 10);
            tilesetDecomp[135 * 4 + 3] = 34 + (4 << 10);

            tilesetDecomp[136 * 4] = 19 + (4 << 10);
            tilesetDecomp[136 * 4 + 1] = (19 + (4 << 10)) | 0x4000;
            tilesetDecomp[136 * 4 + 2] = 35 + (4 << 10);
            tilesetDecomp[136 * 4 + 3] = (35 + (4 << 10)) | 0x4000;

            tilesetDecomp[137 * 4] = (18 + (4 << 10)) | 0x4000;
            tilesetDecomp[137 * 4 + 1] = (17 + (4 << 10)) | 0x4000;
            tilesetDecomp[137 * 4 + 2] = (34 + (4 << 10)) | 0x4000;
            tilesetDecomp[137 * 4 + 3] = (33 + (4 << 10)) | 0x4000;

            tilesetDecomp[138 * 4] = 863 + (4 << 10);
            tilesetDecomp[138 * 4 + 1] = 863 + (4 << 10);
            tilesetDecomp[138 * 4 + 2] = (32 + (4 << 10)) | 0x4000;
            tilesetDecomp[138 * 4 + 3] = 863 + (4 << 10);

            // grass on layer 2
            tilesetDecomp[192 * 4 + 77 * 4] = 512 + (2 << 10);
            tilesetDecomp[192 * 4 + 77 * 4 + 1] = 513 + (2 << 10);
            tilesetDecomp[192 * 4 + 77 * 4 + 2] = 528 + (2 << 10);
            tilesetDecomp[192 * 4 + 77 * 4 + 3] = 529 + (2 << 10);

            // fire on layer1
            tilesetDecomp[143 * 4] = 874 + (6 << 10);
            tilesetDecomp[143 * 4 + 1] = 875 + (6 << 10);
            tilesetDecomp[143 * 4 + 2] = 876 + (6 << 10);
            tilesetDecomp[143 * 4 + 3] = 877 + (6 << 10);

            // write tileset back to a new spot
            List<byte> tilesetCompressed = VanillaTilesetUtil.EncodeTileset16(tilesetDecomp.ToArray());
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
