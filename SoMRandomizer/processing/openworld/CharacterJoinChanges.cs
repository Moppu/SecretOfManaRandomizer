using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.openworld
{
    /// <summary>
    /// A few changes to vanilla code to better handle characters joining your party at times vanilla did not expect.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CharacterJoinChanges : RandoProcessor
    {
        protected override string getName()
        {
            return "Changes for character join";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string startingChar = context.workingData.get(OpenWorldCharacterSelection.STARTING_CHARACTER);

            // hack to make characters learn spells you got when they join (don't zero out that section of their ram)
            // note this overlaps with StartingLevel
            /*
                $C0/4D81 86 BD       STX $BD    [$00:03BD]   A:037E X:0400 Y:001B P:envMxdIzc
                $C0/4D83 20 31 4E    JSR $4E31  [$C0:4E31]   A:037E X:0400 Y:001B P:envMxdIzc
             */
            outRom[0x4D81] = 0x22;
            outRom[0x4D82] = (byte)(context.workingOffset);
            outRom[0x4D83] = (byte)(context.workingOffset >> 8);
            outRom[0x4D84] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x4D85] = 0xEA;

            // similar to 4e31, but skip C8-CF

            // $C0/4D81 86 BD       STX $BD    [$00:03BD]   A:037E X:0400 Y:001B P:envMxdIzc
            outRom[context.workingOffset++] = 0x86;
            outRom[context.workingOffset++] = 0xBD;

            // PHX
            outRom[context.workingOffset++] = 0xDA;

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // TXA
            outRom[context.workingOffset++] = 0x8A;

            // CLC
            outRom[context.workingOffset++] = 0x18;

            // ADC #0070
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x70;
            outRom[context.workingOffset++] = 0x00;

            // TAX
            outRom[context.workingOffset++] = 0xAA;

            // LDA #0070
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x70;
            outRom[context.workingOffset++] = 0x00;

            // TAY
            outRom[context.workingOffset++] = 0xA8;

            // LDA #0000
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;

            // back:

            // STA $E100,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xE1;

            // INX INX INY INY
            outRom[context.workingOffset++] = 0xE8;
            outRom[context.workingOffset++] = 0xE8;
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0xC8;

            // CPY #00C8
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0x00;

            // BNE back
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0xF4;

            // now from D0 on

            // lol
            outRom[context.workingOffset++] = 0xE8;
            outRom[context.workingOffset++] = 0xE8;
            outRom[context.workingOffset++] = 0xE8;
            outRom[context.workingOffset++] = 0xE8;
            outRom[context.workingOffset++] = 0xE8;
            outRom[context.workingOffset++] = 0xE8;
            outRom[context.workingOffset++] = 0xE8;
            outRom[context.workingOffset++] = 0xE8;
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0xC8;

            // STA $E100,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xE1;

            // INX INX INY INY
            outRom[context.workingOffset++] = 0xE8;
            outRom[context.workingOffset++] = 0xE8;
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0xC8;

            // CPY #0100
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x01;

            // BNE back
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0xF4;

            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // PLX
            outRom[context.workingOffset++] = 0xFA;

            // LDA #0F
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x0F;

            // STA $E1EC,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xEC;
            outRom[context.workingOffset++] = 0xE1;

            // LDA #FF
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0xFF;

            // STA $E1F0,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0xE1;

            // STA $E1E5,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0xE1;

            // STA $E1FC,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xFC;
            outRom[context.workingOffset++] = 0xE1;

            // ram object address to pull character join level from - E1 for boy, E3 for girl, E5 for sprite
            byte levelSourceOffset = 0xE1;
            if (startingChar == "girl")
            {
                levelSourceOffset = 0xE3;
            }
            if (startingChar == "sprite")
            {
                levelSourceOffset = 0xE5;
            }

            // starting level for characters joining your party
            int startingLevel = settings.getInt(OpenWorldSettings.PROPERTYNAME_STARTING_LEVEL);
            bool foundCharactersGetYourLevel = context.workingData.getBool(OpenWorldCharacterSelection.FOUND_CHARS_GET_YOUR_LEVEL);

            if (foundCharactersGetYourLevel)
            {
                // if startingLevel > 1, we should check for x == starting char here, and set it, otherwise copying is fine
                if (startingLevel > 1)
                {
                    // CPX #0000, #0200, or #0400 for starting char
                    byte startCharXCheck = (byte)(levelSourceOffset - 0xE1);
                    outRom[context.workingOffset++] = 0xE0;
                    outRom[context.workingOffset++] = 0x00;
                    outRom[context.workingOffset++] = startCharXCheck;

                    // BNE over
                    outRom[context.workingOffset++] = 0xD0;
                    outRom[context.workingOffset++] = 0x14;

                    // LDA #startLevel
                    outRom[context.workingOffset++] = 0xA9;
                    outRom[context.workingOffset++] = (byte)(startingLevel - 1);

                    outRom[context.workingOffset++] = 0x9D;
                    outRom[context.workingOffset++] = 0x81;
                    outRom[context.workingOffset++] = 0xE1;

                    // also copy exp so it isn't forever to hit the next level - need to read the exp table for that
                    // D0/4B58 - exp for level 2 (16) - 24 bit values
                    byte expValueLsb = outRom[0x104B58 + (startingLevel - 2) * 3];
                    byte expValueMb = outRom[0x104B58 + (startingLevel - 2) * 3 + 1];
                    byte expValueMsb = outRom[0x104B58 + (startingLevel - 2) * 3 + 2];

                    outRom[context.workingOffset++] = 0xA9;
                    outRom[context.workingOffset++] = expValueLsb;
                    // STA $E18D,x
                    outRom[context.workingOffset++] = 0x9D;
                    outRom[context.workingOffset++] = 0x8D;
                    outRom[context.workingOffset++] = 0xE1;

                    outRom[context.workingOffset++] = 0xA9;
                    outRom[context.workingOffset++] = expValueMb;
                    // STA $E18E,x
                    outRom[context.workingOffset++] = 0x9D;
                    outRom[context.workingOffset++] = 0x8E;
                    outRom[context.workingOffset++] = 0xE1;

                    outRom[context.workingOffset++] = 0xA9;
                    outRom[context.workingOffset++] = expValueMsb;
                    // STA $E18F,x
                    outRom[context.workingOffset++] = 0x9D;
                    outRom[context.workingOffset++] = 0x8F;
                    outRom[context.workingOffset++] = 0xE1;

                    // over:
                }

                // LDA 7EEx81
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x81;
                outRom[context.workingOffset++] = levelSourceOffset;
                outRom[context.workingOffset++] = 0x7E;

                // STA $E181,x
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0x81;
                outRom[context.workingOffset++] = 0xE1;

                // exp
                // LDA 7EEx8D
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = levelSourceOffset;
                outRom[context.workingOffset++] = 0x7E;

                // STA $E18D,x
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0xE1;

                // LDA 7EEx8E
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x8E;
                outRom[context.workingOffset++] = levelSourceOffset;
                outRom[context.workingOffset++] = 0x7E;

                // STA $E18E,x
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0x8E;
                outRom[context.workingOffset++] = 0xE1;

                // LDA 7EEx8F
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = levelSourceOffset;
                outRom[context.workingOffset++] = 0x7E;

                // STA $E18F,x
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0xE1;

            }
            else if (startingLevel > 1)
            {
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = (byte)(startingLevel - 1);

                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0x81;
                outRom[context.workingOffset++] = 0xE1;

                // also copy exp so it isn't forever to hit the next level - need to read the exp table for that
                // D0/4B58 - exp for level 2 (16) - 24 bit values
                byte expValueLsb = outRom[0x104B58 + (startingLevel - 2) * 3];
                byte expValueMb = outRom[0x104B58 + (startingLevel - 2) * 3 + 1];
                byte expValueMsb = outRom[0x104B58 + (startingLevel - 2) * 3 + 2];

                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = expValueLsb;
                // STA $E18D,x
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0xE1;

                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = expValueMb;
                // STA $E18E,x
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0x8E;
                outRom[context.workingOffset++] = 0xE1;

                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = expValueMsb;
                // STA $E18F,x
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0xE1;
            }

            // RTL
            outRom[context.workingOffset++] = 0x6B;
            return true;
        }
    }
}
