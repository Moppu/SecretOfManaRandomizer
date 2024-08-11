using SoMRandomizer.config;
using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.bossrush
{
    /// <summary>
    /// Hack to change your characters' starting level for boss rush mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class StartingLevel : RandoProcessor
    {
        protected override string getName()
        {
            return "Starting level changes for boss rush mode";
        }

        // 7ee181 = level
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // $C0/4E3A 9D 70 E1    STA $E170,x[$7E:E180]   A:0000 X:0010 Y:0001 P:envmxdIzc
            // replace this bit:
            // $C0/4E5A A9 0F       LDA #$0F                A:0000 X:0200 Y:0000 P:envMxdIzc
            // $C0 / 4E5C 9D EC E1    STA $E1EC,x[$7E:E3EC]   A: 000F X: 0200 Y: 0000 P: envMxdIzc
            // with:
            // same shit
            // lda #xx
            // sta e181,x
            // rtl

            int playerLevel = settings.getInt(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.STARTING_LEVEL);

            outRom[0x4E5A] = 0x22;
            outRom[0x4E5B] = (byte)(context.workingOffset);
            outRom[0x4E5C] = (byte)(context.workingOffset >> 8);
            outRom[0x4E5D] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x4E5E] = 0xEA;

            // (removed code)
            // LDA #0F
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x0F;
            // STA $E1EC,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xEC;
            outRom[context.workingOffset++] = 0xE1;

            // CPX #0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // BCC/BLT over [90 01]
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x01;
            // RTL [6B]
            outRom[context.workingOffset++] = 0x6B;
            // LDA #level
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)(playerLevel - 1);
            // STA $E181,x (character level)
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x81;
            outRom[context.workingOffset++] = 0xE1;

            // now weapon levels .. 7EE1C0-3 for boy
            // LDA #11
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x11;

            // STA E1C0,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xE1;
            // STA E1C1,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xC1;
            outRom[context.workingOffset++] = 0xE1;
            // STA E1C2,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0xE1;
            // STA E1C3,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xC3;
            outRom[context.workingOffset++] = 0xE1;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
