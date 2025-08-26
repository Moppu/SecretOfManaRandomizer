using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that disables collision with walls.
    /// Note this also includes enemies.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class NoCollision : RandoProcessor
    {
        protected override string getName()
        {
            return "Walk through walls";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_WALK_THROUGH_WALLS))
            {
                return false;
            }
            /*
$02/A052 BF 00 00 CB LDA $CB0000,x[$CB:0068] A:0068 X:0068 Y:0000 P:envmxdIzc
$02/A056 91 14       STA ($14),y[$7F:B800]   A:0010 X:0068 Y:0000 P:envmxdIzc
$02/A058 C8          INY                     A:0010 X:0068 Y:0000 P:envmxdIzc
$02/A059 C8          INY                     A:0010 X:0068 Y:0001 P:envmxdIzc
$02/A05A BF 02 00 CB LDA $CB0002,x[$CB:006A] A:0010 X:0068 Y:0002 P:envmxdIzc
$02/A05E 91 14       STA ($14),y[$7F:B802]   A:0000 X:0068 Y:0002 P:envmxdIZc
$02/A060 C8          INY                     A:0000 X:0068 Y:0002 P:envmxdIZc
$02/A061 C8          INY                     A:0000 X:0068 Y:0003 P:envmxdIzc
             */
            // b1 0x01 = wall
            // b3 0x07 = corners?
            outRom[0x2A052] = 0x22;
            outRom[0x2A053] = (byte)context.workingOffset;
            outRom[0x2A054] = (byte)(context.workingOffset >> 8);
            outRom[0x2A055] = (byte)((context.workingOffset >> 16) + 0xC0);

            // (removed code - load collision)
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xCB;
            // AND #FFFE - strip off bits for collision
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFE;
            outRom[context.workingOffset++] = 0xFF;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            outRom[0x2A05A] = 0x22;
            outRom[0x2A05B] = (byte)context.workingOffset;
            outRom[0x2A05C] = (byte)(context.workingOffset >> 8);
            outRom[0x2A05D] = (byte)((context.workingOffset >> 16) + 0xC0);

            // (removed code - load collision)
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xCB;
            // AND #FF00 - strip off bits for collision
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xFF;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
