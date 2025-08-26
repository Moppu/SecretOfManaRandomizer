using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that makes gigas bosses never turn into their indestructible flying dust forms.
    /// Has the side effect of making these fights a little more aggressive.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class GigasesNeverSplit : RandoProcessor
    {
        protected override string getName()
        {
            return "Gigases never split into those sparkly things";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_GIGAS_FIX))
            {
                return false;
            }

            // this is a small change to the table of gigas actions.  see below for the original bank doc:
            /*
                [03: Frost/Fire/Thunder Gigas]
                C2/DF9E:	119C
                C2/DFA0:	0000
                C2/DFA2:	B862
                C2/DFA4:	C262
                C2/DFA6:	0F63
                C2/DFA8:	0000
                C2/DFAA:	B8DF		[??]
                C2/DFAC:	BADF		[??]
                C2/DFAE:	E7DF		[AI Pointer]
                C2/DFB0:	0400
                C2/DFB2:	AB63
                C2/DFB4:	E0FF
                C2/DFB6:	0000

                [??]
                C2/DFB8:	00FF

                [??]
                C2/DFBA:	10FF 00FF 02FF 01FF 0412 0605 13FF 14FF   ** 04 -> FF
                C2/DFCA:	07FF 08FF 0BFF 0CFF 16FF 17FF 1918 FF19
                C2/DFDA:	0DFF 190E FF1A FF1B FF1C FF1D FF
             */
            outRom[0x2DFC2] = 0xFF;
            return true;
        }
    }
}
