using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that re-writes how map palette animations are handled to make them easier to reconfigure for rando.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class AnimatedPaletteSimplification : RandoProcessor
    {
        private const string PROPERTY_UPDATED_OFFSET = "AnimatedPaletteOffset";

        /*
$02/BCA7 BF 03 08 C8 LDA $C80803,x[$C8:0882] A:4210 X:007F Y:0000 P:envMXdIZC
$02/BCAB 85 10       STA $10    [$00:0010]   A:428A X:007F Y:0000 P:eNvMXdIzC
$02/BCAD 64 11       STZ $11    [$00:0011]   A:428A X:007F Y:0000 P:eNvMXdIzC
$02/BCAF BF 02 08 C8 LDA $C80802,x[$C8:0881] A:428A X:007F Y:0000 P:eNvMXdIzC

        change to multiply x by two, then use a new location
        also copy the existing data there first
        dump the offset where this went to into workingData

        TXA
        ASL
        TAX
        LDA tableOffset+1,x
        STA $10
        STZ $11
        LDA tableOffset,x

        later:
        TAX
        $02/BCBF BF 7B 08 C8 LDA $C8087B,x[$C8:08D0] A:D300 X:0055 Y:0000 P:eNvMxdIzC
        and palette animation data is loaded until we hit the offset in $10

        note that x is modified after this; we don't need to restore it here
         */
        protected override string getName()
        {
            return "Simplify lookup for animated map palettes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // modify the table of map palette animation data to not share start/end for sequential offsets, and as such not need
            // to be defined sequentially, so custom palette sets can simply reference existing palette animation data blocks from
            // the vanilla palettes they were copied from.
            // in vanilla, 8-bit offsets for these start at 0x80802 and are relative to 0x8087b, and the data runs from the offset 
            // to the next palette set's offset.
            // this hack moves the 0x80802 table to a new section and adds a distinct start/end for each palette set. these still
            // point into the 0x8087b block, which should not need to change.

            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 300);
            // tell other hacks where to find the table if they want to change animation settings
            context.workingData.setInt(PROPERTY_UPDATED_OFFSET, context.workingOffset);
            // x77 of these
            // copy originals to new spot
            int newTableOffset = context.workingOffset + 0xC00000;
            int newTableOffsetP1 = context.workingOffset + 0xC00000 + 1;
            for (int i=0; i < 0x78; i++)
            {
                outRom[context.workingOffset++] = origRom[0x80802 + i];
                outRom[context.workingOffset++] = origRom[0x80802 + i + 1];
            }

            // add code to read from the modified table
            outRom[0x2BCA7] = 0x22;
            outRom[0x2BCA8] = (byte)(context.workingOffset);
            outRom[0x2BCA9] = (byte)(context.workingOffset >> 8);
            outRom[0x2BCAA] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x2BCAB] = 0xEA;
            outRom[0x2BCAC] = 0xEA;
            outRom[0x2BCAD] = 0xEA;
            outRom[0x2BCAE] = 0xEA;
            outRom[0x2BCAF] = 0xEA;
            outRom[0x2BCB0] = 0xEA;
            outRom[0x2BCB1] = 0xEA;
            outRom[0x2BCB2] = 0xEA;

            // TXA
            outRom[context.workingOffset++] = 0x8A;
            // ASL
            outRom[context.workingOffset++] = 0x0A;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // LDA table+1,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)(newTableOffsetP1);
            outRom[context.workingOffset++] = (byte)(newTableOffsetP1 >> 8);
            outRom[context.workingOffset++] = (byte)(newTableOffsetP1 >> 16);
            // STA $10
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x10;
            // STZ $11
            outRom[context.workingOffset++] = 0x64;
            outRom[context.workingOffset++] = 0x11;
            // LDA table,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)(newTableOffset);
            outRom[context.workingOffset++] = (byte)(newTableOffset >> 8);
            outRom[context.workingOffset++] = (byte)(newTableOffset >> 16);
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            return true;
        }

        public static void copyPaletteAnimationSettings(RandoContext context, int ogPaletteSet, int newPaletteSet)
        {
            // for generated palettes, steal the animation settings of one from the original game
            int tableOffset = context.workingData.getInt(PROPERTY_UPDATED_OFFSET);
            context.outputRom[tableOffset + newPaletteSet * 2] = context.originalRom[0x80802 + ogPaletteSet];
            context.outputRom[tableOffset + newPaletteSet * 2 + 1] = context.originalRom[0x80802 + ogPaletteSet + 1];
        }
    }
}
