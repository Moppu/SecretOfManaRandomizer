using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.util;
using System.Collections.Generic;

//
// MOPPLE: i thought it would be fun to have a message here about not using zsnes; the code below can be used to detect ZSNES from within game:
// SED 
// LDA #$FF 
// CLC 
// ADC #$FF 
// CLD 
// CMP #$64 
// BNE NotZsnes 
// (zsnes handling code)
// NotZsnes:
// (blarp)
namespace SoMRandomizer.processing.hacks.common.enhancement
{
    /// <summary>
    /// Add my name on the squaresoft logo and also an indicator of what type of randomization was done on the title screen.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class TitleScreenAddition : RandoProcessor
    {
        protected override string getName()
        {
            return "Title screen changes";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            string titleString;
            if(randoMode == OpenWorldSettings.MODE_KEY)
            {
                titleString = "open world";
            }
            else if(randoMode == VanillaRandoSettings.MODE_KEY)
            {
                titleString = "randomizer";
            }
            else if(randoMode == AncientCaveSettings.MODE_KEY)
            {
                titleString = "ancient cave";
            }
            else if (randoMode == BossRushSettings.MODE_KEY)
            {
                titleString = "boss rush";
            }
            else if (randoMode == ChaosSettings.MODE_KEY)
            {
                titleString = "chaos mode";
            }
            else
            {
                Logging.log("Unsupported mode for title screen marker: " + randoMode);
                return false;
            }

            // since we're recompressing the tileset placements, we need to modify the code that points to them.
            // that code is also LZ77 compressed, so we decompress, modify, and recompress two blocks.

            // 0x77C00 code location w/ seekback value (1) and size
            List<byte> decodedSomTitleScreenCode = SomLz77.decodeSomLz77(origRom, VanillaRomOffsets.TITLE_SCREEN_COMPRESSED_CODE_OFFSET);
            // ^ [11669 (msb)], [11672 (lsb), 11673 (mb)] refer to vv
            List<byte> decodedSomTitlePlacements = SomLz77.decodeSomLz77(origRom, VanillaRomOffsets.TITLE_SCREEN_COMPRESSED_TILE_PLACEMENTS);
            // ^ [x14A6] is where title screen starts .. 0x61 is A

            // position under the game title to inject the name of the type of rando
            int stringPos = 0x165D;
            for(int i=titleString.Length - 1; i >= 0; i--)
            {
                // convert to proper values for the title screen font
                byte val = 0x60;
                char c = titleString[i];
                if(c >= 'A' && c <= 'Z')
                {
                    val = (byte)(0x61 + (c - 'A'));
                }
                if (c >= 'a' && c <= 'z')
                {
                    val = (byte)(0x61 + (c - 'a'));
                }
                if(c == '.')
                {
                    val = 0x7B;
                }
                decodedSomTitlePlacements[stringPos] = val;
                stringPos--;
            }

            // also moppleton
            decodedSomTitlePlacements[0x1B5A] = 0x61;
            decodedSomTitlePlacements[0x1B5B] = 0x6C;
            decodedSomTitlePlacements[0x1B5C] = 0x73;
            decodedSomTitlePlacements[0x1B5D] = 0x6F;

            decodedSomTitlePlacements[0x1B77] = 0x6D;
            decodedSomTitlePlacements[0x1B78] = 0x6F;
            decodedSomTitlePlacements[0x1B79] = 0x70;
            decodedSomTitlePlacements[0x1B7A] = 0x70;
            decodedSomTitlePlacements[0x1B7B] = 0x6C;
            decodedSomTitlePlacements[0x1B7C] = 0x65;
            decodedSomTitlePlacements[0x1B7D] = 0x74;
            decodedSomTitlePlacements[0x1B7E] = 0x6F;
            decodedSomTitlePlacements[0x1B7F] = 0x6E;

            // recompress modified title screen placements
            List<byte> recompPlacements = SomLz77.encodeSomLz77(decodedSomTitlePlacements.ToArray(), 4);
            // ensure we don't cross a bank boundary, otherwise the game won't even load
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, recompPlacements.Count);
            byte placeMsb = (byte)((context.workingOffset >> 16) + 0xC0);
            byte placeMb = (byte)((context.workingOffset >> 8));
            byte placeLsb = (byte)((context.workingOffset));
            decodedSomTitleScreenCode[11669] = placeMsb;
            decodedSomTitleScreenCode[11672] = placeLsb;
            decodedSomTitleScreenCode[11673] = placeMb;
            foreach (byte b in recompPlacements)
            {
                outRom[context.workingOffset++] = b;
            }

            // recompress modified title screen code
            List<byte> recompCode = SomLz77.encodeSomLz77(decodedSomTitleScreenCode.ToArray(), 1);
            // ensure we don't cross a bank boundary, otherwise the game won't even load
            CodeGenerationUtils.ensureSpaceInBank(ref context.workingOffset, recompCode.Count);
            byte codeMsb = (byte)((context.workingOffset >> 16) + 0xC0);
            byte codeMb = (byte)((context.workingOffset >> 8));
            byte codeLsb = (byte)((context.workingOffset));
            outRom[0x14D3C] = codeMsb;
            outRom[0x14D3E] = codeLsb;
            outRom[0x14D3F] = codeMb;
            foreach(byte b in recompCode)
            {
                outRom[context.workingOffset++] = b;
            }

            return true;
        }
    }
}
