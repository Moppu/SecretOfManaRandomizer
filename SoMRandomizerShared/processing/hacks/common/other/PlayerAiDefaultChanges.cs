using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that changes the default player AI setting to the most aggressive.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class PlayerAiDefaultChanges : RandoProcessor
    {
        protected override string getName()
        {
            return "Default player AI to aggressive";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // 7ECC7A,B,C are the grid setting, left to right, top to bottom, 0x00 - 0x0F
            // 7ECC73,4,5 for some reason are the selected quadrant .. upper left is 0, lower left is 1, upper right is 2, lower right is 3
            // this second value could easily be based on the first, so i'm not sure why they store both.
            // vanilla defaults to 0x0F and quadrant 0x03, which are the most passive.  we change to 0 here for most aggressive.

            // when the character is added these just come right out of a table in ROM.

            // load of initial values:
            /*
             $C0 / 578A AF B7 57 C0 LDA $C057B7[$C0:57B7]   A:0000 X:CC5C Y:0000 P:envmxdIZc **
             $C0 / 578E 8D 73 CC    STA $CC73  [$7E:CC73]   A:0303 X:CC5C Y:0000 P:envmxdIzc
             $C0 / 5791 E2 20       SEP #$20                A:0303 X:CC5C Y:0000 P:envmxdIzc
             $C0 / 5793 AF B9 57 C0 LDA $C057B9[$C0:57B9]   A:0303 X:CC5C Y:0000 P:envMxdIzc **
             $C0 / 5797 8D 75 CC    STA $CC75  [$7E:CC75]   A:0303 X:CC5C Y:0000 P:envMxdIzc
             $C0 / 579A AF BA 57 C0 LDA $C057BA[$C0:57BA]   A:0303 X:CC5C Y:0000 P:envMxdIzc **
             $C0 / 579E 8D 7A CC    STA $CC7A  [$7E:CC7A]   A:030F X:CC5C Y:0000 P:envMxdIzc 
             $C0 / 57A1 AF BB 57 C0 LDA $C057BB[$C0:57BB]   A:030F X:CC5C Y:0000 P:envMxdIzc **
             $C0 / 57A5 8D 7B CC    STA $CC7B  [$7E:CC7B]   A:030F X:CC5C Y:0000 P:envMxdIzc
             $C0 / 57A8 AF BC 57 C0 LDA $C057BC[$C0:57BC]   A:030F X:CC5C Y:0000 P:envMxdIzc **
             $C0 / 57AC 8D 7C CC    STA $CC7C  [$7E:CC7C]   A:030F X:CC5C Y:0000 P:envMxdIzc
            */
            outRom[0x57B7] = 0x00; // quadrants
            outRom[0x57B8] = 0x00;
            outRom[0x57B9] = 0x00;

            outRom[0x57BA] = 0x00; // ai position
            outRom[0x57BB] = 0x00;
            outRom[0x57BC] = 0x00;

            // have to set this too, otherwise the initial load of the menu to change these is out of sync.
            /*
                $C7/5204 A9 0F       LDA #$0F                A:0075 X:0180 Y:0000 P:envMxdIzc
                $C7/5206 8D 57 A2    STA $A257  [$7E:A257]   A:0000 X:0180 Y:0000 P:envMxdIZc
             */
            outRom[0x75205] = 0x00; // ai grid window position to match

            return true;
        }
    }
}
