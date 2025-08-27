using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that replaces all damage indicators with a large zero, so you don't know how much damage is being done to
    /// either you or enemies.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ObscureDamage : RandoProcessor
    {
        protected override string getName()
        {
            return "Obscure damage numbers";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_OBSCURE_DAMAGE))
            {
                return false;
            }

            // skip this entire sequence that generates the sprite numbers and replaces with zero, but maintain 
            // the logic that gives damage the largest font

            /*
            C1/843C:    B002        BCS $8440     -> BRA to branch always
            C1/843E:    8053        BRA $8493     ---
            C1/8440:    C9C800      CMP #$00C8
            C1/8443:    B008        BCS $844D     -> BRA to branch always
            C1/8445:    A508        LDA $08       ---
            C1/8447:    18          CLC           ---
            C1/8448:    698002      ADC #$0280    ---
            C1/844B:    8508        STA $08       ---
            C1/844D:    A504        LDA $04
            C1/844F:    3013        BMI $8464     -> BRA to branch always
            C1/8451:    0A          ASL A         ---
            C1/8452:    0A          ASL A         ---
            C1/8453:    0A          ASL A         ---
            C1/8454:    0A          ASL A         ---
            C1/8455:    0A          ASL A         ---
            C1/8456:    0A          ASL A         ---
            C1/8457:    18          CLC           ---
            C1/8458:    6508        ADC $08       ---
            C1/845A:    990000      STA $0000,Y   ---
            C1/845D:    18          CLC           ---
            C1/845E:    692000      ADC #$0020    ---
            C1/8461:    990400      STA $0004,Y   ---
            C1/8464:    A502        LDA $02
            C1/8466:    3013        BMI $847B     -> BRA to branch always
            C1/8468:    0A          ASL A         ---
            C1/8469:    0A          ASL A         ---
            C1/846A:    0A          ASL A         ---
            C1/846B:    0A          ASL A         ---
            C1/846C:    0A          ASL A         ---
            C1/846D:    0A          ASL A         ---
            C1/846E:    18          CLC           ---
            C1/846F:    6508        ADC $08       ---
            C1/8471:    990800      STA $0008,Y   ---
            C1/8474:    18          CLC           ---
            C1/8475:    692000      ADC #$0020    ---
            C1/8478:    990C00      STA $000C,Y   ---
            C1/847B:    A500        LDA $00
            C1/847D:    3013        BMI $8492    - remove
            C1/847F:    0A          ASL A
            C1/8480:    0A          ASL A
            C1/8481:    0A          ASL A
            C1/8482:    0A          ASL A        -
            C1/8483:    0A          ASL A        -
            C1/8484:    0A          ASL A        - replace with LDA #0000
            C1/8485:    18          CLC 
             */

            outRom[0x1843C] = 0x80;

            outRom[0x18443] = 0x80;
            outRom[0x18444] = 0x08;

            outRom[0x1844F] = 0x80;

            outRom[0x18466] = 0x80;

            outRom[0x1847D] = 0xEA;
            outRom[0x1847E] = 0xEA;

            // LDA #0000
            outRom[0x18482] = 0xA9;
            outRom[0x18483] = 0x00;
            outRom[0x18484] = 0x00;
            
            return true;
        }
    }
}
