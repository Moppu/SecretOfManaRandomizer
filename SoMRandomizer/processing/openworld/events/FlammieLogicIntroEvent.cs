using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;

namespace SoMRandomizer.processing.openworld.events
{
    /// <summary>
    /// Open world intro event for flammie drum in logic.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    class FlammieLogicIntroEvent : RandoProcessor
    {
        protected override string getName()
        {
            return "Event 0x106 - intro (flammie drum in logic)";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(OpenWorldSettings.PROPERTYNAME_FLAMMIE_DRUM_IN_LOGIC))
            {
                return false;
            }
            EventScript eventData = (EventScript)context.replacementEvents[0x106];
            eventData.Add(EventCommandEnum.PLAY_SOUND.Value);
            eventData.Add(0x01);
            eventData.Add(0x11);
            eventData.Add(0x12);
            eventData.Add(0x8f);

            eventData.AddAutoTextDialogueBox(VanillaEventUtil.wordWrapText("Starting here because flammie drum is in logic! Give it a second here it's got a lot to set up"), 0x0A);

            // event 0x107 - initialize event flags
            eventData.Jsr(0x107);

            // -> event x103, the sword grab event
            eventData.Jump(0x103);
            eventData.End();
            return true;
        }
    }
}
