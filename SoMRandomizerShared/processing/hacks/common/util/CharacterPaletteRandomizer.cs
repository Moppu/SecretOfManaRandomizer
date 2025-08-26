using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;

namespace SoMRandomizer.processing.hacks.common.util
{
    /// <summary>
    /// Optional hack to randomize some of the colors of the main characters.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CharacterPaletteRandomizer : RandoProcessor
    {
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(settings.get(CommonSettings.PROPERTYNAME_RANDOMIZE_CHAR_COLORS) == "rando")
            {
                randomizePalettes(origRom, outRom, context.randomCosmetic);
                return true;
            }
            else if(settings.get(CommonSettings.PROPERTYNAME_RANDOMIZE_CHAR_COLORS) == "custom")
            {
                setCustomPalettes(outRom, settings);
                return true;
            }
            return false;
        }

        protected override string getName()
        {
            return "Character palette randomizer";
        }

        public void setCustomPalettes(byte[] rom, RandoSettings settings)
        {
            List<SnesColor> boyColors = new List<SnesColor>();
            List<SnesColor> girlColors = new List<SnesColor>();
            List<SnesColor> spriteColors = new List<SnesColor>();
            for (int i=1; i < 16; i++)
            {
                int boyR = settings.getInt(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "BoyRed" + i);
                int boyG = settings.getInt(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "BoyGreen" + i);
                int boyB = settings.getInt(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "BoyBlue" + i);
                SnesColor boyColor = new SnesColor((byte)boyR, (byte)boyG, (byte)boyB);
                boyColors.Add(boyColor);
                int girlR = settings.getInt(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "GirlRed" + i);
                int girlG = settings.getInt(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "GirlGreen" + i);
                int girlB = settings.getInt(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "GirlBlue" + i);
                SnesColor girlColor = new SnesColor((byte)girlR, (byte)girlG, (byte)girlB);
                girlColors.Add(girlColor);
                int spriteR = settings.getInt(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "SpriteRed" + i);
                int spriteG = settings.getInt(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "SpriteGreen" + i);
                int spriteB = settings.getInt(CommonSettings.PROPERTYNAME_PREFIX_CUSTOM_CHARACTER_COLORS + "SpriteBlue" + i);
                SnesColor spriteColor = new SnesColor((byte)spriteR, (byte)spriteG, (byte)spriteB);
                spriteColors.Add(spriteColor);
            }

            setBoyColors(null, rom, boyColors, 1);
            setGirlColors(null, rom, girlColors, 1);
            setSpriteColors(null, rom, spriteColors, 1);
        }

        public void randomizePalettes(byte[] origRom, byte[] rom, Random r)
        {
            randomizeBoy(rom, r, 0);
            randomizeGirl(rom, r, 1);
            randomizeSprite(rom, r, 2);
        }

