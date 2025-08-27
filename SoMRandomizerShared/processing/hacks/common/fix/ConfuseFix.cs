using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.fix
{
    /// <summary>
    /// Hack that fixes a vanilla bug where saving the game while confused would result in being permanently confused.
    /// Also removes reversal of controls when flying on flammie and using ring menus.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ConfuseFix : RandoProcessor
    {
        protected override string getName()
        {
            return "Confusion status inversion fix";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_CONFUSION_FIX))
            {
                return false;
            }

            // this part: interrupt saveram writing routine to take out saving of 7E00EE so confusion doesn't get.. well, confused.
            outRom[0x7573f] = 0x22;
            outRom[0x75740] = (byte)context.workingOffset;
            outRom[0x75741] = (byte)(context.workingOffset >> 8);
            outRom[0x75742] = (byte)((context.workingOffset >> 16) + 0xC0);

            // CPX #0E95 - x index into saveram where it happens to save this value
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x95;
            outRom[context.workingOffset++] = 0x0E;

            // BNE over [02]
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x02;

            // LDA #00 - write 0 always for the confuse setting
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;

            // STA $306000,x
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x60;
            outRom[context.workingOffset++] = 0x30;

            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // this part: check a few things before we actually do the control inversion
            /*
$00/9440 A5 43       LDA $43    [$00:0043]   A:0001 X:0003 Y:0000 P:envMXdIzc ***
$00/9442 A4 42       LDY $42    [$00:0042]   A:0001 X:0003 Y:0000 P:envMXdIzc ***
$00/9444 20 76 94    JSR $9476  [$00:9476]   A:0001 X:0003 Y:0000 P:envMXdIZc *** 9476-951b, no early returns, no other subr calls

             */

            // steal vanilla sub, and change it to long return
            List<byte> inversionSubr = new List<byte>();
            for(int i=0x9476; i <= 0x951b; i++)
            {
                inversionSubr.Add(outRom[i]);
            }

            inversionSubr[inversionSubr.Count - 1] = 0x6b;

            int inversionSubrAddress = context.workingOffset;
            foreach(byte b in inversionSubr)
            {
                outRom[context.workingOffset++] = b;
            }

            outRom[0x9440] = 0x22;
            outRom[0x9441] = (byte)context.workingOffset;
            outRom[0x9442] = (byte)(context.workingOffset >> 8);
            outRom[0x9443] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x9444] = 0xEA;
            outRom[0x9445] = 0xEA;
            outRom[0x9446] = 0xEA;

            // new subroutine: check some shit first
            // 7E00EC is 0 when controlling character, 0x8 in menu, 0x80 on flammie.  only process confusion controls when it's 0.

            // LDA 7E00EC
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xEC;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;
            // BEQ over
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x05;
            // LDA $43 (replaced code)
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0x43;
            // LDY $42 (replaced code)
            outRom[context.workingOffset++] = 0xA4;
            outRom[context.workingOffset++] = 0x42;
            // RTL - skip doing control inversion
            outRom[context.workingOffset++] = 0x6B;
            // over: (normal stuff)
            // LDA $43 (replaced code)
            outRom[context.workingOffset++] = 0xA5;
            outRom[context.workingOffset++] = 0x43;
            // LDY $42 (replaced code)
            outRom[context.workingOffset++] = 0xA4;
            outRom[context.workingOffset++] = 0x42;
            // JSL inversionSubrAddress - basically run vanilla code to swap controls for confusion
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)inversionSubrAddress;
            outRom[context.workingOffset++] = (byte)(inversionSubrAddress >> 8);
            outRom[context.workingOffset++] = (byte)((inversionSubrAddress >> 16) + 0xC0);
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
