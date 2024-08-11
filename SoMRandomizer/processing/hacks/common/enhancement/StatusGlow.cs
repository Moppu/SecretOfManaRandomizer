using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System;
using static SoMRandomizer.processing.hacks.common.util.CharacterPaletteRandomizer;

namespace SoMRandomizer.processing.hacks.common.enhancement
{
    /// <summary>
    /// Hack that provides an ff6-like palette glow when you have certain statuses.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class StatusGlow : RandoProcessor
    {
        protected override string getName()
        {
            return "Status condition palette glow";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_STATUSGLOW))
            {
                return false;
            }

            bool christmasMode = false;

            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            if (randoMode == VanillaRandoSettings.MODE_KEY)
            {
                christmasMode = settings.get(VanillaRandoSettings.PROPERTYNAME_SPECIAL_MODE) == "xmas";
            }

            // ones we want:
            // 7ee190 0x04 slow
            // 7ee190 0x80 mute
            // 7ee191 0x20 poison
            // 7ee1ae 0x10 moon energy - this is actually 0xF0, the upper nibble counts down the remaining
            // 7ee1b0 0x02 acid rain
            // 7ee1b0 0x14 speedup
            // 7ee1b0 0x48 lunar booat
            // 7ee1b1 0x10 lucid barrier
            // 7ee1b1 0x40 wall
            // 7ee1b0 0x01 defender

            // ones we don't want to do this if are active:
            // 7ee190 0x20 snowman
            // 7ee190 0x40 petrify
            // 7ee191 0x80 dead

            // 7ee1b9 is the timer
            
            // palettes start at.. 7EDF20? and have to be copied then to 7E0720
            
            int paletteLocationStart = context.workingOffset;
            // data first .. generate 16 palettes for each status condition
            // use the palettes from the outrom since we may have modified them
            for (int statusType = 0; statusType <= 9; statusType++)
            {
                // palette color type
                for (int charNum = 0; charNum <= 2; charNum++)
                {
                    // boy/girl/sprite
                    for (int palNum = 0; palNum < 16; palNum++)
                    {
                        // palette glow index
                        // index 0
                        outRom[context.workingOffset++] = 0;
                        outRom[context.workingOffset++] = 0;
                        for (int palIndex = 1; palIndex <= 15; palIndex++)
                        {
                            double sineScale = Math.Abs(Math.Sin((palNum + palIndex) * Math.PI / 16.0)); // half a sine curve
                            double sineScaleOffset1 = Math.Abs(Math.Sin((palNum + palIndex + 5) * Math.PI / 16.0)); // half a sine curve
                            double sineScaleOffset2 = Math.Abs(Math.Sin((palNum + palIndex + 10) * Math.PI / 16.0)); // half a sine curve

                            double sineScaleFast = Math.Abs(Math.Sin((palNum + palIndex) * Math.PI / 8.0)); // half a sine curve
                            double sineScaleFastOffset1 = Math.Abs(Math.Sin((palNum + palIndex + 5) * Math.PI / 8.0)); // half a sine curve
                            double sineScaleFastOffset2 = Math.Abs(Math.Sin((palNum + palIndex + 10) * Math.PI / 8.0)); // half a sine curve
                            // ignore outline and eyes for color shifting
                            // changing outline color on boy affects shadows, which is annoying
                            bool isOutline = (charNum == 0 && palIndex == 1) ||
                                       (charNum == 1 && palIndex == 1) ||
                                       (charNum == 2 && (palIndex == 1 || palIndex == 2 || palIndex == 9)) || palIndex == 15;

                            bool isSkin = (charNum == 0 && (palIndex == 5 || palIndex == 6 || palIndex == 7)) ||
                                       (charNum == 1 && (palIndex == 5 || palIndex == 6 || palIndex == 7)) ||
                                        (charNum == 2 && (palIndex == 6 || palIndex == 7));

                            // color number
                            // extract original color, which may be modified by the randomizer previously
                            SnesColor c = new SnesColor(outRom, 0x80FFE + (0x80 + charNum) * 0x1E + (palIndex) * 2);
                            if(charNum == 2 && (palIndex == 1 || palIndex == 2 || palIndex == 9))
                            {
                                // sprite's odd haircolor borders .. flatten them out at least
                                c.setRed(48);
                                c.setGreen(48);
                                c.setBlue(48);
                            }

                            double sineAmp1 = 200;
                            double sineAmp2 = -100;
                            switch (statusType)
                            {
                                case 0:
                                    // slow
                                    if(!isOutline)
                                    {
                                        // light
                                        c.add(48, 48, 48);
                                        c.add((int)(sineScale * sineScale * sineScale * sineAmp1), (int)(sineScale * sineScale * sineScale * sineAmp1), (int)(sineScale * sineScale * sineScale * sineAmp1));
                                    }
                                    break;
                                case 1:
                                    // mute
                                    if (!isOutline)
                                    {
                                        // dark
                                        c.add(-48, -48, -48);
                                        c.add((int)(sineScale * sineScale * sineScale * sineAmp2), (int)(sineScale * sineScale * sineScale * sineAmp2), (int)(sineScale * sineScale * sineScale * sineAmp2));
                                    }
                                    break;
                                case 2:
                                    // poison
                                    if (isSkin)
                                    {
                                        // purple skin, like the original, but animated
                                        c.add(48, -48, 48);
                                        c.add((int)(sineScale * sineScale * sineScale * sineAmp1), (int)(sineScale * sineScale * sineScale * sineAmp2), (int)(sineScale * sineScale * sineScale * sineAmp1));
                                    }
                                    break;
                                case 3:
                                    // moon energy
                                    if (!isOutline)
                                    {
                                        // rainbow
                                        c.add(-48, -48, -48);
                                        c.add((int)(sineScaleFast * sineScaleFast * sineScaleFast * sineAmp1), (int)(sineScaleFastOffset1 * sineScaleFastOffset1 * sineScaleFastOffset1 * sineAmp1), (int)(sineScaleFastOffset2 * sineScaleFastOffset2 * sineScaleFastOffset2 * sineAmp1));
                                    }
                                    break;
                                case 4:
                                    // acid rain
                                    if (!isOutline)
                                    {
                                        // a gross green color
                                        c.add(-96, -64, -128);
                                        c.add((int)(sineScale * sineScale * sineScale * sineAmp1), 0, (int)(sineScale * sineScale * sineScale * sineAmp2));
                                    }
                                    break;
                                case 5:
                                    // speedup
                                    if (!isOutline)
                                    {
                                        // red
                                        c.add(96, -32, -32);
                                        c.add((int)(sineScale * sineScale * sineScale * sineAmp1 * 2), (int)(sineScaleOffset1 * sineScaleOffset1 * sineScaleOffset1 * sineAmp2), -(int)(sineScaleOffset2 * sineScaleOffset2 * sineScaleOffset2 * sineAmp2));
                                    }
                                    break;
                                case 6:
                                    // lunar boost
                                    if (!isOutline)
                                    {
                                        // fast orange
                                        c.add(48, 0, -48);
                                        c.add((int)(sineScaleFast * sineScaleFast * sineScaleFast * sineAmp1 * 1.5), 0, (int)(sineScaleFast * sineScaleFast * sineScaleFast * sineAmp2 * 1.5));
                                    }
                                    break;
                                case 7:
                                    // lucid barrier
                                    if (!isOutline)
                                    {
                                        // yellow
                                        c.add(48, 48, -48);
                                        c.add((int)(sineScale * sineScale * sineScale * sineAmp1), (int)(sineScaleOffset1 * sineScaleOffset1 * sineScaleOffset1 * sineAmp1), (int)(sineScaleOffset2 * sineScaleOffset2 * sineScaleOffset2 * sineAmp2));
                                    }
                                    break;
                                case 8:
                                    // wall
                                    if (!isOutline)
                                    {
                                        // green
                                        c.add(-48, 48, -48);
                                        c.add((int)(sineScale * sineScale * sineScale * sineAmp2), (int)(sineScaleOffset1 * sineScaleOffset1 * sineScaleOffset1 * sineAmp1), (int)(sineScaleOffset2 * sineScaleOffset2 * sineScaleOffset2 * sineAmp2));
                                    }
                                    break;
                                case 9:
                                    // defender
                                    if (!isOutline)
                                    {
                                        // blue
                                        c.add(0, -24, 96);
                                        c.add((int)(sineScale * sineScale * sineScale * sineAmp2), (int)(sineScaleOffset1 * sineScaleOffset1 * sineScaleOffset1 * sineAmp2), (int)(sineScaleOffset2 * sineScaleOffset2 * sineScaleOffset2 * sineAmp1));
                                    }
                                    break;
                            }
                            c.put(outRom, context.workingOffset);
                            context.workingOffset += 2;
                        }
                    }
                }
            }
            // new code:

