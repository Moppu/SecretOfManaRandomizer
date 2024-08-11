using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.fix
{
    /// <summary>
    /// Hack that fixes an event glitch that sometimes caused softlocks in the vanilla boss death/victory sequence.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class BossDeathFix : RandoProcessor
    {
        protected override string getName()
        {
            return "Boss death softlock fix";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_BOSS_DEATH_FIX))
            {
                return false;
            }
            // event 0x7f8.  this is a really long event, and we just want to replace a 0x03 "move everyone to p1" command
            // at the very end of the event with a 0x00 as a no-op, since this can sometimes hang up forever trying to
            // move the characters.
            List<byte> vanillaData = VanillaEventUtil.getVanillaEvent(origRom, 0x7f8);
            vanillaData[vanillaData.Count - 2] = 0x00;
            context.replacementEvents[0x7f8] = vanillaData;
            return true;
        }
    }
}
