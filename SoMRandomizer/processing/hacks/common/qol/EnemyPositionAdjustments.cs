using SoMRandomizer.config.settings;
using SoMRandomizer.logging;
using SoMRandomizer.processing.common;
using SoMRandomizer.processing.common.structure;
using System.Collections.Generic;
using static SoMRandomizer.processing.common.SomVanillaValues;

namespace SoMRandomizer.processing.hacks.common.qol
{
    /// <summary>
    /// Hack that moves some vanilla enemies away from doors where they can unfairly smack you right after entering,
    /// particularly if they happen to be randomized to certain other enemy types.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class EnemyPositionAdjustments : RandoProcessor
    {
        protected override string getName()
        {
            return "Position adjustments for vanilla enemies";
        }

        Dictionary<int, Dictionary<int, List<DIR>>> directions = new Dictionary<int, Dictionary<int, List<DIR>>>();
        Dictionary<int, Dictionary<int, List<int>>> amounts = new Dictionary<int, Dictionary<int, List<int>>>();
        protected override bool process(byte[] origRom, byte[] outRom, string seed, RandoSettings settings, RandoContext context)
        {
            if(!settings.getBool(CommonSettings.PROPERTYNAME_ENEMY_POSITION_FIX))
            {
                return false;
            }
            string randoMode = settings.get(CommonSettings.PROPERTYNAME_MODE);
            if (randoMode != VanillaRandoSettings.MODE_KEY && randoMode != OpenWorldSettings.MODE_KEY)
            {
                Logging.log("Unsupported mode for vanilla enemy position adjustments");
                return false;
            }

            directions.Clear();
            // map 400 obj 1 up 2
            add(MAPNUM_NTR_INTERIOR_A, 1, DIR.UP, 2);
            // map 400 obj 2 down 2
            add(MAPNUM_NTR_INTERIOR_A, 2, DIR.DOWN, 2);

            // map 401 obj 2: move 2 left+down
            add(MAPNUM_NTR_INTERIOR_B, 2, DIR.LEFT, 2);
            add(MAPNUM_NTR_INTERIOR_B, 2, DIR.DOWN, 2);
            // map 401 obj 3: move 2 right+down
            add(MAPNUM_NTR_INTERIOR_B, 3, DIR.RIGHT, 2);
            add(MAPNUM_NTR_INTERIOR_B, 3, DIR.DOWN, 2);
            // map 401 obj 4: move 2 left+down
            add(MAPNUM_NTR_INTERIOR_B, 4, DIR.LEFT, 2);
            add(MAPNUM_NTR_INTERIOR_B, 4, DIR.DOWN, 2);
            // map 401 obj 5: move 2 right+down
            add(MAPNUM_NTR_INTERIOR_B, 5, DIR.RIGHT, 2);
            add(MAPNUM_NTR_INTERIOR_B, 5, DIR.DOWN, 2);
            // map 401 obj 6: move 2 left+down
            add(MAPNUM_NTR_INTERIOR_B, 6, DIR.LEFT, 2);
            add(MAPNUM_NTR_INTERIOR_B, 6, DIR.DOWN, 2);
            // map 401 obj 7: move 2 right+down
            add(MAPNUM_NTR_INTERIOR_B, 7, DIR.RIGHT, 2);
            add(MAPNUM_NTR_INTERIOR_B, 7, DIR.DOWN, 2);
            // map 401 obj 13: move 2 up
            add(MAPNUM_NTR_INTERIOR_B, 13, DIR.UP, 2);
            // map 401 obj 14: move 2 up
            add(MAPNUM_NTR_INTERIOR_B, 14, DIR.UP, 2);
            // map 401 obj 17: move 2 down
            add(MAPNUM_NTR_INTERIOR_B, 17, DIR.DOWN, 2);
            add(MAPNUM_NTR_INTERIOR_B, 17, DIR.LEFT, 1);
            // map 401 obj 18: move 2 down
            add(MAPNUM_NTR_INTERIOR_B, 18, DIR.DOWN, 2);
            add(MAPNUM_NTR_INTERIOR_B, 18, DIR.RIGHT, 1);

            // map 404 obj 5: move 2 right
            add(MAPNUM_NTR_INTERIOR_E, 5, DIR.RIGHT, 2);
            // map 404 obj 6: move 2 left
            add(MAPNUM_NTR_INTERIOR_E, 6, DIR.LEFT, 2);

            // map 405 obj 0: down 2
            add(MAPNUM_NTR_INTERIOR_F, 0, DIR.DOWN, 2);
            // map 405 obj 1: move 3 left+down
            add(MAPNUM_NTR_INTERIOR_F, 1, DIR.DOWN, 3);
            add(MAPNUM_NTR_INTERIOR_F, 1, DIR.LEFT, 3);
            // map 405 obj 2: move 3 right+down
            add(MAPNUM_NTR_INTERIOR_F, 2, DIR.RIGHT, 3);
            add(MAPNUM_NTR_INTERIOR_F, 2, DIR.DOWN, 3);

            // 385, 3, right 3
            add(MAPNUM_NTC_INTERIOR_H, 3, DIR.RIGHT, 3);
            // 385, 5, up 2
            add(MAPNUM_NTC_INTERIOR_H, 5, DIR.UP, 2);

            // 417, 4, down 2, right 4
            add(MAPNUM_GRANDPALACE_INTERIOR_F, 4, DIR.DOWN, 2);
            add(MAPNUM_GRANDPALACE_INTERIOR_F, 4, DIR.RIGHT, 4);
            // 417, 5, down 2, right 4
            add(MAPNUM_GRANDPALACE_INTERIOR_F, 5, DIR.DOWN, 2);
            add(MAPNUM_GRANDPALACE_INTERIOR_F, 5, DIR.RIGHT, 4);
            // 417, 6, down 2, left 4
            add(MAPNUM_GRANDPALACE_INTERIOR_F, 6, DIR.DOWN, 2);
            add(MAPNUM_GRANDPALACE_INTERIOR_F, 6, DIR.LEFT, 4);
            // 417, 7, down 2, left 4
            add(MAPNUM_GRANDPALACE_INTERIOR_F, 7, DIR.DOWN, 2);
            add(MAPNUM_GRANDPALACE_INTERIOR_F, 7, DIR.LEFT, 4);

            // 385, 3, right 3
            add(MAPNUM_NTC_INTERIOR_H, 3, DIR.RIGHT, 3);
            // 385, 5, up 3
            add(MAPNUM_NTC_INTERIOR_H, 5, DIR.UP, 3);
            // 385, 2, up 3
            add(MAPNUM_NTC_INTERIOR_H, 2, DIR.UP, 3);

            // 436, 0, right 3
            add(MAPNUM_UNDERSEA_AXE_ROOM_C, 0, DIR.RIGHT, 3);

            // 23, 24, right 5
            add(MAPNUM_PANDORA_WEST_FIELD, 24, DIR.RIGHT, 5);

            // 25, 13, left 7, up 10
            add(MAPNUM_GAIASNAVEL_EXTERIOR, 13, DIR.LEFT, 7);
            add(MAPNUM_GAIASNAVEL_EXTERIOR, 13, DIR.UP, 10);

            // 216, 0, left 4
            add(MAPNUM_PURELANDS_Q, 0, DIR.LEFT, 4);
            // 216, 1, right 4
            add(MAPNUM_PURELANDS_Q, 1, DIR.RIGHT, 4);

            // outside jehk cave
            add(MAPNUM_JEHKCAVE_EXTERIOR, 5, DIR.LEFT, 4);

            // NTR treasure room
            add(MAPNUM_NTR_INTERIOR_D, 1, DIR.UP, 3);
            add(MAPNUM_NTR_INTERIOR_D, 2, DIR.UP, 3);

            foreach (int mapNum in directions.Keys)
            {
                Dictionary<int, List<DIR>> dirs = directions[mapNum];
                Dictionary<int, List<int>> distances = amounts[mapNum];
                List<MapObject> mapObjs = VanillaMapUtil.getObjects(outRom, mapNum);

                foreach (int objNum in dirs.Keys)
                {
                    List<DIR> dirList = dirs[objNum];
                    List<int> distanceList = distances[objNum];
                    MapObject obj = mapObjs[objNum];
                    for (int i=0; i < dirList.Count; i++)
                    {
                        DIR dir = dirList[i];
                        int distance = distanceList[i];
                        switch(dir)
                        {
                            case DIR.DOWN:
                                obj.setYpos((byte)(obj.getYpos() + distance));
                                break;
                            case DIR.UP:
                                obj.setYpos((byte)(obj.getYpos() - distance));
                                break;
                            case DIR.LEFT:
                                obj.setXpos((byte)(obj.getXpos() - distance));
                                break;
                            case DIR.RIGHT:
                                obj.setXpos((byte)(obj.getXpos() + distance));
                                break;
                        }
                    }
                    VanillaMapUtil.putObject(outRom, obj, mapNum, objNum);
                }
            }

            return true;
        }

        private void add(int mapNum, int objNum, DIR dir, int distance)
        {
            if(!directions.ContainsKey(mapNum))
            {
                directions[mapNum] = new Dictionary<int, List<DIR>>();
                amounts[mapNum] = new Dictionary<int, List<int>>();
            }

            if(!directions[mapNum].ContainsKey(objNum))
            {
                directions[mapNum][objNum] = new List<DIR>();
                amounts[mapNum][objNum] = new List<int>();
            }
            directions[mapNum][objNum].Add(dir);
            amounts[mapNum][objNum].Add(distance);
        }
        private enum DIR
        {
            LEFT,
            RIGHT,
            UP,
            DOWN
        }
    }
}
