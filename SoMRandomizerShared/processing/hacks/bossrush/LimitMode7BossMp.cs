using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.bossrush
{
    /// <summary>
    /// Hack that removes the constant restoration of lime slime, dread slime, and mana beast's MP during fights.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class LimitMode7BossMp : RandoProcessor
    {
        protected override string getName()
        {
            return "Remove unlimited MP for Mode7 bosses";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // take out the sta that sets mana beast's current/max MP to 99 every frame
            // this also applies to lime slime and dread slime for some reason

            // $C2/8F36 9D 86 01    STA $0186,x[$7E:E786]   A:6363 X:E600 Y:0000 P:envmxdizc
            outRom[0x28F36] = 0xEA;
            outRom[0x28F37] = 0xEA;
            outRom[0x28F38] = 0xEA;

            // slimes
            outRom[0x28458] = 0xEA;
            outRom[0x28458] = 0xEA;
            outRom[0x28458] = 0xEA;

            return true;
        }
    }
}
