using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.hacks.common.util;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Linq;
using static SoMRandomizer.processing.openworld.PlandoProperties;
using static SoMRandomizer.processing.common.SomVanillaValues;
using SoMRandomizer.processing.common.structure;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Hack that randomizes the element of every boss.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class BossElementRando : RandoProcessor
    {
        protected override string getName()
        {
            return "Boss element randomizer";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_BOSS_ELEMENT_RANDO))
            {
                return false;
            }

            Random random = context.randomFunctional;
            // make some space in-bank for spell id tables and code
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 500);
            // everything with a key here we'll try to randomize
            // index into 10bb24 (16 bit) -> bank 2 (0c -> 2f1dc for mantis ant for example)
            Dictionary<int, List<int>> paletteOffsets = new Dictionary<int, List<int>>()
            {
                { BOSSID_MANTISANT,
                    new int[] { 0x0C }.ToList() },
                { BOSSID_WALLFACE,
                    new int[] { 0x70,  // center eye
                    0x6B // wall and side eyes
                    }.ToList() },
                { BOSSID_TROPICALLO,
                    new int[] { 0x20, 0x21 }.ToList() },
                { BOSSID_MINOTAUR,
                    new int[] { 0x1a }.ToList() },
                { BOSSID_SPIKEY,
                    new int[] { 0x2d }.ToList() },
                { BOSSID_JABBER,
                    new int[] { 0x10 }.ToList() },
                { BOSSID_SPRINGBEAK,
                    new int[] { 0x67 }.ToList() },
                { BOSSID_FROSTGIGAS,
                    new int[] { 0x35 }.ToList() },
                { BOSSID_SNAPDRAGON,
                    new int[] { 0x33 }.ToList() },
                { BOSSID_MECHRIDER1,
                    new int[] { 0x57 }.ToList() },
                { BOSSID_DOOMSWALL,
                    new int[] {
                        0x71, // center eye
                        0x73, // wall and side eyes
                        }.ToList() },
                { BOSSID_VAMPIRE,
                    new int[] { 0x39, 0x3a }.ToList() },
                { BOSSID_METALMANTIS,
                    new int[] { 0x0d }.ToList() },
                { BOSSID_MECHRIDER2,
                    new int[] { 0x59 }.ToList() },
                { BOSSID_KILROY,
                    new int[] { 0x12 }.ToList() },
                { BOSSID_GORGON,
                    new int[] { 0x1c }.ToList() },
                // 0x67 brambler
                { BOSSID_BOREAL,
                    new int[] { 0x23, 0x24 }.ToList() },
                { BOSSID_GREATVIPER,
                    new int[] { 0x69 }.ToList() },
                // 0x6a limeslime
                { BOSSID_BLUESPIKE, 
                    new int[] { 0x2f }.ToList() },
                // 0x6c chamber eye
                { BOSSID_HYDRA, 
                    new int[] { 0x11 }.ToList() },
                { BOSSID_WATERMELON,
                    new int[] { 0x2a }.ToList() },
                // 0x6f hexas
                { BOSSID_KETTLEKIN,
                    new int[] { 0x16, 0x17 }.ToList() },
                { BOSSID_TONPOLE, 
                    new int[] { 0x32 }.ToList() },
                { BOSSID_MECHRIDER3,
                    new int[] { 0x5a }.ToList() },
                { BOSSID_SNOWDRAGON,
                    new int[] { 0x74 }.ToList() },
                { BOSSID_FIREGIGAS,
                    new int[] { 0x36 }.ToList() },
                { BOSSID_REDDRAGON,
                    new int[] { 0x75 }.ToList() },
                { BOSSID_AXEBEAK,
                    new int[] { 0x68 }.ToList() },
                { BOSSID_BLUEDRAGON,
                    new int[] { 0x76 }.ToList() },
                { BOSSID_BUFFY,
                    new int[] { 0x3b, 0x3a }.ToList() },
                { BOSSID_DARKLICH,
                    new int[] { 0x3c, 0x3d, 0x3e, 0x3f, 0x40, 0x41, 0x42, 0x43, 0x44 }.ToList() },
                { BOSSID_BITINGLIZARD,
                    new int[] { 0x31 }.ToList() },
                { BOSSID_DRAGONWORM,
                    new int[] { 0x6a }.ToList() },
                // 0x7c dreadslime
                { BOSSID_THUNDERGIGAS,
                    new int[] { 0x7d }.ToList() },
                // 0x7e doom's eye
                // 0x7f mana beast
            };
            
            int bossReplacementTableOffset = context.workingOffset;
            for(int i=0x57; i <= 0x7f; i++)
            {
                if(paletteOffsets.ContainsKey(i))
                {
                    outRom[context.workingOffset++] = 1;
                }
                else
                {
                    outRom[context.workingOffset++] = 0;
                }
            }


            // spell + targetting combos that can be used by each boss
            // targetting 0x01 controls single vs multi, and 0x02 controls ally vs enemy targetting

            // //////////////////////////////////////////////////

            int gnomeSpellsListOffset = context.workingOffset;
            outRom[context.workingOffset++] = 0x01; // gem missile
            outRom[context.workingOffset++] = 0x00; // single target

            outRom[context.workingOffset++] = 0x02; // speed down
            outRom[context.workingOffset++] = 0x00; // single target

            outRom[context.workingOffset++] = 0x00; // earth slide
            outRom[context.workingOffset++] = 0x00; // single target

            outRom[context.workingOffset++] = 0x01; // gem missile
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x02; // speed down
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x00; // earth slide
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x03; // stone saber
            outRom[context.workingOffset++] = 0x02; // self

            outRom[context.workingOffset++] = 0x04; // speedup
            outRom[context.workingOffset++] = 0x02; // self

            outRom[context.workingOffset++] = 0x05; // defender
            outRom[context.workingOffset++] = 0x02; // self

            outRom[context.workingOffset++] = 0x03; // stone saber
            outRom[context.workingOffset++] = 0x01; // all

            int numGnomeSpells = (context.workingOffset - gnomeSpellsListOffset) / 2;

            // //////////////////////////////////////////////////

            int sylphidSpellsListOffset = context.workingOffset;
            outRom[context.workingOffset++] = 0x12; // airblast
            outRom[context.workingOffset++] = 0x00; // single target

            outRom[context.workingOffset++] = 0x13; // lightning
            outRom[context.workingOffset++] = 0x00; // single target

            outRom[context.workingOffset++] = 0x14; // mute
            outRom[context.workingOffset++] = 0x00; // single target

            outRom[context.workingOffset++] = 0x16; // balloon
            outRom[context.workingOffset++] = 0x00; // single target

            outRom[context.workingOffset++] = 0x12; // airblast
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x13; // lightning
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x14; // mute
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x16; // balloon
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x15; // thunder saber
            outRom[context.workingOffset++] = 0x02; // self

            outRom[context.workingOffset++] = 0x15; // thunder saber
            outRom[context.workingOffset++] = 0x01; // all

            int numSylphidSpells = (context.workingOffset - sylphidSpellsListOffset) / 2;

            // //////////////////////////////////////////////////

            int undineSpellsListOffset = context.workingOffset;

            outRom[context.workingOffset++] = 0x06; // freeze
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x06;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x07; // acid storm
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x07;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x08; // hp absorb
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x09; // ice saber
            outRom[context.workingOffset++] = 0x02; // self
            outRom[context.workingOffset++] = 0x09;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x0B; // cure water
            outRom[context.workingOffset++] = 0x02; // self

            int numUndineSpells = (context.workingOffset - undineSpellsListOffset) / 2;

            // //////////////////////////////////////////////////

            int salamandoSpellsListOffset = context.workingOffset;

            outRom[context.workingOffset++] = 0x0C; // fireball
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x0C;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x0D; // exploder
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x0D;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x0E; // lava
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x0E;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x0F; // fire saber
            outRom[context.workingOffset++] = 0x02; // self
            outRom[context.workingOffset++] = 0x0F;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x10; // bouquet
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x10;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x11; // blaze
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x11;
            outRom[context.workingOffset++] = 0x01; // multi target

            int numSalamandoSpells = (context.workingOffset - salamandoSpellsListOffset) / 2;

            // //////////////////////////////////////////////////

            int shadeSpellsListOffset = context.workingOffset;
            outRom[context.workingOffset++] = 0x24; // evilgate
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x24;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x25; // darkforce
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x25;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x26; // dispel
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x26;
            outRom[context.workingOffset++] = 0x01; // multi target

            int numShadeSpells = (context.workingOffset - shadeSpellsListOffset) / 2;

            // //////////////////////////////////////////////////

            int luminaSpellsListOffset = context.workingOffset;
            outRom[context.workingOffset++] = 0x27; // light saber
            outRom[context.workingOffset++] = 0x02; // self
            outRom[context.workingOffset++] = 0x27;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x28; // lucent beam
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x28;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x29; // lucid barrier
            outRom[context.workingOffset++] = 0x02; // self

            int numLuminaSpells = (context.workingOffset - luminaSpellsListOffset) / 2;

            // //////////////////////////////////////////////////

            int lunaSpellsListOffset = context.workingOffset;
            outRom[context.workingOffset++] = 0x19; // mp absorb
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x19;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x1A; // lunar magic
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x1A;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x1B; // moon saber
            outRom[context.workingOffset++] = 0x02; // self
            outRom[context.workingOffset++] = 0x1C; // lunar boost
            outRom[context.workingOffset++] = 0x02; // self
            outRom[context.workingOffset++] = 0x1D; // moon energy
            outRom[context.workingOffset++] = 0x02; // self

            int numLunaSpells = (context.workingOffset - lunaSpellsListOffset) / 2;

            // //////////////////////////////////////////////////

            int dryadSpellsListOffset = context.workingOffset;
            outRom[context.workingOffset++] = 0x1E; // sleep flower
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x1E;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x1F; // burst
            outRom[context.workingOffset++] = 0x00; // single target
            outRom[context.workingOffset++] = 0x1F;
            outRom[context.workingOffset++] = 0x01; // multi target

            outRom[context.workingOffset++] = 0x22; // wall
            outRom[context.workingOffset++] = 0x02; // self
            int numDryadSpells = (context.workingOffset - dryadSpellsListOffset) / 2;

            int numTotalSpells = (context.workingOffset - gnomeSpellsListOffset) / 2;

            // gnome, sylphid, undine, salamando, shade, lumina, luna, dryad, elementless
            int[] spellListSizes = new int[] { numGnomeSpells, numSylphidSpells, numUndineSpells, numSalamandoSpells, numShadeSpells, numLuminaSpells, numLunaSpells, numDryadSpells, numTotalSpells };
            int[] spellListOffsets = new int[] { gnomeSpellsListOffset, sylphidSpellsListOffset, undineSpellsListOffset, salamandoSpellsListOffset, shadeSpellsListOffset, luminaSpellsListOffset, lunaSpellsListOffset, dryadSpellsListOffset, gnomeSpellsListOffset };
            int[] subrLocations = new int[9];
            // generate subroutines to pick a spell to cast for all 8 elements, and elementless
            for (int i = 0; i < 9; i++)
            {
                subrLocations[i] = context.workingOffset + 0xC00000;
                // rng call
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = 0x72;
                outRom[context.workingOffset++] = 0x38;
                outRom[context.workingOffset++] = 0xC0;
                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
                // STA $004204
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x04;
                outRom[context.workingOffset++] = 0x42;
                outRom[context.workingOffset++] = 0x00;
                // SEP 20
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
                // LDA # spell list size
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = (byte)spellListSizes[i];
                // STA $004206
                outRom[context.workingOffset++] = 0x8F;
                outRom[context.workingOffset++] = 0x06;
                outRom[context.workingOffset++] = 0x42;
                outRom[context.workingOffset++] = 0x00;
                // NOP to wait
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
                // LDA $4216
                outRom[context.workingOffset++] = 0xAF;
                outRom[context.workingOffset++] = 0x16;
                outRom[context.workingOffset++] = 0x42;
                outRom[context.workingOffset++] = 0x00;
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
                // ASL because two bytes per thingy
                outRom[context.workingOffset++] = 0x0A;
                // TAX
                outRom[context.workingOffset++] = 0xAA;
                // LDA $ spell list offset,x
                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = (byte)(spellListOffsets[i]);
                outRom[context.workingOffset++] = (byte)(spellListOffsets[i] >> 8);
                outRom[context.workingOffset++] = (byte)((spellListOffsets[i] >> 16) + 0xC0);
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }

            // override spellcasting for randomized-element boss

            /*
             *  replace:
                C2/38BB:   B730   LDA [$30],Y  - load spell number
                C2/38BD:   C8     INY 
                C2/38BE:   C8     INY 
             */
            outRom[0x238BB] = 0x22;
            outRom[0x238BC] = (byte)(context.workingOffset);
            outRom[0x238BD] = (byte)(context.workingOffset >> 8);
            outRom[0x238BE] = (byte)((context.workingOffset >> 16) + 0xC0);
            
            // LDA $01E7,x - species
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xE7;
            outRom[context.workingOffset++] = 0x01;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #0057 - make id relative to mantis ant, the first boss
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x57;
            outRom[context.workingOffset++] = 0x00;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // LDA bossReplacementTableOffset,x 
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)(bossReplacementTableOffset);
            outRom[context.workingOffset++] = (byte)(bossReplacementTableOffset >> 8);
            outRom[context.workingOffset++] = (byte)((bossReplacementTableOffset >> 16) + 0xC0);
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // BNE enemyReplaced
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x06;
            // enemy not replaced - do vanilla behavior
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // (replaced code) LDA [$30],Y
            outRom[context.workingOffset++] = 0xB7;
            outRom[context.workingOffset++] = 0x30;
            // INY twice
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0xC8;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // enemyReplaced:
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // LDA $01A3,x - defense element.. 01=gnome, 02=sylphid, 04=undine, 08=salamando, 10=shade, 20=lumina, 40=luna, 80=dryad, 00=elementless 
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0xA3;
            outRom[context.workingOffset++] = 0x01;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;

            int elementValue = 0x01;
            for(int i=0; i < 8; i++)
            {
                // nextLoop:
                // CMP #element
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = (byte)elementValue;
                outRom[context.workingOffset++] = 0x00;
                // BNE nextLoop
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x08;
                // here: element matches; use spellcast subroutine for that element
                // JSL subrLocations[i]
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = (byte)(subrLocations[i]);
                outRom[context.workingOffset++] = (byte)(subrLocations[i] >> 8);
                outRom[context.workingOffset++] = (byte)(subrLocations[i] >> 16);
                // PLX
                outRom[context.workingOffset++] = 0xFA;
                // INY twice (replaced code)
                outRom[context.workingOffset++] = 0xC8;
                outRom[context.workingOffset++] = 0xC8;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
                // check the next one
                elementValue <<= 1;
            }
            // default case - elementless
            // JSL subrLocations[8] for elementless spell handling
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(subrLocations[8]);
            outRom[context.workingOffset++] = (byte)(subrLocations[8] >> 8);
            outRom[context.workingOffset++] = (byte)(subrLocations[8] >> 16);
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // (replaced code) INY twice
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0xC8;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // for palette shifts, take average hue? and shift by difference to that; maybe toss a couple outliers

            /*
            the palettes come from some sort of boss loader thing listed below.  seems like the 0D and 0E point to palettes that get loaded
            8D 62 ?
            FE 62 -       crystal orb
            FE 62 -       treasure chest
            DC 61 - EA 61 mantis ant        0D [0C] FF 13 04 03 01 00 19 00 00 1D 07 00 FF
            5D 63 - 7A 63 wallface          06 6B 0D [70] 0E [6C] FF 17 13 04 03 01 00 13 CB 02 67 65 16 11 18 F4 7A 1D 1C 00 1D 1D 00 FF
            8D 62 - AF 62 tropicallo        0D [20] 0E [21] FF 13 04 03 01 00 13 CB 02 67 65 16 11 18 A8 44 1D 00 00 11 1E 00 11 27 00 01 25 00 0F 00 FF
            59 62 - 6A 62 minotaur          0D [1A] 0E [1B] FF 13 04 03 01 00 19 01 00 11 08 00 FF
            DC 62 - EC 62 spikey            0D [2D] 0E [2D] FF 13 04 03 01 00 19 0C 00 11 0C 00 FF
            FD 61 - 0D 62 jabber            0D [10] FF 13 04 03 01 00 16 11 18 DB 56 1D 06 00 FF
            8E 63 - 9E 63 springbeak        04 03 00 00 19 0D 00 1D 12 00 FF 0D [67] 0E [67] FF
            1F 63 - 30 63 frost gigas       0D [35] FF 13 04 03 01 00 19 03 00 11 0E 00 11 10 00 FF
            0D 63 - 1E 63 snap dragon       0D [33] FF 13 04 03 00 00 19 03 00 11 0E 00 11 0F 00 FF
            B5 63 - CA 63 mech1             0D [56] 0E [57] FF 13 04 03 01 00 19 07 00 11 13 00 01 58 00 0F 00 FF
            7B 63 - 8D 63 doomwall          06 73 0D [71] 0E [72] FF 13 04 03 00 00 19 0D 00 1D 12 00 FF
            F7 63 - 0C 64 vampire           0D [39] 0E [3A] FF 13 04 03 01 00 13 C0 0F 00 D0 19 04 00 11 15 00 FF
            EB 61 - FC 61 metal mantis      0D [0D] FF FF 13 04 03 00 00 16 11 18 DB 56 1D 06 00 FF
            CB 63 - E0 63 mech2             0D [56] 0E [59] FF 13 04 03 02 00 19 07 00 11 13 00 01 58 00 0F 00 FF
            2A 62 - 47 62 kilroy            0D [12] 0E [13] FF 13 04 03 01 00 19 06 00 04 00 20 F9 D3 04 00 20 5D 41 1D 03 00 1D 04 00 FF
            6A 62 - 8C 62 gorgon            0D [1F] 0E [1F] FF 13 04 03 00 00 13 CB 02 67 65 16 11 18 A8 44 1D 00 00 11 1E 00 11 27 00 01 22 00 0F 00 FF
            8D 62 - AF 62 brambler          0D [20] 0E [21] FF 13 04 03 01 00 13 CB 02 67 65 16 11 18 A8 44 1D 00 00 11 1E 00 11 27 00 01 25 00 0F 00 FF
            B0 62 - CA 62 boreal            0D [23] 0E [24] FF FF 17 19 0E 00 13 C0 0F 00 00 13 C2 0F 00 00 1D 09 00 1D 0A 00 FF
            1F 64 - 2D 64 viper             0D [69] FF 13 04 03 01 00 19 0A 00 1D 14 00 FF
            50 64 - 73 64 limeslime         07 5B 0D [5B] FF 13 04 03 01 00 13 CB 02 01 66 16 11 9C 55 84 1B 1E 1A 00 11 1B 00 11 23 00 01 7E 00 0E 00 FF  - unsure where its other palettes come from?
            ED 62 - FD 62 bluespike         0D [2F] 0E [2F] FF 13 04 03 00 00 19 05 00 11 0D 00 FF
            B5 62 - CA 62 chamber eye       FF 17 19 0E 00 13 C0 0F 00 00 13 C2 0F 00 00 1D 09 00 1D 0A 00 FF
            0E 62 - 29 62 hydra             0D [11] FF 13 04 03 00 00 19 02 00 04 00 20 A0 41 04 00 20 5D 41 1D 03 00 1D 04 00 FF
            CB 62 - DB 62 watermelon        07 2A 0D [2A] FF 13 04 03 00 00 19 0C 00 11 0C 00 FF
            92 64 - A6 64 hexas             0D [37] 0E [78] FF 17 13 04 03 00 00 19 0B 00 1D 01 00 1D 02 00 FF - unsure where its other palettes come from?
            48 62 - 58 62 kettlekin         0D [16] 0E [17] FF 13 04 03 00 00 19 01 00 11 08 00 FF
            FE 62 - 0C 63 tonpole           0D [32] FF 13 04 03 01 00 19 05 00 11 0D 00 FF
            E1 63 - F6 63 mech3             0D [56] 0E [5A] FF 13 04 03 00 00 13 C0 0F 00 D0 19 04 00 11 15 00 FF
            A7 64 - BD 64 snow dragon       07 74 0D [74] 0E [77] FF 17 13 04 03 01 00 19 0B 00 1D 01 00 1D 02 00 FF
            31 63 - 42 63 fire gigas        0D [36] FF 13 04 03 02 00 19 03 00 11 0E 00 11 10 00 FF
            BE 64 - D4 64 red drgn          07 75 0D [75] 0E [77] FF 17 13 04 03 02 00 19 0B 00 1D 01 00 1D 02 00 FF
            9F 63 - B4 63 axe beak          0D [68] 0E [68] FF 13 04 03 00 00 19 07 00 11 13 00 01 58 00 0F 00 FF
            D5 64 - EC 64 blue drgn         07 76 0D [76] 0E [77] FF 17 19 08 00 1D 17 00 1D 26 00 11 16 00 11 25 00 FF
            0D 64 - 1E 64 buffy             0D [3B] 0E [3A] FF FF 13 04 03 00 00 19 0A 00 1D 14 00 FF
            ED 64 - 10 65 lich              06 42 07 3F 0D [3C] FF FF 13 CB 02 01 66 16 0D 18 33 8F 1B 1C 20 8F 1D 24 00 01 82 00 0D 00 01 83 00 0E 00 FF
            FE 62 - 0C 63 biting lizard     0D [32] FF 13 04 03 01 00 19 05 00 11 0D 00 FF
            2E 64 - 4F 64 dragonworm        0D [6A] FF 13 04 03 00 00 13 CB 02 01 66 16 11 9C 55 84 1B 1E 1A 00 11 1B 00 11 23 00 01 7E 00 0E 00 FF
            74 64 - 91 64 dreadslime        07 60 0D [60] FF 13 04 03 00 00 13 CB 02 B4 65 19 09 00 12 21 00 11 22 00 01 38 00 0F 00 FF
            43 63 - 5C 63 thunder gigas     0D [7D] FF 17 13 04 03 00 00 13 CB 02 67 65 16 11 18 F4 7A 1D 1C 00 1D 1D 00 FF
            F4 64 - 10 65 doom's eye        FF 13 CB 02 01 66 16 0D [18] 33 8F 1B 1C 20 8F 1D 24 00 01 82 00 0D 00 01 83 00 0E 00 FF
            11 65 - 19 65 manabeast         04 62 05 63 06 64 07 65 FF ??

            seems like it's just taking [1] in this block == 0x0C for mantisant
            it indexes into bossPaletteBlockData w/ that
            and it gets thiiiis from..
            
            10bb24 are pointers into 21Fxx for palettes
            mantis ant is [0c] up there for some reason, which gives us 10bb3c = (2)F1DC
             */

            // okay so now randomize stuff
            // 01=gnome, 02=sylphid, 04=undine, 08=salamando, 10=shade, 20=lumina, 40=luna, 80=dryad, 00=elementless
            int[] elements = new int[] { 0x01, 0x02, 0x04, 0x08, 0x10, 0x20, 0x40, 0x80, 0x00 };

            double[] targetH = new double[] { 30, 80, 220, 20, 280, 250, 60, 150, -1 };
            double[] targetS = new double[] { 150 / 255.0, 230 / 255.0, 150 / 255.0, 250 / 255.0, 200 / 255.0, 150 / 250.0, 250 / 255.0, 200 / 255.0, 50 / 255.0 };
            double[] targetVFalloff = new double[] { 0.1, 0.1, 0.0, 0.0, 0.2, 0.0, 0, 0.1, 0.3 };
            double[] targetVAdd = new double[] { -0.3, -0.1, 0.0, 0.0, -0.3, 0.2, 0.1, -0.1, 0.1 };

            Dictionary<int, string> bossPlandoNames = new Dictionary<int, string>
            {
                {BOSSID_MANTISANT, KEY_BOSS_ELEMENT_MANTISANT },
                {BOSSID_WALLFACE, KEY_BOSS_ELEMENT_WALLFACE },
                {BOSSID_TROPICALLO, KEY_BOSS_ELEMENT_TROPICALLO },
                {BOSSID_MINOTAUR, KEY_BOSS_ELEMENT_MINOTAUR },
                {BOSSID_SPIKEY, KEY_BOSS_ELEMENT_SPIKEY },
                {BOSSID_JABBER, KEY_BOSS_ELEMENT_JABBERWOCKY },
                {BOSSID_SPRINGBEAK, KEY_BOSS_ELEMENT_SPRINGBEAK },
                {BOSSID_FROSTGIGAS, KEY_BOSS_ELEMENT_FROSTGIGAS },
                {BOSSID_SNAPDRAGON, KEY_BOSS_ELEMENT_SNAPDRAGON },
                // mech rider 1
                {BOSSID_DOOMSWALL, KEY_BOSS_ELEMENT_DOOMSWALL },
                {BOSSID_VAMPIRE, KEY_BOSS_ELEMENT_VAMPIRE },
                {BOSSID_METALMANTIS, KEY_BOSS_ELEMENT_METALMANTIS },
                {BOSSID_MECHRIDER2, KEY_BOSS_ELEMENT_MECHRIDER2 },
                {BOSSID_KILROY, KEY_BOSS_ELEMENT_KILROY },
                {BOSSID_GORGON, KEY_BOSS_ELEMENT_GORGONBULL },
                // brambler
                {BOSSID_BOREAL, KEY_BOSS_ELEMENT_BOREALFACE },
                {BOSSID_GREATVIPER, KEY_BOSS_ELEMENT_GREATVIPER },
                // lime slime
                {BOSSID_BLUESPIKE, KEY_BOSS_ELEMENT_BLUESPIKE },
                // chamber's eye
                {BOSSID_HYDRA, KEY_BOSS_ELEMENT_HYDRA },
                {BOSSID_WATERMELON, KEY_BOSS_ELEMENT_WATERMELON },
                {BOSSID_HEXAS, KEY_BOSS_ELEMENT_HEXAS },
                {BOSSID_KETTLEKIN, KEY_BOSS_ELEMENT_KETTLEKIN },
                {BOSSID_TONPOLE, KEY_BOSS_ELEMENT_TONPOLE },
                {BOSSID_MECHRIDER3, KEY_BOSS_ELEMENT_MECHRIDER3 },
                {BOSSID_SNOWDRAGON, KEY_BOSS_ELEMENT_SNOWDRAGON },
                {BOSSID_FIREGIGAS, KEY_BOSS_ELEMENT_FIREGIGAS },
                {BOSSID_REDDRAGON, KEY_BOSS_ELEMENT_REDDRAGON },
                {BOSSID_AXEBEAK, KEY_BOSS_ELEMENT_AXEBEAK },
                {BOSSID_BLUEDRAGON, KEY_BOSS_ELEMENT_BLUEDRAGON },
                {BOSSID_BUFFY, KEY_BOSS_ELEMENT_BUFFY },
                {BOSSID_DARKLICH, KEY_BOSS_ELEMENT_DARKLICH },
                {BOSSID_BITINGLIZARD, KEY_BOSS_ELEMENT_BITING_LIZARD },
                {BOSSID_DRAGONWORM, KEY_BOSS_ELEMENT_DRAGONWORM },
                // dread slime
                {BOSSID_THUNDERGIGAS, KEY_BOSS_ELEMENT_THUNDERGIGAS },
                // doom's eye
                // mana beast
            };
            Dictionary<int, int> bossBaseHue = new Dictionary<int, int> {
                {BOSSID_MINOTAUR, 20 }, // bull
                {BOSSID_SPIKEY, 20 }, // spikey
                {BOSSID_JABBER, 330 }, // jabber
                {BOSSID_SPRINGBEAK, 300 }, // springbeak
                {BOSSID_FROSTGIGAS, 200 }, // frost gigas
                {BOSSID_MECHRIDER1, 180 }, // mech1
                {BOSSID_VAMPIRE, 330 }, // vampire
                {BOSSID_METALMANTIS, 300 }, // metal mantis
                {BOSSID_MECHRIDER2, 30 }, // mech2
                {BOSSID_GORGON, 130 }, // gordon
                {BOSSID_BOREAL, 30 }, // boreal
                {BOSSID_GREATVIPER, 140 }, // viper
                {BOSSID_KETTLEKIN, 270 }, // kettle
                {BOSSID_TONPOLE, 320 }, // tonpole
                {BOSSID_SNOWDRAGON, 20 }, // snow dragon
                {BOSSID_FIREGIGAS, 50 }, // fire gigas
                {BOSSID_REDDRAGON, 30 }, // red dragon
                {BOSSID_AXEBEAK, 50 }, // axeb
                {BOSSID_BLUEDRAGON, 200 }, // blue drgn
                {BOSSID_DARKLICH, 200 }, // lich
                {BOSSID_BITINGLIZARD, 210 }, // biting lizard
                {BOSSID_DRAGONWORM, 280 }, // dragonworm
                {BOSSID_THUNDERGIGAS, 100 }, // thunder gigas
            };
            Dictionary<int, double> bossSatMultiplier = new Dictionary<int, double> {
                {BOSSID_MECHRIDER1, 500 / 255.0 }, // MR1
                {BOSSID_MECHRIDER2, 800 / 255.0 }, // MR2
                {BOSSID_KETTLEKIN, 450 / 255.0 }, // kettle
                {BOSSID_MECHRIDER3, 300 / 255.0 }, // MR3
                {BOSSID_DARKLICH, 200 / 255.0 }, // lich
                {BOSSID_DRAGONWORM, 700 / 255.0 }, // dragonworm
                {BOSSID_THUNDERGIGAS, 250 / 255.0 }, // thunder gigas
            };

            Dictionary<int, double> hueFlatten = new Dictionary<int, double>
            {
                {BOSSID_DARKLICH, 0.7 }, // lich
                {BOSSID_DRAGONWORM, 0.5 }, // dragonworm
                {BOSSID_THUNDERGIGAS, 0.3 }, // thunder gigas
            };

            // elements that exist in vanilla as palette swaps
            Dictionary<int, List<byte>> dontUseElements = new Dictionary<int, List<byte>>
            {
                { BOSSID_SPRINGBEAK, // beak
                    new byte[] { 0x02, 0x08 }.ToList() },
                { BOSSID_FROSTGIGAS, // gigas
                    new byte[] { 0x02, 0x04, 0x08 }.ToList() },
                { BOSSID_SNOWDRAGON, // dragon
                    new byte[] { 0x02, 0x04, 0x08 }.ToList() },
                { BOSSID_FIREGIGAS, // gigas
                    new byte[] { 0x02, 0x04, 0x08 }.ToList() },
                { BOSSID_REDDRAGON, // dragon
                    new byte[] { 0x02, 0x04, 0x08 }.ToList() },
                { BOSSID_AXEBEAK, // beak
                    new byte[] { 0x02, 0x08 }.ToList() },
                { BOSSID_BLUEDRAGON, // dragon
                    new byte[] { 0x02, 0x04, 0x08 }.ToList() },
                { BOSSID_THUNDERGIGAS, // gigas
                    new byte[] { 0x02, 0x04, 0x08 }.ToList() },
            };
            string[] elementNames = new string[] { VALUE_BOSSELEMENT_GNOME, VALUE_BOSSELEMENT_SYLPHID, VALUE_BOSSELEMENT_UNDINE, VALUE_BOSSELEMENT_SALAMANDO,
                VALUE_BOSSELEMENT_SHADE, VALUE_BOSSELEMENT_LUMINA, VALUE_BOSSELEMENT_LUNA, VALUE_BOSSELEMENT_DRYAD, VALUE_BOSSELEMENT_ELEMENTLESS};
            // for plando, (Elementless), (Default)
            foreach (int bossId in paletteOffsets.Keys)
            {
                string plandoEle = "";
                if(bossPlandoNames.ContainsKey(bossId) && context.plandoSettings.ContainsKey(bossPlandoNames[bossId]))
                {
                    plandoEle = context.plandoSettings[bossPlandoNames[bossId]][0];
                    plandoEle = plandoEle.Replace("(", "").Replace(")", "");
                }

                // No change = let the rando decide (won't be in the plandoSettings)
                // Default = don't change from vanilla
                // Elementless = change to no element and give it all spells
                if (plandoEle != "Default")
                {
                    int ele = 0;
                    byte newEle = (byte)elements[ele];
                    if (plandoEle == "")
                    {
                        byte origEle = outRom[0x101c00 + bossId * 0x1D + 15];
                        ele = random.Next() % elements.Length;
                        newEle = (byte)elements[ele];
                        int iter = 0;
                        while (iter < 100 && (origEle == newEle || (dontUseElements.ContainsKey(bossId) && dontUseElements[bossId].Contains(newEle))))
                        {
                            ele = random.Next() % elements.Length;
                            newEle = (byte)elements[ele];
                            iter++;
                        }
                    }
                    else
                    {
                        ele = elementNames.ToList().IndexOf(plandoEle);
                        newEle = (byte)elements[ele];
                    }

                    Logging.log("Boss " + bossId.ToString("X2") + " (" + context.namesOfThings.getOriginalName(NamesOfThings.INDEX_ENEMIES_START + bossId) + ") element = " + ele + " (" + elementNames[ele] + ")", "spoiler");
                    List<int> paletteIds = paletteOffsets[bossId];
                    foreach (int paletteId in paletteIds)
                    {
                        int paletteOffset = 0x20000 + DataUtil.ushortFromBytes(outRom, 0x10bb24 + paletteId * 2);
                        // fuck with the colors; leave index 0 and 14 out of it though

                        double avgHue;

                        if (!bossBaseHue.ContainsKey(bossId))
                        {
                            double totalWeight = 0;
                            double totalHue = 0;
                            double totalSaturation = 0;
                            for (int i = 1; i <= 14; i++)
                            {
                                SnesColor color = new SnesColor(outRom, paletteOffset + i * 2);
                                ColorUtil.rgbToHsv(color.getRed(), color.getGreen(), color.getBlue(), out double h, out double s, out double v);
                                // weigh colors by saturation & value
                                double weight = s + (0.5 - Math.Abs(0.5 - v)) * 2;
                                totalHue += h * weight;
                                totalWeight += weight;
                                totalSaturation += s * v;
                            }
                            avgHue = totalHue / totalWeight;
                        }
                        else
                        {
                            avgHue = bossBaseHue[bossId];
                        }

                        double targetSValue = targetS[ele];
                        if (bossSatMultiplier.ContainsKey(bossId))
                        {
                            targetSValue = bossSatMultiplier[bossId];
                        }
                        double tH = targetH[ele] < 0 ? -1 : targetH[ele] + ((random.Next() % 100) / 100.0) * 8;
                        double tS = targetSValue + ((random.Next() % 100) / 100.0) * 0.1;
                        double tV = targetVFalloff[ele] + ((random.Next() % 100) / 100.0) * 0.02;
                        double tA = targetVAdd[ele] + ((random.Next() % 100) / 100.0) * 0.05;

                        double hueShift = tH < 0 ? -1 : avgHue - tH; // subtract this later

                        for (int i = 1; i <= 14; i++)
                        {
                            SnesColor color = new SnesColor(outRom, paletteOffset + i * 2);
                            ColorUtil.rgbToHsv(color.getRed(), color.getGreen(), color.getBlue(), out double h, out double s, out double v);

                            double vDiff = (1 - v) * tV;
                            v -= vDiff;
                            v += tA;
                            if (v < 0.1)
                            {
                                v = 0.1;
                            }
                            if (v > 1)
                            {
                                v = 1;
                            }

                            if (hueShift != -1)
                            {
                                h -= hueShift;
                                if (hueFlatten.ContainsKey(bossId))
                                {
                                    double diffFromTh = h - tH;
                                    h -= diffFromTh * hueFlatten[bossId];
                                }
                            }

                            s *= tS;

                            ColorUtil.hsvToRgb(h, s, v, out int r, out int g, out int b);
                            SnesColor colorNew = new SnesColor(r, g, b);
                            colorNew.put(outRom, paletteOffset + i * 2);
                        }

                    }
                    // set def ele on enemy data
                    outRom[0x101c00 + bossId * 0x1D + 15] = newEle;
                }
            }
            return true;
        }
    }
}
