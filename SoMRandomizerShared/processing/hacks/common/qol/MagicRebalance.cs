using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// Some changes to better balance mp cost and power of spells.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MagicRebalance : RandoProcessor
    {
        protected override string getName()
        {
            return "Magic cost/power rebalance";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_MAGIC_REBALANCE))
            {
                return false;
            }

            // --------- gnome ----------
            // 102AD8 00 earth slide
            // power 3D
            // cost 03

            // 102B18 01 gem missile
            // power 2B
            // cost 02

            // 102B58 02 slowdown
            // power 3D
            // cost 01

            // 102B98 03 stone saber
            // power 3d
            // cost 04

            // 102BD8 04 speedup
            // power 3d
            // cost 03

            // 102C18 05 defender
            // power 3d
            // cost 02


            // --------- undine ----------
            // 102C58 06 freeze
            // power 3d
            // cost 02

            // 102C98 07 acid storm
            // power 2b
            // cost 03

            // 102CD8 08 energy absorb
            // power 2b
            // cost 02
            outRom[0x102CD8 + 7] = 0x03; // change cost to 03

            // 102D18 09 ice saber
            // power 3d
            // cost 02

            // 102D58 0A remedy
            // power 3d
            // cost 01

            // 102D98 0B cure water
            // power 3d
            // cost 02


            // --------- salamando ----------
            // 102DD8 0C fireball
            // power 34
            // cost 02

            // 102E18 0D exploder
            // power 3d
            // cost 04
            outRom[0x102E18 + 7] = 0x03; // change cost to 03

            // 102E58 0E lava wave
            // power 2b
            // cost 03
            outRom[0x102E58 + 7] = 0x02; // change cost to 02

            // 102E98 0F flame saber
            // power 3d
            // cost 02

            // 102ED8 10 fire bouquet
            // power 2b
            // cost 03

            // 102F18 11 blaze wall
            // power 20
            // cost 04


            // --------- sylphid ----------
            // 102F58 12 air blast
            // power 2b
            // cost 02

            // 102F98 13 thunderbolt
            // power 3d
            // cost 04
            outRom[0x102F98 + 7] = 0x03; // change cost to 03

            // 102FD8 14 silence
            // power 3d
            // cost 02
            outRom[0x102FD8 + 7] = 0x01; // change cost to 01

            // 103018 15 thunder saber
            // power 3d
            // cost 03

            // 103058 16 balloon
            // power 3d
            // cost 02
            outRom[0x103058 + 7] = 0x03; // change cost to 03

            // 103098 17 analyzer
            // power 3d
            // cost 01


            // --------- luna ----------
            // 1030D8 18 change form
            // power 3d
            // cost 05
            outRom[0x1030D8 + 7] = 0x04; // change cost to 04

            // 103118 19 mp absorb
            // power 2b
            // cost 01

            // 103158 1A lunar magic
            // power 3d
            // cost 08
            outRom[0x103158 + 7] = 0x04; // change cost to 04

            // 103198 1B moon saber
            // power 3d
            // cost 03

            // 1031D8 1C lunar boost
            // power 3d
            // cost 02

            // 103218 1D moon energy
            // power 3d
            // cost 02


            // --------- dryad ----------
            // 103258 1E sleep flower
            // power 3d
            // cost 02

            // 103298 1F burst
            // power 64
            // cost 04

            // 1032D8 20 sprite mana magic
            // power fa
            // cost 01

            // 103318 21 revivify
            // power 3d
            // cost 0a

            // 103358 22 wall
            // power 3d
            // cost 06
            outRom[0x103358 + 7] = 0x04; // change cost to 04

            // 103398 23 girl mana magic
            // power fa
            // cost 01


            // --------- shade ----------
            // 1033D8 24 evil gate
            // power 3d
            // cost 08
            outRom[0x1033D8 + 2] = 0x64; // change power to 64 - looking back i'm pretty sure this does nothing
            outRom[0x1033D8 + 7] = 0x05; // change cost to 05

            // C8/EDBE:	A90100  	LDA #$0001		[Load #$01 into Accumulator] - no damage to bosses
            // hit for half to bosses instead of 1
            outRom[0x8EDBE] = 0x4A; // LSR
            outRom[0x8EDBF] = 0xEA;
            outRom[0x8EDC0] = 0xEA;

            // 103418 25 dark force
            // power 3d
            // cost 02
            outRom[0x103418 + 7] = 0x03; // change cost to 03

            // 103458 26 dispel magic
            // power 3d
            // cost 04
            outRom[0x103458 + 7] = 0x01; // change cost to 01


            // --------- lumina ----------
            // 103498 27 light saber
            // power 3d
            // cost 05

            // 1034D8 28 lucent beam
            // power 3d
            // cost 08
            outRom[0x1034D8 + 7] = 0x04; // change cost to 04

            // 103518 29 lucid barrier
            // power 3d
            // cost 04


            return true;
        }
    }
}
