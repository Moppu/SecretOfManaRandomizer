using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.openworld;
using System;
using static SoMRandomizer.processing.common.SomVanillaValues;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Various fixes to vanilla values and code to allow the game to start with girl or sprite instead of boy.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class StartingCharacterRandomizer : RandoProcessor
    {
        protected override string getName()
        {
            return "Starting character change hack";
        }
        
        protected override bool process(byte[] origRom, byte[] outRom, String seed, RandoSettings settings, RandoContext context)
        {
            string startingChar = context.workingData.get(OpenWorldCharacterSelection.STARTING_CHARACTER);
            // girl/sprite, don't call this for boy
            byte val1 = (byte)(startingChar == "girl" ? 1 : 2);
            // $C7/5F74 A0 00 00    LDY #$0000              A:0000 X:CC5C Y:0000 P:envMxdIZc
            outRom[0x75f75] = val1;

            if (startingChar == "girl")
            {
                // $C7/51CD 9E 80 E1    STZ $E180,x[$7E:E200]   A:007E X:0080 Y:0100 P:envMxdIzc
                outRom[0x751cf] = 0xDF;
                /*
                    C0/0FCC:    A901    LDA #$01
                    C0/0FCE:    85D9    STA $D9
                 */
                outRom[0xFCD] = 0x02;
                outRom[0x75762] = 0x02;
                // name shown on save file when loading
                outRom[0x75ADD] = 0x03 + 0x0C;
                // current hp display for save file load
                outRom[0x75B5F] = 0x85;
                // max hp
                outRom[0x75B7E] = 0x87;
            }
            else
            {
                // $C7/51CD 9E 80 E1    STZ $E180,x[$7E:E200]   A:007E X:0080 Y:0100 P:envMxdIzc
                outRom[0x751d2] = 0xDF;
                /*
                    C0/0FCC:    A901    LDA #$01
                    C0/0FCE:    85D9    STA $D9
                 */
                outRom[0xFCD] = 0x04;
                outRom[0x75762] = 0x04;
                // name shown on save file when loading
                outRom[0x75ADD] = 0x03 + 0x0C + 0x0C;
                // current hp display for save file load
                outRom[0x75B60] = 0x62;
                // max hp
                outRom[0x75B7F] = 0x62;
            }

            /*
                $C0/0FCE 85 D9       STA $D9    [$00:00D9]   A:0002 X:0000 Y:0010 P:envMxdIzC
                $C0/0FD0 8D 00 E2    STA $E200  [$7E:E200]   A:0002 X:0000 Y:0010 P:envMxdIzC
             */
            outRom[0xFCE] = 0x22;
            outRom[0xFCF] = (byte)(context.workingOffset);
            outRom[0xFD0] = (byte)(context.workingOffset >> 8);
            outRom[0xFD1] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0xFD2] = 0xEA;
            // STA $d9 still
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xD9;
            // LDA #01
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x01;
            // STA $E200, or $E400
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x00;
            if (startingChar == "girl")
            {
                outRom[context.workingOffset++] = 0xE2;
            }
            else
            {
                outRom[context.workingOffset++] = 0xE4;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // character npc shown on the starting map
            MapObject startingCharacterObject = VanillaMapUtil.getObjects(outRom, MAPNUM_INTRO_LOG)[0];
            startingCharacterObject.setSpecies((byte)(startingChar == "girl" ? 0x8B : 0x8C));
            VanillaMapUtil.putObject(outRom, startingCharacterObject, MAPNUM_INTRO_LOG, 0);

            return true;
        }
    }
}
