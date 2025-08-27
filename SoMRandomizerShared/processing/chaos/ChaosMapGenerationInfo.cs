
namespace SoMRandomizer.processing.chaos
{
    /// <summary>
    /// Info about a generated map for chaos mode.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ChaosMapGenerationInfo
    {
        // the vanilla map to copy for this floor's main map
        public ChaosVanillaMap map;
        // as generated, and used by the next floor to link together
        public int mainRoomMapId;
        // as generated from the below, and used by next floor to link together
        public int nextFloorDoorId;
        // walkon event, if present
        public int? walkonEventId;
        // whether to remove all enemies and replace some with watts and neko
        public bool townMap;
        // event id to run for shops on this floor
        public int nekoEventNum;

        // door to the next floor
        public int? doorIndexToNextFloor;
        // door back to the previous floor
        public int? doorIndexToLastFloor;
        // index in map's doors to reach mana seed prize room, or null if that isn't on this floor
        public int? doorIndexToManaSeedRoom;
        // index in map's doors to reach element prize room, or null if that isn't on this floor
        public int? doorIndexToElementRoom;


        // if a boss exists, the type of boss
        public byte? bossId;
        // if a boss exists, event to run to enter the boss room
        public int? bossEventId;
        // if a boss exists, event to run when the boss dies
        public int? bossDeathEventId;
        // if a boss exists, the map to copy for its fight
        public ChaosBossMap bossMap;

        // index of mana seed on this floor
        public int? seedIndex;
        // event to run when getting mana seed on this floor
        public int? seedEventIndex;

        // index of element on this floor
        public int? elementIndex;
        // event to run when getting element on this floor
        public int? elementEventIndex;
    }
}