        private void randomizeBoy(byte[] rom, Random r, int destinationSlot)
        {
            // -------- boy palettes
            // --- shoes & bracelet thing index 13, 14
            List<List<SnesColor>> shoesColors = new List<List<SnesColor>>();
            // original shoes
            // 216, 64, 72
            // 248, 168, 40
            List<SnesColor> originalShoes = new List<SnesColor>();
            originalShoes.Add(new SnesColor(216, 64, 72));
            originalShoes.Add(new SnesColor(248, 168, 40));
            shoesColors.Add(originalShoes);

            // gray shoes
            // 64, 64, 64
            // 128, 128, 128
            List<SnesColor> grayShoes = new List<SnesColor>();
            grayShoes.Add(new SnesColor(64, 64, 64));
            grayShoes.Add(new SnesColor(128, 128, 128));
            shoesColors.Add(grayShoes);

            // blue shoes
            // 32, 32, 96
            // 64, 64, 160
            List<SnesColor> blueShoes = new List<SnesColor>();
            blueShoes.Add(new SnesColor(32, 32, 96));
            blueShoes.Add(new SnesColor(64, 64, 160));
            shoesColors.Add(blueShoes);

            List<SnesColor> redWhiteShoes = new List<SnesColor>();
            redWhiteShoes.Add(new SnesColor(216, 64, 72));
            redWhiteShoes.Add(new SnesColor(232, 200, 200));
            shoesColors.Add(redWhiteShoes);

            List<SnesColor> blueWhiteShoes = new List<SnesColor>();
            blueWhiteShoes.Add(new SnesColor(72, 64, 216));
            blueWhiteShoes.Add(new SnesColor(200, 200, 232));
            shoesColors.Add(blueWhiteShoes);

            List<SnesColor> greenWhiteShoes = new List<SnesColor>();
            greenWhiteShoes.Add(new SnesColor(32, 120, 72));
            greenWhiteShoes.Add(new SnesColor(200, 232, 200));
            shoesColors.Add(greenWhiteShoes);


            // --- pants index 10, 11, 12
            List<List<SnesColor>> pantsColors = new List<List<SnesColor>>();
            // original pants
            // 32, 72, 112
            // 56, 144, 192
            // 136, 224, 240
            List<SnesColor> originalPants = new List<SnesColor>();
            originalPants.Add(new SnesColor(32, 72, 112));
            originalPants.Add(new SnesColor(56, 144, 192));
            originalPants.Add(new SnesColor(136, 224, 240));
            pantsColors.Add(originalPants);

            // gray pants
            // 80, 80, 80  maybe all - 40?
            // 144, 144, 144
            // 200, 200, 200
            List<SnesColor> grayPants = new List<SnesColor>();
            grayPants.Add(new SnesColor(80, 80, 80));
            grayPants.Add(new SnesColor(144, 144, 144));
            grayPants.Add(new SnesColor(200, 200, 200));
            pantsColors.Add(grayPants);

            List<SnesColor> greenPants = new List<SnesColor>();
            greenPants.Add(new SnesColor(0, 80, 24));
            greenPants.Add(new SnesColor(0, 128, 48));
            greenPants.Add(new SnesColor(72, 176, 96));
            pantsColors.Add(greenPants);

            // for the scariest adventures
            List<SnesColor> brownPants = new List<SnesColor>();
            brownPants.Add(new SnesColor(64, 32, 32));
            brownPants.Add(new SnesColor(96, 64, 64));
            brownPants.Add(new SnesColor(160, 104, 80));
            pantsColors.Add(brownPants);

            List<SnesColor> pinkPants = new List<SnesColor>();
            pinkPants.Add(new SnesColor(128, 32, 64));
            pinkPants.Add(new SnesColor(224, 56, 96));
            pinkPants.Add(new SnesColor(232, 112, 144));
            pantsColors.Add(pinkPants);



            // --- bandana index 8, 9 - also scarf
            List<List<SnesColor>> bandanaColors = new List<List<SnesColor>>();
            // original bandana
            // 232, 40, 168
            // 248, 176, 160
            List<SnesColor> originalBandana = new List<SnesColor>();
            originalBandana.Add(new SnesColor(232, 40, 168));
            originalBandana.Add(new SnesColor(248, 176, 160));
            bandanaColors.Add(originalBandana);

            // green bandana
            // 0, 168, 104
            // 112, 184, 96
            List<SnesColor> greenBandana = new List<SnesColor>();
            greenBandana.Add(new SnesColor(0, 168, 104));
            greenBandana.Add(new SnesColor(112, 184, 96));
            bandanaColors.Add(greenBandana);

            // blue bandana
            // 56, 96, 160
            // 120, 136, 208
            List<SnesColor> blueBandana = new List<SnesColor>();
            blueBandana.Add(new SnesColor(56, 96, 160));
            blueBandana.Add(new SnesColor(120, 136, 208));
            bandanaColors.Add(blueBandana);

            // gray bandana
            // 80, 80, 80
            // 160, 160, 160
            List<SnesColor> grayBandana = new List<SnesColor>();
            grayBandana.Add(new SnesColor(80, 80, 80));
            grayBandana.Add(new SnesColor(160, 160, 160));
            bandanaColors.Add(grayBandana);

            List<SnesColor> yellowBandana = new List<SnesColor>();
            yellowBandana.Add(new SnesColor(176, 176, 40));
            yellowBandana.Add(new SnesColor(248, 248, 160));
            bandanaColors.Add(yellowBandana);

            List<SnesColor> purpleBandana = new List<SnesColor>();
            purpleBandana.Add(new SnesColor(64, 0, 160));
            purpleBandana.Add(new SnesColor(192, 72, 248));
            bandanaColors.Add(purpleBandana);


            // --- skin index 6, 7
            List<List<SnesColor>> skinColors = new List<List<SnesColor>>();
            // 15
            List<List<SnesColor>> eyeWhiteColors = new List<List<SnesColor>>();
            // original skin
            // 208, 120, 80
            // 240, 184, 120
            List<SnesColor> originalSkin = new List<SnesColor>();
            originalSkin.Add(new SnesColor(208, 120, 80));
            originalSkin.Add(new SnesColor(240, 184, 120));
            skinColors.Add(originalSkin);
            List<SnesColor> originalSkinEye = new List<SnesColor>();
            originalSkin.Add(new SnesColor(248, 248, 248));
            eyeWhiteColors.Add(originalSkinEye);

            // pale
            // 224, 152, 112
            // 248, 216, 152
            List<SnesColor> paleSkin = new List<SnesColor>();
            paleSkin.Add(new SnesColor(224, 152, 112));
            paleSkin.Add(new SnesColor(248, 216, 152));
            skinColors.Add(paleSkin);
            eyeWhiteColors.Add(originalSkinEye);

            // dark
            // 184, 96, 96
            // 232, 160, 96
            List<SnesColor> darkSkin = new List<SnesColor>();
            darkSkin.Add(new SnesColor(184, 96, 96));
            darkSkin.Add(new SnesColor(232, 160, 96));
            skinColors.Add(darkSkin);
            eyeWhiteColors.Add(originalSkinEye);

            List<SnesColor> blueSkin = new List<SnesColor>();
            blueSkin.Add(new SnesColor(88, 128, 208));
            blueSkin.Add(new SnesColor(128, 192, 232));
            skinColors.Add(blueSkin);
            eyeWhiteColors.Add(originalSkinEye);



            // --- hair index 1, 2, 3, 4, 5 (1 is also the eye and outline color) - should stay dark
            List<List<SnesColor>> hairColors = new List<List<SnesColor>>();
            // original hair
            // 48, 40, 32
            // 128, 64, 24
            // 200, 88, 32
            // 248, 152, 32
            // 136, 80, 56
            List<SnesColor> originalHair = new List<SnesColor>();
            originalHair.Add(new SnesColor(128, 64, 64));
            originalHair.Add(new SnesColor(200, 88, 32));
            originalHair.Add(new SnesColor(248, 152, 32));
            originalHair.Add(new SnesColor(136, 80, 56));
            hairColors.Add(originalHair);

            // darker hair
            // 16, 8, 0
            // 96, 32, 0
            // 168, 56, 0
            // 200, 104, 0
            // 104, 48, 24
            List<SnesColor> darkHair = new List<SnesColor>();
            darkHair.Add(new SnesColor(96, 32, 0));
            darkHair.Add(new SnesColor(168, 56, 0));
            darkHair.Add(new SnesColor(200, 104, 0));
            darkHair.Add(new SnesColor(104, 48, 24));
            hairColors.Add(darkHair);

            List<SnesColor> blueHair = new List<SnesColor>();
            blueHair.Add(new SnesColor(16, 72, 104));
            blueHair.Add(new SnesColor(24, 96, 176));
            blueHair.Add(new SnesColor(16, 128, 216));
            blueHair.Add(new SnesColor(48, 88, 112));
            hairColors.Add(blueHair);

            List<SnesColor> greenHair = new List<SnesColor>();
            greenHair.Add(new SnesColor(0, 88, 56));
            greenHair.Add(new SnesColor(8, 160, 80));
            greenHair.Add(new SnesColor(0, 200, 112));
            greenHair.Add(new SnesColor(32, 96, 72));
            hairColors.Add(greenHair);

            // swap g/b in original
            List<SnesColor> pinkHair = new List<SnesColor>();
            pinkHair.Add(new SnesColor(128, 24, 64));
            pinkHair.Add(new SnesColor(200, 32, 88));
            pinkHair.Add(new SnesColor(248, 32, 152));
            pinkHair.Add(new SnesColor(136, 56, 80));
            hairColors.Add(pinkHair);

            // swap r/b in pink
            List<SnesColor> purpleHair = new List<SnesColor>();
            purpleHair.Add(new SnesColor(64, 24, 128));
            purpleHair.Add(new SnesColor(88, 32, 200));
            purpleHair.Add(new SnesColor(152, 32, 248));
            purpleHair.Add(new SnesColor(80, 56, 136));
            hairColors.Add(purpleHair);

            int skinIndex = r.Next() % skinColors.Count;
            List<SnesColor> hair = hairColors[r.Next() % hairColors.Count]; // 1, 2, 3, 4, 5
            List<SnesColor> skin = skinColors[skinIndex]; // 6, 7
            List<SnesColor> bandana = bandanaColors[r.Next() % bandanaColors.Count]; // 8, 9
            List<SnesColor> pants = pantsColors[r.Next() % pantsColors.Count]; // 10, 11, 12
            List<SnesColor> shoes = shoesColors[r.Next() % shoesColors.Count]; // 13, 14
            List<SnesColor> eyeWhite = eyeWhiteColors[skinIndex]; // 15

            switch (destinationSlot)
            {
                case 0:
                    setBoyColors(r, rom, hair, 2);
                    setBoyColors(r, rom, skin, 6);
                    setBoyColors(r, rom, bandana, 8);
                    setBoyColors(r, rom, pants, 10);
                    setBoyColors(r, rom, shoes, 13);
                    setBoyColors(r, rom, eyeWhite, 15);
                    break;
                case 1:
                    setGirlColors(r, rom, hair, 2);
                    setGirlColors(r, rom, skin, 6);
                    setGirlColors(r, rom, bandana, 8);
                    setGirlColors(r, rom, pants, 10);
                    setGirlColors(r, rom, shoes, 13);
                    setGirlColors(r, rom, eyeWhite, 15);
                    break;
                case 2:
                    setSpriteColors(r, rom, hair, 2);
                    setSpriteColors(r, rom, skin, 6);
                    setSpriteColors(r, rom, bandana, 8);
                    setSpriteColors(r, rom, pants, 10);
                    setSpriteColors(r, rom, shoes, 13);
                    setSpriteColors(r, rom, eyeWhite, 15);
                    break;
            }
            // 8D is the charge max colors for all 3 characters
            List<SnesColor> testColors = new List<SnesColor>();
            for (int i = 0; i < 15; i++)
            {
                testColors.Add(new SnesColor(0, 0, 0));
            }
        }
        private void setBoyColors(Random r, byte[] rom, List<SnesColor> colors, int paletteOffset)
        {
            randomHueShift(r, colors);
            // normal sprite palette
            setColors(rom, colors, paletteOffset, 0x80);
            // shrunken
            setColors(rom, colors, paletteOffset, 0xF6);
            // dead
            setColors(rom, copyAndAddBrightness(colors, 64), paletteOffset, 0xE4);
            // poison
            if (paletteOffset != 6) // leave purple skin, change everything else
            {
                setColors(rom, colors, paletteOffset, 0xE7);
            }

            // these ones are in the enemy ID range, but it's the boss section, which has its own palettes,
            // so the characters/NPCs use some of these too.

            // flash 1
            setColors(rom, copyAndAddBrightness(colors, 16), paletteOffset, 0x59);
            // flash 2
            setColors(rom, copyAndAddBrightness(colors, 32), paletteOffset, 0x58);
            // flash 3
            setColors(rom, copyAndAddBrightness(colors, 48), paletteOffset, 0x57);
        }