            int resetPaletteSubr = context.workingOffset;
            // x 16 bit, a 8 bit
            // x is the 0x000, 0x200, 0x400 - nope, it's 0,1,2
            // PHY - push starting y
            outRom[context.workingOffset++] = 0x5A;
            // PHX - push starting x
            outRom[context.workingOffset++] = 0xDA;

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // PHA - push starting a
            outRom[context.workingOffset++] = 0x48;

            //
            // TXA
            outRom[context.workingOffset++] = 0x8A;
            // ASL * 5
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;

            // PHA - push 00,20,40
            outRom[context.workingOffset++] = 0x48;
            // TXA
            outRom[context.workingOffset++] = 0x8A;
            // ASL
            outRom[context.workingOffset++] = 0x0A;
            // EOR #FFFF
            outRom[context.workingOffset++] = 0x49;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0xFF;
            // INC A - minus 00,02,04
            outRom[context.workingOffset++] = 0x1A;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC $01,s - this should be 00,1E,3C
            outRom[context.workingOffset++] = 0x63;
            outRom[context.workingOffset++] = 0x01;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // PLY - y is 00, 20, 40
            outRom[context.workingOffset++] = 0x7A;
            // 
            // loop:
            // LDA #C81EFE,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0xFE;
            outRom[context.workingOffset++] = 0x1E;
            outRom[context.workingOffset++] = 0xC8;
            // STA $DF20,y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = 0x20;
            outRom[context.workingOffset++] = 0xDF;
            // STA $0720,y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = 0x20;
            outRom[context.workingOffset++] = 0x07;
            // INY
            outRom[context.workingOffset++] = 0xC8;
            // INY
            outRom[context.workingOffset++] = 0xC8;
            // INX
            outRom[context.workingOffset++] = 0xE8;
            // INX
            outRom[context.workingOffset++] = 0xE8;
            // TYA
            outRom[context.workingOffset++] = 0x98;
            // AND #001F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x1F;
            outRom[context.workingOffset++] = 0x00;
            // BEQ out (02)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x02;
            // BRA loop
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0xEA;
            //
            // out:
            // PLA
            outRom[context.workingOffset++] = 0x68;

            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLY
            outRom[context.workingOffset++] = 0x7A;
            // RTL
            outRom[context.workingOffset++] = 0x6B;




