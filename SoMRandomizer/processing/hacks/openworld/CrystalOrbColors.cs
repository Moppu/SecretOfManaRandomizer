using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.openworld;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Hack that shows element orbs as a color for their element, and also changes their name to indicate the element.
    /// This hack also adds the enemy level to its name, if enabled.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CrystalOrbColors : RandoProcessor
    {
        protected override string getName()
        {
            return "Set colors and names of spell orbs";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            bool showEnemyLevels = settings.getBool(OpenWorldSettings.PROPERTYNAME_SHOW_ENEMY_LEVEL);
            string enemyLevelType = context.workingData.get(OpenWorldDifficultyProcessor.ENEMY_LEVEL_TYPE);
            
            // pull the element values for each orb off the working data, as set by ElementSwaps
            Dictionary<int, byte> elementValuesByMapNum = ElementSwaps.getCrystalOrbElementMap(context);
            int mapNumSubrCheckOffset = context.workingOffset;
            // LDA $7E00DC - current map number
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xDC;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;
            // load orb elements based on map number we expect them on
            foreach (int mapNum in elementValuesByMapNum.Keys)
            {
                byte elementValue = elementValuesByMapNum[mapNum];
                // CMP #mapnum
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = (byte)mapNum;
                outRom[context.workingOffset++] = (byte)(mapNum >> 8);
                // BNE over
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x00;
                int mapNumBranchSrc = context.workingOffset;
                // SEP 20 for 8bit A
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
                // LDA #40
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x40;
                // STA $E01C, y - values set by analyzer to change the orb color
                outRom[context.workingOffset++] = 0x99;
                outRom[context.workingOffset++] = 0x1C;
                outRom[context.workingOffset++] = 0xE0;
                // LDA #FF
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0xFF;
                // STA $E030, y - values set by analyzer to change the orb color
                outRom[context.workingOffset++] = 0x99;
                outRom[context.workingOffset++] = 0x30;
                outRom[context.workingOffset++] = 0xE0;
                // LDA #01
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x01;
                // STA $E042, y - values set by analyzer to change the orb color
                outRom[context.workingOffset++] = 0x99;
                outRom[context.workingOffset++] = 0x42;
                outRom[context.workingOffset++] = 0xE0;
                // LDA #element
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = elementValue;
                // STA $E011, y - values set by analyzer to change the orb color
                outRom[context.workingOffset++] = 0x99;
                outRom[context.workingOffset++] = 0x11;
                outRom[context.workingOffset++] = 0xE0;
                // REP 20 for 16bit A
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
                // set branch dist
                outRom[mapNumBranchSrc - 1] = (byte)(context.workingOffset - mapNumBranchSrc);
            }
            // RTL at the end
            outRom[context.workingOffset++] = 0x6B;


            /*
             *  override this enemy spawn handler for hp to check for orb type and run the above subroutine for them
                $C0/5625 BF 01 1C D0 LDA $D01C01,x[$D0:25A2] A:09A1 X:09A1 Y:0600 P:envmxdIzC
                $C0/5629 99 82 E1    STA $E182,y[$7E:E782]   A:0001 X:09A1 Y:0600 P:envmxdIzC
             */
            outRom[0x5625] = 0x22;
            outRom[0x5626] = (byte)(context.workingOffset);
            outRom[0x5627] = (byte)(context.workingOffset >> 8);
            outRom[0x5628] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x5629] = 0xEA;
            outRom[0x562A] = 0xEA;
            outRom[0x562B] = 0xEA;
            // CPX #09A1 - crystal orb is id 85 (decimal) and there's 29 (decimal) bytes per thingy
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0xA1;
            outRom[context.workingOffset++] = 0x09;
            // BNE over
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x00;
            int objTypeBranchSrc = context.workingOffset;
            // call map check subr if we're loading a crystal orb
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(mapNumSubrCheckOffset);
            outRom[context.workingOffset++] = (byte)(mapNumSubrCheckOffset >> 8);
            outRom[context.workingOffset++] = (byte)((mapNumSubrCheckOffset >> 16) + 0xC0);
            // set jump distance (4)
            outRom[objTypeBranchSrc - 1] = (byte)(context.workingOffset - objTypeBranchSrc);
            // over:
            // replaced code - load hp from rom
            // LDA $D01C01,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x1C;
            outRom[context.workingOffset++] = 0xD0;
            // STA $E182,y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = 0x82;
            outRom[context.workingOffset++] = 0xE1;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            /*
             *   override to not load for orbs
                 $01/DAEF BD 42 E1    LDA $E142,x[$7E:E342]   A:0103 X:0200 Y:E200 P:envMxdIzC
                 $01/DAF2 9D 11 E0    STA $E011,x[$7E:E211]   A:010A X:0200 Y:E200 P:envMxdIzC
            */
            outRom[0x1DAEF] = 0x22;
            outRom[0x1DAF0] = (byte)(context.workingOffset);
            outRom[0x1DAF1] = (byte)(context.workingOffset >> 8);
            outRom[0x1DAF2] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x1DAF3] = 0xEA;
            outRom[0x1DAF4] = 0xEA;
            // LDA $E1E7,x - species id
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xE7;
            outRom[context.workingOffset++] = 0xE1;
            // CMP #55 - element orb
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x55;
            // BEQ over - skip replaced code
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x06;
            // (replaced code)
            // LDA $E142,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x42;
            outRom[context.workingOffset++] = 0xE1;
            // STA $E011,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x11;
            outRom[context.workingOffset++] = 0xE0;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // change names as they're loaded
            context.namesOfThings.setName(0x124, "Orb");
            /*
                re-create this subroutine to print a single byte of text as a long-callable subroutine

                $C0/172B 20 C7 17    JSR $17C7  [$C0:17C7]   A:009D X:0000 Y:E95D P:envMxdIzC

                $C0/17C7 8B          PHB                     A:009D X:0000 Y:E95D P:envMxdIzC
                $C0/17C8 48          PHA                     A:009D X:0000 Y:E95D P:envMxdIzC
                $C0/17C9 A9 7E       LDA #$7E                A:009D X:0000 Y:E95D P:envMxdIzC
                $C0/17CB 48          PHA                     A:007E X:0000 Y:E95D P:envMxdIzC
                $C0/17CC AB          PLB                     A:007E X:0000 Y:E95D P:envMxdIzC
                $C0/17CD 68          PLA                     A:007E X:0000 Y:E95D P:envMxdIzC
                $C0/17CE 9D A4 A1    STA $A1A4,x[$7E:A1A4]   A:009D X:0000 Y:E95D P:eNvMxdIzC
                $C0/17D1 E8          INX                     A:009D X:0000 Y:E95D P:eNvMxdIzC
                $C0/17D2 EE CE A1    INC $A1CE  [$7E:A1CE]   A:009D X:0001 Y:E95D P:envMxdIzC
                $C0/17D5 AD CA A1    LDA $A1CA  [$7E:A1CA]   A:009D X:0001 Y:E95D P:envMxdIzC
                $C0/17D8 CE CA A1    DEC $A1CA  [$7E:A1CA]   A:001C X:0001 Y:E95D P:envMxdIzC
                $C0/17DB AB          PLB                     A:001C X:0001 Y:E95D P:envMxdIzC
                $C0/17DC 3A          DEC A                   A:001C X:0001 Y:E95D P:eNvMxdIzC
                $C0/17DD D0 24       BNE $24    [$1803]      A:001B X:0001 Y:E95D P:envMxdIzC
                $C0/1803 60          RTS                     A:001B X:0001 Y:E95D P:envMxdIzC
             */
            List<byte> writeEventLetterSubroutine = new List<byte>();
            for (int i = 0x17c7; i <= 0x1803; i++)
            {
                writeEventLetterSubroutine.Add(outRom[i]);
            }
            // RTS -> RTL
            writeEventLetterSubroutine[writeEventLetterSubroutine.Count - 1] = 0x6B;
            int writeEventLetterSubroutineAddr = context.workingOffset;
            foreach (byte b in writeEventLetterSubroutine)
            {
                outRom[context.workingOffset++] = b;
            }

            // generate subroutine for each orb to load its associated element name, using the character loader above
            Dictionary<int, int> mapNumToSubr = new Dictionary<int, int>();
            foreach (int mapNum in elementValuesByMapNum.Keys)
            {
                byte eleValue = elementValuesByMapNum[mapNum];
                mapNumToSubr[mapNum] = context.workingOffset;
                List<byte> nameString = VanillaEventUtil.getBytes(SomVanillaValues.elementOrbByteToName(eleValue, true) + " ");
                // SEP 20
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
                // write name characters
                foreach (byte b in nameString)
                {
                    // LDA #letter
                    outRom[context.workingOffset++] = 0xA9;
                    outRom[context.workingOffset++] = b;
                    // JSL writeEventLetterSubroutineAddr
                    outRom[context.workingOffset++] = 0x22;
                    outRom[context.workingOffset++] = (byte)(writeEventLetterSubroutineAddr);
                    outRom[context.workingOffset++] = (byte)(writeEventLetterSubroutineAddr >> 8);
                    outRom[context.workingOffset++] = (byte)((writeEventLetterSubroutineAddr >> 16) + 0xC0);
                }
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }


            int showEnemyLevelSubroutine = context.workingOffset;
            if(showEnemyLevels)
            {
                // CPY #19d3
                outRom[context.workingOffset++] = 0xC0;
                outRom[context.workingOffset++] = 0xD3;
                outRom[context.workingOffset++] = 0x19;
                // BEQ over [01]
                outRom[context.workingOffset++] = 0xF0;
                outRom[context.workingOffset++] = 0x01;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
                // CPX #99e (rabite)
                outRom[context.workingOffset++] = 0xE0;
                outRom[context.workingOffset++] = 0x9E;
                outRom[context.workingOffset++] = 0x09;
                // BGE over [01]
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0x01;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
                // CPX #a9d (mana beast + 1)
                outRom[context.workingOffset++] = 0xE0;
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0x0A;
                // BLT over [01]
                outRom[context.workingOffset++] = 0x90;
                outRom[context.workingOffset++] = 0x01;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
                // 7ea173 is current text position, 7ea1a4 is the array
                // PHA
                outRom[context.workingOffset++] = 0x48;
                // SEP 20
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
                // LDA #07
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x07;
                // STA 7ea173 - position
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x73;
                outRom[context.workingOffset++] = 0xA1;
                outRom[context.workingOffset++] = 0x7E;
                // STA 7ea1ce - length
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0xCE;
                outRom[context.workingOffset++] = 0xA1;
                outRom[context.workingOffset++] = 0x7E;
                // LDA #A6 "L"
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0xA6;
                // STA 7ea1a4
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0xA4;
                outRom[context.workingOffset++] = 0xA1;
                outRom[context.workingOffset++] = 0x7E;
                // LDA #96 "v"
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x96;
                // STA 7ea1a5
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0xA5;
                outRom[context.workingOffset++] = 0xA1;
                outRom[context.workingOffset++] = 0x7E;
                // LDA #BF "."
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0xBF;
                // STA 7ea1a6
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0xA6;
                outRom[context.workingOffset++] = 0xA1;
                outRom[context.workingOffset++] = 0x7E;
                // LDA #80 " "
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x80;
                // STA 7ea1a7
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0xA7;
                outRom[context.workingOffset++] = 0xA1;
                outRom[context.workingOffset++] = 0x7E;
                // 7e1822 - 16 bit index into 7e1800 for the x value to use for the target data (E600, E800, etc)
                // PHX
                outRom[context.workingOffset++] = 0xDA;
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // LDA 7e1822
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = 0x18;
                outRom[context.workingOffset++] = 0x7E;
                // TAX
                outRom[context.workingOffset++] = 0xAA;
                // LDA 7e0000,x
                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = 0x00;
                outRom[context.workingOffset++] = 0x00;
                outRom[context.workingOffset++] = 0x7E;
                // TAX
                outRom[context.workingOffset++] = 0xAA;
                // SEP 20
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
                // LDA 7E0181,x - these x values are E600, etc - alternate species byte
                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = 0x81;
                outRom[context.workingOffset++] = 0x01;
                outRom[context.workingOffset++] = 0x7E;
                // PLX
                outRom[context.workingOffset++] = 0xFA;
                // INC A - don't do this anymore now that we're pulling from object space
                if (enemyLevelType != "none")
                {
                    // account for memory levels being off by one from what we want to display
                    outRom[context.workingOffset++] = 0x1A;
                }
                // divide by 10
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
                // STA 004204
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x04;
                outRom[context.workingOffset++] = 0x42;
                outRom[context.workingOffset++] = 0x00;
                // SEP 20
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
                // LDA #0A
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x0A;
                // STA 004206
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x06;
                outRom[context.workingOffset++] = 0x42;
                outRom[context.workingOffset++] = 0x00;
                // eight nops to wait
                for (int i = 0; i < 8; i++)
                {
                    outRom[context.workingOffset++] = 0xEA;
                }
                // LDA 004214 - divisor
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x14;
                outRom[context.workingOffset++] = 0x42;
                outRom[context.workingOffset++] = 0x00;
                // CLC
                outRom[context.workingOffset++] = 0x18;
                // ADC #B5 - numeric font character value
                outRom[context.workingOffset++] = 0x69;
                outRom[context.workingOffset++] = 0xB5;
                // STA 7ea1a8
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0xA8;
                outRom[context.workingOffset++] = 0xA1;
                outRom[context.workingOffset++] = 0x7E;
                // LDA 004216 - remainder
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x16;
                outRom[context.workingOffset++] = 0x42;
                outRom[context.workingOffset++] = 0x00;
                // CLC
                outRom[context.workingOffset++] = 0x18;
                // ADC #B5
                outRom[context.workingOffset++] = 0x69;
                outRom[context.workingOffset++] = 0xB5;
                // STA 7ea1a9
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0xA1;
                outRom[context.workingOffset++] = 0x7E;
                // LDA #80 " "
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x80;
                // STA 7ea1aa
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0xAA;
                outRom[context.workingOffset++] = 0xA1;
                outRom[context.workingOffset++] = 0x7E;
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // PLA
                outRom[context.workingOffset++] = 0x68;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }


            // this is name loader for any top of screen text, it seems
            /*
             *  load event (X/2)+0x400 from bank CA - 0x924 in this case (Crystal Orb)
                $C0/1926 BF 00 00 CA LDA $CA0000,x[$CA:0A48] A:0A48 X:0A48 Y:19D3 P:envmxdIzc
                $C0/192A 8F 01 1D 00 STA $001D01[$00:1D01]   A:E95C X:0A48 Y:19D3 P:eNvmxdIzc **
             */
            outRom[0x192A] = 0x22;
            outRom[0x192B] = (byte)(context.workingOffset);
            outRom[0x192C] = (byte)(context.workingOffset >> 8);
            outRom[0x192D] = (byte)((context.workingOffset >> 16) + 0xC0);
            // (removed code) STA $001D01
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x1D;
            outRom[context.workingOffset++] = 0x00;
            // CPX #0A48 - compare to A48 for crystal orb, which is event 924 .. * 2 - 0x800 = A48
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x48;
            outRom[context.workingOffset++] = 0x0A;
            if (showEnemyLevels)
            {
                // BEQ 05 - skip printing levels if it's a crystal orb
                outRom[context.workingOffset++] = 0xF0;
                outRom[context.workingOffset++] = 0x05;
                // JSL showEnemyLevelSubroutine - show enemy levels for other object types
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = (byte)(showEnemyLevelSubroutine);
                outRom[context.workingOffset++] = (byte)(showEnemyLevelSubroutine >> 8);
                outRom[context.workingOffset++] = (byte)((showEnemyLevelSubroutine >> 16) + 0xC0);
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }
            else
            {
                // BEQ 01 - for non-orbs, do nothing special
                outRom[context.workingOffset++] = 0xF0;
                outRom[context.workingOffset++] = 0x01;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // LDX #0000
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // LDA 7E00DC - map number
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xDC;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;
            // compare to each map, use generated subroutines from above to name it
            foreach (int mapNum in elementValuesByMapNum.Keys)
            {
                // next:
                // CMP #mapnum
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = (byte)mapNum;
                outRom[context.workingOffset++] = (byte)(mapNum>>8);
                // BNE next
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x0C;
                // JSL subrAddr
                int subrAddr = mapNumToSubr[mapNum];
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = (byte)(subrAddr);
                outRom[context.workingOffset++] = (byte)(subrAddr >> 8);
                outRom[context.workingOffset++] = (byte)((subrAddr >> 16) + 0xC0);
                // TXA
                outRom[context.workingOffset++] = 0x8A;
                // STA 7EA173 - start printing at the position we left off
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x73;
                outRom[context.workingOffset++] = 0xA1;
                outRom[context.workingOffset++] = 0x7E;
                // PLX
                outRom[context.workingOffset++] = 0xFA;
                // PLA
                outRom[context.workingOffset++] = 0x68;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }
            // next:
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            return true;
        }
    }
}
