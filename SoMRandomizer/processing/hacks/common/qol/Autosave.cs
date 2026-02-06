using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using static SoMRandomizer.processing.common.SomVanillaValues;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// Hack that makes save slot 4 an auto-save that triggers on most door transitions.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class Autosave : RandoProcessor
    {
        protected override string getName()
        {
            return "Auto-save to slot 4 on door transitions";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            if (randoMode == VanillaRandoSettings.MODE_KEY)
            {
                if(!settings.getBool(VanillaRandoSettings.PROPERTYNAME_AUTOSAVE))
                {
                    return false;
                }
            }
            else if (randoMode == OpenWorldSettings.MODE_KEY)
            {
                if (!settings.getBool(OpenWorldSettings.PROPERTYNAME_AUTOSAVE))
                {
                    return false;
                }
            }
            else
            {
                Logging.log("Unsupported mode for autosave");
                return false;
            }

            // saveram info
            // 7EA211 has the slot [3 for last one]
            // save file basic structure, and what all goes into it:
            // 7ECCxx - 0x80 bytes - stuff like character equipment, money, and inventory are here
            // 7ECFxx - 0x80 bytes down from 0x100, since only the lower nibble is used - event flags
            // 7ee180 - 0x80 bytes - boy character stats
            // 7ee380 - 0x80 bytes - girl character stats
            // 7ee580 - 0x80 bytes - sprite character stats
            // 7ee000 - 6 bytes - boy character flags?
            // 7ee200 - 6 bytes - girl character flags?
            // 7ee400 - 6 bytes - sprite character flags?
            // 7e00ee, ef, f0
            // 7e00d9 & 0x0F
            // 7EA23E - 0x11 bytes
            // 7EA25A - door to restore from
            // 7EA24F - 0x0C bytes
            // some other shit in here?
            // 7EA175?
            // checksum

            // 7e010e is location in RAM of the last door transitioned

            // $C0 / 0105 AF 5A A2 7E LDA $7EA25A[$7E:A25A]   A:02FF X:0400 Y:0000 P:envmxdIzc
            // $C0 / 0109 28          PLP A:018C X:0400 Y: 0000 P: envmxdIzc

            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 800);

            // save on (almost) every door transition
            // $01 / E7D6 29 FF 03    AND #$03FF              A:098C X:FFFE Y:0090 P:envmxdIzC
            // $01 / E7D9 8D 0E 01    STA $010E[$7E:010E]   A: 018C X:FFFE Y:0090 P: envmxdIzC
            outRom[0x1E7D6] = 0x22;
            outRom[0x1E7D7] = (byte)(context.workingOffset);
            outRom[0x1E7D8] = (byte)(context.workingOffset >> 8);
            outRom[0x1E7D9] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x1E7DA] = 0xEA;
            outRom[0x1E7DB] = 0xEA;

            // replace with:

            // same code
            // 29 FF 03     AND #$03FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x03;

            // 8D 0E 01     STA $010E[$7E:010E]
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x0E;
            outRom[context.workingOffset++] = 0x01;

            int[] skipDoors = new int[]
            {
                0x75, // death door
                0x82, // after bridge or at elder (depending on flammie drum logic)
                0x38, // jehk return door
                0x200, // santa return door
                0x0C, // potos intro return door
                0x3B, // mana beast outside
                0x271, // shade palace internal door that has a return back
                0x2F, 0x154, // lich fight
                0x366, // tacnisa fight
                0x304, // dopple fight
                0x35D, // NTC roof
                0xFA, // dwarf town item shop
                0x3DF, // hexas
                0x254, // matango item shop
                0x1A3, // desert ship "skip this plz" spot in rando
                0x31a, // hydra
                0x3c7, // kettlekin
                0x20f, // that ice country spot where it sticks you in a tree
                0x84, // similar door in upper land that throws you in a lake
                0x186, // ice country door from cannon to the left that sticks you in trees
                0x211, // ice country downward into the paradise town thing
            };


            // first check if maybe we shouldn't save because everybody is dead

            // PHA
            outRom[context.workingOffset++] = 0x48;
            // LDA #0000
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // PHA
            outRom[context.workingOffset++] = 0x48;

            // LDA $7EE000 - character 0 (boy) exists
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x7E;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // BEQ check2
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x07;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC $7EE182 - boy current hp
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = 0x82;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // check2:
            // LDA $7EE200 - character 1 (girl) exists
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x7E;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // BEQ check3
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x07;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC $7EE382 - girl current hp
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = 0x82;
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x7E;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // check3:
            // LDA $7EE400 - character 2 (sprite) exists
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xE4;
            outRom[context.workingOffset++] = 0x7E;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // BEQ checkEnd
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x07;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC $7EE582 - sprite current hp
            outRom[context.workingOffset++] = 0x6F;
            outRom[context.workingOffset++] = 0x82;
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0x7E;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // checkEnd:
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // BNE someoneAlive
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x02;
            // PLA 
            outRom[context.workingOffset++] = 0x68;
            // RTL - don't save because everyone died
            outRom[context.workingOffset++] = 0x6B;

            // PLA - pull the door id back out
            outRom[context.workingOffset++] = 0x68;

            // compare against all the doors we don't want to save on, and RTL if it's one of those
            foreach (int skipDoor in skipDoors)
            {
                // CMP #xxxx
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = (byte)skipDoor;
                outRom[context.workingOffset++] = (byte)(skipDoor>>8);

                // BNE over [01]
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x01;

                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }
            
            // check for don't autosave (once) flag - this is so loading the autosave door doesn't, again, autosave
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA RamOffsets.DONT_AUTOSAVE
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.DONT_AUTOSAVE;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.DONT_AUTOSAVE >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.DONT_AUTOSAVE >> 16));
            // CMP #01
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            // BNE over [0A]
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x0A;
            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // STA RamOffsets.DONT_AUTOSAVE
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.DONT_AUTOSAVE;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.DONT_AUTOSAVE >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.DONT_AUTOSAVE >> 16));
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // over:
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // check for "inside house" map
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PHX
            outRom[context.workingOffset++] = 0xDA;
            // ASL
            outRom[context.workingOffset++] = 0x0A;
            // ASL
            outRom[context.workingOffset++] = 0x0A;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // LDA C83000,x - load door data, byte 0 and 1
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x30;
            outRom[context.workingOffset++] = 0xC8;
            // AND #01FF - extract the target map from the door data
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x01;

            // here: filter out maps we don't want to save at, too
            int[] dontSaveMaps = new int[]
            {
                MAPNUM_SPIKEY_WITH_NPCS, // spikey arena
                MAPNUM_EARTHPALACE_INTERIOR_C, // before fire gigas arena
                MAPNUM_INTRO_LOG, // intro
                MAPNUM_DEATH_MAP, // intro / death map
                MAPNUM_GOLDCITY_INTERIOR, // gold city interior - some doors are triggers, others are exits
                MAPNUM_NORTHTOWN_INTERIOR_C, // similar to above for another town
                MAPNUM_SOUTHTOWN_INTERIOR_B, // similar to above for another town
                MAPNUM_SOUTHTOWN_INTERIOR_A, // similar to above for another town
                MAPNUM_NORTHTOWN_INTERIOR_B, // similar to above for another town
                MAPNUM_NORTHTOWN_INTERIOR_A, // similar to above for another town
                MAPNUM_KIPPO_INTERIOR, // kippo internal
                MAPNUM_ICECASTLE_SANTA, // pre-santa fight
                MAPNUM_NTC_INTERIOR_N, // before MR2
                MAPNUM_TODO_INTERIOR, // walrus interior
                MAPNUM_PANDORA_INTERIOR_A, // pandora interior
                MAPNUM_ICECASTLE_INTERIOR_I, // pre-triple-tonpole
                MAPNUM_NTR_INTERIOR_H, // NTR indoor rooms
            };

            // if the target map is any of the ones we don't want, RTL and don't autosave.
            foreach(int dontSaveMap in dontSaveMaps)
            {
                // CMP #xxxx
                outRom[context.workingOffset++] = 0xC9;
                outRom[context.workingOffset++] = (byte)dontSaveMap;
                outRom[context.workingOffset++] = (byte)(dontSaveMap >> 8);

                // BNE over [03]
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0x03;

                // PLX
                outRom[context.workingOffset++] = 0xFA;
                // PLA
                outRom[context.workingOffset++] = 0x68;
                // RTL
                outRom[context.workingOffset++] = 0x6B;

            }

            // pull the target map's object list and search for bosses.  if one exists, don't autosave.

            // ASL
            outRom[context.workingOffset++] = 0x0A;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // LDA C84000,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x40;
            outRom[context.workingOffset++] = 0xC8;
            // CMP C84002,x
            outRom[context.workingOffset++] = 0xDF;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0x40;
            outRom[context.workingOffset++] = 0xC8;
            // BNE over [03]
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x03;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // PHY
            outRom[context.workingOffset++] = 0x5A;
            // LDA C87002,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x02;
            outRom[context.workingOffset++] = 0x70;
            outRom[context.workingOffset++] = 0xC8;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC C87000,x
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x70;
            outRom[context.workingOffset++] = 0xC8;
            // TAY
            outRom[context.workingOffset++] = 0xA8;
            // LDA C87000,x
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x70;
            outRom[context.workingOffset++] = 0xC8;
            // CLC
            outRom[context.workingOffset++] = 0x18;
            // ADC #0008 - skip header
            outRom[context.workingOffset++] = 0x69;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x00;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // PHB
            outRom[context.workingOffset++] = 0x8B;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA #C8
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0xC8;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // PLB
            outRom[context.workingOffset++] = 0xAB;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // checkLoop:
            // TYA
            outRom[context.workingOffset++] = 0x98;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC #0008 - subtract each increment; skip header on the first one
            outRom[context.workingOffset++] = 0xE9;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x00;
            // BCC doneLoop - underflow if it was originally zero sized (somehow? safety check)
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x1B;
            // TAY
            outRom[context.workingOffset++] = 0xA8;
            // CPY #0000
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // BEQ doneLoop - checked all the objects
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0x15;
            // LDA $0005,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x05;
            outRom[context.workingOffset++] = 0x00;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // CMP #0057 - boss values
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x57;
            outRom[context.workingOffset++] = 0x00;
            // BLT/BCC checkLoop 
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0xE8;
            // CMP #0080 - boss values
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x00;
            // BGE/BCS checkLoop 
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0xE3;
            // - here: error case; don't save
            // PLB
            outRom[context.workingOffset++] = 0xAB;
            // PLY
            outRom[context.workingOffset++] = 0x7A;
            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // doneLoop: - looped okay
            // PLB
            outRom[context.workingOffset++] = 0xAB;
            // PLY
            outRom[context.workingOffset++] = 0x7A;

            // PLX
            outRom[context.workingOffset++] = 0xFA;
            // PLA
            outRom[context.workingOffset++] = 0x68;

            // 8D 5A A2     STA $A25A[$7E:A25A]
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x5A;
            outRom[context.workingOffset++] = 0xA2;

            // 48           PHA
            outRom[context.workingOffset++] = 0x48;

            // E2 20        SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;

            // A9 03        LDA #03
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x03;

            // 8D 11 A2     STA $A211
            outRom[context.workingOffset++] = 0x8D;
            outRom[context.workingOffset++] = 0x11;
            outRom[context.workingOffset++] = 0xA2;

            // 22 D6 55 C7  JSL $C755D6 - save to slot 3
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0xD6;
            outRom[context.workingOffset++] = 0x55;
            outRom[context.workingOffset++] = 0xC7;

            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;

            // 8F FF 7F 30 STA $307FFF - set most recent slot 
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x7F;
            outRom[context.workingOffset++] = 0x30;

            // C2 20        REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;

            // 68           PLA
            outRom[context.workingOffset++] = 0x68;

            // 6B           RTL
            outRom[context.workingOffset++] = 0x6B;


            // set the name "Autosave" for the location of the save data
            string autosaveText = "Autosave";
            int autosaveSubrLocation = context.workingOffset;
            for (int i=0; i < autosaveText.Length; i++)
            {
                // LDA ##
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = VanillaEventUtil.getByte(autosaveText[i]);
                // 9F 00 9C 7E STA $7E9C00,x
                outRom[context.workingOffset++] = 0x9F;
                outRom[context.workingOffset++] = 0x00;
                outRom[context.workingOffset++] = 0x9C;
                outRom[context.workingOffset++] = 0x7E;
                // INX
                outRom[context.workingOffset++] = 0xE8;
            }

            // zero terminated
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // 9F 00 9C 7E STA $7E9C00,x
            outRom[context.workingOffset++] = 0x9F;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x9C;
            outRom[context.workingOffset++] = 0x7E;
            // INX
            outRom[context.workingOffset++] = 0xE8;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            int oldSubrLocation = context.workingOffset;

            // replace:
            /*
                $C7/5A64 8B          PHB                     A:001E X:0002 Y:001E P:envMxdIzc
                $C7/5A65 A9 CA       LDA #$CA                A:001E X:0002 Y:001E P:envMxdIzc
                $C7/5A67 48          PHA                     A:00CA X:0002 Y:001E P:eNvMxdIzc
                $C7/5A68 AB          PLB                     A:00CA X:0002 Y:001E P:eNvMxdIzc
                $C7/5A69 C2 20       REP #$20                A:00CA X:0002 Y:001E P:eNvMxdIzc
                $C7/5A6B B9 82 0B    LDA $0B82,y[$CA:0BA0]   A:00CA X:0002 Y:001E P:eNvmxdIzc
                $C7/5A6E A8          TAY                     A:B467 X:0002 Y:001E P:eNvmxdIzc
                $C7/5A6F E2 20       SEP #$20                A:B467 X:0002 Y:B467 P:eNvmxdIzc
                $C7/5A71 A9 80       LDA #$80                A:B467 X:0002 Y:B467 P:eNvMxdIzc
                $C7/5A73 9F 00 9C 7E STA $7E9C00,x[$7E:9C02] A:B480 X:0002 Y:B467 P:eNvMxdIzc
                $C7/5A77 E8          INX                     A:B480 X:0002 Y:B467 P:eNvMxdIzc
                $C7/5A78 B9 00 00    LDA $0000,y[$CA:B467]   A:B480 X:0003 Y:B467 P:envMxdIzc
                $C7/5A7B F0 08       BEQ $08    [$5A85]      A:B4A5 X:0003 Y:B467 P:eNvMxdIzc
                $C7/5A7D 9F 00 9C 7E STA $7E9C00,x[$7E:9C03] A:B4A5 X:0003 Y:B467 P:eNvMxdIzc
                $C7/5A81 E8          INX                     A:B4A5 X:0003 Y:B467 P:eNvMxdIzc
                $C7/5A82 C8          INY                     A:B4A5 X:0004 Y:B467 P:envMxdIzc
                $C7/5A83 80 F3       BRA $F3    [$5A78]      A:B4A5 X:0004 Y:B468 P:eNvMxdIzc
             */

            // straight copy the old one 75A64 -> 75A84
            for (int i= 0x75A64; i <= 0x75A85; i++)
            {
                outRom[context.workingOffset++] = outRom[i];
                outRom[i] = 0xEA;
            }
            outRom[context.workingOffset++] = 0x6B;

            // overwrite the original code block
            int off = 0x75A64;

            // new subr:
            // AD 62 A1    LDA $A162
            outRom[off++] = 0xAD;
            outRom[off++] = 0x62;
            outRom[off++] = 0xA1;
            // CMP #03 - only for autosave slot
            outRom[off++] = 0xC9;
            outRom[off++] = 0x03;
            // BNE 06
            outRom[off++] = 0xD0;
            outRom[off++] = 0x06;
            // JSL xx xx xx [autosave text]
            outRom[off++] = 0x22;
            outRom[off++] = (byte)(autosaveSubrLocation);
            outRom[off++] = (byte)(autosaveSubrLocation >> 8);
            outRom[off++] = (byte)((autosaveSubrLocation >> 16) + 0xC0);
            // BRA 04
            outRom[off++] = 0x80;
            outRom[off++] = 0x04;
            // JSL xx xx xx [normal text]
            outRom[off++] = 0x22;
            outRom[off++] = (byte)(oldSubrLocation);
            outRom[off++] = (byte)(oldSubrLocation >> 8);
            outRom[off++] = (byte)((oldSubrLocation >> 16) + 0xC0);

            // menu construction - remove slot [3] as an option to save directly to
            // 77818 - load offset [7e75]
            // 77812 - save offset [7e75]
            // at 7757e: 04 42 03 01 80 00 02 03 41 8E 00 04 15 41 0E 02 04 15 41 8E 03 04 15 41 0E 05 04 15 00
            // remove last 41 xx xx xx xx block for 3
            // put it at 74F80
            byte[] newSaveMenu = new byte[] { 0x04, 0x42, 0x03, 0x01, 0x80, 0x00, 0x02, 0x03, 0x41, 0x8E, 0x00, 0x04, 0x15, 0x41, 0x0E, 0x02, 0x04, 0x15, 0x41, 0x8E, 0x03, 0x04, 0x15, /*0x41, 0x0E, 0x05, 0x04, 0x15,*/ 0x00 };
            for(int i=0; i < newSaveMenu.Length; i++)
            {
                outRom[0x74F80 + i] = newSaveMenu[i];
            }

            // and point to it here
            outRom[0x77812] = 0x80;
            outRom[0x77813] = 0x4F;

            // when loading ..
            // $C7/55C7 22 21 00 C0 JSL $C00021[$C0:0021]   A:0000 X:0001 Y:0003 P:envMxdIzc
            outRom[0x755C7] = 0x22;
            outRom[0x755C8] = (byte)(context.workingOffset);
            outRom[0x755C9] = (byte)(context.workingOffset >> 8);
            outRom[0x755CA] = (byte)((context.workingOffset >> 16) + 0xC0);

            // add here: LDA #01
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x01;

            // STA RamOffsets.DONT_AUTOSAVE
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = (byte)CustomRamOffsets.DONT_AUTOSAVE;
            outRom[context.workingOffset++] = (byte)(CustomRamOffsets.DONT_AUTOSAVE >> 8);
            outRom[context.workingOffset++] = (byte)((CustomRamOffsets.DONT_AUTOSAVE >> 16));

            // replaced code
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = 0x21;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0xC0;

            // rtl
            outRom[context.workingOffset++] = 0x6B;



            // loading menu structure
            // if y is 7566, we're loading main menu, and we want $307FFF to be 03
            // if y is 4F80, we're loading save menu, and we want it to be 00
            // replace:
            // $C0/2456 8F 0A A2 7E STA $7EA20A[$7E:A20A]   A:2080 X:7818 Y:757E P:eNvMxdIzc
            // here: when loading a menu, if it's the save menu, make sure we don't select slot 4, if it's load, make sure we do

            outRom[0x2456] = 0x22;
            outRom[0x2457] = (byte)(context.workingOffset);
            outRom[0x2458] = (byte)(context.workingOffset >> 8);
            outRom[0x2459] = (byte)((context.workingOffset >> 16) + 0xC0);

            // replaced code
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0x0A;
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x7E;

            // CPY #7566
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0x66;
            outRom[context.workingOffset++] = 0x75;

            // BNE over
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x06;

            // LDA #03
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x03;
            // 8F FF 7F 30 STA $307FFF - set most recent slot 
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x7F;
            outRom[context.workingOffset++] = 0x30;

            // CPY #4F80
            outRom[context.workingOffset++] = 0xC0;
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x4F;

            // BNE over
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x0E;

            // LDA $307FFF
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x7F;
            outRom[context.workingOffset++] = 0x30;

            // CMP #03
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x03;

            // BNE over
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x06;

            // LDA #00
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x00;
            // 8F FF 7F 30 STA $307FFF - set most recent slot 
            outRom[context.workingOffset++] = 0x8F;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x7F;
            outRom[context.workingOffset++] = 0x30;

            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