            int zeroSubAddr = context.workingOffset;
            // ////////////////////////
            // timerZeroSub:
            // ////////////////////////
            //
            //             PHY
            outRom[context.workingOffset++] = 0x5A;
            //             REP 20  - temporary 16 bit a
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            //             TXA
            outRom[context.workingOffset++] = 0x8A;
            //             XBA
            outRom[context.workingOffset++] = 0xEB;
            //             LSR
            outRom[context.workingOffset++] = 0x4A;
            //             TAY     - y is 0, 1, 2 based on character
            outRom[context.workingOffset++] = 0xA8;
            // 64 A4       STZ $A4 - bits indicating current status (16 bit)
            outRom[context.workingOffset++] = 0x64;
            outRom[context.workingOffset++] = 0xA4;
            //             SEP 20  - restore 8 bit a
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // 64 98       STZ $98 - counter of how many statuses we've got
            outRom[context.workingOffset++] = 0x64;
            outRom[context.workingOffset++] = 0x98;
            outRom[context.workingOffset++] = 0x64;
            outRom[context.workingOffset++] = 0x99;
            //
            // BD 90 E1    LDA $e190,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE1;
            // 29 04       AND #04  -  slow
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x04;
            // F0 08       BEQ 08
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x08;
            // E6 98       INC $98
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0x98;
            //             LDA $A4
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xA4;
            //             ORA #01
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x01;
            //             STA $A4
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA4;
            //
            // BD 90 E1    LDA $e190,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE1;
            // 29 80       AND #80  -  mute
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x80;
            // F0 08       BEQ 08
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x08;
            // E6 98       INC $98
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0x98;
            //             LDA $A4
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xA4;
            //             ORA #02
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x02;
            //             STA $A4
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA4;
            //
            // BD 91 E1    LDA $e191,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x91;
            outRom[context.workingOffset++] = 0xE1;
            // 29 20       AND #20  -  poison
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x20;
            // F0 08       BEQ 08
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x08;
            // E6 98       INC $98
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0x98;
            //             LDA $A4
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xA4;
            //             ORA #04
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x04;
            //             STA $A4
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA4;
            //
            // BD AE E1    LDA $e1ae,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xAE;
            outRom[context.workingOffset++] = 0xE1;
            // 29 10       AND #10  -  moon energy
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xF0;
            // F0 08       BEQ 08
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x08;
            // E6 98       INC $98
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0x98;
            //             LDA $A4
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xA4;
            //             ORA #08
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x08;
            //             STA $A4
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA4;
            //
            // BD B0 E1    LDA $e1b0,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0xE1;
            // 29 02       AND #02  -  acid rain
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x02;
            // F0 08       BEQ 08
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x08;
            // E6 98       INC $98
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0x98;
            //             LDA $A4
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xA4;
            //             ORA #10
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x10;
            //             STA $A4
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA4;
            //
            // BD B0 E1    LDA $e1b0,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0xE1;
            // 29 14       AND #14  -  speedup
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x14;
            // F0 08       BEQ 08
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x08;
            // E6 98       INC $98
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0x98;
            //             LDA $A4
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xA4;
            //             ORA #20
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x20;
            //             STA $A4
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA4;
            //
            // BD B0 E1    LDA $e1b0,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0xE1;
            // 29 48       AND #48  -  lunar boost
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x48;
            // F0 08       BEQ 08
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x08;
            // E6 98       INC $98
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0x98;
            //             LDA $A4
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xA4;
            //             ORA #40
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x40;
            //             STA $A4
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA4;
            //
            // BD B1 E1    LDA $e1b1,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xB1;
            outRom[context.workingOffset++] = 0xE1;
            // 29 10       AND #10  -  lucid barrier
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x10;
            // F0 08       BEQ 08
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x08;
            // E6 98       INC $98
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0x98;
            //             LDA $A4
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xA4;
            //             ORA #80
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x80;
            //             STA $A4
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA4;
            //
            // BD B1 E1    LDA $e1b1,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xB1;
            outRom[context.workingOffset++] = 0xE1;
            // 29 40       AND #40  -  wall
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x40;
            // F0 08       BEQ 08
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x08;
            // E6 98       INC $98
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0x98;
            //             LDA $A5
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xA5;
            //             ORA #01
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x01;
            //             STA $A5
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA5;
            //
            // BD B1 E1    LDA $e1b0,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0xE1;
            // 29 40       AND #01  -  defender
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x01;
            // F0 08       BEQ 08
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x08;
            // E6 98       INC $98
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0x98;
            //             LDA $A5
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xA5;
            //             ORA #02
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x02;
            //             STA $A5
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA5;
            //
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // TYX
            outRom[context.workingOffset++] = 0xBB;
            // LDA $98
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0x98;
            // BNE over
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x13;

            // ////////////////////
            // LDA 7ECF2A,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1 >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1 >> 16);
            // BEQ over (4)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x04;

