using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;

namespace SoMRandomizer.processing.hacks.common.other
{
    /// <summary>
    /// Fix some event bug that elmagus kept running into in fire palace and I still am not sure the exact cause of.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class FirePalaceDoorFix : RandoProcessor
    {
        protected override string getName()
        {
            return "Workaround for one issue with a fire palace door";
        }

        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            // only valid for vanilla map/door ids
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            if (randoMode != VanillaRandoSettings.MODE_KEY && randoMode != OpenWorldSettings.MODE_KEY)
            {
                Logging.log("Unsupported mode for fire palace door fix");
                return false;
            }

            // elmagus door fix
            // https://clips.twitch.tv/SucculentSmilingLatteNerfRedBlaster
            // still unsure what caused this; event flag 2C out of sync somehow
            // this is a workaround only
            // door 151 to map 347 [x=16 y=29]
            Door doorIntoMinotaurRoom = VanillaDoorUtil.getDoor(origRom, 0x151);
            doorIntoMinotaurRoom.setYpos((byte)(doorIntoMinotaurRoom.getYpos() - 6));
            VanillaDoorUtil.putDoor(outRom, doorIntoMinotaurRoom, 0x151);

            // move the minotaur up too
            MapObject minotaur = VanillaMapUtil.getObjects(outRom, SomVanillaValues.MAPNUM_MINOTAUR_ARENA)[0];
            minotaur.setYpos((byte)(minotaur.getYpos() - 3));
            VanillaMapUtil.putObject(outRom, minotaur, SomVanillaValues.MAPNUM_MINOTAUR_ARENA, 0);

            return true;
        }
    }
}
