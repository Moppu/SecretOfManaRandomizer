using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;

namespace SoMRandomizer.processing.hacks.vanillarando
{
    /// <summary>
    /// Scale enemy stats for vanilla rando mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class EnemyScaling : RandoProcessor
    {
        protected override string getName()
        {
            return "Vanilla rando enemy scaling";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            int scalePercentValue = settings.getInt(VanillaRandoSettings.PROPERTYNAME_ENEMY_SCALING);
            if(scalePercentValue == 100)
            {
                // no scale changes; drop out
                return false;
            }
            double scale = scalePercentValue / 100.0;
            // scale base stats (not hp/mp) by given amount on all 128 enemies
            // 101C00 29 bytes each
            // [4] str
            // [5] agi
            // [6] int
            // [7] wis
            // [8] eva
            // [9] def (16 bit)
            // [11] mev
            // [12] mgdef (16 bit)

            //          Lv  HP   MP   At Sp  In Ws   Ev Def  ME MDef  Tp El  XP    ??   Immn  XX  Abil DS  WL ML Luc
            // D0/29B5: 48  0A1A 63   4A 01  60 60   63 C800 63 A701  20 10  C896  2C2C FC7F  00  0000 01  5  8  7869 {79: Dark Lich} 
            //          48  0F27 63   94 02  C0 C0   C6 9001 C6 4E03  20 10  C896  2C2C FC7F  00  0000 01  5  8  7869
            // index 1,2
            double hpScale = 1.0 + ((scale - 1) / 2.0);

            int[] eightBitStats = new int[] { 4, 5, 6, 7, 8, 11, };

            int[] sixteenBitStats = new int[] { 12 };
            for (int i=0; i < 128; i++)
            {
                foreach (int eightBitStat in eightBitStats)
                {
                    byte orig = outRom[0x101C00 + i * 29 + eightBitStat];
                    int newStat = (int)((orig + 5) * scale);
                    if(newStat > 255)
                    {
                        newStat = 255;
                    }
                    outRom[0x101C00 + i * 29 + eightBitStat] = (byte)newStat;
                }
                foreach (int sixteenBitStat in sixteenBitStats)
                {
                    ushort orig = DataUtil.ushortFromBytes(outRom, 0x101C00 + i * 29 + sixteenBitStat);
                    int newStat = (int)((orig + 5) * hpScale);
                    if (newStat > 65535)
                    {
                        newStat = 65535;
                    }
                    DataUtil.ushortToBytes(outRom, 0x101C00 + i * 29 + sixteenBitStat, (ushort)newStat);
                }
                // hp
                {
                    int sixteenBitStat = 1;
                    ushort orig = DataUtil.ushortFromBytes(outRom, 0x101C00 + i * 29 + sixteenBitStat);
                    int newStat = (int)(orig * hpScale);
                    if (newStat > 65535)
                    {
                        newStat = 65535;
                    }
                    DataUtil.ushortToBytes(outRom, 0x101C00 + i * 29 + sixteenBitStat, (ushort)newStat);
                }
            }

            // enemy weapon 72->189 12 bytes each, byte [8] is the atk
            for(int i=72; i <= 189; i++)
            {
                byte orig = outRom[0x101000 + i * 12 + 8];
                int newStat = (int)(orig * scale);
                if (newStat > 255)
                {
                    newStat = 255;
                }
                outRom[0x101000 + i * 12 + 8] = (byte)newStat;
            }

            // boss weapons
            // 10bdc1, 7 bytes each, [3] is damage, 115 of them
            for(int i=0; i < 115; i++)
            {
                byte orig = outRom[0x10bdc1 + i * 7 + 3];
                int newStat = (int)(orig * scale);
                if (newStat > 255)
                {
                    newStat = 255;
                }
                outRom[0x10bdc1 + i * 7 + 3] = (byte)newStat;

            }
            return true;
        }
    }
}
