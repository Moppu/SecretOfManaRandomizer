using SoMRandomizer.processing.common.structure;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Change most palettes to be icy or snowy for open world winter modes.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class XmasMapPalettes
    {
        public void process(byte[] rom, Random random)
        {
            Dictionary<string, byte[]> groupedPalettes = new Dictionary<string, byte[]>();
            groupedPalettes.Add("cave", new byte[]
            {
                34, // gaia's navel
                101, // undine cave
            });

            groupedPalettes.Add("purelands", new byte[]
            {
                54, // purelands A
                104, // purelands B
            });

            groupedPalettes.Add("icecountry", new byte[]
            {
                37,
            });

            groupedPalettes.Add("lighthouse", new byte[]
            {
                38,
            });

            groupedPalettes.Add("ntc", new byte[]
            {
                83,
            });

            groupedPalettes.Add("karon", new byte[]
            {
                57,
            });

            groupedPalettes.Add("witchforest", new byte[]
            {
                31,
            });

            groupedPalettes.Add("icecastle", new byte[]
            {
                84,
            });

            Dictionary<byte, byte> palSetToDispSetting = new Dictionary<byte, byte>();
            palSetToDispSetting[25] = 4;
            palSetToDispSetting[26] = 1;
            palSetToDispSetting[36] = 9; // sky over jehk area
            palSetToDispSetting[103] = 18; // mana tree
            palSetToDispSetting[109] = 19; // ntc top sky
            palSetToDispSetting[108] = 20; // kakkara cannon guy
            palSetToDispSetting[118] = 23; // ending
            palSetToDispSetting[104] = 24; // pale pure lands
            palSetToDispSetting[54] = 21; // normal pure lands

            byte[] dontChangePalettes = new byte[]
            {
                28, // indoors santa house, others
                30, // indoors matango
                47, // sylphid
                52, // indoors empire
                56, // tasnica inside
                73, // gold city interior
                74, // watermelon
                84, // triple tonpole arena
                85, // ice castle interior
                87, // indoors pandora castle
                88, // undine
                89, // gnome palace (for orb)
                91, // salamando
                92, // shade
                93, // lumina
                95, // luna
                97, // dryad palace
                108, // credits cliff
            };

            // only make them blueish
            byte[] dontDarkenPalettes = new byte[]
            {
                33, // pandora ruins
                34, // gaia's navel, probably other caves
                40, // snake arena, matango outside
                42, // kilroy area
                43, // kilroy area
                44, // manafort exterior
                45, // mana fort interior/mech rider 3
                50, // subway area
                51, // undersea area
                58, // ntc inside
                60, // mountain hint place interior
                67, // pandora ruins rooms
                69, // dwarftown inn etc
                70, // dwarf town
                79, // NT ruins + doomswall + vampire
                80, // NT ruins rooms
                82, // spikey arena
                86, // witch castle interior
                100, // empire sewers
                101, // undine cave
                102, // fire dragon
                106, // shade cave
                113, // dark lich
            };

            // don't make them blueish
            byte[] darkenOnlyPalettes = new byte[]
            {
                29, // before springbeak
                62, // normal ul
                63, // ul spring
                64, // ul summer
                65, // ul fall
                66, // ul winter
                72, // gold city
                94, // gold tower outside
            };

            // take highest of r/g/b
            // 26, 2: 10-14; 3: 0, 11-14; 5: 10-14; 6: 0
            Dictionary<int, Dictionary<int, List<int>>> grayscaleColors = new Dictionary<int, Dictionary<int, List<int>>>();

            // potos
            Dictionary<int, List<int>> pal24Grayscale = new Dictionary<int, List<int>>();
            // trees
            pal24Grayscale[1] = new int[] { 1, 2 }.ToList();
            pal24Grayscale[2] = new int[] { 10, 11, 12, 13, }.ToList();
            pal24Grayscale[3] = new int[] { 11, 12, 13, 14 }.ToList();
            // rooftops
            pal24Grayscale[4] = new int[] { 1, 2 }.ToList();
            grayscaleColors[24] = pal24Grayscale;

            // below potos
            Dictionary<int, List<int>> pal25Grayscale = new Dictionary<int, List<int>>();
            // trees
            pal25Grayscale[1] = new int[] { 1, 2 }.ToList();
            pal25Grayscale[2] = new int[] { 10, 11, 12, 13, }.ToList();
            pal25Grayscale[3] = new int[] { 11, 12, 13, 14 }.ToList();
            pal25Grayscale[5] = new int[] { 10, 11, 12, 13, 14, 15 }.ToList();
            pal25Grayscale[6] = new int[] { 0, 10, 11, 12, 13, 14, 15 }.ToList();
            pal25Grayscale[7] = new int[] { 0 }.ToList();
            grayscaleColors[25] = pal25Grayscale;

            // potos area outside
            Dictionary<int, List<int>> pal26Grayscale = new Dictionary<int, List<int>>();
            // trees
            pal26Grayscale[1] = new int[] { 1, 2 }.ToList();
            pal26Grayscale[2] = new int[] { 10, 11, 12, 13, }.ToList();
            pal26Grayscale[3] = new int[] { 11, 12, 13, 14 }.ToList();
            pal26Grayscale[5] = new int[] { 10, 11, 12, 13, 14, 15 }.ToList();
            pal26Grayscale[6] = new int[] { 0 }.ToList();
            grayscaleColors[26] = pal26Grayscale;

            // kakkara outside
            Dictionary<int, List<int>> pal27Grayscale = new Dictionary<int, List<int>>();
            pal27Grayscale[1] = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }.ToList();
            pal27Grayscale[2] = new int[] { 4, 5, 6, 7, }.ToList();
            pal27Grayscale[3] = new int[] { 1, 2, 3, 4, 5, 6, 7, }.ToList();
            pal27Grayscale[4] = new int[] { 1, 2, 3, 4, 5, 6, 7, }.ToList();
            pal27Grayscale[5] = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }.ToList();
            pal27Grayscale[6] = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }.ToList();
            pal27Grayscale[7] = new int[] { 1, 2, 3, 4, 5, 6, 7,  }.ToList();
            grayscaleColors[27] = pal27Grayscale;

            // witch forest
            Dictionary<int, List<int>> pal31Grayscale = new Dictionary<int, List<int>>();
            pal31Grayscale[1] = new int[] { 10, 11, 12, 13 }.ToList();
            pal31Grayscale[2] = new int[] { 10, 11, 12, 13, 14, 15 }.ToList();
            pal31Grayscale[3] = new int[] { 0 }.ToList();
            pal31Grayscale[4] = new int[] { 3, 4, 5, 6 }.ToList();
            pal31Grayscale[5] = new int[] { 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }.ToList();
            pal31Grayscale[6] = new int[] { 0, 10, 11, 12, 13 }.ToList();
            grayscaleColors[31] = pal31Grayscale;

            // pandora ruins outside
            Dictionary<int, List<int>> pal32Grayscale = new Dictionary<int, List<int>>();
            pal32Grayscale[1] = new int[] { 1, 2, }.ToList();
            pal32Grayscale[3] = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, }.ToList();
            pal32Grayscale[5] = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }.ToList();
            grayscaleColors[32] = pal32Grayscale;

            // below pandora
            Dictionary<int, List<int>> pal35Grayscale = new Dictionary<int, List<int>>();
            // trees
            pal35Grayscale[2] = new int[] { 10, 11, 12, 13, }.ToList();
            pal35Grayscale[3] = new int[] { 11, 12, 13, 14 }.ToList();
            pal35Grayscale[4] = new int[] { 1, 2 }.ToList();
            grayscaleColors[35] = pal35Grayscale;

            // outside shade palace
            Dictionary<int, List<int>> pal36Grayscale = new Dictionary<int, List<int>>();
            // trees
            pal36Grayscale[1] = new int[] { 1, 2 }.ToList();
            pal36Grayscale[2] = new int[] { 10, 11, 12, 13, }.ToList();
            pal36Grayscale[3] = new int[] { 11, 12, 13, 14 }.ToList();
            pal36Grayscale[4] = new int[] { 1, 2, 3, 11, 12, 13, 14, 15 }.ToList();
            pal36Grayscale[5] = new int[] { 10, 11, 12, 13, 14, 15 }.ToList();
            pal36Grayscale[6] = new int[] { 0, 8, 9, 10, }.ToList();
            pal36Grayscale[7] = new int[] { 4 }.ToList();
            grayscaleColors[36] = pal36Grayscale;

            // ice country
            Dictionary<int, List<int>> pal37Grayscale = new Dictionary<int, List<int>>();
            pal37Grayscale[2] = new int[] { 10, 11, 12, 13, 14, 15 }.ToList();
            pal37Grayscale[3] = new int[] { 0, 11, 12, 13, 14 }.ToList();
            pal37Grayscale[5] = new int[] { 10, 11, 12, 13, 14, 15 }.ToList();
            pal37Grayscale[6] = new int[] { 0 }.ToList();
            grayscaleColors[37] = pal37Grayscale;

            // lighthouse outside
            Dictionary<int, List<int>> pal38Grayscale = new Dictionary<int, List<int>>();
            // trees
            pal38Grayscale[5] = new int[] { 1, 2 }.ToList();
            pal38Grayscale[6] = new int[] { 10, 11, 12, 13, }.ToList();
            pal38Grayscale[7] = new int[] { 3, 4, 5, 6, 7 }.ToList();
            grayscaleColors[38] = pal38Grayscale;

            // matango
            Dictionary<int, List<int>> pal40Grayscale = new Dictionary<int, List<int>>();
            pal40Grayscale[2] = new int[] { 3, 4, 5, 6, 7 }.ToList();
            pal40Grayscale[3] = new int[] { 11, 12, 13, 14, 15 }.ToList();
            pal40Grayscale[4] = new int[] { 0 }.ToList();
            pal40Grayscale[5] = new int[] { 7, 15 }.ToList();
            pal40Grayscale[6] = new int[] { 0, 10, 11, 14 }.ToList();
            grayscaleColors[40] = pal40Grayscale;

            // pure lands savepoint
            Dictionary<int, List<int>> pal41Grayscale = new Dictionary<int, List<int>>();
            pal41Grayscale[2] = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }.ToList();
            pal41Grayscale[4] = new int[] { 10, 11, 12, 13, 14 }.ToList();
            pal41Grayscale[5] = new int[] { 11, 13, 14, 15 }.ToList();
            pal41Grayscale[6] = new int[] { 0 }.ToList();
            pal41Grayscale[7] = new int[] { 3, 4, 5 }.ToList();
            grayscaleColors[41] = pal41Grayscale;

            // north town
            Dictionary<int, List<int>> pal48Grayscale = new Dictionary<int, List<int>>();
            pal48Grayscale[1] = new int[] { 1, 2, 3, 4, 5 }.ToList();
            pal48Grayscale[4] = new int[] { 15 }.ToList();
            pal48Grayscale[5] = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }.ToList();
            pal48Grayscale[6] = new int[] { 1, 2 }.ToList();
            pal48Grayscale[7] = new int[] { 10, 11, 12, 13, }.ToList();
            grayscaleColors[48] = pal48Grayscale;

            // snow town
            Dictionary<int, List<int>> pal49Grayscale = new Dictionary<int, List<int>>();
            pal49Grayscale[5] = new int[] { 7, 8, 9 }.ToList();
            pal49Grayscale[6] = new int[] { 10, 11, 12, 13, 14, 15 }.ToList();
            pal49Grayscale[7] = new int[] { 0 }.ToList();
            grayscaleColors[49] = pal49Grayscale;

            // pure lands
            Dictionary<int, List<int>> pal54Grayscale = new Dictionary<int, List<int>>();
            pal54Grayscale[1] = new int[] { 3, 4, 5 }.ToList();
            pal54Grayscale[2] = new int[] { 3, 4, 5, 8, 9, 10, 11, 12, 13, 14, 15 }.ToList();
            pal54Grayscale[3] = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }.ToList();
            pal54Grayscale[4] = new int[] { 5, 6, 13, 14, 15, }.ToList();
            pal54Grayscale[5] = new int[] { 0, 10, 11, 12, 13, 14 }.ToList();
            grayscaleColors[54] = pal54Grayscale;

            // outside wind palace
            Dictionary<int, List<int>> pal55Grayscale = new Dictionary<int, List<int>>();
            pal55Grayscale[2] = new int[] { 11, 12, 13, 14 }.ToList();
            pal55Grayscale[5] = new int[] { 10, 11, 12, 13, 14, 15 }.ToList();
            pal55Grayscale[6] = new int[] { 0, 10, 11, 12, 13, 14, 15 }.ToList();
            pal55Grayscale[7] = new int[] { 0 }.ToList();
            grayscaleColors[55] = pal55Grayscale;

            // kakkara boat thing to luna palace
            Dictionary<int, List<int>> pal57Grayscale = new Dictionary<int, List<int>>();
            pal57Grayscale[1] = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }.ToList();
            pal57Grayscale[3] = new int[] { 3, 4, 5, 6, 7, }.ToList();
            pal57Grayscale[6] = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }.ToList();
            pal57Grayscale[7] = new int[] { 1, 2, 3, 4, 5, 6, 7, }.ToList();
            grayscaleColors[57] = pal57Grayscale;


            // turtle island, upper land shop area
            Dictionary<int, List<int>> pal61Grayscale = new Dictionary<int, List<int>>();
            // trees
            pal61Grayscale[1] = new int[] { 1, 2 }.ToList();
            pal61Grayscale[2] = new int[] { 10, 11, 12, 13, }.ToList();
            pal61Grayscale[3] = new int[] { 11, 12, 13, 14 }.ToList();
            pal61Grayscale[5] = new int[] { 13, 14, 15 }.ToList();
            pal61Grayscale[6] = new int[] { 0 }.ToList();
            grayscaleColors[61] = pal61Grayscale;
            
            // outside mountain area
            Dictionary<int, List<int>> pal68Grayscale = new Dictionary<int, List<int>>();
            pal68Grayscale[5] = new int[] { 1, 2, 3, 11, 12, 13, 14, 15 }.ToList();
            pal68Grayscale[6] = new int[] { 0, 8, 9, 10, }.ToList();
            pal68Grayscale[7] = new int[] { 4 }.ToList();
            grayscaleColors[68] = pal68Grayscale;

            // kakkara
            Dictionary<int, List<int>> pal71Grayscale = new Dictionary<int, List<int>>();
            pal71Grayscale[5] = new int[] { 7, 8, 9 }.ToList();
            pal71Grayscale[6] = new int[] { 10, 11, 12, 13, 14, 15 }.ToList();
            pal71Grayscale[7] = new int[] { 0 }.ToList();
            grayscaleColors[71] = pal71Grayscale;

            // fire palace outside
            Dictionary<int, List<int>> pal74Grayscale = new Dictionary<int, List<int>>();
            pal74Grayscale[2] = new int[] { 11, 12, 13, 14, }.ToList();
            pal74Grayscale[5] = new int[] { 1, 2, 3, 4, 5, 6, 10, 11, 12, 13, 14, 15 }.ToList();
            pal74Grayscale[6] = new int[] { 0 }.ToList();
            grayscaleColors[74] = pal74Grayscale;

            // nt ruins outside
            Dictionary<int, List<int>> pal78Grayscale = new Dictionary<int, List<int>>();
            pal78Grayscale[1] = new int[] { 1, 2, }.ToList();
            pal78Grayscale[3] = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, }.ToList();
            pal78Grayscale[5] = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }.ToList();
            grayscaleColors[78] = pal78Grayscale;

            // ntc outside
            Dictionary<int, List<int>> pal83Grayscale = new Dictionary<int, List<int>>();
            pal83Grayscale[4] = new int[] { 9, 10, 11, 12, 13, 14, 15 }.ToList();
            pal83Grayscale[5] = new int[] { 0, }.ToList();
            pal83Grayscale[6] = new int[] { 10, 11, 12, 13, 14, 15 }.ToList();
            pal83Grayscale[7] = new int[] { 0, 3, 4, 5, 6, 7 }.ToList();
            grayscaleColors[83] = pal83Grayscale;

            // ice castle outside
            Dictionary<int, List<int>> pal84Grayscale = new Dictionary<int, List<int>>();
            pal84Grayscale[4] = new int[] { 11, 12, 13, 14, 15 }.ToList();
            pal84Grayscale[5] = new int[] { 0, }.ToList();
            grayscaleColors[84] = pal84Grayscale;

            // dryad palace outside
            Dictionary<int, List<int>> pal98Grayscale = new Dictionary<int, List<int>>();
            pal98Grayscale[4] = new int[] { 7, 8, 9, 10 }.ToList();
            pal98Grayscale[5] = new int[] { 10, 11, 12, 13, 14, 15 }.ToList();
            pal98Grayscale[6] = new int[] { 0 }.ToList();
            grayscaleColors[98] = pal98Grayscale;

            // water palace outside
            Dictionary<int, List<int>> pal99Grayscale = new Dictionary<int, List<int>>();
            pal99Grayscale[5] = new int[] { 10, 11, 12, 13, 14, 15 }.ToList();
            pal99Grayscale[6] = new int[] { 0 }.ToList();
            grayscaleColors[99] = pal99Grayscale;

            // 4,8; 5,11?
            // 5,9
            // mana tree
            Dictionary<int, List<int>> pal103Grayscale = new Dictionary<int, List<int>>();
            pal103Grayscale[2] = new int[] { 1, 2, 3, 4, 5, 6, 7 }.ToList();
            pal103Grayscale[4] = new int[] { 1 }.ToList();
            pal103Grayscale[5] = new int[] { 8, }.ToList();
            pal103Grayscale[6] = new int[] { 10, 13 }.ToList();
            grayscaleColors[103] = pal103Grayscale;

            // pure lands paler
            Dictionary<int, List<int>> pal104Grayscale = new Dictionary<int, List<int>>();
            pal104Grayscale[1] = new int[] { 3, 4, 5 }.ToList();
            pal104Grayscale[2] = new int[] { 3, 4, 5, 8, 9, 10, 11, 12, 13, 14, 15 }.ToList();
            pal104Grayscale[3] = new int[] { 0, 1, 2, 3, 4, 5, 6, 7 }.ToList();
            pal104Grayscale[4] = new int[] { 5, 6, 13, 14, 15, }.ToList();
            pal104Grayscale[5] = new int[] { 0, 10, 11, 12, 13, 14 }.ToList();
            grayscaleColors[104] = pal104Grayscale;

            // kakkara cannon
            Dictionary<int, List<int>> pal108Grayscale = new Dictionary<int, List<int>>();
            pal108Grayscale[2] = new int[] { 1, 2, 3, 4, 5, 10, 11, 12, 13, 14, 15 }.ToList();
            pal108Grayscale[3] = new int[] { 11, 12, 13, 14, }.ToList();
            pal108Grayscale[5] = new int[] { 10, 11, 12, 13, 14, 15 }.ToList();
            pal108Grayscale[6] = new int[] { 0 }.ToList();
            grayscaleColors[108] = pal108Grayscale;

            // ntc castle roof fight
            Dictionary<int, List<int>> pal109Grayscale = new Dictionary<int, List<int>>();
            pal109Grayscale[5] = new int[] { 1, 2, 3, 4, 5, 6, 7, }.ToList();
            pal109Grayscale[6] = new int[] { 0, 8, 9, 10, }.ToList();
            pal109Grayscale[7] = new int[] { 4 }.ToList();
            grayscaleColors[109] = pal109Grayscale;

            // kakkara outside near luna palace
            Dictionary<int, List<int>> pal110Grayscale = new Dictionary<int, List<int>>();
            pal110Grayscale[1] = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14 }.ToList();
            pal110Grayscale[2] = new int[] { 4, 5, 6, 7, }.ToList();
            pal110Grayscale[3] = new int[] { 1, 2, 3, 4, 5, 6, 7, }.ToList();
            pal110Grayscale[4] = new int[] { 1, 2, 3, 4, 5, 6, 7, }.ToList();
            pal110Grayscale[5] = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }.ToList();
            pal110Grayscale[6] = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8 }.ToList();
            pal110Grayscale[7] = new int[] { 1, 2, 3, 4, 5, 6, 7, }.ToList();
            grayscaleColors[110] = pal110Grayscale;

            // pandora
            Dictionary<int, List<int>> pal112Grayscale = new Dictionary<int, List<int>>();
            // trees
            pal112Grayscale[1] = new int[] { 1, 2 }.ToList();
            pal112Grayscale[2] = new int[] { 10, 11, 12, 13, }.ToList();
            pal112Grayscale[3] = new int[] { 11, 12, 13, 14 }.ToList();
            // rooftops
            pal112Grayscale[4] = new int[] { 1, 2 }.ToList();
            grayscaleColors[112] = pal112Grayscale;

            Dictionary<byte, double> prevH = new Dictionary<byte, double>();
            Dictionary<byte, double> prevS = new Dictionary<byte, double>();
            Dictionary<byte, double> prevV = new Dictionary<byte, double>();

            int palStartOffset = 0xC7FE0;

            // was 220
            double blueHue = 230;

            // rainbow windows in ice country
            Dictionary<int, Dictionary<int, List<int>>> paletteExceptions = new Dictionary<int, Dictionary<int, List<int>>>();
            Dictionary<int, List<int>> pal49Exception = new Dictionary<int, List<int>>();
            pal49Exception[4] = new int[] { 0, 1, 2, 3, 4, 5 }.ToList();
            paletteExceptions[49] = pal49Exception;

            // stars in desert
            Dictionary<int, List<int>> pal27Exception = new Dictionary<int, List<int>>();
            pal27Exception[2] = new int[] { 9, 10, 11, 12, 13, 14, 15 }.ToList(); // cactus
            pal27Exception[3] = new int[] { 0 }.ToList(); // cactus
            pal27Exception[6] = new int[] { 9, 10, 11, 12, 13, 14, 15 }.ToList();
            pal27Exception[7] = new int[] { 0 }.ToList();
            paletteExceptions[27] = pal27Exception;

            Dictionary<int, List<int>> pal110Exception = new Dictionary<int, List<int>>();
            pal110Exception[2] = new int[] { 11, 12, 13, 14, 15 }.ToList(); // cactus
            pal110Exception[3] = new int[] { 0 }.ToList(); // cactus
            pal110Exception[6] = new int[] { 9, 10, 11, 12, 13, 14, 15 }.ToList();
            pal110Exception[7] = new int[] { 0 }.ToList();
            paletteExceptions[110] = pal110Exception;

            // trees in icecountry
            Dictionary<int, List<int>> pal37Exception = new Dictionary<int, List<int>>();
            pal37Exception[1] = new int[] { 1, 2, 3, 4, 7, 9 }.ToList();
            pal37Exception[5] = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 11 }.ToList();
            paletteExceptions[37] = pal37Exception;

            double satThreshold = 0.3;
            Dictionary<int, double> huePreShift = new Dictionary<int, double>();

            Dictionary<byte, byte> palSetSwaps = new Dictionary<byte, byte>();
            // normal ul->use winter ul
            palSetSwaps[62] = 66;

            // gold city
            for (byte paletteSet = 24; paletteSet < 120; paletteSet++)
            {
                double hueMax = 250;

                double hChange = ((random.Next() % (hueMax * 2)) / 1.0) - hueMax; // really funky hue
                double sChange = 0.0;
                double vChange = 0.0;
                if (!dontDarkenPalettes.Contains(paletteSet))
                {
                    sChange = +0.1;
                    vChange = -0.4;
                }
                double hueChange = 0.9;
                double hueShift = 0.0;
                if (huePreShift.ContainsKey((int)paletteSet))
                {
                    hueShift = huePreShift[(int)paletteSet];
                }
                if (palSetToDispSetting.ContainsKey(paletteSet))
                {
                    byte dispSetNum = palSetToDispSetting[paletteSet];
                    // 80100
                    // 9 bytes each [7][8] = color
                    byte p1 = rom[0x80100 + dispSetNum * 9 + 7];
                    byte p2 = rom[0x80100 + dispSetNum * 9 + 8];
                    SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });

                    if (paletteSet == 54 || paletteSet == 104 || paletteSet == 108)
                    {
                        // for purelands, grayscale the background color because it's like bright green and things look weird when it's dark
                        int max = Math.Max(thisCol.getRed(), Math.Max(thisCol.getGreen(), thisCol.getBlue()));
                        if (paletteSet == 71 || paletteSet == 108 || paletteSet == 27 || paletteSet == 74 || paletteSet == 40 || paletteSet == 49 || paletteSet == 37 || paletteSet == 110 || paletteSet == 57)
                        {
                            // desert, matango
                            max += 32;
                        }
                        else
                        {
                            max += 64;
                        }
                        max = DataUtil.clampToEndpoints(max, 0, 248);
                        thisCol.setRed((byte)max);
                        thisCol.setGreen((byte)max);
                        thisCol.setBlue((byte)max);
                    }

                    ColorUtil.rgbToHsv(thisCol.getRed(), thisCol.getGreen(), thisCol.getBlue(), out double h, out double s, out double v);
                    h += hueShift;
                    double dist = h - blueHue;
                    h -= dist * hueChange;
                    s += sChange;
                    if (s < satThreshold)
                    {
                        s = satThreshold;
                        h = blueHue;
                    }

                    v += vChange;

                    ColorUtil.hsvToRgb(h, s, v, out int r, out int g, out int b);
                    thisCol.setRed((byte)r);
                    thisCol.setGreen((byte)g);
                    thisCol.setBlue((byte)b);

                    if (!dontChangePalettes.Contains(paletteSet))
                    {
                        thisCol.put(rom, 0x80100 + dispSetNum * 9 + 7);
                    }
                }

                bool doAnimation = false;
                foreach (byte[] groupedPals in groupedPalettes.Values)
                {
                    if (groupedPals.Contains(paletteSet))
                    {
                        if (groupedPals.ToList().IndexOf(paletteSet) == 0)
                        {
                            doAnimation = true;
                            prevH[paletteSet] = hChange;
                            prevS[paletteSet] = sChange;
                            prevV[paletteSet] = vChange;
                        }
                        else
                        {
                            hChange = prevH[groupedPals[0]];
                            sChange = prevS[groupedPals[0]];
                            vChange = prevV[groupedPals[0]];
                        }
                    }
                }

                for (int palnum = 1; palnum < 8; palnum++)
                {
                    for (int col = 1; col < 16; col++)
                    {
                        byte readPalSet = paletteSet;
                        if (palSetSwaps.ContainsKey(paletteSet))
                        {
                            readPalSet = palSetSwaps[paletteSet];
                        }

                        byte p1 = rom[palStartOffset + readPalSet * 30 * 7 + palnum * 30 + col * 2];
                        byte p2 = rom[palStartOffset + readPalSet * 30 * 7 + palnum * 30 + col * 2 + 1];
                        SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });

                        if (grayscaleColors.ContainsKey((int)paletteSet))
                        {
                            Dictionary<int, List<int>> grayScale = grayscaleColors[(int)paletteSet];
                            if (grayScale.ContainsKey(palnum))
                            {
                                if (grayScale[palnum].Contains(col))
                                {
                                    int max = Math.Max(thisCol.getRed(), Math.Max(thisCol.getGreen(), thisCol.getBlue()));
                                    if (paletteSet == 71 || paletteSet == 108 || paletteSet == 27 || paletteSet == 110 || paletteSet == 57 || paletteSet == 74 || paletteSet == 40 || paletteSet == 49 || paletteSet == 37 || (paletteSet == 103 && (palnum == 4 || palnum == 5)))
                                    {
                                        // desert, matango
                                        max += 32;
                                    }
                                    else
                                    {
                                        max += 64;
                                    }
                                    max = DataUtil.clampToEndpoints(max, 0, 248);
                                    thisCol.setRed((byte)max);
                                    thisCol.setGreen((byte)max);
                                    thisCol.setBlue((byte)max);
                                }
                            }
                        }

                        ColorUtil.rgbToHsv(thisCol.getRed(), thisCol.getGreen(), thisCol.getBlue(), out double h, out double s, out double v);
                        if (!darkenOnlyPalettes.Contains(paletteSet))
                        {
                            h += hueShift;
                            double dist = h - blueHue;
                            h -= dist * hueChange;
                        }

                        s += sChange;
                        if (s < satThreshold)
                        {
                            s = satThreshold;
                            h = blueHue;
                        }

                        v += vChange;

                        // make those desert shell things red
                        bool changedSpecial = false;

                        if ((paletteSet == 27 || paletteSet == 110) && palnum == 3 && col >= 9)
                        {
                            h = 340;
                            s -= sChange;
                            v -= vChange;
                            changedSpecial = true;
                        }
                        if (paletteSet == 28 && palnum == 7 && col >= 1 && col <= 5)
                        {
                            // green carpets
                            h = 130;
                            s -= sChange;
                            v -= vChange;
                            v -= 0.2;
                            changedSpecial = true;
                        }
                        if (paletteSet == 28 && palnum == 2 && col >= 1 && col <= 2)
                        {
                            // red carpets
                            h = 340;
                            s -= sChange;
                            v -= vChange;
                            v -= 0.2;
                            s += 0.3;
                            changedSpecial = true;
                        }
                        if (paletteSet == 28 && palnum == 2 && col >= 5 && col <= 6)
                        {
                            // green color on beds
                            h = 130;
                            s -= sChange;
                            v -= vChange;
                            v -= 0.2;
                            changedSpecial = true;
                        }
                        if ((paletteSet == 24 || paletteSet == 25 || paletteSet == 26 || paletteSet == 112) && palnum == 2 && col >= 8 && col <= 9)
                        {
                            // yellow flowers -> green
                            h = 130;
                            s -= sChange;
                            v -= vChange;
                            s -= 0.3;
                            v -= 0.7;
                            changedSpecial = true;
                        }
                        if (((paletteSet == 24 || paletteSet == 112) && palnum == 3 && col >= 8 && col <= 9) || ((paletteSet == 25 || paletteSet == 26) && palnum == 5 && col >= 8 && col <= 9))
                        {
                            // pink flowers -> red
                            h = 0;
                            s -= sChange;
                            v -= vChange;
                            s -= 0.1;
                            v -= 0.7;
                            changedSpecial = true;
                        }
                        if (((paletteSet == 24 || paletteSet == 112 || paletteSet == 61) && palnum == 3 && col >= 1 && col <= 6) || ((paletteSet == 25 || paletteSet == 26) && palnum == 5 && col >= 1 && col <= 6))
                        {
                            // gray walking stones - darker
                            s -= sChange;
                            v -= vChange;
                            v -= 0.5;
                            changedSpecial = true;
                        }

                        ColorUtil.hsvToRgb(h, s, v, out int r, out int g, out int b);
                        thisCol.setRed((byte)r);
                        thisCol.setGreen((byte)g);
                        thisCol.setBlue((byte)b);

                        bool ok = true;
                        if (dontChangePalettes.Contains(paletteSet))
                        {
                            ok = false;
                        }
                        if (paletteExceptions.ContainsKey((int)paletteSet))
                        {
                            if (paletteExceptions[(int)paletteSet].ContainsKey(palnum))
                            {
                                if (paletteExceptions[(int)paletteSet][palnum].Contains(col))
                                {
                                    ok = false;
                                }
                            }
                        }
                        if (ok || changedSpecial)
                        {
                            thisCol.put(rom, palStartOffset + paletteSet * 30 * 7 + palnum * 30 + col * 2);
                        }
                    }
                }

                if (paletteSet == 83)
                {
                    paletteSet = 83;
                }
                if (doAnimation)
                {
                    int animIndex = rom[0x80802 + paletteSet] + 1;
                    int animIndexNext = rom[0x80802 + paletteSet + 1] + 1;
                    int index = animIndex;
                    {
                        int animPalIndex = rom[0x8087B + index];
                        int sourcePalIndex = (animPalIndex & 0x07) + 1;
                        if (paletteSet == 83)
                        {
                            // unsure why we need to fix this one? etc
                            sourcePalIndex = 4;
                        }
                        if (paletteSet == 57)
                        {
                            sourcePalIndex = 1;
                        }
                        if (animPalIndex >= 0x80)
                        {
                            animPalIndex = rom[0x8087B + index + 1];
                        }
                        int animPalOffset = animPalIndex * 0x1E + 0x808E4;
                        for (int palnum = 0; palnum < 7; palnum++)
                        {
                            for (int col = 0; col < 15; col++)
                            {
                                int off = animPalOffset + palnum * 30 + col * 2;
                                // why do some of these overwrite enemy palettes?  bounds check them here..
                                if (off < 0x80FFE || off > 0x819D6)
                                {
                                    byte p1 = rom[animPalOffset + palnum * 30 + col * 2];
                                    byte p2 = rom[animPalOffset + palnum * 30 + col * 2 + 1];
                                    SnesColor thisCol = new SnesColor(new byte[] { p1, p2 });

                                    if (grayscaleColors.ContainsKey((int)paletteSet))
                                    {
                                        Dictionary<int, List<int>> grayScale = grayscaleColors[(int)paletteSet];
                                        if (grayScale.ContainsKey(sourcePalIndex))
                                        {
                                            if (grayScale[sourcePalIndex].Contains(col))
                                            {
                                                int max = Math.Max(thisCol.getRed(), Math.Max(thisCol.getGreen(), thisCol.getBlue()));
                                                if (paletteSet == 71 || paletteSet == 27 || paletteSet == 74 || paletteSet == 40 || paletteSet == 49 || paletteSet == 37 || paletteSet == 57)
                                                {
                                                    // desert, matango
                                                    max += 32;
                                                }
                                                else
                                                {
                                                    max += 64;
                                                }
                                                max = DataUtil.clampToEndpoints(max, 0, 248);
                                                thisCol.setRed((byte)max);
                                                thisCol.setGreen((byte)max);
                                                thisCol.setBlue((byte)max);
                                            }
                                        }
                                    }

                                    double h, s, v;
                                    ColorUtil.rgbToHsv(thisCol.getRed(), thisCol.getGreen(), thisCol.getBlue(), out h, out s, out v);
                                    h += hueShift;
                                    double dist = h - blueHue;
                                    h -= dist * hueChange;

                                    s += sChange;
                                    if (s < satThreshold)
                                    {
                                        s = satThreshold;
                                        h = blueHue;
                                    }
                                    v += vChange;

                                    int r, g, b;
                                    ColorUtil.hsvToRgb(h, s, v, out r, out g, out b);

                                    bool ok = true;
                                    if (dontChangePalettes.Contains(paletteSet))
                                    {
                                        ok = false;
                                    }
                                    if (paletteExceptions.ContainsKey((int)paletteSet))
                                    {
                                        if (paletteExceptions[(int)paletteSet].First().Value.Contains(col + 1))
                                        {
                                            ok = false;
                                        }
                                    }
                                    if (ok)
                                    {
                                        thisCol.put(rom, animPalOffset + palnum * 30 + col * 2);
                                    }
                                }
                            }
                        }
                    }
                }

            }

            // disable palette animation for ice country
            rom[0x80886] = 0x17;
            rom[0x80887] = 0x17;

            // same for undine cave
            rom[0x808D1] = 0x36;
            rom[0x808D2] = 0x36;

            // and gaia's navel water
            rom[0x80884] = 0x36;
            rom[0x80885] = 0x36;

        }

    }
}
