using SoMRandomizer.config.settings;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System.Collections.Generic;
using static SoMRandomizer.processing.common.SomVanillaValues;

namespace SoMRandomizer.processing.hacks.openworld
{
    /// <summary>
    /// Remove all regular enemies.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class NoEnemies : RandoProcessor
    {
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            for (int mapNum = 16; mapNum < 500; mapNum++)
            {
                // skip tasnica miniboss, karon ferry
                if (mapNum != MAPNUM_TASNICA_THRONE_ROOM_NPC && mapNum != MAPNUM_KARONFERRY_ENDPOINTS && 
                    mapNum != MAPNUM_KARONFERRY_TRAVELING_UP && mapNum != MAPNUM_KARONFERRY_TRAVELING_DOWN)
                {
                    // get all map objs
                    List<MapObject> objs = VanillaMapUtil.getObjects(outRom, mapNum);
                    for(int i=0; i < objs.Count; i++)
                    {
                        MapObject obj = objs[i];
                        // object species in normal enemy range
                        if(obj.getSpecies() < 84)
                        {
                            obj.setNeverVisible();
                        }
                        // write it back
                        VanillaMapUtil.putObject(outRom, obj, mapNum, i);
                    }
                }
            }
            return true;
        }

        protected override string getName()
        {
            return "No enemies mode";
        }
    }
}
