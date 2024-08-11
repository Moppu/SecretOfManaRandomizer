using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Hack that hides your own current/max HP.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ObscureOwnHp : RandoProcessor
    {
        protected override string getName()
        {
            return "Obscure player HP";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_OBSCURE_OWN_HP))
            {
                return false;
            }

            // hide display on bottom of main screen

            // subr to display boy hp on screen
            // C1/E34F:   2069E3   JSR $E369
            outRom[0x1E34F] = 0xEA;
            outRom[0x1E350] = 0xEA;
            outRom[0x1E351] = 0xEA;

            // subr to display girl hp on screen
            // C1/E35A:   2069E3   JSR $E369
            outRom[0x1E35A] = 0xEA;
            outRom[0x1E35B] = 0xEA;
            outRom[0x1E35C] = 0xEA;

            // subr to display sprite hp on screen
            // C1/E365:   2069E3   JSR $E369
            outRom[0x1E365] = 0xEA;
            outRom[0x1E366] = 0xEA;
            outRom[0x1E367] = 0xEA;

            // hide display in stats menu

            // current HP
            // $C7/69D3 B9 82 E1    LDA $E182,y[$7E:E382]   A:0200 X:0018 Y:0200 P:envmxdIzc
            // $C7/69D9 20 17 5D    JSR $5D17  [$C7:5D17]   A:003B X:0018 Y:003B P:envMxdIzc **
            // $C7/69DC 20 1E 5A    JSR $5A1E  [$C7:5A1E]   A:03B5 X:0018 Y:0000 P:envMxdIzC **
            outRom[0x769D9] = 0xEA;
            outRom[0x769DA] = 0xEA;
            outRom[0x769DB] = 0xEA;
            outRom[0x769DC] = 0xEA;
            outRom[0x769DD] = 0xEA;
            outRom[0x769DE] = 0xEA;

            // max HP
            // $C7/69E8 B9 84 E1    LDA $E184,y[$7E:E384]   A:03C1 X:001C Y:0200 P:envmxdIzC
            // $C7/69EE 20 17 5D    JSR $5D17  [$C7:5D17]   A:003B X:001C Y:003B P:envMxdIzC **
            // $C7/69F1 20 1E 5A    JSR $5A1E  [$C7:5A1E]   A:03B5 X:001C Y:0000 P:envMxdIzC **
            outRom[0x769EE] = 0xEA;
            outRom[0x769EF] = 0xEA;
            outRom[0x769F0] = 0xEA;
            outRom[0x769F1] = 0xEA;
            outRom[0x769F2] = 0xEA;
            outRom[0x769F3] = 0xEA;
            return true;
        }
    }
}