            // JSL clear palette
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)resetPaletteSubr;
            outRom[context.workingOffset++] = (byte)(resetPaletteSubr >> 8);
            outRom[context.workingOffset++] = (byte)((resetPaletteSubr >> 16) + 0xC0);

            // ////////////////////

            // STZ 7ECF2A,x
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;

            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1 >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1 >> 16);


            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLY
            outRom[context.workingOffset++] = 0x7A;

            // ////////////////////////////

            // RTL
            outRom[context.workingOffset++] = 0x6B;
            //
            // over:
            //
            // LDA 7ECF2A,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1 >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1 >> 16);

            // CMP $98
            outRom[context.workingOffset++] = 0xC5;
            outRom[context.workingOffset++] = 0x98;
            // BNE over2 -> BLT
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x02;
            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // over2:
            // INC A
            outRom[context.workingOffset++] = 0x1A;
            // STA 7ECF2A,x
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1 >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1 >> 16);


            //
            // now pop bits off of $A4 and $A5 until we hit $98, and the bit index we put .. somewhere else as the palette we pull in the nonzero block
            // LDY #00
            outRom[context.workingOffset++] = 0xA0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;

            // ////////////////////
            // STZ $98
            outRom[context.workingOffset++] = 0x64;
            outRom[context.workingOffset++] = 0x98;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            //
            // loop:
            // LDA $A4
            outRom[context.workingOffset++] = 0xA5; /////////////////////
            outRom[context.workingOffset++] = 0xA4; /////////////////////
            // INY
            outRom[context.workingOffset++] = 0xC8;
            // LSR - note the last bit goes into carry here
            outRom[context.workingOffset++] = 0x4A;
            // STA $A4
            outRom[context.workingOffset++] = 0x85; //////////////////////
            outRom[context.workingOffset++] = 0xA4; //////////////////////
            // BCC skip
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x04 + 0x07; ////////
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // INC $99
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0x99;
            // ///////////////////////////
            // CMP #01
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            // BNE skip - need to change other skip jump too
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x03;
            // TYA
            outRom[context.workingOffset++] = 0x98;
            // STA $98
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x98;
            // ///////////////////////////

            //
            // skip:
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $99
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0x99;
            // BEQ overcheck [6] - no $99 yet?  don't break out with 0
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x06;
            // CMP 7ECF2D,x -> 2A
            outRom[context.workingOffset++] = 0xDF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1 >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1 >> 16);

            // BEQ out - BGE instead
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x0A + 0x07;
            // 
            // CPY #000A - number of statuses
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x00;
            // BEQ loopEnd
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x02;
            // BRA loop
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0xD6;
            //
            // loopEnd:
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $98
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0x98;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // TAY
            outRom[context.workingOffset++] = 0xA8;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // out:
            // TYA
            outRom[context.workingOffset++] = 0x98;
            // DEC
            outRom[context.workingOffset++] = 0x3A;
            // STA 7ECF2D,x
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.STATUSGLOW_STATUS_BYTE_2;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_2 >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_2 >> 16);

            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLY
            outRom[context.workingOffset++] = 0x7A;
            // RTL
            outRom[context.workingOffset++] = 0x6B;



            int nonZeroSubAddr = context.workingOffset;
            // ////////////////////////
            // timerNonZeroSub:
            // ////////////////////////
            //             PHX
            outRom[context.workingOffset++] = 0xDA;
            //             REP 20  - temporary 16 bit a
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            //             TXA
            outRom[context.workingOffset++] = 0x8A;
            //             XBA
            outRom[context.workingOffset++] = 0xEB;
            //             LSR
            outRom[context.workingOffset++] = 0x4A;
            //             TAX
            outRom[context.workingOffset++] = 0xAA;
            //             SEP 20  - 8 bit a
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            //             LDA 7ECF2A,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1 >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1 >> 16);

            //             BNE over
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x02;
            //             PLX
            outRom[context.workingOffset++] = 0xFA;
            //             RTL
            outRom[context.workingOffset++] = 0x6B;
            // over:
            //             PLX
            outRom[context.workingOffset++] = 0xFA;
            // BD 90 E1    LDA $e190,x 
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE1;
            // 29 20       AND #20  -  snowman
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x20;
            //             BNE skip
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x10;
            // BD 90 E1    LDA $e190,x 
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE1;
            // 29 40       AND #40  -  petrify
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x40;
            //             BNE skip
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x09;
            // BD 91 E1    LDA $e191,x 
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x91;
            outRom[context.workingOffset++] = 0xE1;
            // 29 80       AND #80  -  dead
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x80;
            //             BNE skip
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x02;
            //             BRA loadPalette
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x01;
            // skip:
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;
            // loadPalette:
            // - so here, the palette i want is ...
            // - 7ECF2D/E/F * 16 * 3 * [0x20] + charNum * 16 * [0x20] + ((counter&1F)>>2) * x20 (palette size)
            // - so palettes should be arranged:
            // - boy slow palette 1 [0x0000]
            // - boy slow palette 2 [0x0020]
            // ...
            // - boy slow palette 16 [0x01E0]
            // - girl slow palette 1 [0x0200]
            // ..
            // - sprite slow palettes [0x0400]
            // - mute palettes [0x0600]
            // - poison palettes
            // .. lucid barrier palettes [0x3000]
            // - total palettes: # statuses (9) * num chars (3) * num palettes each (64) = 1728, 55296 bytes
            // - maybe we'll cut the palettes by 4 by dividing the counter by 4, so it's 16 each. 13824 bytes
            // - index on paletteLocationStart
            // 
            //             PHY
            outRom[context.workingOffset++] = 0x5A;
            //             PHX
            outRom[context.workingOffset++] = 0xDA;
            //             REP 20  - temporary 16 bit a
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            //             TXA
            outRom[context.workingOffset++] = 0x8A;
            //             XBA
            outRom[context.workingOffset++] = 0xEB;
            //             LSR
            outRom[context.workingOffset++] = 0x4A;
            //             TAX
            outRom[context.workingOffset++] = 0xAA;
            //             ASL
            outRom[context.workingOffset++] = 0x0A;
            //             ASL
            outRom[context.workingOffset++] = 0x0A;
            //             ASL
            outRom[context.workingOffset++] = 0x0A;
            //             ASL
            outRom[context.workingOffset++] = 0x0A;
            //             ASL - multiply by 32 for palette output index
            outRom[context.workingOffset++] = 0x0A;
            //             TAY
            outRom[context.workingOffset++] = 0xA8;
            //             LDA #0000
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            //             SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            //             LDA 7ECF2D,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.STATUSGLOW_STATUS_BYTE_2;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_2 >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_2 >> 16);

            //             ASL
            outRom[context.workingOffset++] = 0x0A;
            //             CLC
            outRom[context.workingOffset++] = 0x18;
            //             ADC 7ECF2D,x - multiply by 3
            outRom[context.workingOffset++] = 0x7F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.STATUSGLOW_STATUS_BYTE_2;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_2 >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_2 >> 16);

            //             REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // -             ASL
            // -             ASL
            //             ASL
            outRom[context.workingOffset++] = 0x0A;
            //             ASL
            outRom[context.workingOffset++] = 0x0A;
            //             ASL
            outRom[context.workingOffset++] = 0x0A;
            //             ASL - multiply by 64 -> 16
            outRom[context.workingOffset++] = 0x0A;

            // ///////////////////// add multiply by 0x20 for palette size
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            // /////////////////////

            //             PHA
            outRom[context.workingOffset++] = 0x48;
            //             TXA - char number
            outRom[context.workingOffset++] = 0x8A;
            // -             ASL
            // -             ASL
            //             ASL
            outRom[context.workingOffset++] = 0x0A;
            //             ASL
            outRom[context.workingOffset++] = 0x0A;
            //             ASL
            outRom[context.workingOffset++] = 0x0A;
            //             ASL - multiply by 64 -> 16
            outRom[context.workingOffset++] = 0x0A;

            // ///////////////////// add multiply by 0x20 for palette size
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            // /////////////////////

            //             CLC
            outRom[context.workingOffset++] = 0x18;
            //             ADC $02,s
            outRom[context.workingOffset++] = 0x63;
            outRom[context.workingOffset++] = 0x01;
            //             STA $02,s - add to what we pushed before
            outRom[context.workingOffset++] = 0x83;
            outRom[context.workingOffset++] = 0x01;
            //             PLA - so we can get to our saved x
            outRom[context.workingOffset++] = 0x68;
            //             PLX - 0x000, 0x200, 0x400
            outRom[context.workingOffset++] = 0xFA;
            //             PHA - back in it goes
            outRom[context.workingOffset++] = 0x48;
            // BD B9 E1    LDA E1B9, x - load timer
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0xE1;
            //             AND #003F - since we're 16 bit, and repeating.
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;// 0x3F;
            outRom[context.workingOffset++] = 0x00;
            //             ASL
            outRom[context.workingOffset++] = 0x0A;
            //             ASL
            outRom[context.workingOffset++] = 0x0A;
            //             ASL
            outRom[context.workingOffset++] = 0x0A;
            //             ASL
            outRom[context.workingOffset++] = 0x0A;
            //             ASL       - multiply by 32, our palette size
            outRom[context.workingOffset++] = 0x0A;
            //             CLC
            outRom[context.workingOffset++] = 0x18;
            //             ADC $02,s
            outRom[context.workingOffset++] = 0x63;
            outRom[context.workingOffset++] = 0x01;
            //             STA $02,s - add to what we pushed before
            outRom[context.workingOffset++] = 0x83;
            outRom[context.workingOffset++] = 0x01;
            //             PLA
            outRom[context.workingOffset++] = 0x68;
            //             PHX - push x back in
            outRom[context.workingOffset++] = 0xDA;
            //             TAX - use this thing as a big ass index
            outRom[context.workingOffset++] = 0xAA;
            // - now dump the palette in both spots .. we don't really have a loop counter available, so fuck it, just dump 16 of these in
            // - 7EDF20, 7E0720
            for (int i = 0; i < 16; i++)
            {
                int srcAddr = paletteLocationStart + i * 2;
                int dest1 = 0xDF20 + i * 2;
                int dest2 = 0x0720 + i * 2;
                // {
                //             LDA paletteLocationStart,x (24 bit, from rom)
                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = (byte)srcAddr;
                outRom[context.workingOffset++] = (byte)(srcAddr >> 8);
                outRom[context.workingOffset++] = (byte)((srcAddr >> 16) + 0xC0);
                //             STA DF20,y
                outRom[context.workingOffset++] = 0x99;
                outRom[context.workingOffset++] = (byte)dest1;
                outRom[context.workingOffset++] = (byte)(dest1>>8);
                //             STA 0720,y
                outRom[context.workingOffset++] = 0x99;
                outRom[context.workingOffset++] = (byte)dest2;
                outRom[context.workingOffset++] = (byte)(dest2 >> 8);
                // } - 16 of these; increment all 3 addresses by 2 each loop
            }

            //             SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // -             PLX
            outRom[context.workingOffset++] = 0xFA;
            //             PLY
            outRom[context.workingOffset++] = 0x7A;

            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;


            outRom[0x3B25] = 0x22;
            outRom[0x3B26] = (byte)context.workingOffset;
            outRom[0x3B27] = (byte)(context.workingOffset >> 8);
            outRom[0x3B28] = (byte)((context.workingOffset >> 16) + 0xC0);

            // ////////////////////////
            // main subr:
            // ////////////////////////
            // 22 7B 4F C0 JSL $C04F7B - replaced code
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x7B;
            outRom[context.workingOffset++] = 0x4F;
            outRom[context.workingOffset++] = 0xC0;

            // ////////////////// 
            // CPX #0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // BGE over
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x0F;
            // ////////////////// 
            // BD B9 E1    LDA E1B9, x - load timer
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0xE1;

            //             AND #3F
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x0F;

            //             BNE nonzero
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x04;
            // zero:
            // 22 xx xx xx JSL timerZeroSub
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)zeroSubAddr;
            outRom[context.workingOffset++] = (byte)(zeroSubAddr >> 8);
            outRom[context.workingOffset++] = (byte)((zeroSubAddr >> 16) + 0xC0);
            // nonzero:
            // 22 xx xx xx JSL timerNonZeroSub
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)nonZeroSubAddr;
            outRom[context.workingOffset++] = (byte)(nonZeroSubAddr >> 8);
            outRom[context.workingOffset++] = (byte)((nonZeroSubAddr >> 16) + 0xC0);

            // over: (enemies)
            // 6B          RTL
            outRom[context.workingOffset++] = 0x6B;

            // for these, compare x to x600 .. if less, call the zeroSubAddr

            // triggers to update the thingy right away:
            // [wall]
            // C8 / ED5B:	0940    	ORA #$40
            // C8 / ED5D:	9DB1E1 STA $E1B1,X
            outRom[0x8ED5B] = 0x22;
            outRom[0x8ED5C] = (byte)context.workingOffset;
            outRom[0x8ED5D] = (byte)(context.workingOffset >> 8);
            outRom[0x8ED5E] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x8ED5F] = 0xEA;
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x40;
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xB1;
            outRom[context.workingOffset++] = 0xE1;

            // cpx #600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // bge 04
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x04;

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)zeroSubAddr;
            outRom[context.workingOffset++] = (byte)(zeroSubAddr >> 8);
            outRom[context.workingOffset++] = (byte)((zeroSubAddr >> 16) + 0xC0);
            outRom[context.workingOffset++] = 0x6B;

            // [lucid barrier]
            // C8 / EEC3:	0910    	ORA #$10
            // C8 / EEC5:	9DB1E1 STA $E1B1,X
            outRom[0x8EEC3] = 0x22;
            outRom[0x8EEC4] = (byte)context.workingOffset;
            outRom[0x8EEC5] = (byte)(context.workingOffset >> 8);
            outRom[0x8EEC6] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x8EEC7] = 0xEA;
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xB1;
            outRom[context.workingOffset++] = 0xE1;

            // cpx #600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // bge 04
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x04;

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)zeroSubAddr;
            outRom[context.workingOffset++] = (byte)(zeroSubAddr >> 8);
            outRom[context.workingOffset++] = (byte)((zeroSubAddr >> 16) + 0xC0);
            outRom[context.workingOffset++] = 0x6B;

            // [slow, other statuses? if non zero, run the thing]
            // $C8 / E31C 25 A4       AND $A4    [$00:03A4]   A:0004 X:0000 Y:0400 P:envmxdIzC
            // $C8 / E31E 9D 90 E1 STA $E190,x[$7E:E190]   A: 0004 X: 0000 Y: 0400 P: envmxdIzC
            // 98, a4 (16bit) need to be preserved?
            outRom[0x8E31C] = 0x22;
            outRom[0x8E31D] = (byte)context.workingOffset;
            outRom[0x8E31E] = (byte)(context.workingOffset >> 8);
            outRom[0x8E31F] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x8E320] = 0xEA;
            outRom[context.workingOffset++] = 0x25;
            outRom[context.workingOffset++] = 0xA4;

            // ////////////////// only do this stuff if the status changed
            // CMP $E190,X
            outRom[context.workingOffset++] = 0xDD;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE1;
            // BNE continue (01)
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // //////////////////

            // continue:
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE1;

            // cpx #600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // bge 04
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x12;

            // /////////////////
            // lda $98
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0x98;
            // pha
            outRom[context.workingOffset++] = 0x48;
            // lda $a4
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0xA4;
            // pha
            outRom[context.workingOffset++] = 0x48;
            // /////////////////

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)zeroSubAddr;
            outRom[context.workingOffset++] = (byte)(zeroSubAddr >> 8);
            outRom[context.workingOffset++] = (byte)((zeroSubAddr >> 16) + 0xC0);
            // /////////////////
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // pla
            outRom[context.workingOffset++] = 0x68;
            // sta $a4
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA4;
            // pla
            outRom[context.workingOffset++] = 0x68;
            // sta $98
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0x98;
            // /////////////////

            outRom[context.workingOffset++] = 0x6B;


            // [acid storm]
            // C8 / EB49:	0902    	ORA #$02		[Set Acid Storm]
            // C8 / EB4B:	9DB0E1 STA $E1B0,X[Store into Stat Buffs]
            outRom[0x8EB49] = 0x22;
            outRom[0x8EB4A] = (byte)context.workingOffset;
            outRom[0x8EB4B] = (byte)(context.workingOffset >> 8);
            outRom[0x8EB4C] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x8EB4D] = 0xEA;
            bool freezingRainEnabled = true;

            if (christmasMode && freezingRainEnabled)
            {
                outRom[context.workingOffset++] = 0x09;
                outRom[context.workingOffset++] = 0x02;
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0xE1;

                // slow
                outRom[context.workingOffset++] = 0xBD;
                outRom[context.workingOffset++] = 0x90;
                outRom[context.workingOffset++] = 0xE1;

                outRom[context.workingOffset++] = 0x09;
                outRom[context.workingOffset++] = 0x04;
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0x90;
                outRom[context.workingOffset++] = 0xE1;
            }
            else
            {
                outRom[context.workingOffset++] = 0x09;
                outRom[context.workingOffset++] = 0x02;
                outRom[context.workingOffset++] = 0x9D;
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0xE1;
            }
            // cpx #600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // bge 04
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x04;

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)zeroSubAddr;
            outRom[context.workingOffset++] = (byte)(zeroSubAddr >> 8);
            outRom[context.workingOffset++] = (byte)((zeroSubAddr >> 16) + 0xC0);
            outRom[context.workingOffset++] = 0x6B;

            // [speedup]
            // C8/EB12:	0914    	ORA #$14
            // C8 / EB14:	9DB0E1 STA $E1B0,X
            outRom[0x8EB12] = 0x22;
            outRom[0x8EB13] = (byte)context.workingOffset;
            outRom[0x8EB14] = (byte)(context.workingOffset >> 8);
            outRom[0x8EB15] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x8EB16] = 0xEA;
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x14;
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0xE1;

            // cpx #600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // bge 04
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x04;

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)zeroSubAddr;
            outRom[context.workingOffset++] = (byte)(zeroSubAddr >> 8);
            outRom[context.workingOffset++] = (byte)((zeroSubAddr >> 16) + 0xC0);
            outRom[context.workingOffset++] = 0x6B;

            // [lunar boost]
            // C8/ECC3:	0948    	ORA #$48
            // C8 / ECC5:	9DB0E1 STA $E1B0,X
            outRom[0x8ECC3] = 0x22;
            outRom[0x8ECC4] = (byte)context.workingOffset;
            outRom[0x8ECC5] = (byte)(context.workingOffset >> 8);
            outRom[0x8ECC6] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x8ECC7] = 0xEA;
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x48;
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0xE1;

            // cpx #600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // bge 04
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x04;

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)zeroSubAddr;
            outRom[context.workingOffset++] = (byte)(zeroSubAddr >> 8);
            outRom[context.workingOffset++] = (byte)((zeroSubAddr >> 16) + 0xC0);
            outRom[context.workingOffset++] = 0x6B;

            // [moon energy]
            // C8/ECE4:	05A4    	ORA $A4
            // C8 / ECE6:	9DAEE1 STA $E1AE,X
            outRom[0x8ECE4] = 0x22;
            outRom[0x8ECE5] = (byte)context.workingOffset;
            outRom[0x8ECE6] = (byte)(context.workingOffset >> 8);
            outRom[0x8ECE7] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x8ECE8] = 0xEA;
            outRom[context.workingOffset++] = 0x05;
            outRom[context.workingOffset++] = 0xA4;
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xAE;
            outRom[context.workingOffset++] = 0xE1;

            // cpx #600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // bge 04
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x04;

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)zeroSubAddr;
            outRom[context.workingOffset++] = (byte)(zeroSubAddr >> 8);
            outRom[context.workingOffset++] = (byte)((zeroSubAddr >> 16) + 0xC0);
            outRom[context.workingOffset++] = 0x6B;

            // [defender]
            // C8/EB2E:	0901    	ORA #$01
            // C8 / EB30:	9DB0E1 STA $E1B0,X
            outRom[0x8EB2E] = 0x22;
            outRom[0x8EB2F] = (byte)context.workingOffset;
            outRom[0x8EB30] = (byte)(context.workingOffset >> 8);
            outRom[0x8EB31] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x8EB32] = 0xEA;
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0xE1;

            // cpx #600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // bge 04
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x04;

            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)zeroSubAddr;
            outRom[context.workingOffset++] = (byte)(zeroSubAddr >> 8);
            outRom[context.workingOffset++] = (byte)((zeroSubAddr >> 16) + 0xC0);
            outRom[context.workingOffset++] = 0x6B;

            // menu opening and closing
            // $C0/692F A9 01 00    LDA #$0001              A:0000 X:0000 Y:0000 P:envmxdIZC
            // $C0/6932 8D 26 18    STA $1826  [$00:1826]   A:0001 X:0000 Y:0000 P:envmxdIzC
            // -> - 7EDF20, 7E0720
            outRom[0x692F] = 0x22;
            outRom[0x6930] = (byte)context.workingOffset;
            outRom[0x6931] = (byte)(context.workingOffset >> 8);
            outRom[0x6932] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x6933] = 0xEA;
            outRom[0x6934] = 0xEA;

            // replace with:
            //
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x10;
            // LDX #0000
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // loop1:
            // LDA C81EFE,X - boy palette
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0xFE;
            outRom[context.workingOffset++] = 0x1E;
            outRom[context.workingOffset++] = 0xC8;
            // STA $DF20,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x20;
            outRom[context.workingOffset++] = 0xDF;
            // STA $0720,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x20;
            outRom[context.workingOffset++] = 0x07;
            // INX
            outRom[context.workingOffset++] = 0xE8;
            // INX
            outRom[context.workingOffset++] = 0xE8;
            // CPX #0020
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x20;
            outRom[context.workingOffset++] = 0x00;
            // BLT loop1
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xEF;
            //
            // LDX #0000
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // loop2:
            // LDA C81F1C,X - girl palette
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x1C;
            outRom[context.workingOffset++] = 0x1F;
            outRom[context.workingOffset++] = 0xC8;
            // STA $DF40,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x40;
            outRom[context.workingOffset++] = 0xDF;
            // STA $0740,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x40;
            outRom[context.workingOffset++] = 0x07;
            // INX
            outRom[context.workingOffset++] = 0xE8;
            // INX
            outRom[context.workingOffset++] = 0xE8;
            // CPX #0020
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x20;
            outRom[context.workingOffset++] = 0x00;
            // BLT loop2
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xEF;
            //
            // LDX #0000
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // loop3:
            // LDA C81F3A,X - sprite palette
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x3A;
            outRom[context.workingOffset++] = 0x1F;
            outRom[context.workingOffset++] = 0xC8;
            // STA $DF60,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x60;
            outRom[context.workingOffset++] = 0xDF;
            // STA $0760,x
            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x60;
            outRom[context.workingOffset++] = 0x07;
            // INX
            outRom[context.workingOffset++] = 0xE8;
            // INX
            outRom[context.workingOffset++] = 0xE8;
            // CPX #0020
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x20;
            outRom[context.workingOffset++] = 0x00;
            // BLT loop3
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xEF;

            // new: plp
            outRom[context.workingOffset++] = 0x28;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // A9 01 00 - original code
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x00;
            // 8D 26 18 - original code
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x26;
            outRom[context.workingOffset++] = 0x18;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // menu close call not needed


            int subr1D90Dcopy = context.workingOffset;
            for(int srcAddr = 0x1D90D; srcAddr <= 0x1D941; srcAddr++)
            {
                byte orig = outRom[srcAddr];
                if(orig == 0x60)
                {
                    // convert rts to rtl
                    orig = 0x6B;
                }
                outRom[context.workingOffset++] = orig;
            }
            // energy recharge glow removal
            // $01/D934 99 00 07    STA $0700,y[$7E:0722]   A:14A5 X:0000 Y:0022 P:envmxdIzc
            // $01/D937 E6 0D       INC $0D    [$00:000D] A:14A5 X:0000 Y:0022 P:envmxdIzc
            // also:
            // $00/ED98 99 00 07    STA $0700,y[$7E:0722]   A:14A5

            // $00/EB68 BD 00 E1    LDA $E100,x[$7E:E100]   A:7389 X:0000 Y:0000 P:eNvMxdIzC
            // $00 / EB6B 10 03       BPL $03[$EB70]      A: 7389 X: 0000 Y: 0000 P: eNvMxdIzC
            // $00/EB6D 20 08 ED    JSR $ED08  [$00:ED08]   A:7389 X:0000 Y:0000 P:eNvMxdIzC
            // $00/EB70 BD 04 E1    LDA $E104,x[$7E:E104]   A:7FFF X:0000 Y:0040 P:envMxdIZc

            // $01/CA44 09 88       ORA #$88                A:7301 X:0000 Y:0000 P:envMxdIzc
            // $01 / CA46 9D 00 E1 STA $E100,x[$7E:E100]   A: 7389 X: 0000 Y: 0000 P: eNvMxdIzc
            // $01/CA49 20 0D D9    JSR $D90D  [$01:D90D]   A:7389 X:0000 Y:0000 P:eNvMxdIzc
            // replace this.
            outRom[0x1CA44] = 0x22;
            outRom[0x1CA45] = (byte)context.workingOffset;
            outRom[0x1CA46] = (byte)(context.workingOffset >> 8);
            outRom[0x1CA47] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x1CA48] = 0xEA;
            outRom[0x1CA49] = 0xEA;
            outRom[0x1CA4A] = 0xEA;
            outRom[0x1CA4B] = 0xEA;

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PHX
            outRom[context.workingOffset++] = 0xDA;

            // CPX #0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // BLT skipXCheck (e)
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x0E;
            // (enemy .. do default action)
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // replaced code
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x88;

            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xE1;
            // replacement subr
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)subr1D90Dcopy;
            outRom[context.workingOffset++] = (byte)(subr1D90Dcopy >> 8);
            outRom[context.workingOffset++] = (byte)((subr1D90Dcopy >> 16) + 0xC0);

            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // skipXCheck:

            // TXA
            outRom[context.workingOffset++] = 0x8A;
            // XBA
            outRom[context.workingOffset++] = 0xEB;
            // LSR
            outRom[context.workingOffset++] = 0x4A;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 7ECF2A,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1 >> 8);
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.STATUSGLOW_STATUS_BYTE_1 >> 16);

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // BNE skip
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x0E;
            // (palette not active)
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // replaced code
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x88;

            outRom[context.workingOffset++] = 0x9D;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xE1;
            // replacement subr
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)subr1D90Dcopy;
            outRom[context.workingOffset++] = (byte)(subr1D90Dcopy >> 8);
            outRom[context.workingOffset++] = (byte)((subr1D90Dcopy >> 16) + 0xC0);

            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // skip: (palette active)
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
