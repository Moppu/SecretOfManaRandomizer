using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.processing.openworld.randomization;
using SoMRandomizer.processing.vanillarando;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Random nesoberi on the table on map 306, tileset 15 (copied to 33), palette set 86
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class Nesoberi : RandoProcessor
    {
        private class IndividualNeso
        {
            public string filename;
            public string outfitName;
            public IndividualNeso(string filename, string outfitName)
            {
                this.filename = filename;
                this.outfitName = outfitName;
            }
        }

        private class NesoCharInfo
        {
            public IndividualNeso[] outfits;
            public string characterName;
            public int birthMonth;
            public int birthDay;
            public NesoCharInfo(IndividualNeso[] outfits, string characterName, int birthMonth, int birthDay)
            {
                this.outfits = outfits;
                this.characterName = characterName;
                this.birthMonth = birthMonth;
                this.birthDay = birthDay;
            }
        }

        protected override string getName()
        {
            return "Random nesoberi with hint";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            Random r = context.randomCosmetic;
            List<NesoCharInfo> nesoResources = new List<NesoCharInfo>();

            // niji
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_rina.png", "Nijigasaki Winter"),
                new IndividualNeso("nesoberi_rina_noboard.png", "Nijigasaki Winter - No board"),
                new IndividualNeso("nesoberi_rina_summer.png", "Nijigasaki Summer"),
                new IndividualNeso("nesoberi_rina_summer_noboard.png", "Nijigasaki Summer - No board"),
            }, "Tennoji Rina", 11, 13));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_ai.png", "Nijigasaki Winter"),
            }, "Miyashita Ai", 5, 30));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_kasumi.png", "Nijigasaki Winter"),
            }, "Nakasu Kasumi", 1, 23));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_shizuku.png", "Nijigasaki Winter"),
            }, "Osaka Shizuku", 4, 3));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_ayumu.png", "Nijigasaki Winter"),
            }, "Uehara Ayumu", 3, 1));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_setsuna.png", "Nijigasaki Winter - No wink"),
                new IndividualNeso("nesoberi_setsuna_summer.png", "Nijigasaki Summer - Wink"),
            }, "Yuki Setsuna", 8, 8));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_karin.png", "Nijigasaki Winter"),
                new IndividualNeso("nesoberi_karin_summer.png", "Nijigasaki Summer"),
            }, "Asaka Karin", 6, 26));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_kanata.png", "Nijigasaki Winter"),
            }, "Konoe Kanata", 12, 16));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_emma.png", "Nijigasaki Winter"),
            }, "Emma Verde", 2, 5));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_shioriko.png", "Nijigasaki Winter"),
            }, "Mifune Shioriko", 10, 5));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_yu.png", "Nijigasaki Winter"),
            }, "Takasaki Yu", 1, 29));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_mia.png", "Nijigasaki Winter"),
            }, "Mia Taylor", 12, 6));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_lanzhu_queendom.png", "Queendom"),
            }, "Zhong Lanzhu", 2, 15));

            // aqours
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_hanamaru.png", "Uranohoshi Winter"),
                new IndividualNeso("nesoberi_hanamaru_wink.png", "Winking Jersey"),
            }, "Kunikida Hanamaru", 3, 4));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_yohane.png", "Uranohoshi Winter"),
                new IndividualNeso("nesoberi_yohane_wink.png", "Winking Jersey"),
            }, "Fallen Angel Yohane", 7, 13));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_ruby.png", "Uranohoshi Winter"),
                new IndividualNeso("nesoberi_ruby_wink.png", "Winking Jersey"),
            }, "Kurosawa Ruby", 9, 21));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_you.png", "Uranohoshi Winter"),
                new IndividualNeso("nesoberi_you_wink.png", "Winking Jersey"),
                new IndividualNeso("nesoberi_you_training.png", "Training Outfit"),
            }, "Watanabe You", 4, 17));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_chika.png", "Uranohoshi Winter"),
                new IndividualNeso("nesoberi_chika_wink.png", "Winking Jersey"),
            }, "Takami Chika", 8, 1));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_riko.png", "Uranohoshi Winter"),
                new IndividualNeso("nesoberi_riko_wink.png", "Winking Jersey"),
            }, "Sakarauchi Riko", 9, 19));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_mari.png", "Uranohoshi Winter"),
                new IndividualNeso("nesoberi_mari_wink.png", "Winking Jersey"),
            }, "Ohara Mari", 6, 13));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_dia.png", "Uranohoshi Winter"),
                new IndividualNeso("nesoberi_dia_wink.png", "Winking Jersey"),
            }, "Kurosawa Dia", 1, 1));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_kanan.png", "Uranohoshi Winter"),
                new IndividualNeso("nesoberi_kanan_wink.png", "Winking Jersey"),
            }, "Matsuura Kanan", 2, 10));

            // muse
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_nico.png", "Otonokizaka Winter"),
            }, "Yazawa Nico", 7, 22));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_honk.png", "Otonokizaka Winter"),
                new IndividualNeso("nesoberi_honk_sleep.png", "Sleeping"),
            }, "Kousaka Honoka", 8, 3));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_maki.png", "Otonokizaka Winter"),
            }, "Nishikino Maki", 4, 19));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_rin.png", "Otonokizaka Winter"),
            }, "Hoshizora Rin", 11, 1));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_hanayo.png", "Otonokizaka Winter"),
            }, "Koizumi Hanayo", 1, 17));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_umi.png", "Otonokizaka Winter"),
            }, "Sonoda Umi", 3, 15));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_kotori.png", "Otonokizaka Winter"),
            }, "Minami Kotori", 9, 12));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_eli.png", "Otonokizaka Winter"),
            }, "Ayase Eli", 10, 21));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_nozomi.png", "Otonokizaka Winter"),
            }, "Tojo Nozomi", 6, 9));

            // saint snow
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_leah.png", "Seisen Academy"),
            }, "Kazuno Leah", 12, 12));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_sarah.png", "Seisen Academy"),
            }, "Kazuno Sarah", 5, 4));

            // liella
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_kanon.png", "Yuigaoka Winter"),
                new IndividualNeso("nesoberi_kanon_happy.png", "Extra Happy"),
            }, "Shibuya Kanon", 5, 1));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_keke.png", "Yuigaoka Winter"),
            }, "Tang Keke", 7, 17));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_chisato.png", "Yuigaoka Winter"),
            }, "Arashi Chisato", 2, 25));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_ren.png", "Yuigaoka Winter"),
            }, "Hazuki Ren", 11, 24));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_sumire.png", "Yuigaoka Winter"),
            }, "Heanna Sumire", 9, 28));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_kinako.png", "Yuigaoka Winter"),
            }, "Sakurakoji Kinako", 4, 10));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_shiki.png", "Yuigaoka Winter"),
            }, "Wakana Shiki", 6, 17));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_mei.png", "Yuigaoka Winter"),
            }, "Yoneme Mei", 10, 29));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_natsumi.png", "Yuigaoka Winter"),
            }, "Onitsuka Natsumi", 8, 7));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_tomari.png", "Yuigaoka Winter"),
            }, "Onitsuka Tomari", 12, 28));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_margarete.png", "Yuigaoka Winter"),
            }, "Wien Margarete", 1, 20));

            // hasunosora
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_kaho.png", "Hasunosora"),
            }, "Hinoshita Kaho", 5, 22));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_sayaka.png", "Hasunosora"),
            }, "Murano Sayaka", 1, 13));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_kozue.png", "Hasunosora"),
            }, "Otomune Kozue", 6, 15));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_tsuzuri.png", "Hasunosora"),
            }, "Yugiri Tsuzuri", 11, 17));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_rurino.png", "Hasunosora"),
            }, "Osawa Rurino", 8, 31));
            nesoResources.Add(new NesoCharInfo(new IndividualNeso[] {
                new IndividualNeso("nesoberi_megumi.png", "Hasunosora"),
            }, "Fujishima Megumi", 12, 20));

            // MOPPLE: should add:
            // - margarete and tomari from liella
            // - the newest 3 Hasu characters
            // - some other series characters? maybe touhou ones? they have different designs though and don't have the solid white eyes of lovelive ones

            int totalNesos = 0;
            foreach (NesoCharInfo neso in nesoResources)
            {
                totalNesos += neso.outfits.Length;
            }
            Logging.log(totalNesos + " nesos to choose from. The nesoberi library grows ever larger.", "spoiler");

            DateTime currentTime = DateTime.Now;
            int currentMonth = currentTime.Month;
            int currentDay = currentTime.Day;
            NesoCharInfo birthdayNeso = null;
            foreach (NesoCharInfo neso in nesoResources)
            {
                if (neso.birthMonth == currentMonth && neso.birthDay == currentDay)
                {
                    Logging.log(neso.characterName + "'s birthday today! Choosing this neso.", "spoiler");
                    birthdayNeso = neso;
                    break;
                }
            }

            // even if it's a birthday, we roll one anyway, so not to change the rng.
            NesoCharInfo nesoInfo = nesoResources[r.Next() % nesoResources.Count];
            if (birthdayNeso != null)
            {
                nesoInfo = birthdayNeso;
            }
            IndividualNeso outfitNeso = nesoInfo.outfits[r.Next() % nesoInfo.outfits.Length];
            string resourceName = outfitNeso.filename;
            string outfitName = outfitNeso.outfitName;
            Logging.log("Nesoberi: " + nesoInfo.characterName + " (" + outfitName + "). Headpats are encouraged and greatly appreciated.", "spoiler");
            byte[] pngData = DataUtil.readResource("SoMRandomizer.Resources.nesoberi." + resourceName);

            Bitmap b = new Bitmap(new MemoryStream(pngData));
            // presumes 0,0 is the bg color; should be true of all these
            Color bgColor = b.GetPixel(0, 0);
            int bgColorInt = getColorInt(bgColor);

            // generate palette and 8-bit image by searching for colors in the loaded png; presumes 15 or less unique colors used
            List<int> palette = new List<int>();
            List<byte> eightBitImage = new List<byte>();
            for (int y = 0; y < 24; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    Color c = b.GetPixel(x, y);
                    int cInt = getColorInt(c);
                    if (!palette.Contains(cInt))
                    {
                        palette.Add(cInt);
                    }
                    eightBitImage.Add((byte)palette.IndexOf(cInt));
                }
            }

            // ----------------------------------------------------------------------------------
            // change things that use palette 4 to use palette 5 instead, and it mostly looks fine
            // change the skull thing on the table to use palette 4 - that will become the nesoberi
            // and will be the only thing left in the tileset using that palette
            // ----------------------------------------------------------------------------------
            // tileset16 15; xb4000 -> xb402d -> 0BA6AE (next=0BAEC7)
            byte[] tilesetRaw = VanillaTilesetUtil.getCompressedVanillaTileset16(origRom, 15);
            List<short> tilesetDecomp = VanillaTilesetUtil.DecodeTileset16(tilesetRaw);
            byte targetPalNum = 4;
            byte replacementPalNum = 5;
            // result: VHAPPPTTTTTTTTT
            for (int i = 0; i < 384 * 4; i++)
            {
                byte pal = (byte)((tilesetDecomp[i] & 0x1C00) >> 10);
                // anything that uses 4, make it use 5
                if (pal == targetPalNum)
                {
                    pal = replacementPalNum;
                    int f = tilesetDecomp[i] & 0xE3FF;
                    f |= (pal << 10);
                    tilesetDecomp[i] = (short)f;
                }
                // make the thing we're changing into a neso use 4
                if ((i / 4) == 192 + 133 || (i / 4) == 192 + 149)
                {
                    pal = targetPalNum;
                    int f = tilesetDecomp[i] & 0xE3FF;
                    f |= (pal << 10);
                    tilesetDecomp[i] = (short)f;
                }
            }

            List<byte> tilesetCompressed = VanillaTilesetUtil.EncodeTileset16(tilesetDecomp.ToArray());
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, tilesetCompressed.Count);
            int tilesetNewOffset = context.workingOffset;
            foreach (byte bb in tilesetCompressed)
            {
                outRom[context.workingOffset++] = bb;
            }

            // there's some space here for tilesets, use one
            int targetTileset16 = 33;
            // collision data
            // b3580->b36ff
            // originally b1a80
            for (int i = 0; i < 0x180; i++)
            {
                outRom[0xb3580 + i] = outRom[0xb1a80 + i];
            }

            // file offsets instead of rom offsets for some reason here
            DataUtil.int24ToBytes(outRom, VanillaRomOffsets.TILESET16_OFFSETS + targetTileset16 * 3, tilesetNewOffset);

            int palStartOffset = 0xC7FE0;
            int srcPalSet = 86;
            int palSet = 90;
            for (int palNum = 0; palNum <= 7; palNum++)
            {
                for (int palIndex = 1; palIndex <= 15; palIndex++)
                {
                    outRom[palStartOffset + palSet * 30 * 7 + palNum * 30 + palIndex * 2] = outRom[palStartOffset + srcPalSet * 30 * 7 + palNum * 30 + palIndex * 2];
                    outRom[palStartOffset + palSet * 30 * 7 + palNum * 30 + palIndex * 2 + 1] = outRom[palStartOffset + srcPalSet * 30 * 7 + palNum * 30 + palIndex * 2 + 1];
                }
            }

            outRom[0x8cac9] = (byte)(palSet);
            outRom[0x8caca] = (byte)(0x80 + targetTileset16);


            // ----------------------------------------------------------------------------------
            // replace palette 7 in palette set 86
            // ----------------------------------------------------------------------------------

            int palnum = targetPalNum;
            for (int i = 1; i < palette.Count; i++)
            {
                int palIndex = i;
                SnesColor col = new SnesColor((byte)palette[i], (byte)(palette[i] >> 8), (byte)(palette[i] >> 16));
                col.put(outRom, palStartOffset + palSet * 30 * 7 + palnum * 30 + palIndex * 2);
            }

            // ----------------------------------------------------------------------------------
            // convert the 8-bit image to a 4-bit snes format one
            // replace the images for the skull thing with a nesoberi; tile 2237
            // ----------------------------------------------------------------------------------
            // 1e17a0, 1e17c0   top
            // 1e19a0, 1e19c0   middle
            // 1e1ba0, 1e1bc0   bottom
            // 4bpp, planar composite
            int[] tileOffsets = new int[] { 0x1e17a0, 0x1e17c0, 0x1e19a0, 0x1e19c0, 0x1e1ba0, 0x1e1bc0, };
            for (int i = 0; i < 6; i++)
            {
                SnesTile tileData = new SnesTile();
                int _xpos = (i % 2) * 8;
                int _ypos = (i / 2) * 8;
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        tileData.data[y * 8 + x] = eightBitImage[(_ypos + y) * 16 + _xpos + x];
                    }
                }
                SnesTile.WriteTile4bpp(tileData, outRom, tileOffsets[i]);
            }

            // ----------------------------------------------------------------------------------
            // remove animation on the skull thing that used to mirror the top part back and forth
            // ----------------------------------------------------------------------------------
            // 8069c->806a0
            // 2D C8 14 CD
            // replace with:
            outRom[0x8069c] = 0x3D;
            outRom[0x8069d] = 0x4F;
            outRom[0x8069e] = 0x4D;
            outRom[0x8069f] = 0x4F;

            // remove the nesoberi/skull thing in the ice palace before the boss
            // EDF2C, EDF3D, EDF41, EDF31, EDF44, EDF47
            outRom[0xEDF2C] = 0x00;
            outRom[0xEDF3D] = 0x00;
            outRom[0xEDF41] = 0x00;
            outRom[0xEDF31] = 0x00;
            outRom[0xEDF44] = 0x00;
            outRom[0xEDF47] = 0x00;

            // -------------------------------------------------------
            // event when you step next to it that says you patted it
            // -------------------------------------------------------
            // modify map data to have the trigger tile
            outRom[0xE1B41] = 0x41;
            outRom[0xE1C10] = 0x5E;
            outRom[0xE1C11] = 0xC0;
            outRom[0xE1C12] = 0xC2;
            // modify list of triggers to be in a different spot and have 6 items, including event 0x18B
            outRom[0x88FBE] = 0xE0;
            outRom[0x88FBF] = 0x01;
            outRom[0x88FC0] = 0x2F;
            outRom[0x88FC1] = 0x09;
            outRom[0x88FC2] = 0x30;
            outRom[0x88FC3] = 0x09;
            outRom[0x88FC4] = 0x8B;
            outRom[0x88FC5] = 0x01;
            outRom[0x88FC6] = 0x2B;
            outRom[0x88FC7] = 0x09;
            outRom[0x88FC8] = 0x2B;
            outRom[0x88FC9] = 0x09;
            // move it
            outRom[0x84264] = 0xBE;
            outRom[0x84265] = 0x8F;

            EventScript newEvent18B = new EventScript();
            context.replacementEvents[0x18B] = newEvent18B;

            string an = "a";
            if (nesoInfo.characterName.StartsWith("A") || nesoInfo.characterName.StartsWith("E") || nesoInfo.characterName.StartsWith("I") || nesoInfo.characterName.StartsWith("O") || nesoInfo.characterName.StartsWith("U"))
            {
                an = "an";
            }
            newEvent18B.OpenDialogueBox();
            newEvent18B.AddDialogue(VanillaEventUtil.wordWrapText("It's " + an + " " + nesoInfo.characterName + " nesoberi plush."));
            newEvent18B.Add(EventCommandEnum.CLEAR_DIALOGUE.Value);
            newEvent18B.AddDialogue("You pat its head.");

            int numNesoHints = OpenWorldHints.NUM_NESO_HINTS;
            string[] nesoNames = nesoInfo.characterName.Split(new char[] { ' ' });
            string nesoFirstname = nesoNames[1];
            // western names; swap
            if (nesoFirstname == "Taylor" || nesoFirstname == "Verde")
            {
                nesoFirstname = nesoNames[0];
            }
            // yohane name has 3 words
            if (nesoFirstname == "Angel")
            {
                nesoFirstname = nesoNames[2];
            }

            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 30);

            /*
             * replace event opcode 0B, which isn't used, with a command to jump ahead by the given parameter
             * number of bytes in the current script, to avoid having to use multiple events for event logic.
             * This allows the neso to give a bunch of different hints without jumping to different events
             * to do so.
                C1/EAA3:    C220        REP #$20
                C1/EAA5:    AD0E01      LDA $010E
             */
            outRom[0x1EAA3] = 0x22;
            outRom[0x1EAA4] = (byte)(context.workingOffset);
            outRom[0x1EAA5] = (byte)(context.workingOffset >> 8);
            outRom[0x1EAA6] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x1EAA7] = 0x60; // RTS after

            // 16bit A
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // inc $D1 - cmd itself
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0xD1;
            // LDA $[D1]
            outRom[context.workingOffset++] = 0xA7;
            outRom[context.workingOffset++] = 0xD1;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC $D1
            outRom[context.workingOffset++] = 0x65;
            outRom[context.workingOffset++] = 0xD1;
            // STA $D1
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xD1;
            // inc $D1 - param
            outRom[context.workingOffset++] = 0xE6;
            outRom[context.workingOffset++] = 0xD1;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // ^ so now events should be able to do 0B [xx] to skip ahead xx bytes after the end of the command

            // Neso will only give hints for stuff if you haven't gotten them already, and will use the following flags to determine that
            Dictionary<string, byte> eventFlags = new Dictionary<string, byte>();
            eventFlags["undine spells"] = EventFlags.ELEMENT_UNDINE_FLAG;
            eventFlags["gnome spells"] = EventFlags.ELEMENT_GNOME_FLAG;
            eventFlags["sylphid spells"] = EventFlags.ELEMENT_SYLPHID_FLAG;
            eventFlags["salamando spells"] = EventFlags.ELEMENT_SALAMANDO_FLAG;
            eventFlags["lumina spells"] = EventFlags.ELEMENT_LUMINA_FLAG;
            eventFlags["shade spells"] = EventFlags.ELEMENT_SHADE_FLAG;
            eventFlags["luna spells"] = EventFlags.ELEMENT_LUNA_FLAG;
            eventFlags["dryad spells"] = EventFlags.ELEMENT_DRYAD_FLAG;
            eventFlags["boy"] = 0x0C;
            eventFlags["girl"] = 0x0D;
            eventFlags["sprite"] = 0x0E;
            eventFlags["axe"] = 0xCA;
            eventFlags["sword"] = 0xC9;
            eventFlags["whip"] = 0xCC;
            eventFlags["water seed"] = 0x90;
            eventFlags["earth seed"] = 0x91;
            eventFlags["wind seed"] = 0x92;
            eventFlags["fire seed"] = 0x93;
            eventFlags["light seed"] = 0x94;
            eventFlags["dark seed"] = 0x95;
            eventFlags["moon seed"] = 0x96;
            eventFlags["dryad seed"] = 0x97;
            eventFlags["moogle belt"] = EventFlags.OPENWORLD_MOOGLE_BELT_FLAG;
            eventFlags["midge mallet"] = EventFlags.MIDGE_MALLET_FLAG;

            newEvent18B.Add(EventCommandEnum.CLEAR_DIALOGUE.Value);

            Logging.log("Neso hints:", "spoiler");
            for (int i = 0; i < numNesoHints; i++)
            {
                string nesoHintItem = context.workingData.get(OpenWorldHints.NESO_PRIZE_PREFIX + i);
                string nesoHintText = context.workingData.get(OpenWorldHints.NESO_HINT_PREFIX + i);
                byte eventFlag = 0x00;
                if (eventFlags.ContainsKey(nesoHintItem))
                {
                    eventFlag = eventFlags[nesoHintItem];
                }
                string nesoHint = nesoFirstname + " neso says: find " + nesoHintItem + " " + nesoHintText;
                Logging.log(nesoHint, "spoiler");
                string headpatDialogue = VanillaEventUtil.wordWrapText(nesoHint);
                List<byte> headpatDialogueBytes = VanillaEventUtil.getBytes(headpatDialogue);
                int dialogueSkipCount = headpatDialogueBytes.Count + 4;
                // event logic to skip ahead if we've already gotten it
                newEvent18B.Logic(eventFlag, 0x1, 0xF, EventScript.GetNesoJumpAhead((byte)dialogueSkipCount));
                newEvent18B.AddDialogue(headpatDialogue); // if we didn't skip, give the hint
                newEvent18B.CloseDialogueBox();
                newEvent18B.End();
            }
            // skipped all
            newEvent18B.AddDialogue(VanillaEventUtil.wordWrapText(nesoFirstname + " neso has no more hints for you."));
            newEvent18B.CloseDialogueBox();
            newEvent18B.End();

            // replace snowman with happy kanon snow neso
            // tiles: 
            // 0x1F42A0 (upper left)
            // 0x1F42C0 (upper right)
            // 0x1F42E0 (middle left)
            // (gap)
            // 0x1F5100 (middle right)
            // 0x1F5120 (lower left)
            // 0x1F5140 (lower right)
            Dictionary<int, byte> snowNesoPaletteIndexes = new Dictionary<int, byte>();
            // colors from the loaded png -> snowman palette (palette set 49, pal 1)
            snowNesoPaletteIndexes[getColorInt(Color.FromArgb(26, 187, 109))] = 0; // background
            snowNesoPaletteIndexes[getColorInt(Color.FromArgb(248, 248, 248))] = 1; // snow lightest
            snowNesoPaletteIndexes[getColorInt(Color.FromArgb(232, 240, 248))] = 2;
            snowNesoPaletteIndexes[getColorInt(Color.FromArgb(216, 216, 248))] = 3;
            snowNesoPaletteIndexes[getColorInt(Color.FromArgb(192, 184, 232))] = 4;
            snowNesoPaletteIndexes[getColorInt(Color.FromArgb(184, 168, 208))] = 5;
            snowNesoPaletteIndexes[getColorInt(Color.FromArgb(152, 136, 192))] = 6; // snow darkest

            byte[] snowNesoPngData = DataUtil.readResource("SoMRandomizer.Resources.nesoberi.snow_neso.png");

            Bitmap snowNesoBitmap = new Bitmap(new MemoryStream(snowNesoPngData));
            // 8-bit image by using the mapped colors above
            List<byte> snowNesoEightBitImage = new List<byte>();
            for (int y = 0; y < 24; y++)
            {
                for (int x = 0; x < 16; x++)
                {
                    Color c = snowNesoBitmap.GetPixel(x, y);
                    snowNesoEightBitImage.Add(snowNesoPaletteIndexes[getColorInt(c)]);
                }
            }

            int[] snowNesoTileOffsets = new int[] { 0x1f42a0, 0x1f42c0, 0x1f42e0, 0x1f5100, 0x1f5120, 0x1f5140, };
            for (int i = 0; i < 6; i++)
            {
                SnesTile tileData = new SnesTile();
                int _xpos = (i % 2) * 8;
                int _ypos = (i / 2) * 8;
                for (int y = 0; y < 8; y++)
                {
                    for (int x = 0; x < 8; x++)
                    {
                        tileData.data[y * 8 + x] = snowNesoEightBitImage[(_ypos + y) * 16 + _xpos + x];
                    }
                }
                SnesTile.WriteTile4bpp(tileData, outRom, snowNesoTileOffsets[i]);
            }

            return true;
        }

        private static int getColorInt(Color c)
        {
            return c.R + (c.G << 8) + (c.B << 16);
        }
    }
}
