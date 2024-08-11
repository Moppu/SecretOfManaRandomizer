using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.common.SomVanillaValues;

namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// Randomization data for each vanilla boss map - what bosses can be there, where they have to be positioned, etc.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class VanillaBossMaps
    {
        // ---------------------------------------------------------------------------
        // mantis ant
        // ---------------------------------------------------------------------------
        public static VanillaBossMap MANTISANT = new VanillaBossMap
            (
            "Mantis Ant",
            // og map / obj id
            MAPNUM_MANTISANT_ARENA, 0, 
            // supported boss ids for swapping
            new byte[] {
                /*SomVanillaValues.BOSSID_MANTISANT, SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(11 * 16, 9 * 16), new XyPos(9 * 16, 17 * 16), new XyPos(17 * 16, 8 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(11 * 16, 11 * 16),
            new XyPos[] { new XyPos(11 * 16, 11 * 16), new XyPos(9 * 16, 18 * 16), new XyPos(17 * 16, 10 * 16), new XyPos(8 * 16, 20 * 16), new XyPos(9 * 16, 12 * 16) }.ToList(),
            new XyPos[] { new XyPos(9 * 16, 18 * 16), new XyPos(17 * 16, 10 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(11 * 16, 9 * 16), new XyPos(9 * 16, 17 * 16), new XyPos(17 * 16, 8 * 16) }.ToList(),
            new XyPos[] { new XyPos(6 * 16, 13 * 16), new XyPos(19 * 16, 10 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(16, 18),
            // boss pos
            new XyPos(13, 14),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // wall face
        // ---------------------------------------------------------------------------
        public static VanillaBossMap WALLFACE = new VanillaBossMap
            (
            "Wall Face",
            // og map / obj id
            MAPNUM_WALLFACE_ARENA, 0, 
            // supported boss ids for swapping
            new byte[] {
                /*SomVanillaValues.BOSSID_MANTISANT,*/ SomVanillaValues.BOSSID_WALLFACE, /*SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,*/
                /*SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1,*/ SomVanillaValues.BOSSID_DOOMSWALL, /*SomVanillaValues.BOSSID_VAMPIRE,*/
                /*SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,*/
                /*SomVanillaValues.BOSSID_LIMESLIME, SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA,*/ SomVanillaValues.BOSSID_WATERMELON, /*SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,*/
                /*SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,*/ SomVanillaValues.BOSSID_SNOWDRAGON, /*SomVanillaValues.BOSSID_FIREGIGAS,*/ SomVanillaValues.BOSSID_REDDRAGON, /*SomVanillaValues.BOSSID_AXEBEAK,*/
                SomVanillaValues.BOSSID_BLUEDRAGON, /*SomVanillaValues.BOSSID_BUFFY,*/ SomVanillaValues.BOSSID_DARKLICH, /*SomVanillaValues.BOSSID_DRAGONWORM, SomVanillaValues.BOSSID_DREADSLIME, SomVanillaValues.BOSSID_THUNDERGIGAS,*/
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, SomVanillaValues.BOSSID_WALLFACE, SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, SomVanillaValues.BOSSID_DOOMSWALL, SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/ SomVanillaValues.BOSSID_BLUESPIKE, /*SomVanillaValues.BOSSID_HYDRA,*/ SomVanillaValues.BOSSID_WATERMELON, SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3, SomVanillaValues.BOSSID_SNOWDRAGON, SomVanillaValues.BOSSID_FIREGIGAS, SomVanillaValues.BOSSID_REDDRAGON, SomVanillaValues.BOSSID_AXEBEAK,
                SomVanillaValues.BOSSID_BLUEDRAGON, SomVanillaValues.BOSSID_BUFFY, SomVanillaValues.BOSSID_DARKLICH, SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(14 * 16, 12 * 16), new XyPos(9 * 16, 15 * 16), new XyPos(18 * 16, 14 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(14 * 16, 12 * 16),
            new XyPos[] { new XyPos(10 * 16, 11 * 16), new XyPos(17 * 16, 11 * 16), new XyPos(18 * 16, 18 * 16), new XyPos(12 * 16, 15 * 16), new XyPos(15 * 16, 12 * 16), new XyPos(13 * 16, 10 * 16), new XyPos(9 * 16, 19 * 16) }.ToList(),
            new XyPos[] { new XyPos(9 * 16, 15 * 16), new XyPos(18 * 16, 14 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(14 * 16, 12 * 16), new XyPos(9 * 16, 10 * 16), new XyPos(18 * 16, 16 * 16) }.ToList(),
            new XyPos[] { new XyPos(10 * 16, 18 * 16), new XyPos(17 * 16, 13 * 16) }.ToList(),
            // y position at which the wall boss kills you
            0x118,
            // entry pos
            new XyPos(14, 18),
            // boss pos
            new XyPos(14, 6),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                objsOffset += 8; // skip map header
                objsOffset += bossMap.originalObjNum * 8;
                objsOffset += 2; // index for x/y
                // adjust wall position if it spawns here
                if (bossId == SomVanillaValues.BOSSID_WALLFACE || bossId == SomVanillaValues.BOSSID_DOOMSWALL)
                {
                    // x,y
                    rom[objsOffset] = 14;
                    rom[objsOffset + 1] = 6;
                }
                else
                {
                    // x,y
                    rom[objsOffset] = 14;
                    rom[objsOffset + 1] = 10;
                }
            }
            );

        // ---------------------------------------------------------------------------
        // tropicallo
        // ---------------------------------------------------------------------------
        public static VanillaBossMap TROPICALLO = new VanillaBossMap
            (
            "Tropicallo",
            // og map / obj id
            MAPNUM_TROPICALLO_ARENA, 0, 
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE, SomVanillaValues.BOSSID_TROPICALLO,*/SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(22 * 16, 36 * 16), new XyPos(18 * 16, 35 * 16), new XyPos(29 * 16, 34 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(22 * 16, 36 * 16),
            new XyPos[] { new XyPos(22 * 16, 34 * 16), new XyPos(17 * 16, 40 * 16), new XyPos(24 * 16, 42 * 16), new XyPos(30 * 16, 40 * 16), new XyPos(32 * 16, 37 * 16) }.ToList(),
            new XyPos[] { new XyPos(18 * 16, 35 * 16), new XyPos(29 * 16, 34 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(22 * 16, 36 * 16), new XyPos(18 * 16, 36 * 16), new XyPos(28 * 16, 34 * 16) }.ToList(),
            new XyPos[] { new XyPos(21 * 16, 34 * 16), new XyPos(27 * 16, 38 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(18 + 6, 13 + 28),
            // boss pos
            new XyPos(14 + 6, 10 + 28),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // modify parts of the boss arena map piece to allow you to walk inside
                rom[0xDD632] = 0xAF;
                rom[0xDD637] = 0xAF;
                rom[0xDD63D] = 0x14;
                rom[0xDD650] = 0x2B;
                rom[0xDD668] = 0x2B;
                rom[0xDD672] = 0x2B;
                rom[0xDD687] = 0x2B;
                rom[0xDD684] = 0x6A;
            }
            );

        // ---------------------------------------------------------------------------
        // minotaur
        // ---------------------------------------------------------------------------
        public static VanillaBossMap MINOTAUR = new VanillaBossMap
            (
            "Minotaur",
            // og map / obj id
            MAPNUM_MINOTAUR_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO,/*SomVanillaValues.BOSSID_MINOTAUR,*/SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(11 * 16, 14 * 16), new XyPos(10 * 16, 16 * 16), new XyPos(21 * 16, 19 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(11 * 16, 14 * 16),
            new XyPos[] { new XyPos(13 * 16, 13 * 16), new XyPos(11 * 16, 22 * 16), new XyPos(19 * 16, 16 * 16), new XyPos(18 * 16, 24 * 16), new XyPos(14 * 16, 18 * 16) }.ToList(),
            new XyPos[] { new XyPos(10 * 16, 16 * 16), new XyPos(21 * 16, 19 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(11 * 16, 14 * 16), new XyPos(9 * 16, 17 * 16), new XyPos(20 * 16, 22 * 16) }.ToList(),
            new XyPos[] { new XyPos(8 * 16, 11 * 16), new XyPos(23 * 16, 25 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(16, 23),
            // boss pos
            new XyPos(16, 17),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // spikey
        // ---------------------------------------------------------------------------
        public static VanillaBossMap SPIKEY = new VanillaBossMap
            (
            "Spikey Tiger",
            // og map / obj id
            MAPNUM_SPIKEY_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR,/*SomVanillaValues.BOSSID_SPIKEY,*/SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(13 * 16, 14 * 16), new XyPos(7 * 16, 12 * 16), new XyPos(19 * 16, 12 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(13 * 16, 13 * 16),
            new XyPos[] { new XyPos(10 * 16, 19 * 16), new XyPos(15 * 16, 15 * 16), new XyPos(11 * 16, 13 * 16), new XyPos(14 * 16, 22 * 16), new XyPos(13 * 16, 18 * 16) }.ToList(),
            new XyPos[] { new XyPos(7 * 16, 13 * 16), new XyPos(19 * 16, 13 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(12 * 16, 14 * 16), new XyPos(14 * 16, 19 * 16), new XyPos(19 * 16, 13 * 16), new XyPos(7 * 16, 13 * 16) }.ToList(),
            new XyPos[] { new XyPos(11 * 16 + 8, 10 * 16), new XyPos(14 * 16 + 8, 10 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(13, 22),
            // boss pos
            new XyPos(13, 13),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                if (bossId == SomVanillaValues.BOSSID_SPRINGBEAK || bossId == SomVanillaValues.BOSSID_AXEBEAK)
                {
                    // disable beak hitbox. note this happens for all bosses of this type if one appears on this map
                    rom[0x7F7F0] = 0;
                    rom[0x7F7F1] = 0;
                }

                // snakes here hit you immediately, so move them
                if (bossId == SomVanillaValues.BOSSID_GREATVIPER || bossId == SomVanillaValues.BOSSID_DRAGONWORM)
                {
                    int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                    objsOffset += 8; // skip map header
                    objsOffset += bossMap.originalObjNum * 8;
                    // index for y
                    objsOffset += 3; 
                    // move y up
                    rom[objsOffset]--;
                }
            }
            );

        // ---------------------------------------------------------------------------
        // jabberwocky
        // ---------------------------------------------------------------------------
        public static VanillaBossMap JABBER = new VanillaBossMap
            (
            "Jabberwocky",
            // og map / obj id
            MAPNUM_JABBERWOCKY_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY,/*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(17 * 16, 37 * 16), new XyPos(12 * 16, 39 * 16), new XyPos(21 * 16, 43 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(16 * 16, 38 * 16),
            new XyPos[] { new XyPos(21 * 16, 36 * 16), new XyPos(14 * 16, 44 * 16), new XyPos(12 * 16, 36 * 16), new XyPos(21 * 16, 42 * 16), new XyPos(16 * 16, 36 * 16) }.ToList(),
            new XyPos[] { new XyPos(12 * 16, 39 * 16), new XyPos(21 * 16, 43 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(14 * 16, 37 * 16), new XyPos(17 * 16, 38 * 16), new XyPos(22 * 16, 39 * 16) }.ToList(),
            new XyPos[] { new XyPos(15 * 16, 34 * 16), new XyPos(18 * 16, 34 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(16, 19 + 26),
            // boss pos
            new XyPos(17, 12 + 26),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // spring beak
        // ---------------------------------------------------------------------------
        public static VanillaBossMap SPRINGBEAK = new VanillaBossMap
            (
            "Spring Beak",
            // og map / obj id
            MAPNUM_SPRINGBEAK_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                /*SomVanillaValues.BOSSID_SPRINGBEAK,*/SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(15 * 16, 14 * 16), new XyPos(8 * 16, 13 * 16), new XyPos(21 * 16, 14 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(15 * 16, 14 * 16),
            new XyPos[] { new XyPos(9 * 16, 9 * 16), new XyPos(15 * 16, 11 * 16), new XyPos(20 * 16, 10 * 16), new XyPos(9 * 16, 19 * 16), new XyPos(15 * 16, 18 * 16), new XyPos(8 * 16, 13 * 16), new XyPos(21 * 16, 14 * 16) }.ToList(),
            new XyPos[] { new XyPos(8 * 16, 13 * 16), new XyPos(21 * 16, 14 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(15 * 16, 14 * 16), new XyPos(8 * 16, 13 * 16), new XyPos(21 * 16, 14 * 16) }.ToList(),
            new XyPos[] { new XyPos(11 * 16, 16 * 16), new XyPos(17 * 16, 18 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(18, 21),
            // boss pos
            new XyPos(15, 10),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // frost gigas
        // ---------------------------------------------------------------------------
        public static VanillaBossMap FROSTGIGAS = new VanillaBossMap
            (
            "Frost Gigas",
            // og map / obj id
            MAPNUM_FROSTGIGAS_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK,/*SomVanillaValues.BOSSID_FROSTGIGAS,*/SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(10 * 16, 12 * 16), new XyPos(6 * 16, 7 * 16), new XyPos(13 * 16, 7 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(9 * 16, 13 * 16),
            new XyPos[] { new XyPos(12 * 16, 19 * 16), new XyPos(7 * 16, 18 * 16), new XyPos(14 * 16, 15 * 16), new XyPos(9 * 16, 11 * 16), new XyPos(5 * 16, 14 * 16), new XyPos(9 * 16, 16 * 16), new XyPos(15 * 16, 14 * 16) }.ToList(),
            new XyPos[] { new XyPos(6 * 16, 7 * 16), new XyPos(13 * 16, 7 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(10 * 16, 11 * 16), new XyPos(9 * 16, 6 * 16), new XyPos(8 * 16, 16 * 16) }.ToList(),
            new XyPos[] { new XyPos(4 * 16, 12 * 16), new XyPos(15 * 16, 12 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(10, 19),
            // boss pos
            new XyPos(10, 12),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // snap dragon
        // ---------------------------------------------------------------------------
        public static VanillaBossMap SNAPDRAGON = new VanillaBossMap
            (
            "Snap Dragon",
            // og map / obj id
            MAPNUM_SNAPDRAGON_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS,/*SomVanillaValues.BOSSID_SNAPDRAGON,*/SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(24 * 16, 26 * 16), new XyPos(19 * 16, 26 * 16), new XyPos(28 * 16, 26 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(24 * 16, 26 * 16),
            new XyPos[] { new XyPos(26 * 16, 23 * 16), new XyPos(24 * 16, 25 * 16), new XyPos(21 * 16, 28 * 16), new XyPos(26 * 16, 28 * 16), new XyPos(21 * 16, 24 * 16), new XyPos(19 * 16, 26 * 16), new XyPos(29 * 16, 26 * 16) }.ToList(),
            new XyPos[] { new XyPos(21 * 16, 24 * 16), new XyPos(26 * 16, 28 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(21 * 16, 23 * 16), new XyPos(26 * 16, 25 * 16), new XyPos(23 * 16, 28 * 16) }.ToList(),
            new XyPos[] { new XyPos(18 * 16, 25 * 16), new XyPos(29 * 16, 25 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(23, 15 + 14),
            // boss pos
            new XyPos(23, 12 + 14),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { 2, 3, 4 }, new int[] { 1, 2, 3 }, // the bridge pieces connecting to the middle
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // snakes
                if (bossId == SomVanillaValues.BOSSID_GREATVIPER || bossId == SomVanillaValues.BOSSID_DRAGONWORM)
                {
                    // move up
                    int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                    objsOffset += 8; // skip map header
                    objsOffset += bossMap.originalObjNum * 8;
                    // index for x/y
                    objsOffset += 2;
                    // shift y position so it's not on top of you
                    rom[objsOffset + 1] = 23;
                }
            }
            );

        // ---------------------------------------------------------------------------
        // mech rider 1
        // ---------------------------------------------------------------------------
        public static VanillaBossMap MECHRIDER1 = new VanillaBossMap
            (
            "Mech Rider 1",
            // og map / obj id
            MAPNUM_MECHRIDER1_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS,SomVanillaValues.BOSSID_SNAPDRAGON,/*SomVanillaValues.BOSSID_MECHRIDER1, SomVanillaValues.BOSSID_DOOMSWALL,SomVanillaValues.BOSSID_VAMPIRE,*/
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, /*SomVanillaValues.BOSSID_HYDRA, SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,SomVanillaValues.BOSSID_BUFFY,SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,SomVanillaValues.BOSSID_VAMPIRE,*/
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE,/*SomVanillaValues.BOSSID_HYDRA, SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,SomVanillaValues.BOSSID_BUFFY,SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(19 * 16, 15 * 16), new XyPos(10 * 16, 14 * 16), new XyPos(28 * 16, 16 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(19 * 16, 15 * 16),
            new XyPos[] { new XyPos(14 * 16, 19 * 16), new XyPos(24 * 16, 20 * 16), new XyPos(18 * 16, 17 * 16), new XyPos(15 * 16, 13 * 16), new XyPos(26 * 16, 14 * 16), new XyPos(17 * 16, 22 * 16), new XyPos(21 * 16, 22 * 16) }.ToList(),
            new XyPos[] { new XyPos(10 * 16, 14 * 16), new XyPos(28 * 16, 16 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(19 * 16, 15 * 16), new XyPos(10 * 16, 14 * 16), new XyPos(28 * 16, 16 * 16) }.ToList(),
            new XyPos[] { new XyPos(22 * 16, 21 * 16), new XyPos(15 * 16, 22 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(7, 12),
            // boss pos
            new XyPos(15, 12),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // doom's wall
        // ---------------------------------------------------------------------------
        public static VanillaBossMap DOOMSWALL = new VanillaBossMap
            (
            "Doom's Wall",
            // og map / obj id
            MAPNUM_DOOMSWALL_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                /*SomVanillaValues.BOSSID_MANTISANT,*/ SomVanillaValues.BOSSID_WALLFACE, /*SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,*/
                /*SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1,*/ SomVanillaValues.BOSSID_DOOMSWALL, /*SomVanillaValues.BOSSID_VAMPIRE,*/
                /*SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,*/
                /*SomVanillaValues.BOSSID_LIMESLIME, SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, SomVanillaValues.BOSSID_WATERMELON, SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,*/
                /*SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3, SomVanillaValues.BOSSID_SNOWDRAGON, SomVanillaValues.BOSSID_FIREGIGAS, SomVanillaValues.BOSSID_REDDRAGON, SomVanillaValues.BOSSID_AXEBEAK,*/
                /*SomVanillaValues.BOSSID_BLUEDRAGON, SomVanillaValues.BOSSID_BUFFY, SomVanillaValues.BOSSID_DARKLICH, SomVanillaValues.BOSSID_DRAGONWORM, SomVanillaValues.BOSSID_DREADSLIME, SomVanillaValues.BOSSID_THUNDERGIGAS,*/
            },
            // all supported boss ids
            new byte[] {
                /*SomVanillaValues.BOSSID_MANTISANT,*/ SomVanillaValues.BOSSID_WALLFACE, /*SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,*/
                /*SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1,*/ SomVanillaValues.BOSSID_DOOMSWALL, /*SomVanillaValues.BOSSID_VAMPIRE,*/
                /*SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,*/
                /*SomVanillaValues.BOSSID_LIMESLIME, SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, SomVanillaValues.BOSSID_WATERMELON, SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,*/
                /*SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3, SomVanillaValues.BOSSID_SNOWDRAGON, SomVanillaValues.BOSSID_FIREGIGAS, SomVanillaValues.BOSSID_REDDRAGON, SomVanillaValues.BOSSID_AXEBEAK,*/
                /*SomVanillaValues.BOSSID_BLUEDRAGON, SomVanillaValues.BOSSID_BUFFY, SomVanillaValues.BOSSID_DARKLICH, SomVanillaValues.BOSSID_DRAGONWORM, SomVanillaValues.BOSSID_DREADSLIME, SomVanillaValues.BOSSID_THUNDERGIGAS,*/
            },
            // spikey positions - null since we don't support spikey here
            null, null, null,
            // spring beak positions - center, jump, podium
            null,
            null,
            null,
            // tropicallo positions - main, brambler
            null,
            null,
            // y position at which the wall boss kills you
            0x118,
            // entry pos
            new XyPos(14, 18),
            // boss pos
            new XyPos(14, 8),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // wall position
                if (bossId == SomVanillaValues.BOSSID_WALLFACE || bossId == SomVanillaValues.BOSSID_DOOMSWALL)
                {
                    int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                    objsOffset += 8; // skip map header
                    objsOffset += bossMap.originalObjNum * 8;
                    objsOffset += 2; // index for x/y
                    // x,y
                    rom[objsOffset] = 14;
                    rom[objsOffset + 1] = 8;
                }
            }
            );

        // ---------------------------------------------------------------------------
        // vampire
        // ---------------------------------------------------------------------------
        public static VanillaBossMap VAMPIRE = new VanillaBossMap
            (
            "Vampire",
            // og map / obj id
            MAPNUM_VAMPIRE_ARENA, 2,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,SomVanillaValues.BOSSID_VAMPIRE,*/
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(14 * 16, 18 * 16), new XyPos(9 * 16, 21 * 16), new XyPos(18 * 16, 21 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(14 * 16, 18 * 16),
            new XyPos[] { new XyPos(11 * 16, 17 * 16), new XyPos(16 * 16, 17 * 16), new XyPos(11 * 16, 23 * 16), new XyPos(16 * 16, 23 * 16), new XyPos(19 * 16, 25 * 16), new XyPos(8 * 16, 25 * 16), new XyPos(13 * 16, 20 * 16) }.ToList(),
            new XyPos[] { new XyPos(9 * 16, 21 * 16), new XyPos(18 * 16, 21 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(14 * 16, 20 * 16), new XyPos(9 * 16, 17 * 16), new XyPos(18 * 16, 23 * 16) }.ToList(),
            new XyPos[] { new XyPos(10 * 16, 14 * 16), new XyPos(17 * 16, 14 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(14, 26),
            // boss pos
            new XyPos(14, 18),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 2 collision
            true,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // 85330 - map 408 pieces -> 8852A
                // FE 00 0F D5 00 00 FF 00 0F 6C 00 01
                // -> FE 00 0F 6C 00 01
                // change whole block to FE 00 0F 6C 00 01 FF FE 0F 6C 00 01
                // map 408 header - 87330 -> 8DA58
                // 89 4F 89 00 28 08 00 FF
                // collision -> layer1
                // layer 1 -> foreground - [2] 0x40 on
                // mosaic off - display settings 5? - [5] lower 6 bits
                // [4] is the scrolling?  set to 0
                // -> 89 4F C9 00 00 05 00 FF
                // door for layer 2 collision - 83c62 0x01
                int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                rom[objsOffset + 2] |= 0x40; // layer 1 use FG tiles
                rom[objsOffset + 4] = 0; // scrolly off
                rom[objsOffset + 5] = 5; // disp settings; no mosaic

                int piecesOffset = VanillaMapUtil.getPiecePlacementOffset(rom, bossMap.originalMapNum);
                // show layer 2 piece on layer 1, and nothing on layer 2
                rom[piecesOffset + 0] = 0xFE;
                rom[piecesOffset + 1] = 0x00;
                rom[piecesOffset + 2] = 0x0F;
                rom[piecesOffset + 3] = 0x6C;
                rom[piecesOffset + 4] = 0x00;
                rom[piecesOffset + 5] = 0x01;
                rom[piecesOffset + 6] = 0xFF;
                rom[piecesOffset + 7] = 0xFE;
                rom[piecesOffset + 8] = 0x0F;
                rom[piecesOffset + 9] = 0x6C;
                rom[piecesOffset + 10] = 0x00;
                rom[piecesOffset + 11] = 0x01;
            }
            );

        // ---------------------------------------------------------------------------
        // metal mantis
        // ---------------------------------------------------------------------------
        public static VanillaBossMap METALMANTIS = new VanillaBossMap
            (
            "Metal Mantis",
            // og map / obj id
            MAPNUM_METALMANTIS_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                /*SomVanillaValues.BOSSID_METALMANTIS,*/SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, /*SomVanillaValues.BOSSID_HYDRA, SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, /*SomVanillaValues.BOSSID_HYDRA, SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(15 * 16, 13 * 16), new XyPos(12 * 16, 12 * 16), new XyPos(18 * 16, 12 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(15 * 16, 13 * 16),
            new XyPos[] { new XyPos(15 * 16, 19 * 16), new XyPos(10 * 16, 15 * 16), new XyPos(19 * 16, 16 * 16), new XyPos(18 * 16, 14 * 16), new XyPos(13 * 16, 16 * 16), new XyPos(11 * 16, 13 * 16), new XyPos(19 * 16, 13 * 16) }.ToList(),
            new XyPos[] { new XyPos(11 * 16, 16 * 16), new XyPos(19 * 16, 16 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(12 * 16, 12 * 16), new XyPos(18 * 16, 12 * 16), new XyPos(15 * 16, 19 * 16) }.ToList(),
            new XyPos[] { new XyPos(13 * 16, 10 * 16), new XyPos(17 * 16, 10 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(15, 20),
            // boss pos
            new XyPos(12, 16),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // mech rider 2
        // ---------------------------------------------------------------------------
        public static VanillaBossMap MECHRIDER2 = new VanillaBossMap
            (
            "Mech Rider 2",
            // og map / obj id
            MAPNUM_MECHRIDER2_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS,SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1,/*SomVanillaValues.BOSSID_DOOMSWALL,SomVanillaValues.BOSSID_VAMPIRE,*/
                SomVanillaValues.BOSSID_METALMANTIS,/*SomVanillaValues.BOSSID_MECHRIDER2,*/SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, /*SomVanillaValues.BOSSID_HYDRA, SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,SomVanillaValues.BOSSID_BUFFY,SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,SomVanillaValues.BOSSID_VAMPIRE,*/
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE,/*SomVanillaValues.BOSSID_HYDRA, SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,SomVanillaValues.BOSSID_BUFFY,SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(24 * 16, 19 * 16), new XyPos(14 * 16, 19 * 16), new XyPos(33 * 16, 19 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(24 * 16, 21 * 16),
            new XyPos[] { new XyPos(16 * 16, 19 * 16), new XyPos(35 * 16, 21 * 16), new XyPos(21 * 16, 21 * 16), new XyPos(32 * 16, 19 * 16), new XyPos(21 * 16, 18 * 16), new XyPos(28 * 16, 18 * 16), new XyPos(24 * 16, 21 * 16) }.ToList(),
            new XyPos[] { new XyPos(14 * 16, 21 * 16), new XyPos(33 * 16, 21 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(14 * 16, 19 * 16), new XyPos(33 * 16, 19 * 16), new XyPos(24 * 16, 19 * 16) }.ToList(),
            new XyPos[] { new XyPos(15 * 16, 17 * 16), new XyPos(32 * 16, 17 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(8, 20),
            // boss pos
            new XyPos(16, 20),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // kilroy
        // ---------------------------------------------------------------------------
        public static VanillaBossMap KILROY = new VanillaBossMap
            (
            "Kilroy",
            // og map / obj id
            MAPNUM_KILROY_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS,SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1,/*SomVanillaValues.BOSSID_DOOMSWALL,SomVanillaValues.BOSSID_VAMPIRE,*/
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2,/*SomVanillaValues.BOSSID_KILROY,*/SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, /*SomVanillaValues.BOSSID_HYDRA, SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,SomVanillaValues.BOSSID_BUFFY,SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,SomVanillaValues.BOSSID_VAMPIRE,*/
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE,/*SomVanillaValues.BOSSID_HYDRA, SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,SomVanillaValues.BOSSID_BUFFY,SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(19 * 16, 22 * 16), new XyPos(15 * 16, 22 * 16), new XyPos(23 * 16, 22 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(19 * 16, 22 * 16),
            new XyPos[] { new XyPos(18 * 16, 22 * 16), new XyPos(15 * 16, 21 * 16), new XyPos(23 * 16, 23 * 16), new XyPos(19 * 16, 24 * 16), new XyPos(20 * 16, 25 * 16), new XyPos(19 * 16, 18 * 16), new XyPos(23 * 16, 19 * 16) }.ToList(),
            new XyPos[] { new XyPos(15 * 16, 22 * 16), new XyPos(23 * 16, 22 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(19 * 16, 22 * 16), new XyPos(15 * 16, 18 * 16), new XyPos(23 * 16, 26 * 16) }.ToList(),
            new XyPos[] { new XyPos(23 * 16, 18 * 16), new XyPos(15 * 16, 26 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(16, 23),
            // boss pos
            new XyPos(16, 16),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // adjust its background scrolling - header[4] |= 0x06 - 8CC6C
                int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                objsOffset += 4; // index for scroll pos in header
                rom[objsOffset] |= 0x06;
            }
            );

        // ---------------------------------------------------------------------------
        // gorgon bull
        // ---------------------------------------------------------------------------
        public static VanillaBossMap GORGON = new VanillaBossMap
            (
            "Gorgon Bull",
            // og map / obj id
            MAPNUM_GORGONBULL_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY,/*SomVanillaValues.BOSSID_GORGON,*/SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(15 * 16, 18 * 16), new XyPos(8 * 16, 19 * 16), new XyPos(22 * 16, 15 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(15 * 16, 18 * 16),
            new XyPos[] { new XyPos(9 * 16, 15 * 16), new XyPos(12 * 16, 25 * 16), new XyPos(20 * 16, 23 * 16), new XyPos(18 * 16, 20 * 16), new XyPos(16 * 16, 16 * 16) }.ToList(),
            new XyPos[] { new XyPos(8 * 16, 19 * 16), new XyPos(22 * 16, 15 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(8 * 16, 19 * 16), new XyPos(15 * 16, 18 * 16), new XyPos(22 * 16, 15 * 16) }.ToList(),
            new XyPos[] { new XyPos(13 * 16, 10 * 16), new XyPos(16 * 16, 10 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(14, 24),
            // boss pos
            new XyPos(14, 14),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { 5, 6, }, new int[] { 2, 3, }, // remove the stairway tower in the middle
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // boreal face
        // ---------------------------------------------------------------------------
        public static VanillaBossMap BOREAL = new VanillaBossMap
            (
            "Boreal Face",
            // og map / obj id
            MAPNUM_BOREALFACE_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON,/*SomVanillaValues.BOSSID_BOREAL,*/SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(11 * 16, 10 * 16), new XyPos(11 * 16, 21 * 16), new XyPos(21 * 16, 10 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(11 * 16, 21 * 16),
            new XyPos[] { new XyPos(14 * 16, 23 * 16), new XyPos(21 * 16, 19 * 16), new XyPos(10 * 16, 18 * 16), new XyPos(21 * 16, 18 * 16), new XyPos(17 * 16, 22 * 16) }.ToList(),
            new XyPos[] { new XyPos(21 * 16, 21 * 16), new XyPos(11 * 16, 11 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(11 * 16, 10 * 16), new XyPos(20 * 16, 10 * 16), new XyPos(11 * 16, 21 * 16), new XyPos(21 * 16, 21 * 16) }.ToList(),
            new XyPos[] { new XyPos(11 * 16 + 8, 16 * 16), new XyPos(20 * 16 + 8, 16 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(16, 9),
            // boss pos
            new XyPos(16, 22),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // move up
                int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                objsOffset += 8; // skip map header
                objsOffset += bossMap.originalObjNum * 8;
                // index for x/y
                objsOffset += 2; 
                // x,y
                rom[objsOffset + 1] = 11;
            }
            );

        // ---------------------------------------------------------------------------
        // great viper
        // ---------------------------------------------------------------------------
        public static VanillaBossMap GREATVIPER = new VanillaBossMap
            (
            "Great Viper",
            // og map / obj id
            MAPNUM_GREATVIPER_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL,/*SomVanillaValues.BOSSID_GREATVIPER,*/
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(18 * 16, 15 * 16), new XyPos(14 * 16, 14 * 16), new XyPos(25 * 16, 17 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(20 * 16, 19 * 16),
            new XyPos[] { new XyPos(17 * 16, 21 * 16), new XyPos(26 * 16, 23 * 16), new XyPos(18 * 16, 16 * 16), new XyPos(26 * 16, 17 * 16), new XyPos(13 * 16, 20 * 16) }.ToList(),
            new XyPos[] { new XyPos(14 * 16, 15 * 16), new XyPos(26 * 16, 18 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(15 * 16, 15 * 16), new XyPos(17 * 16, 22 * 16), new XyPos(26 * 16, 21 * 16), new XyPos(23 * 16, 14 * 16) }.ToList(),
            new XyPos[] { new XyPos(17 * 16, 13 * 16), new XyPos(20 * 16, 16 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(18, 24),
            // boss pos
            new XyPos(3, 13),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 2 collision
            true,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // need to move for non-snake - snake starts in the wall somewhere - move to 19,13
                if (bossId != SomVanillaValues.BOSSID_GREATVIPER || bossId != SomVanillaValues.BOSSID_DRAGONWORM)
                {
                    int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                    objsOffset += 8; // skip map header
                    objsOffset += bossMap.originalObjNum * 8;
                    // index for x/y
                    objsOffset += 2; 
                    // x,y
                    rom[objsOffset] = 0x16;
                    rom[objsOffset + 1] = 0x0D;

                    // more bullshit, need to layerswap here
                    // // ->[FE] [00 0F 65 01 00] [FF] [00 0F E9 00 00] [00 0F 68 01 00] [00 0F 68 31 00] [00 0F 68 01 38] [00 0F 68 31 38] 
                    // swap layer pieces
                    rom[0x87DBE] = 0xFE;
                    rom[0x87DBF] = 0x00;
                    rom[0x87DC0] = 0x0F;
                    rom[0x87DC1] = 0x65;
                    rom[0x87DC2] = 0x01;
                    rom[0x87DC3] = 0x00;
                    rom[0x87DC4] = 0xFF;
                    rom[0x87DC5] = 0x00;
                    rom[0x87DC6] = 0x0F;
                    rom[0x87DC7] = 0xE9;
                    rom[0x87DC8] = 0x00;
                    rom[0x87DC9] = 0x00;
                    rom[0x87DCA] = 0x00;
                    rom[0x87DCB] = 0x0F;
                    rom[0x87DCC] = 0x68;
                    rom[0x87DCD] = 0x01;
                    rom[0x87DCE] = 0x00;
                    rom[0x87DCF] = 0x00;
                    rom[0x87DD0] = 0x0F;
                    rom[0x87DD1] = 0x68;
                    rom[0x87DD2] = 0x31;
                    rom[0x87DD3] = 0x00;
                    rom[0x87DD4] = 0x00;
                    rom[0x87DD5] = 0x0F;
                    rom[0x87DD6] = 0x68;
                    rom[0x87DD7] = 0x01;
                    rom[0x87DD8] = 0x38;
                    rom[0x87DD9] = 0x00;
                    rom[0x87DDA] = 0x0F;
                    rom[0x87DDB] = 0x68;
                    rom[0x87DDC] = 0x31;
                    rom[0x87DDD] = 0x38;

                    // 8CDC8 - swap layers on map header
                    rom[0x8CDCA] = 0x90;
                    // change display setting to make L2 transparent
                    rom[0x8CDCD] = 0x0D;

                    // swap the layer the door tells me to collide on (x2)
                    rom[0x83532] = 0x30;
                    rom[0x835B6] = 0x10;
                }
            }
            );

        // ---------------------------------------------------------------------------
        // lime slime
        // ---------------------------------------------------------------------------
        public static VanillaBossMap LIMESLIME = new VanillaBossMap
            (
            "Lime Slime",
            // og map / obj id
            MAPNUM_LIMESLIME_ARENA, 0,
            // supported boss ids for swapping
            new byte[] { SomVanillaValues.BOSSID_LIMESLIME },
            // all supported boss ids
            new byte[] { SomVanillaValues.BOSSID_LIMESLIME },
            // all positions null since not used in vanilla rando / open world
            null, null, null,
            // spring beak positions - center, jump, podium
            null,
            null,
            null,
            // tropicallo positions - main, brambler
            null,
            null,
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(12, 13),
            // boss pos
            new XyPos(12, 12),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // blue spike
        // ---------------------------------------------------------------------------
        public static VanillaBossMap BLUESPIKE = new VanillaBossMap
            (
            "Blue Spike",
            // og map / obj id
            MAPNUM_BLUESPIKE_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME, SomVanillaValues.BOSSID_BLUESPIKE, */SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(15 * 16, 18 * 16), new XyPos(8 * 16, 19 * 16), new XyPos(22 * 16, 15 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(15 * 16, 18 * 16),
            new XyPos[] { new XyPos(9 * 16, 15 * 16), new XyPos(12 * 16, 25 * 16), new XyPos(20 * 16, 23 * 16), new XyPos(18 * 16, 20 * 16), new XyPos(16 * 16, 16 * 16) }.ToList(),
            new XyPos[] { new XyPos(8 * 16, 19 * 16), new XyPos(22 * 16, 15 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(8 * 16, 19 * 16), new XyPos(15 * 16, 18 * 16), new XyPos(22 * 16, 15 * 16) }.ToList(),
            new XyPos[] { new XyPos(13 * 16, 10 * 16), new XyPos(16 * 16, 10 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(14, 24),
            // boss pos
            new XyPos(14, 14),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { 5, 6, }, new int[] { 2, 3, }, // remove the stairway tower in the middle
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // hydra
        // ---------------------------------------------------------------------------
        public static VanillaBossMap HYDRA = new VanillaBossMap
            (
            "Hydra",
            // og map / obj id
            MAPNUM_HYDRA_ARENA, 3,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME, */SomVanillaValues.BOSSID_BLUESPIKE, /*SomVanillaValues.BOSSID_HYDRA, SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(15 * 16, 21 * 16), new XyPos(9 * 16, 17 * 16), new XyPos(20 * 16, 18 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(15 * 16, 25 * 16),
            new XyPos[] { new XyPos(10 * 16, 25 * 16), new XyPos(18 * 16, 27 * 16), new XyPos(20 * 16, 24 * 16), new XyPos(14 * 16, 24 * 16), new XyPos(11 * 16, 22 * 16) }.ToList(),
            new XyPos[] { new XyPos(9 * 16, 17 * 16), new XyPos(20 * 16, 18 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(11 * 16, 22 * 16), new XyPos(15 * 16, 26 * 16), new XyPos(19 * 16, 22 * 16) }.ToList(),
            new XyPos[] { new XyPos(10 * 16, 27 * 16), new XyPos(20 * 16, 27 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(15, 28),
            // boss pos
            new XyPos(15, 23),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 2 collision
            true,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // layer 1 tiles - swap
                // layer 2 tiles - swap
                // door x31A - swap collision layer
                // map pieces - two per layer; swap ids
                // display settings 16 -> 13

                int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                rom[objsOffset + 2] |= 0x40; // layer 1 use FG tiles
                rom[objsOffset + 2] &= 0x7F; // layer 2 use BG tiles
                rom[objsOffset + 5] = 13; // disp settings

                // swap 777/776, 742/741
                //          00   01 02 03 04 05   06 07 08 09 0a   0b   0c 0d 0e 0f 10   11 12 13 14 15
                // 88800 -> FE   00 0F 06 01 01   40 3F E3 1A 21   FF   00 0F 07 01 01   40 3F E4 1A 21
                //                     ^^  ^  ^
                //                     piecenum (lsbx8, middle bit, msb)
                int piecesOffset = VanillaMapUtil.getPiecePlacementOffset(rom, bossMap.originalMapNum);
                // swap layers
                rom[piecesOffset + 0x03] = 0x07;
                rom[piecesOffset + 0x08] = 0xE4;
                rom[piecesOffset + 0x0E] = 0x06;
                rom[piecesOffset + 0x13] = 0xE3;

                int doorOffset = 0x83000 + 0x31A * 4;
                // third byte, 0x01 bit
                rom[doorOffset + 2] &= 0xFE;
            }
            );

        // ---------------------------------------------------------------------------
        // watermelon boss, i don't think this is the actual name but it's kinda close
        // ---------------------------------------------------------------------------
        public static VanillaBossMap AEGRTOJNFSDGHOPALNIAPLEIAN = new VanillaBossMap
            (
            "Watermelon",
            // og map / obj id
            MAPNUM_WATERMELON_ARENA, 0, 
            // supported boss ids for swapping
            new byte[] {
                /*SomVanillaValues.BOSSID_MANTISANT,*/ SomVanillaValues.BOSSID_WALLFACE, /*SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,*/
                /*SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1,*/ SomVanillaValues.BOSSID_DOOMSWALL, /*SomVanillaValues.BOSSID_VAMPIRE,*/
                /*SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,*/
                /*SomVanillaValues.BOSSID_LIMESLIME, SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA,*/ SomVanillaValues.BOSSID_WATERMELON, /*SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,*/
                /*SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,*/ SomVanillaValues.BOSSID_SNOWDRAGON, /*SomVanillaValues.BOSSID_FIREGIGAS,*/ SomVanillaValues.BOSSID_REDDRAGON, /*SomVanillaValues.BOSSID_AXEBEAK,*/
                SomVanillaValues.BOSSID_BLUEDRAGON, /*SomVanillaValues.BOSSID_BUFFY,*/ SomVanillaValues.BOSSID_DARKLICH, /*SomVanillaValues.BOSSID_DRAGONWORM, SomVanillaValues.BOSSID_DREADSLIME, SomVanillaValues.BOSSID_THUNDERGIGAS,*/
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, SomVanillaValues.BOSSID_WALLFACE, SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, SomVanillaValues.BOSSID_DOOMSWALL, SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/ SomVanillaValues.BOSSID_BLUESPIKE, /*SomVanillaValues.BOSSID_HYDRA,*/ SomVanillaValues.BOSSID_WATERMELON, SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3, SomVanillaValues.BOSSID_SNOWDRAGON, SomVanillaValues.BOSSID_FIREGIGAS, SomVanillaValues.BOSSID_REDDRAGON, SomVanillaValues.BOSSID_AXEBEAK,
                SomVanillaValues.BOSSID_BLUEDRAGON, SomVanillaValues.BOSSID_BUFFY, SomVanillaValues.BOSSID_DARKLICH, SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(14 * 16, 10 * 16), new XyPos(9 * 16, 11 * 16), new XyPos(18 * 16, 9 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(14 * 16, 10 * 16), 
            new XyPos[] { new XyPos(9 * 16, 9 * 16), new XyPos(15 * 16, 11 * 16), new XyPos(17 * 16, 10 * 16), new XyPos(9 * 16, 15 * 16), new XyPos(15 * 16, 14 * 16), new XyPos(8 * 16, 13 * 16), new XyPos(18 * 16, 14 * 16) }.ToList(),
            new XyPos[] { new XyPos(10 * 16, 8 * 16), new XyPos(17 * 16, 12 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(10 * 16, 13 * 16), new XyPos(17 * 16, 6 * 16), new XyPos(10 * 16, 7 * 16) }.ToList(),
            new XyPos[] { new XyPos(15 * 16, 11 * 16), new XyPos(13 * 16, 9 * 16) }.ToList(),
            // y position at which the wall boss kills you
            0xF8,
            // entry pos
            new XyPos(9, 16),
            // boss pos
            new XyPos(13, 11),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) => 
            {
                // adjust wall position if it spawns here
                if (bossId == SomVanillaValues.BOSSID_WALLFACE || bossId == SomVanillaValues.BOSSID_DOOMSWALL)
                {
                    int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                    objsOffset += 8; // skip map header
                    objsOffset += bossMap.originalObjNum * 8;
                    // index for x/y
                    objsOffset += 2; 
                    // adjust x,y
                    rom[objsOffset] = 14;
                    rom[objsOffset + 1] = 4;
                }
            }
            );

        // ---------------------------------------------------------------------------
        // hexas
        // ---------------------------------------------------------------------------
        public static VanillaBossMap HEXAS = new VanillaBossMap
            (
            "Hexas",
            // og map / obj id
            MAPNUM_HEXAS_ARENA, 2,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME, */SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,SomVanillaValues.BOSSID_HEXAS, */SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(15 * 16, 20 * 16), new XyPos(10 * 16, 18 * 16), new XyPos(20 * 16, 22 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(15 * 16, 20 * 16),
            new XyPos[] { new XyPos(10 * 16, 23 * 16), new XyPos(10 * 16, 17 * 16), new XyPos(15 * 16, 20 * 16), new XyPos(20 * 16, 23 * 16), new XyPos(20 * 16, 17 * 16) }.ToList(),
            new XyPos[] { new XyPos(10 * 16, 18 * 16), new XyPos(20 * 16, 22 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(11 * 16, 16 * 16), new XyPos(15 * 16, 23 * 16), new XyPos(19 * 16, 16 * 16) }.ToList(),
            new XyPos[] { new XyPos(12 * 16, 23 * 16), new XyPos(18 * 16, 23 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(15, 27),
            // boss pos
            new XyPos(15, 20),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // kettle kin
        // ---------------------------------------------------------------------------
        public static VanillaBossMap KETTLEKIN = new VanillaBossMap
            (
            "Kettle Kin",
            // og map / obj id
            MAPNUM_KETTLEKIN_ARENA, 3,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME, */SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, /*SomVanillaValues.BOSSID_KETTLEKIN,*/
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(11 * 16, 11 * 16), new XyPos(6 * 16, 10 * 16), new XyPos(15 * 16, 9 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(10 * 16, 13 * 16),
            new XyPos[] { new XyPos(6 * 16, 14 * 16), new XyPos(14 * 16, 15 * 16), new XyPos(8 * 16, 12 * 16), new XyPos(16 * 16, 12 * 16), new XyPos(7 * 16, 10 * 16) }.ToList(),
            new XyPos[] { new XyPos(6 * 16, 13 * 16), new XyPos(15 * 16, 11 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(6 * 16, 10 * 16), new XyPos(10 * 16, 12 * 16), new XyPos(16 * 16, 11 * 16) }.ToList(),
            new XyPos[] { new XyPos(8 * 16, 9 * 16), new XyPos(13 * 16, 9 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(11, 15),
            // boss pos
            new XyPos(11, 11),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // tonpole
        // ---------------------------------------------------------------------------
        public static VanillaBossMap TONPOLE = new VanillaBossMap
            (
            "Tonpole",
            // og map / obj id
            MAPNUM_TONPOLE_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME, */SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                /*SomVanillaValues.BOSSID_TONPOLE, */SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(18 * 16, 10 * 16), new XyPos(16 * 16, 8 * 16), new XyPos(22 * 16, 11 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(19 * 16, 13 * 16),
            new XyPos[] { new XyPos(20 * 16, 14 * 16), new XyPos(16 * 16, 12 * 16), new XyPos(13 * 16, 12 * 16), new XyPos(23 * 16, 10 * 16), new XyPos(17 * 16, 10 * 16) }.ToList(),
            new XyPos[] { new XyPos(14 * 16, 13 * 16), new XyPos(23 * 16, 11 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(18 * 16, 9 * 16), new XyPos(14 * 16, 14 * 16), new XyPos(20 * 16, 14 * 16) }.ToList(),
            new XyPos[] { new XyPos(23 * 16, 15 * 16), new XyPos(13 * 16, 10 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(16, 17),
            // boss pos
            new XyPos(19, 12),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { 1 }, new int[] { }, // remove the door leading to undine
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // for vanilla rando, change the triple tonpole as well
                int tripleTonpoleMapNum = 361;
                int mapObjOffset = VanillaMapUtil.getObjectOffset(rom, tripleTonpoleMapNum);
                mapObjOffset += 8; // skip header

                rom[mapObjOffset + 8 * 0 + 5] = (byte)bossId; // adjust species data of obj 0
                rom[mapObjOffset + 8 * 1 + 5] = (byte)bossId; // adjust species data of obj 1
                rom[mapObjOffset + 8 * 2 + 5] = (byte)bossId; // adjust species data of obj 2

                // map piece 52 - modify?
                // D0068 -> D62E6 - D63E4
                // replace all 43 [2B] with 175 [AF]
                rom[0xD635F] = 0xAF;
                rom[0xD636A] = 0xAF;
            }
            );

        // ---------------------------------------------------------------------------
        // triple tonpole
        // ---------------------------------------------------------------------------
        public static VanillaBossMap TRIPLE_TONPOLE = new VanillaBossMap
            (
            "Triple Tonpole",
            // og map / obj id
            MAPNUM_TRIPLETONPOLE_ARENA, -1,
            // supported boss ids for swapping - unused for open world/vanilla rando
            null,
            // all supported boss ids - unused for open world/vanilla rando
            null,
            // spikey positions - center, left, right
            null, null, null,
            // spring beak positions - center, jump, podium
            null,
            null,
            null,
            // tropicallo positions - main, brambler
            null,
            null,
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(11, 20),
            // boss pos
            new XyPos(11, 13),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // mech rider 3
        // ---------------------------------------------------------------------------
        public static VanillaBossMap MECHRIDER3 = new VanillaBossMap
            (
            "Mech Rider 3",
            // og map / obj id
            MAPNUM_MECHRIDER3_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS,SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1,/*SomVanillaValues.BOSSID_DOOMSWALL,SomVanillaValues.BOSSID_VAMPIRE,*/
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, /*SomVanillaValues.BOSSID_HYDRA, SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, /*SomVanillaValues.BOSSID_MECHRIDER3,SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,SomVanillaValues.BOSSID_BUFFY,SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,SomVanillaValues.BOSSID_VAMPIRE,*/
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE,/*SomVanillaValues.BOSSID_HYDRA, SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,SomVanillaValues.BOSSID_BUFFY,SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(18 * 16, 18 * 16), new XyPos(12 * 16, 18 * 16), new XyPos(24 * 16, 18 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(18 * 16, 18 * 16),
            new XyPos[] { new XyPos(12 * 16, 24 * 16), new XyPos(18 * 16, 24 * 16), new XyPos(24 * 16, 24 * 16), new XyPos(12 * 16, 12 * 16), new XyPos(18 * 16, 12 * 16), new XyPos(24 * 16, 12 * 16), new XyPos(18 * 16, 18 * 16) }.ToList(),
            new XyPos[] { new XyPos(12 * 16, 18 * 16), new XyPos(24 * 16, 18 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(15 * 16, 15 * 16), new XyPos(20 * 16, 15 * 16), new XyPos(20 * 16, 20 * 16), new XyPos(15 * 16, 20 * 16) }.ToList(),
            new XyPos[] { new XyPos(18 * 16, 18 * 16), new XyPos(20 * 16, 17 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(10, 12),
            // boss pos
            new XyPos(14, 20),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // snow dragon
        // ---------------------------------------------------------------------------
        public static VanillaBossMap SNOWDRAGON = new VanillaBossMap
            (
            "Snow Dragon",
            // og map / obj id
            MAPNUM_SNOWDRAGON_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                /*SomVanillaValues.BOSSID_MANTISANT,*/ SomVanillaValues.BOSSID_WALLFACE, /*SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,*/
                /*SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1,*/ SomVanillaValues.BOSSID_DOOMSWALL, /*SomVanillaValues.BOSSID_VAMPIRE,*/
                /*SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,*/
                /*SomVanillaValues.BOSSID_LIMESLIME, SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA,*/ SomVanillaValues.BOSSID_WATERMELON, /*SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,*/
                /*SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3, SomVanillaValues.BOSSID_SNOWDRAGON, SomVanillaValues.BOSSID_FIREGIGAS,*/ SomVanillaValues.BOSSID_REDDRAGON, /*SomVanillaValues.BOSSID_AXEBEAK,*/
                SomVanillaValues.BOSSID_BLUEDRAGON, /*SomVanillaValues.BOSSID_BUFFY,*/ SomVanillaValues.BOSSID_DARKLICH, /*SomVanillaValues.BOSSID_DRAGONWORM, SomVanillaValues.BOSSID_DREADSLIME, SomVanillaValues.BOSSID_THUNDERGIGAS,*/
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, SomVanillaValues.BOSSID_WALLFACE, SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, SomVanillaValues.BOSSID_DOOMSWALL, SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/ SomVanillaValues.BOSSID_BLUESPIKE, /*SomVanillaValues.BOSSID_HYDRA,*/ SomVanillaValues.BOSSID_WATERMELON, SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3, SomVanillaValues.BOSSID_SNOWDRAGON, SomVanillaValues.BOSSID_FIREGIGAS, SomVanillaValues.BOSSID_REDDRAGON, SomVanillaValues.BOSSID_AXEBEAK,
                SomVanillaValues.BOSSID_BLUEDRAGON, SomVanillaValues.BOSSID_BUFFY, SomVanillaValues.BOSSID_DARKLICH, SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(14 * 16, 17 * 16), new XyPos(14 * 16, 11 * 16), new XyPos(20 * 16, 14 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(14 * 16, 17 * 16),
            new XyPos[] { new XyPos(14 * 16, 11 * 16), new XyPos(20 * 16, 14 * 16), new XyPos(18 * 16, 11 * 16), new XyPos(13 * 16, 16 * 16), new XyPos(13 * 16, 9 * 16), new XyPos(17 * 16, 17 * 16), new XyPos(17 * 16, 16 * 16) }.ToList(),
            new XyPos[] { new XyPos(14 * 16, 11 * 16), new XyPos(20 * 16, 14 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(14 * 16, 17 * 16), new XyPos(14 * 16, 11 * 16), new XyPos(20 * 16, 14 * 16) }.ToList(),
            new XyPos[] { new XyPos(13 * 16, 15 * 16), new XyPos(20 * 16, 18 * 16) }.ToList(),
            // y position at which the wall boss kills you
            0xF8,
            // entry pos
            new XyPos(12, 21),
            // boss pos
            new XyPos(12, 16),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // adjust wall position if it spawns here
                if (bossId == SomVanillaValues.BOSSID_WALLFACE || bossId == SomVanillaValues.BOSSID_DOOMSWALL)
                {
                    int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                    objsOffset += 8; // skip map header
                    objsOffset += bossMap.originalObjNum * 8;
                    // index for x/y
                    objsOffset += 2; 
                    // adjust x,y
                    rom[objsOffset] = 13;
                    rom[objsOffset + 1] = 7;
                }
            }
            );

        // ---------------------------------------------------------------------------
        // fire gigas
        // ---------------------------------------------------------------------------
        public static VanillaBossMap FIREGIGAS = new VanillaBossMap
            (
            "Fire Gigas",
            // og map / obj id
            MAPNUM_FIREGIGAS_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME, */SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON, SomVanillaValues.BOSSID_FIREGIGAS, SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(15 * 16, 20 * 16), new XyPos(10 * 16, 18 * 16), new XyPos(20 * 16, 22 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(15 * 16, 20 * 16),
            new XyPos[] { new XyPos(10 * 16, 23 * 16), new XyPos(10 * 16, 17 * 16), new XyPos(15 * 16, 20 * 16), new XyPos(20 * 16, 23 * 16), new XyPos(20 * 16, 17 * 16) }.ToList(),
            new XyPos[] { new XyPos(10 * 16, 18 * 16), new XyPos(20 * 16, 22 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(11 * 16, 16 * 16), new XyPos(15 * 16, 23 * 16), new XyPos(19 * 16, 16 * 16) }.ToList(),
            new XyPos[] { new XyPos(12 * 16, 23 * 16), new XyPos(18 * 16, 23 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(15, 27),
            // boss pos
            new XyPos(15, 20),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // move the boss up a bit
                int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                objsOffset += 8; // skip map header
                objsOffset += bossMap.originalObjNum * 8;
                // index for x/y
                objsOffset += 2; 
                // y pos
                rom[objsOffset + 1] = 17;
            }
            );

        // ---------------------------------------------------------------------------
        // red dragon
        // ---------------------------------------------------------------------------
        public static VanillaBossMap REDDRAGON = new VanillaBossMap
            (
            "Red Dragon",
            // og map / obj id
            MAPNUM_REDDRAGON_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                /*SomVanillaValues.BOSSID_MANTISANT,*/ SomVanillaValues.BOSSID_WALLFACE, /*SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,*/
                /*SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1,*/ SomVanillaValues.BOSSID_DOOMSWALL, /*SomVanillaValues.BOSSID_VAMPIRE,*/
                /*SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,*/
                /*SomVanillaValues.BOSSID_LIMESLIME, SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA,*/ SomVanillaValues.BOSSID_WATERMELON, /*SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,*/
                /*SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,*/SomVanillaValues.BOSSID_SNOWDRAGON,/*SomVanillaValues.BOSSID_FIREGIGAS, SomVanillaValues.BOSSID_REDDRAGON, SomVanillaValues.BOSSID_AXEBEAK,*/
                SomVanillaValues.BOSSID_BLUEDRAGON, /*SomVanillaValues.BOSSID_BUFFY,*/ SomVanillaValues.BOSSID_DARKLICH, /*SomVanillaValues.BOSSID_DRAGONWORM, SomVanillaValues.BOSSID_DREADSLIME, SomVanillaValues.BOSSID_THUNDERGIGAS,*/
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, SomVanillaValues.BOSSID_WALLFACE, SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, SomVanillaValues.BOSSID_DOOMSWALL, SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/ SomVanillaValues.BOSSID_BLUESPIKE, /*SomVanillaValues.BOSSID_HYDRA,*/ SomVanillaValues.BOSSID_WATERMELON, SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3, SomVanillaValues.BOSSID_SNOWDRAGON, SomVanillaValues.BOSSID_FIREGIGAS, SomVanillaValues.BOSSID_REDDRAGON, SomVanillaValues.BOSSID_AXEBEAK,
                SomVanillaValues.BOSSID_BLUEDRAGON, SomVanillaValues.BOSSID_BUFFY, SomVanillaValues.BOSSID_DARKLICH, SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(14 * 16, 17 * 16), new XyPos(14 * 16, 11 * 16), new XyPos(20 * 16, 14 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(14 * 16, 17 * 16),
            new XyPos[] { new XyPos(14 * 16, 11 * 16), new XyPos(20 * 16, 14 * 16), new XyPos(18 * 16, 11 * 16), new XyPos(13 * 16, 16 * 16), new XyPos(13 * 16, 9 * 16), new XyPos(17 * 16, 17 * 16), new XyPos(17 * 16, 16 * 16) }.ToList(),
            new XyPos[] { new XyPos(14 * 16, 11 * 16), new XyPos(20 * 16, 14 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(14 * 16, 17 * 16), new XyPos(14 * 16, 11 * 16), new XyPos(20 * 16, 14 * 16) }.ToList(),
            new XyPos[] { new XyPos(13 * 16, 15 * 16), new XyPos(20 * 16, 18 * 16) }.ToList(),
            // y position at which the wall boss kills you
            0x108,
            // entry pos
            new XyPos(19, 23),
            // boss pos
            new XyPos(12, 16),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // adjust wall position if it spawns here
                if (bossId == SomVanillaValues.BOSSID_WALLFACE || bossId == SomVanillaValues.BOSSID_DOOMSWALL)
                {
                    int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                    objsOffset += 8; // skip map header
                    objsOffset += bossMap.originalObjNum * 8;
                    // index for x/y
                    objsOffset += 2;
                    // adjust x,y
                    rom[objsOffset] = 18;
                    rom[objsOffset + 1] = 8;
                }
            }
            );

        // ---------------------------------------------------------------------------
        // axe beak
        // ---------------------------------------------------------------------------
        public static VanillaBossMap AXEBEAK = new VanillaBossMap
            (
            "Axe Beak",
            // og map / obj id
            MAPNUM_AXEBEAK_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME, */SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON, SomVanillaValues.BOSSID_AXEBEAK,*/
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(20 * 16, 22 * 16), new XyPos(14 * 16, 26 * 16), new XyPos(26 * 16, 18 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(0x130, 0x1B0),
            new XyPos[] { new XyPos(0xB0, 0x140), new XyPos(0x1E0, 0x140), new XyPos(0xC0, 0x1A0), new XyPos(0xD0, 0x180), new XyPos(0x190, 0x190), new XyPos(0x1B0, 0x1A0), new XyPos(0xF0, 0x120), new XyPos(0x170, 0x120) }.ToList(),
            new XyPos[] { new XyPos(0xE8, 0x120), new XyPos(0x198, 0x120) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(25 * 16, 19 * 16), new XyPos(23 * 16, 27 * 16), new XyPos(18 * 16, 25 * 16) }.ToList(),
            new XyPos[] { new XyPos(18 * 16, 18 * 16), new XyPos(22 * 16, 18 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(19, 28),
            // boss pos
            new XyPos(19, 21),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { 1 }, new int[] { }, // remove the stairs leading out
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // move the boss a bit
                int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                objsOffset += 8; // skip map header
                objsOffset += bossMap.originalObjNum * 8;
                // index for x/y
                objsOffset += 2;
                // x,y
                rom[objsOffset] = 20; // 19;
                rom[objsOffset + 1] = 23; // 13;
            }
            );

        // ---------------------------------------------------------------------------
        // blue dragon
        // ---------------------------------------------------------------------------
        public static VanillaBossMap BLUEDRAGON = new VanillaBossMap
            (
            "Blue Dragon",
            // og map / obj id
            MAPNUM_BLUEDRAGON_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                /*SomVanillaValues.BOSSID_MANTISANT,*/ SomVanillaValues.BOSSID_WALLFACE, /*SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,*/
                /*SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1,*/ SomVanillaValues.BOSSID_DOOMSWALL, /*SomVanillaValues.BOSSID_VAMPIRE,*/
                /*SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,*/
                /*SomVanillaValues.BOSSID_LIMESLIME, SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA,*/ SomVanillaValues.BOSSID_WATERMELON, /*SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,*/
                /*SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,*/SomVanillaValues.BOSSID_SNOWDRAGON,/*SomVanillaValues.BOSSID_FIREGIGAS,*/ SomVanillaValues.BOSSID_REDDRAGON, /*SomVanillaValues.BOSSID_AXEBEAK,*/
                /*SomVanillaValues.BOSSID_BLUEDRAGON, SomVanillaValues.BOSSID_BUFFY,*/ SomVanillaValues.BOSSID_DARKLICH, /*SomVanillaValues.BOSSID_DRAGONWORM, SomVanillaValues.BOSSID_DREADSLIME, SomVanillaValues.BOSSID_THUNDERGIGAS,*/
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, SomVanillaValues.BOSSID_WALLFACE, SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, SomVanillaValues.BOSSID_DOOMSWALL, SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/ SomVanillaValues.BOSSID_BLUESPIKE, /*SomVanillaValues.BOSSID_HYDRA,*/ SomVanillaValues.BOSSID_WATERMELON, SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3, SomVanillaValues.BOSSID_SNOWDRAGON, SomVanillaValues.BOSSID_FIREGIGAS, SomVanillaValues.BOSSID_REDDRAGON, SomVanillaValues.BOSSID_AXEBEAK,
                SomVanillaValues.BOSSID_BLUEDRAGON, SomVanillaValues.BOSSID_BUFFY, SomVanillaValues.BOSSID_DARKLICH, SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(14 * 16, 10 * 16), new XyPos(9 * 16, 11 * 16), new XyPos(18 * 16, 9 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(14 * 16, 10 * 16),
            new XyPos[] { new XyPos(9 * 16, 9 * 16), new XyPos(15 * 16, 11 * 16), new XyPos(17 * 16, 10 * 16), new XyPos(9 * 16, 15 * 16), new XyPos(15 * 16, 14 * 16), new XyPos(8 * 16, 13 * 16), new XyPos(18 * 16, 14 * 16) }.ToList(),
            new XyPos[] { new XyPos(10 * 16, 8 * 16), new XyPos(17 * 16, 12 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(10 * 16, 13 * 16), new XyPos(17 * 16, 16 * 16), new XyPos(10 * 16, 17 * 16) }.ToList(),
            new XyPos[] { new XyPos(15 * 16, 11 * 16), new XyPos(13 * 16, 9 * 16) }.ToList(),
            // y position at which the wall boss kills you
            0x118,
            // entry pos
            new XyPos(13, 21),
            // boss pos
            new XyPos(12, 16),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // adjust wall position if it spawns here
                if (bossId == SomVanillaValues.BOSSID_WALLFACE || bossId == SomVanillaValues.BOSSID_DOOMSWALL)
                {
                    int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                    objsOffset += 8; // skip map header
                    objsOffset += bossMap.originalObjNum * 8;
                    // index for x/y
                    objsOffset += 2;
                    // adjust x,y
                    rom[objsOffset] = 13;
                    rom[objsOffset + 1] = 9;
                }
            }
            );

        // ---------------------------------------------------------------------------
        // buffy
        // ---------------------------------------------------------------------------
        public static VanillaBossMap BUFFY = new VanillaBossMap
            (
            "Buffy",
            // og map / obj id
            MAPNUM_BUFFY_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME, */SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON, */SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON, SomVanillaValues.BOSSID_BUFFY, SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(17 * 16, 39 * 16), new XyPos(13 * 16, 41 * 16), new XyPos(22 * 16, 43 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(16 * 16, 41 * 16),
            new XyPos[] { new XyPos(11 * 16, 42 * 16), new XyPos(20 * 16, 39 * 16), new XyPos(12 * 16, 39 * 16), new XyPos(23 * 16, 44 * 16), new XyPos(19 * 16, 44 * 16), new XyPos(16 * 16, 38 * 16), new XyPos(14 * 16, 36 * 16) }.ToList(),
            new XyPos[] { new XyPos(23 * 16, 48 * 16), new XyPos(10 * 16, 48 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(14 * 16, 40 * 16), new XyPos(17 * 16, 44 * 16), new XyPos(21 * 16, 39 * 16) }.ToList(),
            new XyPos[] { new XyPos(15 * 16, 35 * 16), new XyPos(18 * 16, 35 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(16, 48),
            // boss pos
            new XyPos(17, 45),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // dark lich
        // ---------------------------------------------------------------------------
        public static VanillaBossMap DARKLICH = new VanillaBossMap
            (
            "Dark Lich",
            // og map / obj id
            MAPNUM_DARKLICH_ARENA_B, 2,
            // supported boss ids for swapping
            new byte[] {
                /*SomVanillaValues.BOSSID_MANTISANT,*/ SomVanillaValues.BOSSID_WALLFACE, /*SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,*/
                /*SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1,*/ SomVanillaValues.BOSSID_DOOMSWALL, /*SomVanillaValues.BOSSID_VAMPIRE,*/
                /*SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,*/
                /*SomVanillaValues.BOSSID_LIMESLIME, SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA,*/ SomVanillaValues.BOSSID_WATERMELON, /*SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,*/
                /*SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,*/SomVanillaValues.BOSSID_SNOWDRAGON,/*SomVanillaValues.BOSSID_FIREGIGAS,*/ SomVanillaValues.BOSSID_REDDRAGON, /*SomVanillaValues.BOSSID_AXEBEAK,*/
                SomVanillaValues.BOSSID_BLUEDRAGON, /*SomVanillaValues.BOSSID_BUFFY, SomVanillaValues.BOSSID_DARKLICH, SomVanillaValues.BOSSID_DRAGONWORM, SomVanillaValues.BOSSID_DREADSLIME, SomVanillaValues.BOSSID_THUNDERGIGAS,*/
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, SomVanillaValues.BOSSID_WALLFACE, SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, /*SomVanillaValues.BOSSID_JABBER,*/
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, SomVanillaValues.BOSSID_DOOMSWALL, SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/ SomVanillaValues.BOSSID_BLUESPIKE, /*SomVanillaValues.BOSSID_HYDRA,*/ SomVanillaValues.BOSSID_WATERMELON, SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3, SomVanillaValues.BOSSID_SNOWDRAGON, SomVanillaValues.BOSSID_FIREGIGAS, SomVanillaValues.BOSSID_REDDRAGON, SomVanillaValues.BOSSID_AXEBEAK,
                SomVanillaValues.BOSSID_BLUEDRAGON, SomVanillaValues.BOSSID_BUFFY, SomVanillaValues.BOSSID_DARKLICH, SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(18 * 16, 18 * 16), new XyPos(14 * 16, 18 * 16), new XyPos(23 * 16, 18 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(18 * 16, 18 * 16),
            new XyPos[] { new XyPos(18 * 16, 18 * 16), new XyPos(14 * 16, 14 * 16), new XyPos(22 * 16, 22 * 16), new XyPos(14 * 16, 22 * 16), new XyPos(22 * 16, 14 * 16), new XyPos(18 * 16, 22 * 16), new XyPos(18 * 16, 14 * 16) }.ToList(),
            new XyPos[] { new XyPos(14 * 16, 16 * 16), new XyPos(21 * 16, 21 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(14 * 16, 16 * 16), new XyPos(18 * 16, 19 * 16), new XyPos(22 * 16, 22 * 16) }.ToList(),
            new XyPos[] { new XyPos(19 * 16, 17 * 16), new XyPos(15 * 16, 20 * 16) }.ToList(),
            // y position at which the wall boss kills you
            0x1A0,
            // entry pos
            new XyPos(16, 23),
            // boss pos
            new XyPos(16, 17),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // adjust wall position if it spawns here
                if (bossId == SomVanillaValues.BOSSID_WALLFACE || bossId == SomVanillaValues.BOSSID_DOOMSWALL)
                {
                    int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                    objsOffset += 8; // skip map header
                    objsOffset += bossMap.originalObjNum * 8;
                    // index for x/y
                    objsOffset += 2;
                    // adjust x,y
                    rom[objsOffset] = 18;
                    rom[objsOffset + 1] = 9;
                }
            }
            );

        // ---------------------------------------------------------------------------
        // dragon worm
        // ---------------------------------------------------------------------------
        public static VanillaBossMap DRAGONWORM = new VanillaBossMap
            (
            "Dragon Worm",
            // og map / obj id
            MAPNUM_DRAGONWORM_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME, */SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON, */SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/ SomVanillaValues.BOSSID_BUFFY, /*SomVanillaValues.BOSSID_DARKLICH, SomVanillaValues.BOSSID_DRAGONWORM, SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(16 * 16, 13 * 16), new XyPos(14 * 16, 17 * 16), new XyPos(21 * 16, 11 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(17 * 16, 14 * 16),
            new XyPos[] { new XyPos(17 * 16, 24 * 16), new XyPos(12 * 16, 21 * 16), new XyPos(14 * 16, 16 * 16), new XyPos(16 * 16, 14 * 16), new XyPos(24 * 16, 14 * 16) }.ToList(),
            new XyPos[] { new XyPos(14 * 16, 17 * 16), new XyPos(23 * 16, 15 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(21 * 16, 12 * 16), new XyPos(14 * 16, 17 * 16), new XyPos(16 * 16, 13 * 16), new XyPos(17 * 16, 23 * 16) }.ToList(),
            new XyPos[] { new XyPos(19 * 16, 16 * 16), new XyPos(16 * 16, 13 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(18, 26),
            // boss pos
            new XyPos(10, 10),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // move for non-snake - snake starts in the wall somewhere - move to 21,12
                if (bossId != SomVanillaValues.BOSSID_GREATVIPER || bossId != SomVanillaValues.BOSSID_DRAGONWORM)
                {
                    int objsOffset = VanillaMapUtil.getObjectOffset(rom, bossMap.originalMapNum);
                    objsOffset += 8; // skip map header
                    objsOffset += bossMap.originalObjNum * 8;
                    // index for x/y
                    objsOffset += 2; 
                    // x,y
                    rom[objsOffset] = 21;
                    rom[objsOffset + 1] = 12;
                }
            }
            );

        // ---------------------------------------------------------------------------
        // dread slime
        // ---------------------------------------------------------------------------
        public static VanillaBossMap DREADSLIME = new VanillaBossMap
            (
            "Dread Slime",
            // og map / obj id
            MAPNUM_DREADSLIME_ARENA, 0,
            // supported boss ids for swapping
            new byte[] { SomVanillaValues.BOSSID_DREADSLIME },
            // all supported boss ids
            new byte[] { SomVanillaValues.BOSSID_DREADSLIME },
            // all positions null since not used in vanilla rando / open world
            null, null, null,
            // spring beak positions - center, jump, podium
            null,
            null,
            null,
            // tropicallo positions - main, brambler
            null,
            null,
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(12, 13),
            // boss pos
            new XyPos(12, 12),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        // ---------------------------------------------------------------------------
        // thunder gigas
        // ---------------------------------------------------------------------------
        public static VanillaBossMap THUNDERGIGAS = new VanillaBossMap
            (
            "Thunder Gigas",
            // og map / obj id
            MAPNUM_THUNDERGIGAS_ARENA, 0,
            // supported boss ids for swapping
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT,/*SomVanillaValues.BOSSID_WALLFACE,*/SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME, */SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON, */SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/ SomVanillaValues.BOSSID_BUFFY, /*SomVanillaValues.BOSSID_DARKLICH, */SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME, SomVanillaValues.BOSSID_THUNDERGIGAS,*/
            },
            // all supported boss ids
            new byte[] {
                SomVanillaValues.BOSSID_MANTISANT, /*SomVanillaValues.BOSSID_WALLFACE,*/ SomVanillaValues.BOSSID_TROPICALLO, SomVanillaValues.BOSSID_MINOTAUR, SomVanillaValues.BOSSID_SPIKEY, SomVanillaValues.BOSSID_JABBER,
                SomVanillaValues.BOSSID_SPRINGBEAK, SomVanillaValues.BOSSID_FROSTGIGAS, SomVanillaValues.BOSSID_SNAPDRAGON, SomVanillaValues.BOSSID_MECHRIDER1, /*SomVanillaValues.BOSSID_DOOMSWALL,*/SomVanillaValues.BOSSID_VAMPIRE,
                SomVanillaValues.BOSSID_METALMANTIS, SomVanillaValues.BOSSID_MECHRIDER2, SomVanillaValues.BOSSID_KILROY, SomVanillaValues.BOSSID_GORGON, SomVanillaValues.BOSSID_BOREAL, SomVanillaValues.BOSSID_GREATVIPER,
                /*SomVanillaValues.BOSSID_LIMESLIME,*/SomVanillaValues.BOSSID_BLUESPIKE, SomVanillaValues.BOSSID_HYDRA, /*SomVanillaValues.BOSSID_WATERMELON,*/SomVanillaValues.BOSSID_HEXAS, SomVanillaValues.BOSSID_KETTLEKIN,
                SomVanillaValues.BOSSID_TONPOLE, SomVanillaValues.BOSSID_MECHRIDER3,/*SomVanillaValues.BOSSID_SNOWDRAGON,*/SomVanillaValues.BOSSID_FIREGIGAS,/*SomVanillaValues.BOSSID_REDDRAGON,*/SomVanillaValues.BOSSID_AXEBEAK,
                /*SomVanillaValues.BOSSID_BLUEDRAGON,*/SomVanillaValues.BOSSID_BUFFY,/*SomVanillaValues.BOSSID_DARKLICH,*/SomVanillaValues.BOSSID_DRAGONWORM, /*SomVanillaValues.BOSSID_DREADSLIME,*/ SomVanillaValues.BOSSID_THUNDERGIGAS,
            },
            // spikey positions - center, left, right
            new XyPos(24 * 16, 33 * 16), new XyPos(17 * 16, 36 * 16), new XyPos(28 * 16, 34 * 16),
            // spring beak positions - center, jump, podium
            new XyPos(24 * 16, 33 * 16),
            new XyPos[] { new XyPos(22 * 16, 37 * 16), new XyPos(26 * 16, 34 * 16), new XyPos(21 * 16, 34 * 16), new XyPos(17 * 16, 36 * 16), new XyPos(22 * 16, 31 * 16), new XyPos(25 * 16, 34 * 16), new XyPos(25 * 16, 38 * 16) }.ToList(),
            new XyPos[] { new XyPos(18 * 16, 37 * 16), new XyPos(28 * 16, 35 * 16) }.ToList(),
            // tropicallo positions - main, brambler
            new XyPos[] { new XyPos(24 * 16, 33 * 16), new XyPos(28 * 16, 33 * 16), new XyPos(17 * 16, 36 * 16) }.ToList(),
            new XyPos[] { new XyPos(27 * 16, 30 * 16), new XyPos(20 * 16, 37 * 16) }.ToList(),
            // y position at which the wall boss kills you (N/A because it can't be here)
            0,
            // entry pos
            new XyPos(16 + 8, 20 + 20),
            // boss pos
            new XyPos(16 + 8, 12 + 20),
            // don't include pieces by index when importing for ancient cave/boss rush/etc. bg, fg
            new int[] { }, new int[] { },
            // layer 1 collision
            false,
            // post-processing based on boss selection
            (rom, bossId, bossMap) =>
            {
                // nothing
            }
            );

        public static Dictionary<byte, VanillaBossMap> BY_VANILLA_BOSS_ID = new Dictionary<byte, VanillaBossMap>
        {
            { 0x57, MANTISANT },
            { 0x58, WALLFACE },
            { 0x59, TROPICALLO },
            { 0x5a, MINOTAUR },
            { 0x5b, SPIKEY },
            { 0x5c, JABBER },
            { 0x5d, SPRINGBEAK },
            { 0x5e, FROSTGIGAS },
            { 0x5f, SNAPDRAGON },
            { 0x60, MECHRIDER1 },
            { 0x61, DOOMSWALL },
            { 0x62, VAMPIRE },
            { 0x63, METALMANTIS },
            { 0x64, MECHRIDER2 },
            { 0x65, KILROY },
            { 0x66, GORGON },
            // 0x67 brambler
            { 0x68, BOREAL },
            { 0x69, GREATVIPER },
            { 0x6a, LIMESLIME },
            { 0x6b, BLUESPIKE },
            // 0x6c chamber's eye
            { 0x6d, HYDRA },
            { 0x6e, AEGRTOJNFSDGHOPALNIAPLEIAN },
            { 0x6f, HEXAS },
            { 0x70, KETTLEKIN },
            { 0x71, TONPOLE },
            { 0x72, MECHRIDER3 },
            { 0x73, SNOWDRAGON },
            { 0x74, FIREGIGAS },
            { 0x75, REDDRAGON },
            { 0x76, AXEBEAK },
            { 0x77, BLUEDRAGON },
            { 0x78, BUFFY },
            { 0x79, DARKLICH },
            // 0x7a biting lizard
            { 0x7b, DRAGONWORM },
            { 0x7c, DREADSLIME },
            { 0x7d, THUNDERGIGAS },
            { (byte)VanillaBossMap.TRIPLE_TONPOLE_OBJECT_INDICATOR, TRIPLE_TONPOLE },
            // 0x7e doom's eye
            // 0x7f mana beast
        };

        public class VanillaBossMap
        {
            // an id > the valid boss ids to indicate triple tonpole on map
            public const int TRIPLE_TONPOLE_OBJECT_INDICATOR = 0x80;
            public string name;
            public int originalMapNum;
            public int originalObjNum;
            public byte[] supportedSwapBosses;
            public byte[] allSupportedBosses;
            public XyPos spikeyCenterPos;
            public XyPos spikeyLeftPos;
            public XyPos spikeyRightPos;
            public XyPos springBeakCenterPos;
            public List<XyPos> springBeakJumpPositions;
            public List<XyPos> springBeakPodiumPositions;
            public List<XyPos> tropicalloPositions;
            public List<XyPos> bramblerPositions;
            public int wallBossDeathYValue;
            public XyPos entryPos;
            public XyPos bossPos;
            public int[] bgPieceExclusions;
            public int[] fgPieceExclusions;
            public bool collideLayer2;
            private Action<byte[], int, VanillaBossMap> processor;
            public VanillaBossMap(string name, int originalMapNum, int originalObjNum,  // OG map data
                byte[] supportedSwapBosses, // bosses that can swap here in boss swaps mode
                byte[] allSupportedBosses, // bosses other than original that can be here
                XyPos spikeyCenterPos, XyPos spikeyLeftPos, XyPos spikeyRightPos, // spikey settings
                XyPos springBeakCenterPos, List<XyPos> springBeakJumpPositions, List<XyPos> springBeakPodiumPositions, // beak settings
                List<XyPos> tropicalloPositions, List<XyPos> bramblerPositions, // tropicallo settings
                int wallBossDeathYValue, XyPos entryPos, XyPos bossPos, int[] bgPieceExclusions, int[] fgPieceExclusions, bool collideLayer2,
                Action<byte[], int, VanillaBossMap> processor)
            {
                this.name = name;
                this.originalMapNum = originalMapNum;
                this.originalObjNum = originalObjNum;
                this.supportedSwapBosses = supportedSwapBosses;
                this.allSupportedBosses = allSupportedBosses;
                this.spikeyCenterPos = spikeyCenterPos;
                this.spikeyLeftPos = spikeyLeftPos;
                this.spikeyRightPos = spikeyRightPos;
                this.springBeakCenterPos = springBeakCenterPos;
                this.springBeakJumpPositions = springBeakJumpPositions;
                this.springBeakPodiumPositions = springBeakPodiumPositions;
                this.tropicalloPositions = tropicalloPositions;
                this.bramblerPositions = bramblerPositions;
                this.wallBossDeathYValue = wallBossDeathYValue;
                this.entryPos = entryPos;
                this.bossPos = bossPos;
                this.bgPieceExclusions = bgPieceExclusions;
                this.fgPieceExclusions = fgPieceExclusions;
                this.collideLayer2 = collideLayer2;
                this.processor = processor;
            }

            public void process(byte[] rom, int bossNum)
            {
                processor.Invoke(rom, bossNum, this);
            }

            public byte getOriginalBossId(byte[] rom)
            {
                if(originalObjNum == -1)
                {
                    // special case triple tonpole to differentiate it from single tonpole map
                    return TRIPLE_TONPOLE_OBJECT_INDICATOR;
                }
                int objsOffset = VanillaMapUtil.getObjectOffset(rom, originalMapNum);
                // skip map header
                objsOffset += 8; 
                objsOffset += originalObjNum * 8;
                // index for #
                objsOffset += 5; 
                return rom[objsOffset];
            }

            public override string ToString()
            {
                return name;
            }
        }
    }

}
