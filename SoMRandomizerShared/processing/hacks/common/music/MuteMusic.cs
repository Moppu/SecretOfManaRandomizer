using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System.Linq;

namespace SoMRandomizer.processing.hacks.common.music
{
    /// <summary>
    /// set volumes to zero in vanilla songs.
    /// this mutes music while still preserving SPC loadtimes to make races more fair if this option is used.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MuteMusic : RandoProcessor
    {
        protected override string getName()
        {
            return "Mute music";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // 24 = boss death sound
            // 25 = cannon shot
            // 26 = cannon flight
            // 30 = fire sound
            // 40 = flammie summon
            // 45 = squaresoft intro
            int[] skipSongIndexes = new int[] { 24, 25, 26, 30, 40, 45 };
            for(int id=0; id < 59; id++)
            {
                if (!skipSongIndexes.Contains(id))
                {
                    int offset = DataUtil.int24FromBytes(outRom, VanillaRomOffsets.MUSIC_OFFSETS + id * 3);
                    offset -= 0xC00000;
                    int size = DataUtil.ushortFromBytes(outRom, offset);
                    // size + start + 8 channels header
                    offset += 4 + 16;
                    // look for specific music commands for volume.
                    // 0xD2 = channel volume
                    // 0xF8 = global volume
                    // 0xFD = ignore global volume by sample num
                    for (int i = offset; i < offset + 10; i++)
                    {
                        if (outRom[i] == 0xF8)
                        {
                            outRom[i + 1] = 0;
                            break;
                        }
                    }
                    for (int i = offset; i < offset + size - 18; i++)
                    {
                        if (outRom[i] == 0xFD && outRom[i + 1] == 0x25)
                        {
                            outRom[i] = 0xD2;
                            outRom[i + 1] = 0;
                        }
                        if (outRom[i] == 0xD2 && outRom[i + 1] == 0xFF)
                        {
                            outRom[i + 1] = 0;
                        }
                    }
                }
            }
            return true;
        }
    }
}