        private void randomizeGirl(byte[] rom, Random r, int destinationSlot)
        {
            // -------- girl palettes
            // --- eye index 14
            List<List<SnesColor>> eyeColors = new List<List<SnesColor>>();
            List<SnesColor> originalEyes = new List<SnesColor>();
            originalEyes.Add(new SnesColor(0, 88, 112));
            eyeColors.Add(originalEyes);

            List<SnesColor> greenEyes = new List<SnesColor>();
            greenEyes.Add(new SnesColor(0, 160, 88));
            eyeColors.Add(greenEyes);

            List<SnesColor> brownEyes = new List<SnesColor>();
            brownEyes.Add(new SnesColor(144, 40, 0));
            eyeColors.Add(brownEyes);

            List<SnesColor> redEyes = new List<SnesColor>();
            redEyes.Add(new SnesColor(192, 40, 40));
            eyeColors.Add(redEyes);

            List<SnesColor> brightBlueEyes = new List<SnesColor>();
            brightBlueEyes.Add(new SnesColor(56, 160, 232));
            eyeColors.Add(brightBlueEyes);

            // leave 13 alone? it's the shiniest part of both the hair and pants
            // actually make it grayer
            List<List<SnesColor>> specularColors = new List<List<SnesColor>>();
            List<SnesColor> graySpecular = new List<SnesColor>();
            graySpecular.Add(new SnesColor(232, 232, 232));
            specularColors.Add(graySpecular);


            // --- pants index 10, 11, 12
            List<List<SnesColor>> pantsColors = new List<List<SnesColor>>();

            List<SnesColor> originalPants = new List<SnesColor>();
            originalPants.Add(new SnesColor(160, 40, 112));
            originalPants.Add(new SnesColor(232, 80, 144));
            originalPants.Add(new SnesColor(248, 144, 232));
            pantsColors.Add(originalPants);

            List<SnesColor> boyPants = new List<SnesColor>();
            boyPants.Add(new SnesColor(32, 72, 112));
            boyPants.Add(new SnesColor(56, 144, 192));
            boyPants.Add(new SnesColor(136, 224, 240));
            pantsColors.Add(boyPants);

            List<SnesColor> grayPants = new List<SnesColor>();
            grayPants.Add(new SnesColor(80, 80, 80));
            grayPants.Add(new SnesColor(144, 144, 144));
            grayPants.Add(new SnesColor(200, 200, 200));
            pantsColors.Add(grayPants);

            List<SnesColor> darkGrayPants = new List<SnesColor>();
            darkGrayPants.Add(new SnesColor(48, 48, 48));
            darkGrayPants.Add(new SnesColor(112, 112, 112));
            darkGrayPants.Add(new SnesColor(168, 168, 168));
            pantsColors.Add(darkGrayPants);

            List<SnesColor> greenPants = new List<SnesColor>();
            greenPants.Add(new SnesColor(32, 104, 0));
            greenPants.Add(new SnesColor(8, 160, 32));
            greenPants.Add(new SnesColor(104, 208, 144));
            pantsColors.Add(greenPants);

            List<SnesColor> yellowPants = new List<SnesColor>();
            yellowPants.Add(new SnesColor(144, 96, 8));
            yellowPants.Add(new SnesColor(216, 144, 48));
            yellowPants.Add(new SnesColor(232, 216, 96));
            pantsColors.Add(yellowPants);

            // r/b swap orig
            List<SnesColor> purplePants = new List<SnesColor>();
            purplePants.Add(new SnesColor(112, 40, 160));
            purplePants.Add(new SnesColor(144, 80, 232));
            purplePants.Add(new SnesColor(232, 144, 248));
            pantsColors.Add(purplePants);



            // --- shoes, bracelet, hair tie thing 8, 9
            List<List<SnesColor>> shoesColors = new List<List<SnesColor>>();

            List<SnesColor> originalShoes = new List<SnesColor>();
            originalShoes.Add(new SnesColor(80, 168, 104));
            originalShoes.Add(new SnesColor(152, 232, 168));
            shoesColors.Add(originalShoes);

            // colors from boy palette bandana
            List<SnesColor> boyShoes = new List<SnesColor>();
            boyShoes.Add(new SnesColor(232, 40, 168));
            boyShoes.Add(new SnesColor(248, 176, 160));
            shoesColors.Add(boyShoes);

            List<SnesColor> greenShoes = new List<SnesColor>();
            greenShoes.Add(new SnesColor(0, 168, 104));
            greenShoes.Add(new SnesColor(112, 184, 96));
            shoesColors.Add(greenShoes);

            List<SnesColor> blueShoes = new List<SnesColor>();
            blueShoes.Add(new SnesColor(56, 96, 160));
            blueShoes.Add(new SnesColor(120, 136, 208));
            shoesColors.Add(blueShoes);

            List<SnesColor> grayShoes = new List<SnesColor>();
            grayShoes.Add(new SnesColor(80, 80, 80));
            grayShoes.Add(new SnesColor(160, 160, 160));
            shoesColors.Add(grayShoes);


            // --- skin index 6, 7
            List<List<SnesColor>> skinColors = new List<List<SnesColor>>();
            // 15
            List<List<SnesColor>> eyeWhiteColors = new List<List<SnesColor>>();
            List<SnesColor> originalSkin = new List<SnesColor>();
            originalSkin.Add(new SnesColor(216, 128, 88));
            originalSkin.Add(new SnesColor(248, 216, 168));
            skinColors.Add(originalSkin);
            List<SnesColor> originalSkinEye = new List<SnesColor>();
            originalSkin.Add(new SnesColor(248, 248, 248));
            eyeWhiteColors.Add(originalSkinEye);

            List<SnesColor> boySkin = new List<SnesColor>();
            boySkin.Add(new SnesColor(208, 120, 80));
            boySkin.Add(new SnesColor(240, 184, 120));
            skinColors.Add(boySkin);
            eyeWhiteColors.Add(originalSkinEye);

            List<SnesColor> paleSkin = new List<SnesColor>();
            paleSkin.Add(new SnesColor(224, 152, 112));
            paleSkin.Add(new SnesColor(248, 216, 152));
            skinColors.Add(paleSkin);
            eyeWhiteColors.Add(originalSkinEye);

            List<SnesColor> darkSkin = new List<SnesColor>();
            darkSkin.Add(new SnesColor(184, 96, 96));
            darkSkin.Add(new SnesColor(232, 160, 96));
            skinColors.Add(darkSkin);
            eyeWhiteColors.Add(originalSkinEye);

            List<SnesColor> blueSkin = new List<SnesColor>();
            blueSkin.Add(new SnesColor(112, 152, 232));
            blueSkin.Add(new SnesColor(152, 216, 248));
            skinColors.Add(blueSkin);
            eyeWhiteColors.Add(originalSkinEye);


            // --- hair index 1, 2, 3, 4 (1 is also the eye and outline color) - should stay light
            // index 5 not used?
            List<List<SnesColor>> hairColors = new List<List<SnesColor>>();
            List<SnesColor> originalHair = new List<SnesColor>();
            originalHair.Add(new SnesColor(160, 88, 56));
            originalHair.Add(new SnesColor(216, 120, 0));
            originalHair.Add(new SnesColor(240, 168, 32));
            hairColors.Add(originalHair);

            List<SnesColor> blueHair = new List<SnesColor>();
            blueHair.Add(new SnesColor(64, 96, 200));
            blueHair.Add(new SnesColor(32, 152, 216));
            blueHair.Add(new SnesColor(80, 216, 248));
            hairColors.Add(blueHair);

            List<SnesColor> greenHair = new List<SnesColor>();
            greenHair.Add(new SnesColor(72, 128, 56));
            greenHair.Add(new SnesColor(40, 176, 112));
            greenHair.Add(new SnesColor(72, 224, 80));
            hairColors.Add(greenHair);

            List<SnesColor> platinumHair = new List<SnesColor>();
            platinumHair.Add(new SnesColor(168, 152, 96));
            platinumHair.Add(new SnesColor(232, 192, 80));
            platinumHair.Add(new SnesColor(248, 248, 104));
            hairColors.Add(platinumHair);

            List<SnesColor> strawberryHair = new List<SnesColor>();
            strawberryHair.Add(new SnesColor(192, 88, 88));
            strawberryHair.Add(new SnesColor(248, 120, 96));
            strawberryHair.Add(new SnesColor(248, 168, 96));
            hairColors.Add(strawberryHair);

            List<SnesColor> purpleHair = new List<SnesColor>();
            purpleHair.Add(new SnesColor(152, 56, 96));
            purpleHair.Add(new SnesColor(208, 32, 168));
            purpleHair.Add(new SnesColor(232, 96, 248));
            hairColors.Add(purpleHair);

            List<SnesColor> orangeHair = new List<SnesColor>();
            orangeHair.Add(new SnesColor(192, 40, 0));
            orangeHair.Add(new SnesColor(248, 72, 0));
            orangeHair.Add(new SnesColor(248, 128, 0));
            hairColors.Add(orangeHair);

            int skinIndex = r.Next() % skinColors.Count;
            List<SnesColor> hair = hairColors[r.Next() % hairColors.Count]; // 1, 2, 3, 4, 5
            List<SnesColor> skin = skinColors[skinIndex]; // 6, 7
            List<SnesColor> shoes = shoesColors[r.Next() % shoesColors.Count]; // 8, 9
            List<SnesColor> pants = pantsColors[r.Next() % pantsColors.Count]; // 10, 11, 12
            List<SnesColor> eye = eyeColors[r.Next() % eyeColors.Count]; // 13, 14
            List<SnesColor> eyeWhite = eyeWhiteColors[skinIndex]; // 15

            switch (destinationSlot)
            {
                case 0:
                    setBoyColors(r, rom, hair, 2);
                    setBoyColors(r, rom, skin, 6);
                    setBoyColors(r, rom, shoes, 8);
                    setBoyColors(r, rom, pants, 10);
                    setBoyColors(r, rom, graySpecular, 13);
                    setBoyColors(r, rom, eye, 14);
                    setBoyColors(r, rom, eyeWhite, 15);
                    break;
                case 1:
                    setGirlColors(r, rom, hair, 2);
                    setGirlColors(r, rom, skin, 6);
                    setGirlColors(r, rom, shoes, 8);
                    setGirlColors(r, rom, pants, 10);
                    setGirlColors(r, rom, graySpecular, 13);
                    setGirlColors(r, rom, eye, 14);
                    setGirlColors(r, rom, eyeWhite, 15);
                    break;
                case 2:
                    setSpriteColors(r, rom, hair, 2);
                    setSpriteColors(r, rom, skin, 6);
                    setSpriteColors(r, rom, shoes, 8);
                    setSpriteColors(r, rom, pants, 10);
                    setSpriteColors(r, rom, graySpecular, 13);
                    setSpriteColors(r, rom, eye, 14);
                    setSpriteColors(r, rom, eyeWhite, 15);
                    break;
            }

            // 8D is the charge max colors for all 3 characters
            List<SnesColor> testColors = new List<SnesColor>();
            for (int i = 0; i < 15; i++)
            {
                testColors.Add(new SnesColor(0, 0, 0));
            }
        }

