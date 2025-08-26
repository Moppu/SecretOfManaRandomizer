using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Manipulate some seemingly-unused vanilla flag that causes boss AI to run every frame instead of only every 5 on the frame rule.
    /// Makes boss fights considerably more difficult and sometimes downright unfair.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FastBosses : RandoProcessor
    {
        protected override string getName()
        {
            return "Faster boss AI";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_AGGRESSIVE_BOSSES))
            {
                return false;
            }

            // $C2/3445 A0 00 00    LDY #$0000              A:D080 X:0000 Y:0000 P:eNvmxdIzc
            // $C2/3448 B7 0C LDA[$0C], y[$C2:D080]   A: D080 X:0000 Y: 0000 P: envmxdIZc

            outRom[0x23445] = 0x22;
            outRom[0x23446] = (byte)(context.workingOffset);
            outRom[0x23447] = (byte)(context.workingOffset >> 8);
            outRom[0x23448] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x23449] = 0xEA;

            // (replaced code)
            // LDY #0000
            outRom[context.workingOffset++] = 0xA0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // LDA [$0C],y
            outRom[context.workingOffset++] = 0xB7;
            outRom[context.workingOffset++] = 0x0C;

            // ora #4000 - enable the boss wacky speedup bit
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x40;

            // rtl
            outRom[context.workingOffset++] = 0x6B;

            // this flag goes in 7ee698 for 7ee6xx enemy - this can be changed in battle to enable this

            // $C2/0DB7 AD 56 00    LDA $0056  [$7E:0056]   A:0200 X:0800 Y:0200 P:envMxdizc ?
            // $C2/0DF6 D0 3E       BNE $3E    [$0E36]      A:0001 X:E600 Y:0200 P:envmxdizc

            // this one's for the slimes and mana beast - remove the BNE to process boss AI every frame instead
            // of only when the 5-step frame counter 7E0056 is 0
            outRom[0x20DF6] = 0xEA;
            outRom[0x20DF7] = 0xEA;
            return true;
        }
    }
}
