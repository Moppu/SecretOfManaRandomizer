using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Replace vanilla logic that uses the boy's weapon levels and sublevels to determine the charge level bonus
    /// granted while Mana Magic is active, with just a constant of +6 charge levels.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ManaMagicFix : RandoProcessor
    {
        protected override string getName()
        {
            return "Mana magic fixed charge level";
        }

        // when casting mana magic:
        /*
         *  vanilla mana magic formula:
            $C0/4927 AD E4 E1    LDA $E1E4  [$7E:E1E4]   A:0008 X:0000 Y:0000 P:envMxdIzC
            $C0/492A C9 01       CMP #$01                A:0001 X:0000 Y:0000 P:envMxdIzC
            $C0/492C F0 01       BEQ $01    [$492F]      A:0001 X:0000 Y:0000 P:envMxdIZC - think this bit is checking for mana magic active
            $C0/492F A9 80       LDA #$80                A:0001 X:0000 Y:0000 P:envMxdIZC
            $C0/4931 9D 95 E1    STA $E195,x[$7E:E195]   A:0080 X:0000 Y:0000 P:eNvMxdIzC
            $C0/4934 C2 20       REP #$20                A:0080 X:0000 Y:0000 P:eNvMxdIzC
            $C0/4936 64 A4       STZ $A4    [$00:03A4]   A:0080 X:0000 Y:0000 P:eNvmxdIzC
            $C0/4938 A0 07 00    LDY #$0007              A:0080 X:0000 Y:0000 P:eNvmxdIzC - loop counter

            loop:
            $C0/493B B9 D0 E1    LDA $E1D0,y[$7E:E1D7]   A:0080 X:0000 Y:0007 P:envmxdIzC
            $C0/493E 29 FF 00    AND #$00FF              A:0000 X:0000 Y:0007 P:envmxdIZC
            $C0/4941 18          CLC                     A:0000 X:0000 Y:0007 P:envmxdIZC
            $C0/4942 65 A4       ADC $A4    [$00:03A4]   A:0000 X:0000 Y:0007 P:envmxdIZc
            $C0/4944 85 A4       STA $A4    [$00:03A4]   A:0000 X:0000 Y:0007 P:envmxdIZc - sum all weapon level fractions (0-99 each)
            $C0/4946 88          DEY                     A:0000 X:0000 Y:0007 P:envmxdIZc
            $C0/4947 10 F2       BPL $F2    [$493B]      A:0000 X:0000 Y:0006 P:envmxdIzc
            ... (x8)

            $C0/4949 8F 04 42 00 STA $004204[$00:4204]   A:0044 X:0000 Y:FFFF P:eNvmxdIzc
            $C0/494D E2 20       SEP #$20                A:0044 X:0000 Y:FFFF P:eNvmxdIzc
            $C0/494F A9 64       LDA #$64                A:0044 X:0000 Y:FFFF P:eNvMxdIzc
            $C0/4951 8F 06 42 00 STA $004206[$00:4206]   A:0064 X:0000 Y:FFFF P:envMxdIzc - divide the whole sum by 100
            $C0/4955 EA          NOP                     A:0064 X:0000 Y:FFFF P:envMxdIzc
            $C0/4956 EA          NOP                     A:0064 X:0000 Y:FFFF P:envMxdIzc
            $C0/4957 EA          NOP                     A:0064 X:0000 Y:FFFF P:envMxdIzc
            $C0/4958 EA          NOP                     A:0064 X:0000 Y:FFFF P:envMxdIzc
            $C0/4959 EA          NOP                     A:0064 X:0000 Y:FFFF P:envMxdIzc
            $C0/495A EA          NOP                     A:0064 X:0000 Y:FFFF P:envMxdIzc
            $C0/495B EA          NOP                     A:0064 X:0000 Y:FFFF P:envMxdIzc
            $C0/495C EA          NOP                     A:0064 X:0000 Y:FFFF P:envMxdIzc
            $C0/495D AF 14 42 00 LDA $004214[$00:4214]   A:0064 X:0000 Y:FFFF P:envMxdIzc
            $C0/4961 85 A4       STA $A4    [$00:03A4]   A:0000 X:0000 Y:FFFF P:envMxdIZc - $A4 is weapon level fractions / 100; max this should ever be is 7

            $C0/4963 A9 07       LDA #$07                A:0000 X:0000 Y:FFFF P:envMxdIZc - loop counter
            $C0/4965 85 A6       STA $A6    [$00:03A6]   A:0007 X:0000 Y:FFFF P:envMxdIzc
            $C0/4967 A9 00       LDA #$00                A:0007 X:0000 Y:FFFF P:envMxdIzc
            $C0/4969 85 AA       STA $AA    [$00:03AA]   A:0000 X:0000 Y:FFFF P:envMxdIZc
            $C0/496B EB          XBA                     A:0000 X:0000 Y:FFFF P:envMxdIZc

            loop:
            $C0/496C A5 A6       LDA $A6    [$00:03A6]   A:0000 X:0000 Y:FFFF P:envMxdIZc
            $C0/496E 4A          LSR A                   A:0007 X:0000 Y:FFFF P:envMxdIzc
            $C0/496F 08          PHP                     A:0003 X:0000 Y:FFFF P:envMxdIzC
            $C0/4970 A8          TAY                     A:0003 X:0000 Y:FFFF P:envMxdIzC
            $C0/4971 B9 C0 E1    LDA $E1C0,y[$7E:E1C3]   A:0003 X:0000 Y:0003 P:envMxdIzC
            $C0/4974 85 A8       STA $A8    [$00:03A8]   A:0010 X:0000 Y:0003 P:envMxdIzC 
            $C0/4976 28          PLP                     A:0010 X:0000 Y:0003 P:envMxdIzC
            $C0/4977 B0 08       BCS $08    [$4981]      A:0010 X:0000 Y:0003 P:envMxdIzC
            $C0/4979 46 A8       LSR $A8    [$00:03A8]   A:0010 X:0000 Y:0003 P:envMxdIzc
            $C0/497B 46 A8       LSR $A8    [$00:03A8]   A:0010 X:0000 Y:0003 P:envMxdIzc
            $C0/497D 46 A8       LSR $A8    [$00:03A8]   A:0010 X:0000 Y:0003 P:envMxdIzc
            $C0/497F 46 A8       LSR $A8    [$00:03A8]   A:0010 X:0000 Y:0003 P:envMxdIzc
            $C0/4981 A5 A8       LDA $A8    [$00:03A8]   A:0010 X:0000 Y:0003 P:envMxdIzC
            $C0/4983 29 0F       AND #$0F                A:0010 X:0000 Y:0003 P:envMxdIzC
            $C0/4985 18          CLC                     A:0000 X:0000 Y:0003 P:envMxdIZC
            $C0/4986 65 AA       ADC $AA    [$00:03AA]   A:0000 X:0000 Y:0003 P:envMxdIZc
            $C0/4988 85 AA       STA $AA    [$00:03AA]   A:0000 X:0000 Y:0003 P:envMxdIZc - sum all whole weapon levels (0-8 each)
            $C0/498A C6 A6       DEC $A6    [$00:03A6]   A:0000 X:0000 Y:0003 P:envMxdIZc
            $C0/498C 10 DE       BPL $DE    [$496C]      A:0000 X:0000 Y:0003 P:envMxdIzc
            ... (x8)

            $C0/498E A5 AA       LDA $AA    [$00:03AA]   A:0001 X:0000 Y:0000 P:eNvMxdIzc - $AA is sum of all weapon levels; max this should ever be is 64
            $C0/4990 4A          LSR A                   A:0001 X:0000 Y:0000 P:envMxdIzc
            $C0/4991 4A          LSR A                   A:0000 X:0000 Y:0000 P:envMxdIZC
            $C0/4992 4A          LSR A                   A:0000 X:0000 Y:0000 P:envMxdIZc - divide weapon level by 8; max this should ever be is 8
            $C0/4993 18          CLC                     A:0000 X:0000 Y:0000 P:envMxdIZc
            $C0/4994 65 A4       ADC $A4    [$00:03A4]   A:0000 X:0000 Y:0000 P:envMxdIZc - add to sublevel sum from earlier (optimally 14-15ish, but 6 is a solid average, so we use that in the hack)
            $C0/4996 8D 76 CC    STA $CC76  [$7E:CC76]   A:0000 X:0000 Y:0000 P:envMxdIZc - final value - this represents the weapon charge level bonus granted by mana magic while it's active
            $C0/4999 60          RTS                     A:0000 X:0000 Y:0000 P:envMxdIZc
         */
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_MANAMAGIC_FIX))
            {
                return false;
            }

            // allow all the bullshit above to run, but then do nothing with it and just load 6 at the end.
            // LDA #06 at 4994
            outRom[0x4994] = 0xA9;
            outRom[0x4995] = 0x06;
            return true;
        }
    }
}