        private void setGirlColors(Random r, byte[] rom, List<SnesColor> colors, int paletteOffset)
        {
            randomHueShift(r, colors);
            // normal sprite palette
            setColors(rom, colors, paletteOffset, 0x81);
            // shrunken
            setColors(rom, colors, paletteOffset, 0xF7);
            setColors(rom, colors, paletteOffset, 0x8B);
            // dead
            setColors(rom, copyAndAddBrightness(colors, 64), paletteOffset, 0xE5);
            // poison
            if (paletteOffset != 6) // leave purple skin, change everything else
            {
                setColors(rom, colors, paletteOffset, 0xE8);
            }

            // these ones are in the enemy ID range, but it's the boss section, which has its own palettes,
            // so the characters/NPCs use some of these too.

            // flash 1
            setColors(rom, copyAndAddBrightness(colors, 16), paletteOffset, 0x5C);
            // flash 2
            setColors(rom, copyAndAddBrightness(colors, 32), paletteOffset, 0x5B);
            // flash 3
            setColors(rom, copyAndAddBrightness(colors, 48), paletteOffset, 0x5A);
        }


        private void randomizeSprite(byte[] rom, Random r, int destinationSlot)
        {
            // -------- sprite palettes
            
            // --- skin index 5
            List<List<SnesColor>> skinColors = new List<List<SnesColor>>();
            List<SnesColor> originalSkin = new List<SnesColor>();
            originalSkin.Add(new SnesColor(136, 96, 40));
            originalSkin.Add(new SnesColor(208, 136, 112));
            originalSkin.Add(new SnesColor(248, 192, 168));
            skinColors.Add(originalSkin);

            List<SnesColor> paleSkin = new List<SnesColor>();
            paleSkin.Add(new SnesColor(160, 120, 64));
            paleSkin.Add(new SnesColor(224, 152, 128));
            paleSkin.Add(new SnesColor(248, 208, 184));
            skinColors.Add(paleSkin);

            List<SnesColor> darkSkin = new List<SnesColor>();
            darkSkin.Add(new SnesColor(120, 64, 8));
            darkSkin.Add(new SnesColor(192, 96, 72));
            darkSkin.Add(new SnesColor(224, 152, 112));
            skinColors.Add(darkSkin);

            // --- hair index 2, 3, 4 - continued at 8, 9 in part2
            List<List<SnesColor>> hairColors1 = new List<List<SnesColor>>();
            List<SnesColor> originalHair1 = new List<SnesColor>();
            originalHair1.Add(new SnesColor(168, 48, 64));
            originalHair1.Add(new SnesColor(208, 88, 104));
            originalHair1.Add(new SnesColor(232, 144, 120));
            hairColors1.Add(originalHair1);

            List<SnesColor> blueHair1 = new List<SnesColor>();
            blueHair1.Add(new SnesColor(64, 48, 168));
            blueHair1.Add(new SnesColor(104, 88, 208));
            blueHair1.Add(new SnesColor(120, 144, 232));
            hairColors1.Add(blueHair1);

            List<SnesColor> blondeHair1 = new List<SnesColor>();
            blondeHair1.Add(new SnesColor(184, 112, 80));
            blondeHair1.Add(new SnesColor(224, 152, 120));
            blondeHair1.Add(new SnesColor(248, 208, 136));
            hairColors1.Add(blondeHair1);

            List<SnesColor> purpleHair1 = new List<SnesColor>();
            purpleHair1.Add(new SnesColor(128, 8, 72));
            purpleHair1.Add(new SnesColor(168, 48, 112));
            purpleHair1.Add(new SnesColor(200, 112, 128));
            hairColors1.Add(purpleHair1);

            List<SnesColor> brownHair1 = new List<SnesColor>();
            brownHair1.Add(new SnesColor(96, 0, 0));
            brownHair1.Add(new SnesColor(136, 32, 0));
            brownHair1.Add(new SnesColor(160, 80, 8));
            hairColors1.Add(brownHair1);


            // --- hair index 8, 9
            List<List<SnesColor>> hairColors2 = new List<List<SnesColor>>();
            List<SnesColor> originalHair2 = new List<SnesColor>();
            originalHair2.Add(new SnesColor(248, 192, 128));
            originalHair2.Add(new SnesColor(144, 24, 24));
            hairColors2.Add(originalHair2);

            List<SnesColor> blueHair2 = new List<SnesColor>();
            blueHair2.Add(new SnesColor(128, 192, 248));
            blueHair2.Add(new SnesColor(24, 24, 144));
            hairColors2.Add(blueHair2);

            List<SnesColor> blondeHair2 = new List<SnesColor>();
            blondeHair2.Add(new SnesColor(248, 248, 144));
            blondeHair2.Add(new SnesColor(160, 88, 40));
            hairColors2.Add(blondeHair2);

            List<SnesColor> purpleHair2 = new List<SnesColor>();
            purpleHair2.Add(new SnesColor(224, 168, 136));
            purpleHair2.Add(new SnesColor(104, 0, 32));
            hairColors2.Add(purpleHair2);

            List<SnesColor> brownHair2 = new List<SnesColor>();
            brownHair2.Add(new SnesColor(184, 104, 8));
            brownHair2.Add(new SnesColor(72, 0, 0));
            hairColors2.Add(brownHair2);

            // --- shirt index 10, 11, 12
            List<List<SnesColor>> shirtColors = new List<List<SnesColor>>();
            List<SnesColor> originalShirt = new List<SnesColor>();
            originalShirt.Add(new SnesColor(32, 120, 88));
            originalShirt.Add(new SnesColor(80, 176, 104));
            originalShirt.Add(new SnesColor(112, 248, 176));
            shirtColors.Add(originalShirt);

            List<SnesColor> blueShirt = new List<SnesColor>();
            blueShirt.Add(new SnesColor(32, 88, 120));
            blueShirt.Add(new SnesColor(80, 104, 176));
            blueShirt.Add(new SnesColor(112, 176, 248));
            shirtColors.Add(blueShirt);

            List<SnesColor> pinkShirt = new List<SnesColor>();
            pinkShirt.Add(new SnesColor(120, 32, 88));
            pinkShirt.Add(new SnesColor(176, 80, 104));
            pinkShirt.Add(new SnesColor(248, 112, 176));
            shirtColors.Add(pinkShirt);

            List<SnesColor> yellowShirt = new List<SnesColor>();
            yellowShirt.Add(new SnesColor(120, 120, 32));
            yellowShirt.Add(new SnesColor(176, 176, 64));
            yellowShirt.Add(new SnesColor(248, 248, 120));
            shirtColors.Add(yellowShirt);

            int hairIndex = (r.Next() % hairColors1.Count);
            List<SnesColor> hair1 = hairColors1[hairIndex]; // 2, 3, 4
            List<SnesColor> skin = skinColors[r.Next() % skinColors.Count]; // 5, 6, 7
            List<SnesColor> hair2 = hairColors2[hairIndex]; // 8, 9
            List<SnesColor> shirt = shirtColors[r.Next() % shirtColors.Count]; // 8, 9

            switch (destinationSlot)
            {
                case 0:
                    setBoyColors(r, rom, hair1, 2);
                    setBoyColors(r, rom, skin, 5);
                    setBoyColors(r, rom, hair2, 8);
                    setBoyColors(r, rom, shirt, 10);
                    break;
                case 1:
                    setGirlColors(r, rom, hair1, 2);
                    setGirlColors(r, rom, skin, 5);
                    setGirlColors(r, rom, hair2, 8);
                    setGirlColors(r, rom, shirt, 10);
                    break;
                case 2:
                    setSpriteColors(r, rom, hair1, 2);
                    setSpriteColors(r, rom, skin, 5);
                    setSpriteColors(r, rom, hair2, 8);
                    setSpriteColors(r, rom, shirt, 10);
                    break;
            }

            // 8D is the charge max colors for all 3 characters
            List<SnesColor> testColors = new List<SnesColor>();
            for (int i = 0; i < 15; i++)
            {
                testColors.Add(new SnesColor(0, 0, 0));
            }
        }

