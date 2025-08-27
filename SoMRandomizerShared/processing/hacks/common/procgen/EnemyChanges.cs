using SoMRandomizer.config;
using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.ancientcave;
using SoMRandomizer.processing.bossrush;
using SoMRandomizer.processing.chaos;
using SoMRandomizer.processing.common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SoMRandomizer.processing.hacks.common.procgen
{
    /// <summary>
    /// Changes to enemies to make their stats scale to the floor,
    /// based on configurable-growth stats passed in from the display.
    /// This is only used for procgen modes, since it operates on floor numbers.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class EnemyChanges : RandoProcessor
    {
        protected override string getName()
        {
            return "Stat generation for non-vanilla modes";
        }
        // MOPPLE: i'd like to eventually remove this and replace it with what open world uses for stat generation
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            int numMaps = 0;
            double scalingRate = 1.0;
            int floorTableLocation = 0;
            if (randoMode == AncientCaveSettings.MODE_KEY)
            {
                numMaps = AncientCaveGenerator.LENGTH_CONVERSIONS[settings.get(AncientCaveSettings.PROPERTYNAME_LENGTH)];
                scalingRate = Math.Ceiling(24.0 / numMaps);
                floorTableLocation = -1;
            }
            else if (randoMode == BossRushSettings.MODE_KEY)
            {
                numMaps = BossOrderRandomizer.allPossibleBosses.Count;
                scalingRate = 1.0;
                floorTableLocation = -1;
            }
            else if (randoMode == ChaosSettings.MODE_KEY)
            {
                numMaps = ChaosRandomizer.MAPNUM_SETTING_VALUES[settings.get(ChaosSettings.PROPERTYNAME_NUM_FLOORS)];
                scalingRate = 24.0 / numMaps;
                floorTableLocation = context.workingData.getInt(ChaosRandomizer.FLOOR_TABLE_LOCATION);
            }
            else
            {
                Logging.log("Unsupported mode for ancient cave-style stat generation");
                return false;
            }

            byte manaBeastScaledMap = (byte)(numMaps * 2);
            // write stat tables.. N normal-enemy values followed by N boss-values
            // if we're looking at a boss, add N to the index value to load the second set of values
            // X = mapnum; if enemy id >= boss threshold; x+=numMaps
            // LDA strRomAddr,x .. etc

            Dictionary<string, int> statOffsets = new Dictionary<string, int>();
            double bossStatMul = settings.getDouble(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.BOSS_STAT_MULTIPLIER);
            double bossExpMul = settings.getDouble(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.BOSS_EXP_MULTIPLIER);
            double bossGoldMul = settings.getDouble(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.BOSS_GOLD_MULTIPLIER);
            double bossHpMul = settings.getDouble(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.BOSS_HP_MULTIPLIER);
            // level
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_LEVEL, settings,
                statOffsets, 255, bossStatMul, false, scalingRate);
            // str
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_STR, settings,
                statOffsets, 255, bossStatMul, false, scalingRate);
            // agi
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_AGI, settings,
                statOffsets, 255, bossStatMul, false, scalingRate);
            // int
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_INT, settings,
                statOffsets, 255, bossStatMul, false, scalingRate);
            // wis
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_WIS, settings,
                statOffsets, 255, bossStatMul, false, scalingRate);
            // eva
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_EVADE, settings,
                statOffsets, 255, bossStatMul, false, scalingRate);
            // meva
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_MEVADE, settings,
                statOffsets, 255, bossStatMul, false, scalingRate);
            // mp
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_MP, settings,
                statOffsets, 255, bossStatMul, false, scalingRate);
            // weapon level
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_WEAPON_LEV, settings,
                statOffsets, 8, bossStatMul, false, scalingRate);
            // magic level
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_MAGIC_LEV, settings,
                statOffsets, 8, bossStatMul, false, scalingRate);

            // weapon dmg
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_WEAPON_DAMAGE, settings,
                statOffsets, 255, bossStatMul, false, scalingRate);

            // def
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_DEF, settings,
                statOffsets, 65535, bossStatMul, true, scalingRate);
            // mdef
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_MDEF, settings,
                statOffsets, 65535, bossStatMul, true, scalingRate);
            // exp
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_EXP, settings,
                statOffsets, 65535, bossExpMul, true, scalingRate);
            // hp
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_HP, settings,
                statOffsets, 65535, bossHpMul, true, scalingRate);
            // gold
            writeStatTable(outRom, context, numMaps, DifficultySettings.ENEMY_GOLD, settings,
                statOffsets, 65535, bossGoldMul, true, scalingRate);


            // MOPPLE: i don't remember exactly what these changes were for but some changes to logic in event types
            // 0x49 and 0x4a .. probably not the best fix for whatever this was a workaround for?

            // $01/F244 B0 26       BCS $26    [$F26C]      A:0700 X:0781 Y:0002 P:eNvMxdIzc
            outRom[0x1F244] = 0xEA;
            outRom[0x1F245] = 0xEA;
            // $01/F24C F0 1E       BEQ $1E    [$F26C]      A:0701 X:0781 Y:0002 P:envMxdIZC
            outRom[0x1F24C] = 0xEA;
            outRom[0x1F24D] = 0xEA;

            Dictionary<string, byte> statRamOffsets = new Dictionary<string, byte>();

            statRamOffsets[DifficultySettings.ENEMY_LEVEL] = 0x81;
            statRamOffsets[DifficultySettings.ENEMY_STR] = 0x88;
            statRamOffsets[DifficultySettings.ENEMY_AGI] = 0x89;
            statRamOffsets[DifficultySettings.ENEMY_INT] = 0x8B;
            statRamOffsets[DifficultySettings.ENEMY_WIS] = 0x8C;
            statRamOffsets[DifficultySettings.ENEMY_EVADE] = 0xA4;
            statRamOffsets[DifficultySettings.ENEMY_MEVADE] = 0xA7;
            statRamOffsets[DifficultySettings.ENEMY_MP] = 0x86; // current, then max (8 bit)
            statRamOffsets[DifficultySettings.ENEMY_DEF] = 0xA5;
            statRamOffsets[DifficultySettings.ENEMY_MDEF] = 0xA8;
            statRamOffsets[DifficultySettings.ENEMY_EXP] = 0x8D;
            statRamOffsets[DifficultySettings.ENEMY_HP] = 0x82; // current, then max (16 bit)
            statRamOffsets[DifficultySettings.ENEMY_GOLD] = 0xC8;
            statRamOffsets[DifficultySettings.ENEMY_WEAPON_LEV] = 0xC0;
            statRamOffsets[DifficultySettings.ENEMY_MAGIC_LEV] = 0xC4;

            // there are two different stat loaders here - one of these is used when the object is loaded (second one, i think?) and the
            // other is called now and then to refresh things like status

            // remove a few vanilla stat loaders first
            for (int i = 0x5625; i <= 0x562e; i++)
            {
                outRom[i] = 0xEA;
            }
            for (int i = 0x5636; i <= 0x5643; i++)
            {
                outRom[i] = 0xEA;
            }
            for (int i = 0x5652; i <= 0x5666; i++)
            {
                outRom[i] = 0xEA;
            }
            for (int i = 0x567f; i <= 0x5688; i++)
            {
                outRom[i] = 0xEA;
            }

            outRom[0x5625] = 0x22;
            outRom[0x5626] = (byte)(context.workingOffset);
            outRom[0x5627] = (byte)(context.workingOffset >> 8);
            outRom[0x5628] = (byte)((context.workingOffset >> 16) + 0xC0);

            int tonpoleMp = settings.getInt(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + DifficultySettings.TONPOLE_MP);
            // replace with subroutine for hp, def, mdef, str, agi, int, wis, exp
            writeStatInjectionSubroutine(outRom, context, numMaps, statOffsets, statRamOffsets, true, tonpoleMp, floorTableLocation);


            // remove a couple vanilla stat loaders in the second block, too
            for (int i = 0x477c; i <= 0x47A0; i++)
            {
                outRom[i] = 0xEA;
            }
            for (int i = 0x47a8; i <= 0x47b5; i++)
            {
                outRom[i] = 0xEA;
            }

            outRom[0x477c] = 0x22;
            outRom[0x477d] = (byte)(context.workingOffset);
            outRom[0x477e] = (byte)(context.workingOffset >> 8);
            outRom[0x477f] = (byte)((context.workingOffset >> 16) + 0xC0);
            // replace with subroutine for str, agi, int, wis, evade, def, mevade, mdef, [species, element]
            // same as above except without hp/mp.
            writeStatInjectionSubroutine(outRom, context, numMaps, statOffsets, statRamOffsets, false, tonpoleMp, floorTableLocation);

            // weapon changes
            outRom[0x49C5] = 0x22;
            outRom[0x49C6] = (byte)(context.workingOffset);
            outRom[0x49C7] = (byte)(context.workingOffset >> 8);
            outRom[0x49C8] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x49C9] = 0xEA;

            // CPX #0600 - check object id
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;

            // BCS enemy (06)
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x06;

            // (replaced code, for non-enemies, then return)
            // LDY #$1008
            outRom[context.workingOffset++] = 0xA0;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x10;

            // LDA [$C0], y
            outRom[context.workingOffset++] = 0xB7;
            outRom[context.workingOffset++] = 0xC0;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // enemy:
            // AF DC 00 7E LDA 7E00DC
            // 18          CLC
            // 69 02       ADC #$08
            // 6B          RTL
            // ^ old
            // new:
            // PHX
            outRom[context.workingOffset++] = 0xDA;

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // LDA 7E00DC - load map number, to determine floor number
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xDC;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;

            if (floorTableLocation < 0)
            {
                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
            }

            // check for mana beast (not necessary here, since it's normal enemies, but just to be safe)
            // CMP #00FD
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xFD;
            outRom[context.workingOffset++] = 0x00;

            // not mana beast? skip
            // BNE skip
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x03;

            // special case mana beast
            // LDA #(numMaps - 1) * 2
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)((numMaps - 1) * 2);
            outRom[context.workingOffset++] = 0x00;

            // skip:
            if (floorTableLocation < 0)
            {
                // shift right since two maps per floor
                // LSR
                outRom[context.workingOffset++] = 0x4A;
            }
            else
            {
                // look up floorTableLocation first
                // TAX
                outRom[context.workingOffset++] = 0xAA;
                // LDA floorTableLocation,X
                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = (byte)(floorTableLocation);
                outRom[context.workingOffset++] = (byte)(floorTableLocation >> 8);
                outRom[context.workingOffset++] = (byte)((floorTableLocation >> 16) + 0xC0);

                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
            }

            // TAX
            outRom[context.workingOffset++] = 0xAA;

            int weaponRomOffset = statOffsets[DifficultySettings.ENEMY_WEAPON_DAMAGE];

            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // LDA romOffset, x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)weaponRomOffset;
            outRom[context.workingOffset++] = (byte)(weaponRomOffset >> 8);
            outRom[context.workingOffset++] = (byte)(weaponRomOffset >> 16);

            // PLX
            outRom[context.workingOffset++] = 0xFA;

            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // boss weapon changes
            // essentially the same as above but pull from rom loc + numMaps for the second set of values.
            outRom[0x462C] = 0x22;
            outRom[0x462D] = (byte)(context.workingOffset);
            outRom[0x462E] = (byte)(context.workingOffset >> 8);
            outRom[0x462F] = (byte)((context.workingOffset >> 16) + 0xC0);

            // PHX
            outRom[context.workingOffset++] = 0xDA;

            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // LDA 7E00DC
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xDC;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;

            if (floorTableLocation < 0)
            {
                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
            }

            // check for mana beast
            // CMP #00FD
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xFD;
            outRom[context.workingOffset++] = 0x00;

            // not mana beast? skip
            // BNE skip
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x03;

            // special case mana beast
            // LDA #(numMaps - 1) * 2
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)((numMaps - 1) * 2);
            outRom[context.workingOffset++] = 0x00;

            // skip:
            if (floorTableLocation < 0)
            {
                // shift right since two maps per floor
                // LSR
                outRom[context.workingOffset++] = 0x4A;
            }
            else
            {
                // look up floorTableLocation first
                // TAX
                outRom[context.workingOffset++] = 0xAA;
                // LDA floorTableLocation,X
                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = (byte)(floorTableLocation);
                outRom[context.workingOffset++] = (byte)(floorTableLocation >> 8);
                outRom[context.workingOffset++] = (byte)((floorTableLocation >> 16) + 0xC0);

                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
            }

            // TAX
            outRom[context.workingOffset++] = 0xAA;

            int bossWeaponRomOffset = statOffsets[DifficultySettings.ENEMY_WEAPON_DAMAGE] + numMaps;

            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // LDA romOffset, x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)bossWeaponRomOffset;
            outRom[context.workingOffset++] = (byte)(bossWeaponRomOffset >> 8);
            outRom[context.workingOffset++] = (byte)(bossWeaponRomOffset >> 16);

            // PLX
            outRom[context.workingOffset++] = 0xFA;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }

        private void writeStatInjectionSubroutine(byte[] outRom, RandoContext context, int numMaps, Dictionary<string, int> statOffsets,
            Dictionary<string, byte> statRamOffsets, bool includeHpMp, int tonpoleMp, int floorTableLocation)
        {
            if (!includeHpMp)
            {
                // LDY $BD
                outRom[context.workingOffset++] = 0xA4;
                outRom[context.workingOffset++] = 0xBD;
            }

            // load map num
            // LDA $7E00DC
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xDC;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x7E;

            if (floorTableLocation < 0)
            {
                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
            }

            // check for mana beast
            // CMP #00FD
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0xFD;
            outRom[context.workingOffset++] = 0x00;

            // not mana beast? skip
            // BNE skip
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x03;

            // special case mana beast
            // LDA #(numMaps - 1) * 2
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = (byte)((numMaps - 1) * 2);
            outRom[context.workingOffset++] = 0x00;

            // skip:
            if (floorTableLocation < 0)
            {
                // shift right since two maps per floor
                // LSR
                outRom[context.workingOffset++] = 0x4A;
            }
            else
            {
                // PHX
                outRom[context.workingOffset++] = 0xDA;
                // look up floorTableLocation first
                // TAX
                outRom[context.workingOffset++] = 0xAA;
                // LDA floorTableLocation,X
                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = (byte)(floorTableLocation);
                outRom[context.workingOffset++] = (byte)(floorTableLocation >> 8);
                outRom[context.workingOffset++] = (byte)((floorTableLocation >> 16) + 0xC0);

                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
                // PLX
                outRom[context.workingOffset++] = 0xFA;
            }

            // CPX #09DB - check for boss
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0xDB;
            outRom[context.workingOffset++] = 0x09;

            // BCC skip if not boss
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x04;

            // CLC
            outRom[context.workingOffset++] = 0x18;

            // ADC #numMaps for boss
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = (byte)numMaps;
            outRom[context.workingOffset++] = (byte)(numMaps >> 8);

            // skip:
            // PHX
            outRom[context.workingOffset++] = 0xDA;

            // TAX
            outRom[context.workingOffset++] = 0xAA;

            // 8bit A mode
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // 8bit stats
            writeCodeForRamInjection(outRom, context, statOffsets[DifficultySettings.ENEMY_LEVEL], statRamOffsets[DifficultySettings.ENEMY_LEVEL]);
            writeCodeForRamInjection(outRom, context, statOffsets[DifficultySettings.ENEMY_STR], statRamOffsets[DifficultySettings.ENEMY_STR]);
            writeCodeForRamInjection(outRom, context, statOffsets[DifficultySettings.ENEMY_AGI], statRamOffsets[DifficultySettings.ENEMY_AGI]);
            writeCodeForRamInjection(outRom, context, statOffsets[DifficultySettings.ENEMY_INT], statRamOffsets[DifficultySettings.ENEMY_INT]);
            writeCodeForRamInjection(outRom, context, statOffsets[DifficultySettings.ENEMY_WIS], statRamOffsets[DifficultySettings.ENEMY_WIS]);
            writeCodeForRamInjection(outRom, context, statOffsets[DifficultySettings.ENEMY_EVADE], statRamOffsets[DifficultySettings.ENEMY_EVADE]);
            writeCodeForRamInjection(outRom, context, statOffsets[DifficultySettings.ENEMY_MEVADE], statRamOffsets[DifficultySettings.ENEMY_MEVADE]);
            if (includeHpMp)
            {
                writeCodeForRamInjectionMp(outRom, context, statOffsets[DifficultySettings.ENEMY_MP], statRamOffsets[DifficultySettings.ENEMY_MP], tonpoleMp);
                writeCodeForRamInjectionMp(outRom, context, statOffsets[DifficultySettings.ENEMY_MP], (byte)(statRamOffsets[DifficultySettings.ENEMY_MP] + 1), tonpoleMp);
            }
            writeCodeForRamInjection(outRom, context, statOffsets[DifficultySettings.ENEMY_WEAPON_LEV], statRamOffsets[DifficultySettings.ENEMY_WEAPON_LEV]);
            writeCodeForRamInjection(outRom, context, statOffsets[DifficultySettings.ENEMY_MAGIC_LEV], statRamOffsets[DifficultySettings.ENEMY_MAGIC_LEV]);

            // turn off 8bit A mode
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // now double x for the 16 bit stats

            // TXA
            outRom[context.workingOffset++] = 0x8A;

            // ASL
            outRom[context.workingOffset++] = 0x0A;

            // TAX
            outRom[context.workingOffset++] = 0xAA;

            // 16bit stats
            writeCodeForRamInjection(outRom, context, statOffsets[DifficultySettings.ENEMY_DEF], statRamOffsets[DifficultySettings.ENEMY_DEF]);
            writeCodeForRamInjection(outRom, context, statOffsets[DifficultySettings.ENEMY_MDEF], statRamOffsets[DifficultySettings.ENEMY_MDEF]);
            writeCodeForRamInjection(outRom, context, statOffsets[DifficultySettings.ENEMY_EXP], statRamOffsets[DifficultySettings.ENEMY_EXP]);
            if (includeHpMp)
            {
                writeCodeForRamInjection(outRom, context, statOffsets[DifficultySettings.ENEMY_HP], statRamOffsets[DifficultySettings.ENEMY_HP]);
                writeCodeForRamInjection(outRom, context, statOffsets[DifficultySettings.ENEMY_HP], (byte)(statRamOffsets[DifficultySettings.ENEMY_HP] + 2));
            }
            writeCodeForRamInjection(outRom, context, statOffsets[DifficultySettings.ENEMY_GOLD], statRamOffsets[DifficultySettings.ENEMY_GOLD]);

            // PLX
            outRom[context.workingOffset++] = 0xFA;

            // RTL
            outRom[context.workingOffset++] = 0x6B;
        }

        private void writeCodeForRamInjection(byte[] outRom, RandoContext context, int romOffset, byte ramOffset)
        {
            // LDA romOffset, x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)romOffset;
            outRom[context.workingOffset++] = (byte)(romOffset >> 8);
            outRom[context.workingOffset++] = (byte)(romOffset >> 16);
            // STA $EBxx, y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = ramOffset;
            outRom[context.workingOffset++] = 0xE1;
        }


        private void writeCodeForRamInjectionMp(byte[] outRom, RandoContext context, int romOffset, byte ramOffset, int tonpoleMp)
        {
            // LDA romOffset, x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = (byte)romOffset;
            outRom[context.workingOffset++] = (byte)(romOffset >> 8);
            outRom[context.workingOffset++] = (byte)(romOffset >> 16);

            // need to restore x in here temporarily .. god what a clusterfuck to do this though
            // PHX - 2 bytes
            outRom[context.workingOffset++] = 0xDA;
            // PHA - 1 byte
            outRom[context.workingOffset++] = 0x48;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // LDA 4,s - seek back 5 bytes to the X we pushed earlier
            outRom[context.workingOffset++] = 0xA3;
            outRom[context.workingOffset++] = 0x04;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // PLA
            outRom[context.workingOffset++] = 0x68;

            // see if we should overwrite A with tonpole mp
            byte[] lowerMpEnemies = new byte[] { 95, 113, 122 };
            foreach (byte b in lowerMpEnemies)
            {
                int val = b * 0x1D;
                // cpx #enemyIndex
                outRom[context.workingOffset++] = 0xE0;
                outRom[context.workingOffset++] = (byte)val;
                outRom[context.workingOffset++] = (byte)(val >> 8);

                // bne to skip / run next iter
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x02;

                // LDA #tonpoleMp
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = (byte)tonpoleMp;
            }

            // 99 xx EB    STA $EBxx, y
            outRom[context.workingOffset++] = 0x99;
            outRom[context.workingOffset++] = ramOffset;
            outRom[context.workingOffset++] = 0xE1;

            // PLX - even out the stack; restore table read position
            outRom[context.workingOffset++] = 0xFA;
        }

        private void writeStatTable(byte[] outRom, RandoContext context, int numMaps, string statName,
            RandoSettings settings, Dictionary<string, int> statOffsets, int maxValue, double bossMultiplier, bool sixteenBit,
            double scalingRate)
        {
            bool agi = statName == DifficultySettings.ENEMY_AGI;
            Logging.log("Writing stat table for " + statName + " at offset " + context.workingOffset.ToString("X6"), "debug");
            statOffsets[statName] = context.workingOffset + 0xC00000;
            double baseValue = settings.getDouble(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + statName + "_base");
            double growthValue = settings.getDouble(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + statName + "_growth") * scalingRate;
            double expValue = settings.getDouble(DifficultySettings.DIFFICULTY_PROPERTY_PREFIX + statName + "_exp");
            for (int i = 0; i < numMaps; i++)
            {
                int statThisFloor = (int)(baseValue + Math.Pow(i, expValue) * growthValue);
                int bossStatThisFloor = (int)(statThisFloor * bossMultiplier);

                if (statThisFloor > maxValue)
                {
                    statThisFloor = maxValue;
                }
                if (bossStatThisFloor > maxValue)
                {
                    bossStatThisFloor = maxValue;
                }
                // 0 agi for aggressive bosses, otherwise you basically don't get to play
                if (agi && settings.getBool(CommonSettings.PROPERTYNAME_AGGRESSIVE_BOSSES))
                {
                    bossStatThisFloor = 0;
                }
                if (sixteenBit)
                {
                    outRom[context.workingOffset] = (byte)statThisFloor;
                    outRom[context.workingOffset + 1] = (byte)(statThisFloor>>8);
                    outRom[context.workingOffset + numMaps * 2] = (byte)bossStatThisFloor;
                    outRom[context.workingOffset + numMaps * 2 + 1] = (byte)(bossStatThisFloor >> 8);
                    context.workingOffset += 2;
                }
                else
                {
                    outRom[context.workingOffset] = (byte)statThisFloor;
                    outRom[context.workingOffset + numMaps] = (byte)bossStatThisFloor;
                    context.workingOffset++;
                }
            }

            // now skip the pointer past the boss stuff
            if(sixteenBit)
            {
                context.workingOffset += numMaps * 2;
            }
            else
            {
                context.workingOffset += numMaps;
            }
        }
    }
}
