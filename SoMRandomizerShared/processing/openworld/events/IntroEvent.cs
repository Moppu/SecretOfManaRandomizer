using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using static SoMRandomizer.processing.common.SomVanillaValues;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Create the starting event for open world mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class IntroEvent : RandoProcessor
    {
        protected override string getName()
        {
            return "Event 0x106 - intro";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            string goalType = context.workingData.get(OpenWorldGoalProcessor.GOAL_SHORT_NAME);

            // ////////////////////////////////////////////////////
            // x106 - waterfall intro
            // ////////////////////////////////////////////////////
            EventScript newEvent106 = new EventScript();
            context.replacementEvents[0x106] = newEvent106;

            // 1F03 - set state of world map for best flammie landings (with some mods)
            newEvent106.Add(0x1F);
            newEvent106.Add(0x03);

            // add starting gold if specified
            int goldAmount = settings.getInt(OpenWorldSettings.PROPERTYNAME_NUMERIC_START_GOLD);
            if (goldAmount > 0)
            {
                newEvent106.Add(EventCommandEnum.ADD_GOLD.Value);
                newEvent106.Add((byte)goldAmount);
                newEvent106.Add((byte)(goldAmount >> 8));
            }

            // add whichever intro script decides it's applicable
            // note these end the 0x106 event, so anything common should be added before this point
            new GiftDeliveryIntroEvent().add(origRom, outRom, seed, settings, context);
            new ReindeerIntroEvent().add(origRom, outRom, seed, settings, context);
            new FlammieLogicIntroEvent().add(origRom, outRom, seed, settings, context);
            new WaterfallIntroEvent().add(origRom, outRom, seed, settings, context);

            new InitialEventFlagStates().add(origRom, outRom, seed, settings, context);


            // load 0xC0 into 7E00E3 to unlock both purelands and the purple continent on world map
            outRom[0x1EB93] = 0xC0;
            // and in saveram loading, too - LDA instead of AND
            outRom[0x2AA57] = 0xA9;

            if (goalType == OpenWorldGoalProcessor.GOAL_MTR || goalType == OpenWorldGoalProcessor.GOAL_GIFTMODE || goalType == OpenWorldGoalProcessor.GOAL_REINDEER)
            {
                // disable flying manafort, but keep purelands open
                outRom[0x2b115] = 0x00;
            }

            // short-circuit the 1F03 event so it doesn't do that mode 7 hover scene
            outRom[0x1EBA8] = 0x60;
            outRom[0x1EBA9] = 0xEA;
            outRom[0x1EBAA] = 0xEA;
            outRom[0x1EBAB] = 0xEA;

            // make the sword visible 0x10 00-0A
            int mapNum = MAPNUM_RUSTYSWORD; // sword map
            int mapObjOffset = 0x80000 + outRom[0x87000 + mapNum * 2] + (outRom[0x87000 + mapNum * 2 + 1] << 8);
            mapObjOffset += 8; // skip header

            outRom[mapObjOffset + 8 * 0 + 0] = 0x10; // adjust event data of obj i
            outRom[mapObjOffset + 8 * 0 + 1] = 0x0A; // adjust event data of obj i
            return true;
        }
    }
}
