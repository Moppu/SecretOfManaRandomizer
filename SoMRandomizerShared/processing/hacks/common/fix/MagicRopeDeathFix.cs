using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.fix
{
    /// <summary>
    /// Hack to fix an issue where using the magic rope as a non-boy character would allow the user to still be hit during the animation.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    class MagicRopeDeathFix : RandoProcessor
    {
        protected override string getName()
        {
            return "Magic rope animation/death fix for girl/sprite";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_MAGIC_ROPE_DEATH_FIX))
            {
                return false;
            }
            if(settings.get(CommonSettings.PROPERTYNAME_MODE) == AncientCaveSettings.MODE_KEY)
            {
                // conflicts with MagicRope hack for ancient cave
                return false;
            }

            // from ancient cave MagicRope hack:
            // C1 / DC48:	ADF319 LDA $19F3
            // C1 / DC4B:	2903 AND #$03
            // C1 / DB4D:   0A   ASL


            outRom[0x1DC48] = 0x22;
            outRom[0x1DC49] = (byte)(context.workingOffset);
            outRom[0x1DC4A] = (byte)(context.workingOffset >> 8);
            outRom[0x1DC4B] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x1DC4C] = 0xEA;
            outRom[0x1DC4D] = 0xEA;

            // CMP #06
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x06;

            // BEQ xx = 07
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x07;

            // (replaced code+rtl)
            // LDA $19F3
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0xF3;
            outRom[context.workingOffset++] = 0x19;

            // AND #03
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x03;

            // ASL
            outRom[context.workingOffset++] = 0x0A;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // branch point:
            // LDA $1808 - more reliable address for which character is using the item, to later give them the rope animation (and invulnerability)
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x18;

            // AND #03
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x03;

            // ASL
            outRom[context.workingOffset++] = 0x0A;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