        private void setSpriteColors(Random r, byte[] rom, List<SnesColor> colors, int paletteOffset)
        {
            randomHueShift(r, colors);

            // normal sprite palette
            setColors(rom, colors, paletteOffset, 0x82);
            // shrunken
            setColors(rom, colors, paletteOffset, 0xF8);
            setColors(rom, colors, paletteOffset, 0x8C);
            // dead
            setColors(rom, copyAndAddBrightness(colors, 64), paletteOffset, 0xE6);
            // poison
            if (paletteOffset != 5) // leave purple skin, change everything else
            {
                setColors(rom, colors, paletteOffset, 0xE9);
            }

            // these ones are in the enemy ID range, but it's the boss section, which has its own palettes,
            // so the characters/NPCs use some of these too.

            // flash 1
            setColors(rom, copyAndAddBrightness(colors, 16), paletteOffset, 0x5F);
            // flash 2
            setColors(rom, copyAndAddBrightness(colors, 32), paletteOffset, 0x5E);
            // flash 3
            setColors(rom, copyAndAddBrightness(colors, 48), paletteOffset, 0x5D);
        }

        private static List<SnesColor> copyAndAddBrightness(List<SnesColor> original, int amount)
        {
            List<SnesColor> newList = new List<SnesColor>();
            foreach(SnesColor cc in original)
            {
                SnesColor c = new SnesColor(cc.getRed(), cc.getGreen(), cc.getBlue(), cc.getNonColorBit());
                c.add(amount, amount, amount);
                newList.Add(c);
            }
            return newList;
        }

        private static byte clamp(int value)
        {
            return (byte)DataUtil.clampToEndpoints(value, 0, 255);
        }

        private void randomHueShift(Random random, List<SnesColor> colors)
        {
            if (random != null)
            {
                double hueChange = ((random.Next() % 300) - 150) / 10.0;
                for (int i = 0; i < colors.Count; i++)
                {
                    SnesColor c = colors[i];
                    double h,s,v;
                    ColorUtil.rgbToHsv(c.getRed(), c.getGreen(), c.getBlue(), out h, out s, out v);
                    h += hueChange;
                    int r, g, b;
                    ColorUtil.hsvToRgb(h, s, v, out r, out g, out b);
                    c.setRed((byte)r);
                    c.setGreen((byte)g);
                    c.setBlue((byte)b);
                }
            }
        }

        private void setColors(byte[] rom, List<SnesColor> colors, int paletteOffset, int spriteIndex)
        {
            // character palette offsets start = 80FFE
            for (int i = 0; i < colors.Count; i++)
            {
                colors[i].put(rom, 0x80FFE + spriteIndex * 0x1E + (paletteOffset + i) * 2);
            }
        }
    }
}
