using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.openworld;
using SoMRandomizer.util;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Hack that swaps the stats and capabilities of a character to match one of the three vanilla characters, or a random one.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class CharacterClasses : RandoProcessor
    {
        protected override string getName()
        {
            return "Character roles/classes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string boyClass = context.workingData.get(OpenWorldClassSelection.BOY_CLASS);
            string girlClass = context.workingData.get(OpenWorldClassSelection.GIRL_CLASS);
            string spriteClass = context.workingData.get(OpenWorldClassSelection.SPRITE_CLASS);
            if(boyClass == "OGboy" && girlClass == "OGgirl" && spriteClass == "OGsprite")
            {
                // no changes; skip
                return false;
            }

            // MOPPLE: this hack should include changes to mana magic to allow casting it on a "boy" type character
            // holding the mana sword.  there is a ton of stuff hardcoded to look specifically for the boy that needs
            // to be changed for it to work.
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 800);

            // if i remember right, this is some kind of spell menu loader for the girl/sprite in vanilla,
            // and we modify it here to call the existing loaders based on which character got which role

            /*
                $D0/D4EF AD 08 18    LDA $1808  [$00:1808]   A:0000 X:D566 Y:0004 P:envmxdIZc - id of the character whose menu is open (0,1,2)
                $D0/D4F2 29 01 00    AND #$0001              A:0101 X:D566 Y:0004 P:envmxdIzc
                // replace:
                $D0/D4F5 F0 06       BEQ $06    [$D4FD]      A:0001 X:D566 Y:0004 P:envmxdIzc
                $D0/D4F7 22 B0 D0 D0 JSL $D0D0B0[$D0:D0B0]   A:0001 X:D566 Y:0004 P:envmxdIzc
             */
            outRom[0x10d4f5] = 0xEA;
            outRom[0x10d4f6] = 0xEA;
            outRom[0x10d4f7] = 0x22;
            outRom[0x10d4f8] = (byte)(context.workingOffset);
            outRom[0x10d4f9] = (byte)(context.workingOffset >> 8);
            outRom[0x10d4fa] = (byte)((context.workingOffset >> 16) + 0xC0);
            // lda $1808
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x18;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // BNE notBoy
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // (if boy is ogGirl, 22 D0D0B0, if boy is ogSprite, 22 D0D0DA)
            if (boyClass == "OGgirl")
            {
                // JSL $D0D0B0
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0xD0;
            }
            else if (boyClass == "OGsprite")
            {
                // JSL $D0D0DA
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = 0xDA;
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0xD0;
            }
            else
            {
                // nothing (same length for the branch)
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // notBoy:
            // cmp #0001 - check if girl
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x00;
            // BNE notGirl
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            if (girlClass == "OGgirl")
            {
                // JSL $D0D0B0
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0xD0;
            }
            else if (girlClass == "OGsprite")
            {
                // JSL $D0D0DA
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = 0xDA;
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0xD0;
            }
            else
            {
                // nothing (same length for the branch)
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
            }
            outRom[context.workingOffset++] = 0x6B;

            // notGirl:
            // assume sprite if not the other two.
            if (spriteClass == "OGgirl")
            {
                // JSL $D0D0B0
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = 0xB0;
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0xD0;
            }
            else if (spriteClass == "OGsprite")
            {
                // JSL $D0D0DA
                outRom[context.workingOffset++] = 0x22;
                outRom[context.workingOffset++] = 0xDA;
                outRom[context.workingOffset++] = 0xD0;
                outRom[context.workingOffset++] = 0xD0;
            }
            else
            {
                // nothing (same length for the branch)
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
                outRom[context.workingOffset++] = 0xEA;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // make subroutines to be called below to set up element menus for each character.
            // these will set carry if no spells (boy class) and clear it if spells available
            int boyElementMenuSub = context.workingOffset + 0xC00000;
            makeElementSub(outRom, context, 0, boyClass);
            int girlElementMenuSub = context.workingOffset + 0xC00000;
            makeElementSub(outRom, context, 1, girlClass);
            int spriteElementMenuSub = context.workingOffset + 0xC00000;
            makeElementSub(outRom, context, 2, spriteClass);

            /*
             *  replace:
                $D0/D128 AD 08 18    LDA $1808  [$00:1808]   A:0002 X:0002 Y:1880 P:envmxdIzc  - id of player whose menu is open (0,1,2)
                $D0/D12B 29 FF 00    AND #$00FF              A:0100 X:0002 Y:1880 P:envmxdIzc  
                $D0/D12E 09 00 00    ORA #$0000              A:0000 X:0002 Y:1880 P:envmxdIZc  

                followed by:
                $D0/D131 F0 F4       BEQ $F4    [$D127]      A:0000 X:0002 Y:1880 P:envmxdIZc  break out if boy - always skip spells - change to BCS (B0)
                ->
                $D0/D131 F0 F4       BCS $60
                forward to $D193 instead of back to $D127 based on carry instead of zero:
                $D0/D193 AD 74 18    LDA $1874  [$00:1874]   A:0000 X:0007 Y:188E P:envmxdIZC
                $D0/D196 AA          TAX                     A:0400 X:0007 Y:188E P:envmxdIzC
                $D0/D197 AD 08 18    LDA $1808  [$00:1808]   A:0400 X:0400 Y:188E P:envmxdIzC
                $D0/D19A 29 FF 00    AND #$00FF              A:0102 X:0400 Y:188E P:envmxdIzC
                ...
             */
            // 10d128-10d130
            outRom[0x10d128] = 0x22;
            outRom[0x10d129] = (byte)(context.workingOffset);
            outRom[0x10d12a] = (byte)(context.workingOffset >> 8);
            outRom[0x10d12b] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x10d12c] = 0xEA;
            outRom[0x10d12d] = 0xEA;
            outRom[0x10d12e] = 0xEA;
            outRom[0x10d12f] = 0xEA;
            outRom[0x10d130] = 0xEA;
            // change from BEQ to BCS so we have control over it from the new code above
            outRom[0x10d131] = 0xB0; 
            outRom[0x10d133] = 0x60;
            // LDA $1808
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x18;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // BNE 05  - skip over the JSL/return if not boy id
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // JSL boyElementMenuSub
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(boyElementMenuSub);
            outRom[context.workingOffset++] = (byte)(boyElementMenuSub >> 8);
            outRom[context.workingOffset++] = (byte)((boyElementMenuSub >> 16));
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // CMP #0001 - check for girl
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x00;
            // BNE 05 - skip over the JSL/return if not the girl id
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // JSL girlElementMenuSub
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(girlElementMenuSub);
            outRom[context.workingOffset++] = (byte)(girlElementMenuSub >> 8);
            outRom[context.workingOffset++] = (byte)((girlElementMenuSub >> 16));
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // by now it must be the sprite
            // JSL spriteElementMenuSub
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(spriteElementMenuSub);
            outRom[context.workingOffset++] = (byte)(spriteElementMenuSub >> 8);
            outRom[context.workingOffset++] = (byte)((spriteElementMenuSub >> 16));
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // make subroutines to be called below to set up spell menus for each character based on selected element.
            // these set "x" to the index where spells should start being loaded.
            int boySpellMenuSub = context.workingOffset + 0xC00000;
            makeSpellLoadSub(outRom, context, 0, boyClass);
            int girlSpellMenuSub = context.workingOffset + 0xC00000;
            makeSpellLoadSub(outRom, context, 1, girlClass);
            int spriteSpellMenuSub = context.workingOffset + 0xC00000;
            makeSpellLoadSub(outRom, context, 2, spriteClass);

            /*
             *  replace:
                $D0/D05F AD 08 18    LDA $1808  [$00:1808]   A:0001 X:0000 Y:1880 P:envmxdIzc - id of player with menu open (0,1,2)
                $D0/D062 29 FF 00    AND #$00FF              A:0102 X:0000 Y:1880 P:envmxdIzc 
                $D0/D065 3A          DEC A                   A:0002 X:0000 Y:1880 P:envmxdIzc - subtract 1, since vanilla assumes boy can never open this menu
                $D0/D066 D0 06       BNE $06    [$D06E]      A:0001 X:0000 Y:1880 P:envmxdIzc - if not 0, it's the sprite's menu; skip the following code
                $D0/D068 8A          TXA                     A:0000 X:0000 Y:1880 P:envmxdIZc - girl only section - add 6 to x index value
                $D0/D069 18          CLC                     A:0000 X:0000 Y:1880 P:envmxdIZc 
                $D0/D06A 69 06 00    ADC #$0006              A:0000 X:0000 Y:1880 P:envmxdIZc 
                $D0/D06D AA          TAX                     A:0006 X:0000 Y:1880 P:envmxdIzc 
                instead of this, we just do the clc/adc 6 if the character's class is ogGirl (see makeSpellLoadSub above)
             */
            // 10d05f-10d06d
            outRom[0x10d05f] = 0x22;
            outRom[0x10d060] = (byte)(context.workingOffset);
            outRom[0x10d061] = (byte)(context.workingOffset >> 8);
            outRom[0x10d062] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x10d063] = 0xEA;
            outRom[0x10d064] = 0xEA;
            outRom[0x10d065] = 0xEA;
            outRom[0x10d066] = 0xEA;
            outRom[0x10d067] = 0xEA;
            outRom[0x10d068] = 0xEA;
            outRom[0x10d069] = 0xEA;
            outRom[0x10d06a] = 0xEA;
            outRom[0x10d06b] = 0xEA;
            outRom[0x10d06c] = 0xEA;
            outRom[0x10d06d] = 0xEA;
            // LDA $1808 - player id with menu open
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x18;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // BNE over the boy section
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // JSL boySpellMenuSub
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(boySpellMenuSub);
            outRom[context.workingOffset++] = (byte)(boySpellMenuSub >> 8);
            outRom[context.workingOffset++] = (byte)((boySpellMenuSub >> 16));
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // CMP #0001
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x00;
            // BNE over the girl section
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // JSL girlSpellMenuSub
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(girlSpellMenuSub);
            outRom[context.workingOffset++] = (byte)(girlSpellMenuSub >> 8);
            outRom[context.workingOffset++] = (byte)((girlSpellMenuSub >> 16));
            outRom[context.workingOffset++] = 0x6B;
            // assume sprite
            // JSL spriteSpellMenuSub
            outRom[context.workingOffset++] = 0x22;
            outRom[context.workingOffset++] = (byte)(spriteSpellMenuSub);
            outRom[context.workingOffset++] = (byte)(spriteSpellMenuSub >> 8);
            outRom[context.workingOffset++] = (byte)((spriteSpellMenuSub >> 16));
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            

            /*
             *  replace:
                $D0/CE61 AD 08 18    LDA $1808  [$00:1808]   A:0080 X:D566 Y:0003 P:envMxdIzc **
                $D0/CE64 29 01       AND #$01                A:0002 X:D566 Y:0003 P:envMxdIzc ** replace with subroutine that sets carry based on girl/sprite spells, and change the BEQ to a BCS
                $D0/CE66 F0 2A       BEQ $2A    [$CE92]      A:0000 X:D566 Y:0003 P:envMxdIZc -> sprite - change to BCS
             */
            outRom[0x10ce61] = 0x22;
            outRom[0x10ce62] = (byte)(context.workingOffset);
            outRom[0x10ce63] = (byte)(context.workingOffset >> 8);
            outRom[0x10ce64] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x10ce65] = 0xEA;

            outRom[0x10ce66] = 0xB0;
            // LDA $1808 - id of character with menu open (0,1,2)
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x18;
            // BNE to girl section
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x02;
            if (boyClass == "OGgirl")
            {
                // CLC - indicate to outer code to process girl spells
                outRom[context.workingOffset++] = 0x18;
            }
            else
            {
                // SEC - indicate to outer code to process sprite spells
                outRom[context.workingOffset++] = 0x38;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // CMP #01 - check if girl menu
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            // BNE to sprite section
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x02;
            if (girlClass == "OGgirl")
            {
                // CLC - indicate to outer code to process girl spells
                outRom[context.workingOffset++] = 0x18;
            }
            else
            {
                // SEC - indicate to outer code to process sprite spells
                outRom[context.workingOffset++] = 0x38;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // default to sprite menu
            if (spriteClass == "OGgirl")
            {
                // CLC - indicate to outer code to process girl spells
                outRom[context.workingOffset++] = 0x18;
            }
            else
            {
                // SEC - indicate to outer code to process sprite spells
                outRom[context.workingOffset++] = 0x38;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            /*
             * similar to the above. replace:
                $C0/6B82 AD 08 18    LDA $1808  [$00:1808]   A:1800 X:1876 Y:0054 P:envMxdIZc **
                $C0/6B85 C9 01       CMP #$01                A:1801 X:1876 Y:0054 P:envMxdIzc ** change to new loader that sets carry for sprite spell loading
                $C0/6B87 D0 21       BNE $21    [$6BAA]      A:1801 X:1876 Y:0054 P:envMxdIZC ** change to BCS for sprite
             */
            outRom[0x6b82] = 0x22;
            outRom[0x6b83] = (byte)(context.workingOffset);
            outRom[0x6b84] = (byte)(context.workingOffset >> 8);
            outRom[0x6b85] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x6b86] = 0xEA;

            outRom[0x6b87] = 0xB0;
            // LDA $1808
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x18;
            // BNE to girl section
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x02;
            if (boyClass == "OGgirl")
            {
                // CLC - indicate to outer code to process girl spells
                outRom[context.workingOffset++] = 0x18;
            }
            else
            {
                // SEC - indicate to outer code to process sprite spells
                outRom[context.workingOffset++] = 0x38;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // CMP #01 - check if girl menu
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            // BNE to sprite section
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x02;
            if (girlClass == "OGgirl")
            {
                // CLC - indicate to outer code to process girl spells
                outRom[context.workingOffset++] = 0x18;
            }
            else
            {
                // SEC - indicate to outer code to process sprite spells
                outRom[context.workingOffset++] = 0x38;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // default to sprite menu
            if (spriteClass == "OGgirl")
            {
                // CLC - indicate to outer code to process girl spells
                outRom[context.workingOffset++] = 0x18;
            }
            else
            {
                // SEC - indicate to outer code to process sprite spells
                outRom[context.workingOffset++] = 0x38;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // fix issue with stuff being grayed out
            /*
             * replace, similarly:
                $D0/D031 AD 08 18    LDA $1808  [$00:1808]   A:0018 X:0004 Y:1880 P:envmxdIzc **
                $D0/D034 89 01 00    BIT #$0001              A:0100 X:0004 Y:1880 P:envmxdIzc ** again, set carry for sprite spells
                $D0/D037 F0 07       BEQ $07    [$D040]      A:0100 X:0004 Y:1880 P:envmxdIZc -- change to BCS
             */
            outRom[0x10d031] = 0x22;
            outRom[0x10d032] = (byte)(context.workingOffset);
            outRom[0x10d033] = (byte)(context.workingOffset >> 8);
            outRom[0x10d034] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x10d035] = 0xEA;
            outRom[0x10d036] = 0xEA;
            outRom[0x10d037] = 0xB0;
            // LDA $1808
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x18;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // BNE to girl section
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x02;
            if (boyClass == "OGgirl")
            {
                // CLC - indicate to outer code to process girl spells
                outRom[context.workingOffset++] = 0x18;
            }
            else
            {
                // SEC - indicate to outer code to process sprite spells
                outRom[context.workingOffset++] = 0x38;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // CMP #0001 - check if girl menu
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x00;
            // BNE to sprite section
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x02;
            if (girlClass == "OGgirl")
            {
                // CLC - indicate to outer code to process girl spells
                outRom[context.workingOffset++] = 0x18;
            }
            else
            {
                // SEC - indicate to outer code to process sprite spells
                outRom[context.workingOffset++] = 0x38;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // default to sprite menu
            if (spriteClass == "OGgirl")
            {
                // CLC - indicate to outer code to process girl spells
                outRom[context.workingOffset++] = 0x18;
            }
            else
            {
                // SEC - indicate to outer code to process sprite spells
                outRom[context.workingOffset++] = 0x38;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            
            /*
             * more loaders like the above.  replace:
                $D0/CFFA AD 08 18    LDA $1808  [$00:1808]   A:0080 X:0020 Y:1880 P:envmxdIzC **
                $D0/CFFD 29 FF 00    AND #$00FF              A:0100 X:0020 Y:1880 P:envmxdIzC **
                $D0/D000 3A          DEC A                   A:0000 X:0020 Y:1880 P:envmxdIZC **
                $D0/D001 D0 11       BNE $11    [$D014]      A:FFFF X:0020 Y:1880 P:eNvmxdIzC ** change to BCS for sprite
                $D0/D014 22 DA D0 D0 JSL $D0D0DA[$D0:D0DA]   A:FFFF X:0020 Y:1880 P:eNvmxdIzC
             */
            outRom[0x10cffa] = 0x22;
            outRom[0x10cffb] = (byte)(context.workingOffset);
            outRom[0x10cffc] = (byte)(context.workingOffset >> 8);
            outRom[0x10cffd] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x10cffe] = 0xEA;
            outRom[0x10cfff] = 0xEA;
            outRom[0x10d000] = 0xEA;

            outRom[0x10d001] = 0xB0;
            // LDA $1808
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x18;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // BNE to girl section
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x02;
            if (boyClass == "OGgirl")
            {
                // CLC - indicate to outer code to process girl spells
                outRom[context.workingOffset++] = 0x18;
            }
            else
            {
                // SEC - indicate to outer code to process sprite spells
                outRom[context.workingOffset++] = 0x38;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // CMP #0001 - check if girl menu
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x00;
            // BNE to sprite section
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x02;
            if (girlClass == "OGgirl")
            {
                // CLC - indicate to outer code to process girl spells
                outRom[context.workingOffset++] = 0x18;
            }
            else
            {
                // SEC - indicate to outer code to process sprite spells
                outRom[context.workingOffset++] = 0x38;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // default to sprite menu
            if (spriteClass == "OGgirl")
            {
                // CLC - indicate to outer code to process girl spells
                outRom[context.workingOffset++] = 0x18;
            }
            else
            {
                // SEC - indicate to outer code to process sprite spells
                outRom[context.workingOffset++] = 0x38;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // $D0D0B0 - girl spell loader; need to have it use 7ex instead of hardcoding to 7e3 
            /*
             * replace:
                $D0/D0B0 C2 10       REP #$10                A:0000 X:D566 Y:0001 P:envMxdIZc
                $D0/D0B2 E2 20       SEP #$20                A:0000 X:D566 Y:0001 P:envMxdIZc
                $D0/D0B4 A2 00 00    LDX #$0000              A:0000 X:D566 Y:0001 P:envMxdIZc
                $D0/D0B7 AD 74 18    LDA $1874  [$00:1874]   A:0000 X:0000 Y:0001 P:envMxdIZc
                $D0/D0BA 1A          INC A                   A:0000 X:0000 Y:0001 P:envMxdIZc
                $D0/D0BB C9 07       CMP #$07                A:0001 X:0000 Y:0001 P:envMxdIzc
                $D0/D0BD 90 02       BCC $02    [$D0C1]      A:0001 X:0000 Y:0001 P:eNvMxdIzc
                $D0/D0BF A9 07       LDA #07   
                $D0/D0C1 48          PHA                     A:0001 X:0000 Y:0001 P:eNvMxdIzc
                $D0/D0C2 BF C8 E3 7E LDA $7EE3C8,x[$7E:E3C8] A:0001 X:0000 Y:0001 P:eNvMxdIzc
                $D0/D0C6 E8          INX                     A:0038 X:0000 Y:0001 P:envMxdIzc
                $D0/D0C7 29 38       AND #$38                A:0038 X:0001 Y:0001 P:envMxdIzc
                $D0/D0C9 F0 F7       BEQ $F7    [$D0C2]      A:0038 X:0001 Y:0001 P:envMxdIzc
                $D0/D0CB 68          PLA                     A:0038 X:0001 Y:0001 P:envMxdIzc
                $D0/D0CC 3A          DEC A                   A:0001 X:0001 Y:0001 P:envMxdIzc
                $D0/D0CD D0 F2       BNE $F2    [$D0C1]      A:0000 X:0001 Y:0001 P:envMxdIZc
                $D0/D0CF CA          DEX                     A:0000 X:0001 Y:0001 P:envMxdIZc
                $D0/D0D0 BF C8 E3 7E LDA $7EE3C8,x[$7E:E3C8] A:0000 X:0000 Y:0001 P:envMxdIZc
                $D0/D0D4 4A          LSR A                   A:0038 X:0000 Y:0001 P:envMxdIzc
                $D0/D0D5 4A          LSR A                   A:001C X:0000 Y:0001 P:envMxdIzc
                $D0/D0D6 4A          LSR A                   A:000E X:0000 Y:0001 P:envMxdIzc
                $D0/D0D7 C2 20       REP #$20                A:0007 X:0000 Y:0001 P:envMxdIzc
                $D0/D0D9 6B          RTL                     A:0007 X:0000 Y:0001 P:envmxdIzc
             */
            outRom[0x10d0b0] = 0x22;
            outRom[0x10d0b1] = (byte)(context.workingOffset);
            outRom[0x10d0b2] = (byte)(context.workingOffset >> 8);
            outRom[0x10d0b3] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x10d0b4] = 0x6B;
            for (int i = 0x10d0b5; i <= 0x10d0d9; i++)
            {
                outRom[i] = 0xEA;
            }
            // REP 10 (vanilla)
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x10;
            // SEP 20 (vanilla)
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $1808
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x18;
            // BNE girlMenu
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // LDX #0000 - boy index
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // BRA next
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x0C;
            // girlMenu:
            // CMP #01
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            // BNE spriteMenu
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // LDX #0200 - girl index
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x02;
            // BRA next
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x03;
            // spriteMenu:
            // LDX #0400 - sprite index
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x04;
            // next:
            // LDA $1874 (vanilla) - girl spells selected element
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x74;
            outRom[context.workingOffset++] = 0x18;
            // INC A (vanilla)
            outRom[context.workingOffset++] = 0x1A;
            // CMP #07 (vanilla)
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x07;
            // BCC 02 (vanilla)
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x02;
            // LDA #07 (vanilla)
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x07;
            // PHA (vanilla)
            outRom[context.workingOffset++] = 0x48;
            // LDA 7EE1C8,x - vanilla but use the x from earlier for which character's menu is open
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // INX (vanilla)
            outRom[context.workingOffset++] = 0xE8;
            // AND #38 (vanilla)
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x38;
            // BEQ $F7 (vanilla)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0xF7;
            // PLA (vanilla)
            outRom[context.workingOffset++] = 0x68;
            // DEC A (vanilla)
            outRom[context.workingOffset++] = 0x3A;
            // BNE $F2 (vanilla)
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0xF2;
            // DEX (vanilla)
            outRom[context.workingOffset++] = 0xCA;
            // LDA $7EE1C8,x - vanilla but use the x from earlier for which character's menu is open
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // LSR (vanilla)
            outRom[context.workingOffset++] = 0x4A;
            // LSR (vanilla)
            outRom[context.workingOffset++] = 0x4A;
            // LSR (vanilla)
            outRom[context.workingOffset++] = 0x4A;
            // REP 20 (vanilla)
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PHA
            outRom[context.workingOffset++] = 0x48;
            // TXA
            outRom[context.workingOffset++] = 0x8A;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            

            // D0D0DA - basically the same as the above but for the sprite spell loader.
            /*
             *  replace:
                $D0/D0DA C2 10       REP #$10                A:FFFF X:0020 Y:1880 P:eNvmxdIzC
                $D0/D0DC E2 20       SEP #$20                A:FFFF X:0020 Y:1880 P:eNvmxdIzC
                $D0/D0DE A2 00 00    LDX #$0000              A:FFFF X:0020 Y:1880 P:eNvMxdIzC
                $D0/D0E1 AD 75 18    LDA $1875  [$00:1875]   A:FFFF X:0000 Y:1880 P:envMxdIZC
                $D0/D0E4 1A          INC A                   A:FF00 X:0000 Y:1880 P:envMxdIZC
                $D0/D0E5 48          PHA                     A:FF01 X:0000 Y:1880 P:envMxdIzC
                $D0/D0E6 BF C8 E5 7E LDA $7EE5C8,x[$7E:E5C8] A:FF01 X:0000 Y:1880 P:envMxdIzC
                $D0/D0EA E8          INX                     A:FF00 X:0000 Y:1880 P:envMxdIZC
                $D0/D0EB 29 07       AND #$07                A:FF00 X:0001 Y:1880 P:envMxdIzC
                $D0/D0ED F0 F7       BEQ $F7    [$D0E6]      A:FF00 X:0001 Y:1880 P:envMxdIZC
                $D0/D0EF 68          PLA                     A:FF02 X:001F Y:1880 P:envMxdIzC
                $D0/D0F0 3A          DEC A                   A:FF01 X:001F Y:1880 P:envMxdIzC
                $D0/D0F1 D0 F2       BNE $F2    [$D0E5]      A:FF00 X:001F Y:1880 P:envMxdIZC
                $D0/D0F3 CA          DEX                     A:FF00 X:001F Y:1880 P:envMxdIZC
                $D0/D0F4 BF C8 E5 7E LDA $7EE5C8,x[$7E:E5E6] A:FF00 X:001E Y:1880 P:envMxdIzC
                $D0/D0F8 C2 20       REP #$20                A:FF02 X:001E Y:1880 P:envMxdIzC
                $D0/D0FA 6B          RTL                     A:FF02 X:001E Y:1880 P:envmxdIzC
             */
            for (int i = 0x10d0bf; i <= 0x10d0fa; i++)
            {
                outRom[i] = 0xEA;
            }
            outRom[0x10d0da] = 0x22;
            outRom[0x10d0db] = (byte)(context.workingOffset);
            outRom[0x10d0dc] = (byte)(context.workingOffset >> 8);
            outRom[0x10d0dd] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x10d0de] = 0x6B;
            // REP 10
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x10;
            // SEP 20
            outRom[context.workingOffset++] = 0xE2;
            outRom[context.workingOffset++] = 0x20;
            // LDA $1808
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x18;
            // BNE girlMenu
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // LDX #0000
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x00;
            // BRA next
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x0C;
            // girlMenu:
            // CMP #01
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            // BNE spriteMenu
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // LDX #0200
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x02;
            // BRA next
            outRom[context.workingOffset++] = 0x80;
            outRom[context.workingOffset++] = 0x03;
            // spriteMenu:
            // LDX #0400
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x04;
            // next:
            // LDA $1875 (vanilla) - sprite spells selected element
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x75;
            outRom[context.workingOffset++] = 0x18;
            // INC A (vanilla)
            outRom[context.workingOffset++] = 0x1A;
            // PHA (vanilla)
            outRom[context.workingOffset++] = 0x48;
            // LDA 7EE1C8,x - vanilla but use the x from earlier for which character's menu is open
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // INX (vanilla)
            outRom[context.workingOffset++] = 0xE8;
            // AND #07 (vanilla)
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0x07;
            // BEQ $F7 (vanilla)
            outRom[context.workingOffset++] = 0xF0;
            outRom[context.workingOffset++] = 0xF7;
            // PLA (vanilla)
            outRom[context.workingOffset++] = 0x68;
            // DEC A (vanilla)
            outRom[context.workingOffset++] = 0x3A;
            // BNE $F2 (vanilla)
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0xF2;
            // DEX (vanilla)
            outRom[context.workingOffset++] = 0xCA;
            // LDA 7EE1C8,x - vanilla but use the x from earlier for which character's menu is open
            outRom[context.workingOffset++] = 0xBF;
            outRom[context.workingOffset++] = 0xC8;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // REP 20
            outRom[context.workingOffset++] = 0xC2;
            outRom[context.workingOffset++] = 0x20;
            // PHA  (vanilla)
            outRom[context.workingOffset++] = 0x48;
            // TXA
            outRom[context.workingOffset++] = 0x8A;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // TAX
            outRom[context.workingOffset++] = 0xAA;
            // PLA
            outRom[context.workingOffset++] = 0x68;
            // RTL
            outRom[context.workingOffset++] = 0x6B;


            // subrs to load current and max mp using $1808 to indicate which character we want
            int currentMpLoaderOffset = context.workingOffset + 0xC00000;
            // LDA $1808
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x18;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // BNE notBoy (5)
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // LDA $7EE186
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x86;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // CMP #0001
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x00;
            // BNE notGirl (5)
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // LDA $7EE386
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x86;
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x7E;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // notGirl:
            // LDA $7EE586
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x86;
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0x7E;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            int maxMpLoaderOffset = context.workingOffset + 0xC00000;
            // LDA $1808
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x18;
            // AND #00FF
            outRom[context.workingOffset++] = 0x29;
            outRom[context.workingOffset++] = 0xFF;
            outRom[context.workingOffset++] = 0x00;
            // BNE notBoy (5)
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // LDA $7EE186
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x87;
            outRom[context.workingOffset++] = 0xE1;
            outRom[context.workingOffset++] = 0x7E;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // CMP #0001
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            outRom[context.workingOffset++] = 0x00;
            // BNE notGirl (5)
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x05;
            // LDA $7EE386
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x87;
            outRom[context.workingOffset++] = 0xE3;
            outRom[context.workingOffset++] = 0x7E;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // notGirl:
            // LDA $7EE586
            outRom[context.workingOffset++] = 0xAF;
            outRom[context.workingOffset++] = 0x87;
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0x7E;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            /*
            //    $D0/D69C AD 08 18    LDA $1808  [$00:1808]   A:AAA7 X:19DA Y:0003 P:envmxdIzc -- remove
            //    $D0/D69F 29 01 00    AND #$0001              A:0100 X:19DA Y:0003 P:envmxdIzc -- remove
            //    $D0/D6A2 F0 0D       BEQ $0D    [$D6B1]      A:0000 X:19DA Y:0003 P:envmxdIZc -- replace with BRA always
            //    (girl mp block)
            //    $D0/D6A4 AF 86 E3 7E LDA $7EE386[$7E:E386]   A:0000 X:19D8 Y:0004 P:envmxdIZc -- (not called anymore)
            //    $D0/D6A8 20 87 D7    JSR $D787  [$D0:D787]   A:0808 X:19DA Y:0003 P:envmxdIzc -- (not called anymore)
            //    $D0/D6AB AF 87 E3 7E LDA $7EE387[$7E:E387]   A:00C1 X:19DB Y:0000 P:envmxdIzC -- (not called anymore)
            //    $D0/D6AF 80 0B       BRA $0B (D6BC)                                           -- (not called anymore)
            //
            //    (sprite mp block) -- everyone's mp block now
            //    $D0/D6B1 AF 86 E5 7E LDA $7EE586[$7E:E586]   A:0000 X:19D8 Y:0004 P:envmxdIZc -- replace with current MP loader based on $1808
            //    $D0/D6B5 20 87 D7    JSR $D787  [$D0:D787]   A:0808 X:19DA Y:0003 P:envmxdIzc -- keep
            //    ...
            //    $D0/D6B8 AF 87 E5 7E LDA $7EE587[$7E:E587]   A:00C1 X:19DB Y:0000 P:envmxdIzC -- replace with max MP loader based on $1808
            //    $D0/D6BC 20 A9 D7    JSR $D7A9  [$D0:D7A9]   A:0808 X:19DD Y:0008 P:envmxdIzC -- keep
            //    ...
            //    $D0/D6BF 60          RTS                     A:0000 X:19E0 Y:0008 P:envmxdIzc -- keep
            */
            outRom[0x10d69c] = 0xEA;
            outRom[0x10d69d] = 0xEA;
            outRom[0x10d69e] = 0xEA;

            outRom[0x10d69f] = 0xEA;
            outRom[0x10d6a0] = 0xEA;
            outRom[0x10d6a1] = 0xEA;

            outRom[0x10d6a2] = 0x80;

            outRom[0x10d6b1] = 0x22;
            outRom[0x10d6b2] = (byte)(currentMpLoaderOffset);
            outRom[0x10d6b3] = (byte)(currentMpLoaderOffset >> 8);
            outRom[0x10d6b4] = (byte)(currentMpLoaderOffset >> 16);

            outRom[0x10d6b8] = 0x22;
            outRom[0x10d6b9] = (byte)(maxMpLoaderOffset);
            outRom[0x10d6ba] = (byte)(maxMpLoaderOffset >> 8);
            outRom[0x10d6bb] = (byte)(maxMpLoaderOffset >> 16);

            // include boy spells menu when transitioning menus downward
            /*
                $C0/6F47 AD 08 18    LDA $1808  [$00:1808]   A:0001 X:0001 Y:0004 P:eNvMXdIzc
                $C0/6F4A D0 0B       BNE $0B    [$6F57]      A:0000 X:0001 Y:0004 P:envMXdIZc -> branch always
             */
            outRom[0x6f4a] = 0x80;

            /*
             * replace:
                $C0/73C2 A2 86 01    LDX #$0186              A:E000 X:E060 Y:1880 P:envMxdIZC **
                $C0/73C5 AD 08 18    LDA $1808  [$00:1808]   A:E000 X:0186 Y:1880 P:envMxdIzC **
                $C0/73C8 29 01       AND #$01                A:E000 X:0186 Y:1880 P:envMxdIZC **
                $C0/73CA D0 03       BNE $03    [$73CF]      A:E000 X:0186 Y:1880 P:envMxdIZC **
                $C0/73CC A2 86 03    LDX #$0386              A:E000 X:0186 Y:1880 P:envMxdIZC **
                with a dynamic loader based on $1808
                $C0/73CF BF 00 E2 7E LDA $7EE200,x[$7E:E586] A:E000 X:0386 Y:1880 P:envMxdIzC --> change to 7EE000 since x will be 186/386/586
             */
            outRom[0x73C2] = 0x22;
            outRom[0x73C3] = (byte)(context.workingOffset);
            outRom[0x73C4] = (byte)(context.workingOffset >> 8);
            outRom[0x73C5] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x73C6] = 0xEA;
            outRom[0x73C7] = 0xEA;
            outRom[0x73C8] = 0xEA;
            outRom[0x73C9] = 0xEA;
            outRom[0x73CA] = 0xEA;
            outRom[0x73CB] = 0xEA;
            outRom[0x73CC] = 0xEA;
            outRom[0x73CD] = 0xEA;
            outRom[0x73CE] = 0xEA;
            outRom[0x73D1] = 0xE0;
            // LDA $1808
            outRom[context.workingOffset++] = 0xAD;
            outRom[context.workingOffset++] = 0x08;
            outRom[context.workingOffset++] = 0x18;
            // BNE notBoy
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x04;
            // LDX #0186
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x86;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // notBoy:
            // CMP #01
            outRom[context.workingOffset++] = 0xC9;
            outRom[context.workingOffset++] = 0x01;
            // BNE notGirl
            outRom[context.workingOffset++] = 0xD0;
            outRom[context.workingOffset++] = 0x04;
            // LDX #0386
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x86;
            outRom[context.workingOffset++] = 0x03;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // LDX #0586
            outRom[context.workingOffset++] = 0xA2;
            outRom[context.workingOffset++] = 0x86;
            outRom[context.workingOffset++] = 0x05;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // move stats around as needed
            // 104210 - 104528 = boy
            // 104528 - 104840 = girl
            // 104840 - 104B58 = sprite
            // 8 bytes per level
            if (boyClass != "OGboy")
            {
                int offset = 0;
                if (boyClass == "OGgirl")
                {
                    offset = 792;
                }
                if (boyClass == "OGsprite")
                {
                    offset = 1584;
                }
                for (int i = 0; i < 99 * 8; i++)
                {
                    outRom[0x104210 + i] = origRom[0x104210 + i + offset];
                }
            }
            if (girlClass != "OGgirl")
            {
                int offset = 0;
                if (girlClass == "OGboy")
                {
                    offset = -792;
                }
                if (girlClass == "OGsprite")
                {
                    offset = 792;
                }
                for (int i = 0; i < 99 * 8; i++)
                {
                    outRom[0x104528 + i] = origRom[0x104528 + i + offset];
                }
            }
            if (spriteClass != "OGsprite")
            {
                int offset = 0;
                if (spriteClass == "OGboy")
                {
                    offset = -1584;
                }
                if (spriteClass == "OGgirl")
                {
                    offset = -792;
                }
                for (int i = 0; i < 99 * 8; i++)
                {
                    outRom[0x104840 + i] = origRom[0x104840 + i + offset];
                }
            }

            // spells menu stuff
            /*
            $C0/73E5 AD 08 18    LDA $1808  [$00:1808]   A:0000 X:19FD Y:0004 P:envMxdIZc
            $C0/73E8 95 01       STA $01,x  [$00:19FE]   A:0001 X:19FD Y:0004 P:envMxdIzc
            ..
            $C0/1DA5 AF FE 19 00 LDA $0019FE[$00:19FE]   A:0002 X:0122 Y:0000 P:envMxdIZc
            $C0/1DA9 8F 1F A2 7E STA $7EA21F[$7E:A21F]   A:0001 X:0122 Y:0000 P:envMxdIzc
            ..
            $C0/298C AF FE 19 00 LDA $0019FE[$00:19FE]   A:0009 X:03E0 Y:7649 P:envMxdIZC **
            $C0/2990 D0 12       BNE $12    [$29A4]      A:0001 X:03E0 Y:7649 P:envMxdIzC ** always branch here
            ..
            for hitting R in the weapon/spell menu:
            $C0/2D36 AF FE 19 00 LDA $0019FE[$00:19FE]   A:0054 X:0054 Y:0004 P:envMxdIzc
            $C0/2D3A F0 13       BEQ $13    [$2D4F]      A:0000 X:0054 Y:0004 P:envMxdIZc ** never branch
            */
            outRom[0x2990] = 0x80;
            outRom[0x2d3a] = 0xEA;
            outRom[0x2d3b] = 0xEA;


            // armor wearable swap
            // armor[5] 0x80 = boy wearable, 0x40 = girl, 0x20 = sprite
            // 103ED0, 10 byte blocks
            // don't change wristband because sprite wearable on it fucks up menus; it ends up with the same id as the trashcan and the game gets confused
            // 0 - 61, wristband is 62
            for(int i=0; i <= 61; i++)
            {
                bool boyWearable = (origRom[0x103ED0 + i * 10 + 5] & 0x80) > 0;
                bool girlWearable = (origRom[0x103ED0 + i * 10 + 5] & 0x40) > 0;
                bool spriteWearable = (origRom[0x103ED0 + i * 10 + 5] & 0x20) > 0;

                bool boyWearableNew = boyClass == "OGboy" ? boyWearable : boyClass == "OGgirl" ? girlWearable : spriteWearable;
                bool girlWearableNew = girlClass == "OGboy" ? boyWearable : girlClass == "OGgirl" ? girlWearable : spriteWearable;
                bool spriteWearableNew = spriteClass == "OGboy" ? boyWearable : spriteClass == "OGgirl" ? girlWearable : spriteWearable;

                outRom[0x103ED0 + i * 10 + 5] = (byte)(outRom[0x103ED0 + i * 10 + 5] & 0x1F);
                if (boyWearableNew)
                {
                    outRom[0x103ED0 + i * 10 + 5] |= 0x80;
                }
                if (girlWearableNew)
                {
                    outRom[0x103ED0 + i * 10 + 5] |= 0x40;
                }
                if (spriteWearableNew)
                {
                    outRom[0x103ED0 + i * 10 + 5] |= 0x20;
                }
            }
            return true;
        }

        private void makeElementSub(byte[] outRom, RandoContext context, int charId, string charClass)
        {
            // menu element icons loader
            if(charClass == "OGboy")
            {
                // no spells to load for boy class
                // SEC
                outRom[context.workingOffset++] = 0x38;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
            }
            else
            {
                // LDX #0000
                outRom[context.workingOffset++] = 0xA2;
                outRom[context.workingOffset++] = 0x00;
                outRom[context.workingOffset++] = 0x00;
                // LDA #0007
                outRom[context.workingOffset++] = 0xA9;
                outRom[context.workingOffset++] = 0x07;
                outRom[context.workingOffset++] = 0x00;
                // loopJumpBack
                int loopJumpBackDest = context.workingOffset;
                // PHA - push number of iters remaining
                outRom[context.workingOffset++] = 0x48;
                // SEP 20
                outRom[context.workingOffset++] = 0xE2;
                outRom[context.workingOffset++] = 0x20;
                // LDA 7EExC8,x - 1 for boy, 3 for girl, 5 for sprite - spells available per element
                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = 0xC8;
                outRom[context.workingOffset++] = (byte)(0xE1 + charId * 2);
                outRom[context.workingOffset++] = 0x7E;

                if(charClass == "OGgirl")
                {
                    // CPX #0006
                    outRom[context.workingOffset++] = 0xE0;
                    outRom[context.workingOffset++] = 0x06;
                    outRom[context.workingOffset++] = 0x00;
                    // BNE notLumina
                    outRom[context.workingOffset++] = 0xD0;
                    outRom[context.workingOffset++] = 0x04;
                    // LDA 7EExC9,x - handle lumina being offset by 1 byte
                    outRom[context.workingOffset++] = 0xBF;
                    outRom[context.workingOffset++] = 0xC9;
                    outRom[context.workingOffset++] = (byte)(0xE1 + charId * 2);
                    outRom[context.workingOffset++] = 0x7E;
                }
                // INX - increment element id
                outRom[context.workingOffset++] = 0xE8;
                // REP 20
                outRom[context.workingOffset++] = 0xC2;
                outRom[context.workingOffset++] = 0x20;
                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
                // BEQ endLoop - don't have this element
                outRom[context.workingOffset++] = 0xF0;
                int jumpSkipElementSource = context.workingOffset;
                outRom[context.workingOffset++] = 0x00; // - fill in later
                // PHX - push next element index
                outRom[context.workingOffset++] = 0xDA;
                // TXA
                outRom[context.workingOffset++] = 0x8A;
                // DEC A
                outRom[context.workingOffset++] = 0x3A;
                // PHA - push current element id
                outRom[context.workingOffset++] = 0x48;
                // ASL
                outRom[context.workingOffset++] = 0x0A;
                // CLC
                outRom[context.workingOffset++] = 0x18;
                // ADC $1828 - add spell icon/palette lookup index
                outRom[context.workingOffset++] = 0x6D;
                outRom[context.workingOffset++] = 0x28;
                outRom[context.workingOffset++] = 0x18;
                // TAX
                outRom[context.workingOffset++] = 0xAA;

                if (charClass == "OGgirl")
                {
                    // LDA $01,s - load current element id off stack
                    outRom[context.workingOffset++] = 0xA3;
                    outRom[context.workingOffset++] = 0x01;
                    // CMP #0006 - is it lumina?
                    outRom[context.workingOffset++] = 0xC9;
                    outRom[context.workingOffset++] = 0x06;
                    outRom[context.workingOffset++] = 0x00;
                    // BNE nope
                    outRom[context.workingOffset++] = 0xD0;
                    outRom[context.workingOffset++] = 0x02;
                    // INX twice to offset for lumina
                    outRom[context.workingOffset++] = 0xE8;
                    outRom[context.workingOffset++] = 0xE8;
                }
                // LDA $D8FD60,x - load menu icon
                outRom[context.workingOffset++] = 0xBF;
                outRom[context.workingOffset++] = 0x60;
                outRom[context.workingOffset++] = 0xFD;
                outRom[context.workingOffset++] = 0xD8;
                // STA $0000,y - store for menu stuff
                outRom[context.workingOffset++] = 0x99;
                outRom[context.workingOffset++] = 0x00;
                outRom[context.workingOffset++] = 0x00;
                // INY
                outRom[context.workingOffset++] = 0xC8;
                // INY
                outRom[context.workingOffset++] = 0xC8;
                // PLX
                outRom[context.workingOffset++] = 0xFA;
                // PLX
                outRom[context.workingOffset++] = 0xFA;
                // endLoop:
                int jumpSkipElementDest = context.workingOffset;
                // PLA - number of iters remaining
                outRom[context.workingOffset++] = 0x68;
                // DEC A
                outRom[context.workingOffset++] = 0x3A;
                // BNE loopJumpBack
                outRom[context.workingOffset++] = 0xD0;
                int loopJumpBackSource = context.workingOffset;
                outRom[context.workingOffset++] = 0x00; // - to be filled in later

                if (charClass == "OGgirl")
                {
                    // LDA $1874 - girl selected element in menu
                    outRom[context.workingOffset++] = 0xAD;
                    outRom[context.workingOffset++] = 0x74;
                    outRom[context.workingOffset++] = 0x18;
                }
                else
                {
                    // LDA $1875 - sprite selected element in menu
                    outRom[context.workingOffset++] = 0xAD;
                    outRom[context.workingOffset++] = 0x75;
                    outRom[context.workingOffset++] = 0x18;
                }
                // AND #00FF
                outRom[context.workingOffset++] = 0x29;
                outRom[context.workingOffset++] = 0xFF;
                outRom[context.workingOffset++] = 0x00;
                // STA $182E
                outRom[context.workingOffset++] = 0x8D;
                outRom[context.workingOffset++] = 0x2E;
                outRom[context.workingOffset++] = 0x18;
                // CLC
                outRom[context.workingOffset++] = 0x18;
                // RTL
                outRom[context.workingOffset++] = 0x6B;
                // set the branch distances above
                setJumpLocation(outRom, jumpSkipElementSource, jumpSkipElementDest);
                setJumpLocation(outRom, loopJumpBackSource, loopJumpBackDest);
            }
        }

        private void makeSpellLoadSub(byte[] outRom, RandoContext context, int charId, string charClass)
        {
            // TXA
            outRom[context.workingOffset++] = 0x8A;
            if (charClass == "OGgirl")
            {
                // CLC
                outRom[context.workingOffset++] = 0x18;
                // ADC #0006
                outRom[context.workingOffset++] = 0x69;
                outRom[context.workingOffset++] = 0x06;
                outRom[context.workingOffset++] = 0x00;
                // TAX
                outRom[context.workingOffset++] = 0xAA;
            }
            // RTL
            outRom[context.workingOffset++] = 0x6B;
        }

        private void setJumpLocation(byte[] outRom, int src, int dest)
        {
            // utility for pairing branch instructions with the spot they branch to
            int srcPos = src + 1;
            byte diff = (byte)(dest - srcPos);
            outRom[src] = diff;
        }
    }
}
