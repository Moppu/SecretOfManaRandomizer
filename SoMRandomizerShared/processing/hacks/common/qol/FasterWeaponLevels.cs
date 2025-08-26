using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// Make weapons level 4x faster for less of a grind than vanilla.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FasterWeaponLevels : RandoProcessor
    {
        protected override string getName()
        {
            return "Faster weapon levels";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if (!settings.getBool(CommonSettings.PROPERTYNAME_FASTER_WEAPON_LEVELS))
            {
                return false;
            }

            // increment weapon level in vanilla..
            // subtract level from 9 to determine increment amount
            // $C0/4358 A9 09       LDA #$09                A:0002 X:0800 Y:0200 P:envMxdIzc
            // $C0/435A 38          SEC                     A:0009 X:0800 Y:0200 P:envMxdIzc
            // $C0/435B F9 9C E1    SBC $E19C,y[$7E:E39C]   A:0009 X:0800 Y:0200 P:envMxdIzC
            // $C0/435E 85 A6 STA $A6[$00:03A6]             A:0009 X:0800 Y:0200 P:envMxdIzC
            
            outRom[0x4358] = 0x22;
            outRom[0x4359] = (byte)(context.workingOffset);
            outRom[0x435A] = (byte)(context.workingOffset >> 8);
            outRom[0x435B] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x435C] = 0xEA;
            outRom[0x435D] = 0xEA;
            outRom[0x435E] = 0xEA;
            outRom[0x435F] = 0xEA;

            // LDA #09 (vanilla)
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x09;
            // SEC A (vanilla)
            outRom[context.workingOffset++] = 0x38;
            // SBC $E19C,y (vanilla)
            outRom[context.workingOffset++] = 0xF9;
            outRom[context.workingOffset++] = 0x9C;
            outRom[context.workingOffset++] = 0xE1;
            // ASL twice to multiply by 4
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0x0A;
            // STA $A6 (vanilla)
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA6;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            return true;
        }
    }
}
