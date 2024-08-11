using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.fix
{
    /// <summary>
    /// Hack to remove the animation of mech rider 1/2 driving away on death, which on some maps would cause a layer of the
    /// map to be dragged along with him.
    /// </summary>
    public class MechRiderDeathFix : RandoProcessor
    {
        protected override string getName()
        {
            return "Mech rider death fix";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_MECHRIDER_DEATH_FIX))
            {
                return false;
            }
            // this removes the entry in his table defining the animation to run when he dies
            // from the mana banks docs:
            /*
                [EB49~ECFC Mech Rider I/II/III]
                [07: Mech Rider I/II/III]
                C2/EB49:	019C
                C2/EB4B:	0000
                C2/EB4D:	B370
                C2/EB4F:	C570
                C2/EB51:	1071
                C2/EB53:	0000
                C2/EB55:	7BEB		[??]
                C2/EB57:	6BEB		[??]
                C2/EB59:	9DEB		[AI Pointers]
                C2/EB5B:	0400
                C2/EB5D:	7272
                C2/EB5F:	D0FF
                C2/EB61:	63EB		[Extra]

                [Extra]
                C2/EB63:	7D72
                C2/EB65:	C172
                C2/EB67:	FF72
                C2/EB69:	FF72
             */
            outRom[0x2EB61] = 0x00;
            outRom[0x2EB62] = 0x00;
            return true;
        }
    }
}
