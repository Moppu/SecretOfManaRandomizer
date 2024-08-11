using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using System.Linq;

namespace SoMRandomizer.processing.hacks.common.util
{
    /// <summary>
    /// Hack that sets initial RAM values necessary for a rando run.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class InitialValues : RandoProcessor
    {
        protected override string getName()
        {
            return "Initialization of values for start of run";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(context.initialValues.Count == 0)
            {
                Logging.log("No initial values to set");
                return false;
            }
            /*
$C0/245A A9 00       LDA #$00                A:2003 X:780C Y:7566 P:envMxdIzC
$C0/245C EB          XBA                     A:2000 X:780C Y:7566 P:envMxdIZC
$C0/245D B9 00 00    LDA $0000,y[$C7:7566]   A:0020 X:780C Y:7566 P:envMxdIzC
             */
            // compare y to 0x7566 like we do in autosave to recognize starting menu
            outRom[0x245A] = 0x22;
            outRom[0x245B] = (byte)(context.workingOffset);
            outRom[0x245C] = (byte)(context.workingOffset >> 8);
            outRom[0x245D] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x245E] = 0xEA;
            outRom[0x245F] = 0xEA;


            // replaced code should be last

            // CPY #7566
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0x66;
            outRom[context.workingOffset++] = 0x75;
            // BEQ doStuff
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x07;
            // (replaced code)
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xEB;
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // doStuff:
            // (for each value)
            // LDA #value
            // STA offset
            foreach (int addr in context.initialValues.Keys)
            {
                byte value = context.initialValues[addr];
                Logging.log("Initial value addr=" + addr.ToString("X6") + " value=" + value.ToString("X2"), "debug");
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = value;
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = (byte)(addr);
                outRom[context.workingOffset++] = (byte)(addr >> 8);
                outRom[context.workingOffset++] = (byte)((addr >> 16));
            }
            // (replaced code)
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xEB;
            outRom[context.workingOffset++] = 0xB9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            return true;
        }
    }
}
