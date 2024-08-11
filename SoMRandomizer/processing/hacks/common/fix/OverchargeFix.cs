using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.fix
{
    /// <summary>
    /// Patch out the overcharge glitch used in speedruns by resetting the current weapon charge when you 
    /// 1. change AI charge settings and 2. change weapons.
    /// This is provided as an optional hack since some may still prefer to use these glitches.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class OverchargeFix : RandoProcessor
    {
        protected override string getName()
        {
            return "Overcharge fix";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_OVERCHARGE_FIX))
            {
                return false;
            }

            // replace the following that vanilla uses in the AI menu to set the requested max charge level:
            // $C7/6CD0 AD 11 A2    LDA $A211  [$7E:A211]   A:000F X:000F Y:0001 P:envMxdIzC
            // $C7/6CD3 99 7D CC STA $CC7D,y[$7E:CC7E] A:0001 X:000F Y:0001 P:envMxdIzC
            outRom[0x76CD0] = 0xEA;
            outRom[0x76CD1] = 0xEA;
            outRom[0x76CD2] = 0x22;
            outRom[0x76CD3] = (byte)(context.workingOffset);
            outRom[0x76CD4] = (byte)(context.workingOffset >> 8);
            outRom[0x76CD5] = (byte)((context.workingOffset >> 16) + 0xC0);

            // orig code - LDA $A211 - load AI charge max setting
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x11;
            outRom[context.workingOffset++] = 0xA2;

            // orig code - STA $CC7D, y - write AI charge max setting
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = 0x7D;
            outRom[context.workingOffset++] = 0xCC;

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // PHA
            outRom[context.workingOffset++] = 0x48;

            // TYA
            outRom[context.workingOffset++] = 0x98;

            // XBA
            outRom[context.workingOffset++] = 0xEB;

            // ASL
            outRom[context.workingOffset++] = 0x0A;

            // TAX -- 000, 200, 400 in x
            outRom[context.workingOffset++] = 0xAA;

            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;

            // STA 7EE01A, X - zero out the charge level of the character (percent)
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x1A;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;

            // STA 7EE01B, X - zero out the charge level of the character (full level)
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x1B;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // PLA
            outRom[context.workingOffset++] = 0x68;

            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // replace code used to set weapon value for character, to fix overcharge when swapping character weapons
            // $00 / EA67 99 68 E0    STA $E068,y[$7E:E068]   A:0206 X:0288 Y:0000 P:envMxdIzC
            // $00 / EA6A EB XBA
            outRom[0xEA67] = 0x22;
            outRom[0xEA68] = (byte)(context.workingOffset);
            outRom[0xEA69] = (byte)(context.workingOffset >> 8);
            outRom[0xEA6A] = (byte)((context.workingOffset >> 16) + 0xC0);

            // PHX
            outRom[context.workingOffset++] = 0xDA;

            // TYX
            outRom[context.workingOffset++] = 0xBB;
            
            // when setting player weapon, if the weapon id is changing, reset charge levels.
            // i believe this check was needed because there are occasions when this code runs
            // for existing weapons (dispel? weapon effect changes?), and we don't want to reset 
            // the charge level for those

            // CMP E068,y - compare to existing weapon
            outRom[context.workingOffset++] = 0xD9;
            outRom[context.workingOffset++] = 0x68;
            outRom[context.workingOffset++] = 0xE0;

            // BNE over
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x06;

            // here: run vanilla code and return, since weapon did not change
            // PLX
            outRom[context.workingOffset++] = 0xFA;

            // STA $E068,y replaced code
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = 0x68;
            outRom[context.workingOffset++] = 0xE0;
            // XBA replaced code
            outRom[context.workingOffset++] = 0xEB;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // over:

            // STZ E01A,x - zero out weapon charge level (percent)
            outRom[context.workingOffset++] = 0x9E;
            outRom[context.workingOffset++] = 0x1A;
            outRom[context.workingOffset++] = 0xE0;

            // STZ E01B,x - zero out weapon charge level (full charge)
            outRom[context.workingOffset++] = 0x9E;
            outRom[context.workingOffset++] = 0x1B;
            outRom[context.workingOffset++] = 0xE0;

            // PLX
            outRom[context.workingOffset++] = 0xFA;

            // replaced code - set weapon type number, then xba
            // STA $E068,y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = 0x68;
            outRom[context.workingOffset++] = 0xE0;
            // XBA
            outRom[context.workingOffset++] = 0xEB;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
