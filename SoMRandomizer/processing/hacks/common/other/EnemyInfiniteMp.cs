using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that gives enemies infinite MP.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class EnemyInfiniteMp : RandoProcessor
    {
        protected override string getName()
        {
            return "Enemies have infinite MP";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_ENEMY_INFINITE_MP))
            {
                return false;
            }
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, 40); // idk a bunch

            // spell casting - for enemies (x >= 0x600), override to succeed regardless of current mp

            /*
            $C0/3F80 BD 86 E1    LDA $E186,x[$7E:E786]   A:0004 X:0600 Y:0600 P:envMxdIzc * [replace]
            $C0/3F83 38          SEC                     A:0007 X:0600 Y:0600 P:envMxdIzc * [replace]
            $C0/3F84 E5 A4       SBC $A4    [$00:03A4]   A:0007 X:0600 Y:0600 P:envMxdIzC * [replace]
            $C0/3F86 10 04       BPL $04    [$3F8C]      A:0003 X:0600 Y:0600 P:envMxdIzC - branch if have enough
                $C0/3F88 A9 FF       LDA #$FF                A:00FC X:0600 Y:0600 P:eNvMxdIzc - don't have enough
                $C0/3F8A 80 74       BRA $74    [$4000]      A:00FF X:0600 Y:0600 P:eNvMxdIzc
            $C0/3F8C 22 FD 5A C0 JSL $C05AFD[$C0:5AFD]   A:0003 X:0600 Y:0600 P:envMxdIzC 
             */

            outRom[0x3f80] = 0x22;
            outRom[0x3f81] = (byte)(context.workingOffset);
            outRom[0x3f82] = (byte)(context.workingOffset >> 8);
            outRom[0x3f83] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x3f84] = 0xEA;
            outRom[0x3f85] = 0xEA;
            // CPX #0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // BGE processEnemy (7)
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x07;

            // (removed code)
            // LDA $E186,x
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x86;
            outRom[context.workingOffset++] = 0xE1;
            // SEC
            outRom[context.workingOffset++] = 0x38;
            // SBC $A4
            outRom[context.workingOffset++] = 0xE5;
            outRom[context.workingOffset++] = 0xA4;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // processEnemy:
            // LDA #01 - for the BPL; always pass and allow casts
            outRom[context.workingOffset++] = 0xA9;
            outRom[context.workingOffset++] = 0x01;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            // mp steal - change to ignore enemy target's mp remaining
            // C8/E833:	3BEC		[19: MP Absorb]
            /*
before this block: calculate damage for MP absorb via some math ending in a divide, and in this spot vanilla loads the divide result,
then compares to the target's current MP, to max at that amount.  here, we remove that check and just take the fully calculated amount.
...
C8/EC6B:	AF144200	LDA $004214 - divide result
C8/EC6F:	D002    	BNE $EC73
C8/EC71:	A901    	LDA #$01    - minimum of 1 damage
C8/EC73:	DD86E1  	CMP $E186,X - compare to current mp                   *
C8/EC76:	9003    	BCC $EC7B   - jump over if there's enough             *   - change to BRA to branch always? we only want to do this is x >= 600 though
C8/EC78:	BD86E1  	LDA $E186,X - otherwise load current mp (steal all)   *
C8/EC7B:	85A4    	STA $A4     - stick in a4, damage amount              *
C8/EC7D:	BDF5E1  	LDA $E1F5,X - mp damage on target
C8/EC80:	18      	CLC 
C8/EC81:	65A4    	ADC $A4     - add the steal amount
C8/EC83:	9DF5E1  	STA $E1F5,X - write back
C8/EC86:	B9F6E1  	LDA $E1F6,Y - mp heal on caster
C8/EC89:	18      	CLC 
C8/EC8A:	65A4    	ADC $A4
C8/EC8C:	99F6E1  	STA $E1F6,Y - mp heal on caster
C8/EC8F:	60      	RTS
             */
            outRom[0x8ec73] = 0x22;
            outRom[0x8ec74] = (byte)(context.workingOffset);
            outRom[0x8ec75] = (byte)(context.workingOffset >> 8);
            outRom[0x8ec76] = (byte)((context.workingOffset >> 16) + 0xC0);
            outRom[0x8ec77] = 0xEA;
            outRom[0x8ec78] = 0xEA;
            outRom[0x8ec79] = 0xEA;
            outRom[0x8ec7A] = 0xEA;
            outRom[0x8ec7B] = 0xEA;
            outRom[0x8ec7C] = 0xEA;

            // check for enemy target
            // CPX #0600
            outRom[context.workingOffset++] = 0xE0;
            outRom[context.workingOffset++] = 0x00;
            outRom[context.workingOffset++] = 0x06;
            // BGE infiniteMpLogic (0x0B)
            outRom[context.workingOffset++] = 0xB0;
            outRom[context.workingOffset++] = 0x0B; // lol boob

            // player target: vanilla logic + rtl
            // CMP $E186,x - compare to current mp
            outRom[context.workingOffset++] = 0xDD;
            outRom[context.workingOffset++] = 0x86;
            outRom[context.workingOffset++] = 0xE1;
            // BCC 03 - branch if enough mp
            outRom[context.workingOffset++] = 0x90;
            outRom[context.workingOffset++] = 0x03;
            // LDA $E186,x - otherwise load all of current MP
            outRom[context.workingOffset++] = 0xBD;
            outRom[context.workingOffset++] = 0x86;
            outRom[context.workingOffset++] = 0xE1;
            // STA $A4
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA4;
            // RTL
            outRom[context.workingOffset++] = 0x6B;
            // infiniteMpLogic:
            // enemy target: no logic; take the calculated amount always
            // STA $84
            outRom[context.workingOffset++] = 0x85;
            outRom[context.workingOffset++] = 0xA4;
            // RTL
            outRom[context.workingOffset++] = 0x6B;

            return true;
        }
    }
}
