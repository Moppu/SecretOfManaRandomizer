using SoMRandomizer.util;

namespace SoMRandomizer.processing.common.structure
{
    /// <summary>
    /// 8-byte mana structure for objects on maps.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MapObject : VanillaStructure
    {
        public const byte DIR_UP = 0;
        public const byte DIR_DOWN = 1;
        public const byte DIR_LEFT = 2;
        public const byte DIR_RIGHT = 3;

        public MapObject() : base(new byte[8]) { }
        public MapObject(byte[] objectData) : base(objectData) { }

        private const int EVENT_VIS_FLAG_BYTE = 0;
        private const int EVENT_VIS_FLAG_BITS = 0xFF;

        private const int EVENT_VIS_MIN_BYTE = 1;
        private const int EVENT_VIS_MIN_BITS = 0xF0;

        private const int EVENT_VIS_MAX_BYTE = 1;
        private const int EVENT_VIS_MAX_BITS = 0x0F;

        private const int OBJECT_XPOS_BYTE = 2;
        private const int OBJECT_XPOS_BITS = 0x7F;

        // make sure these spawn (orbs, chests, etc)
        private const int OBJECT_SPAWN_PRIORITY_BYTE = 2;
        private const int OBJECT_SPAWN_PRIORITY_BITS = 0x80;

        private const int OBJECT_YPOS_BYTE = 3;
        private const int OBJECT_YPOS_BITS = 0x7F;

        private const int OBJECT_FROZEN_BYTE = 3;
        private const int OBJECT_FROZEN_BITS = 0x80;

        private const int OBJECT_DIR_BYTE = 4;
        private const int OBJECT_DIR_BITS = 0xC0;

        // MOPPLE: not sure what this is, but seems necessary for some NPCs to work right
        private const int OBJECT_UNKNOWN_4A_BYTE = 4;
        private const int OBJECT_UNKNOWN_4A_BITS = 0x20;

        private const int OBJECT_LAYER2_COLLISION_BYTE = 4;
        private const int OBJECT_LAYER2_COLLISION_BITS = 0x08;

        // byte 4 bits 0x10 and 0x07: unknown

        // 00-7F: enemies; 80-FF: npcs, players, and other stuff
        private const int OBJECT_SPECIES_BYTE = 5;
        private const int OBJECT_SPECIES_BITS = 0xFF;

        private const int OBJECT_EVENT_LSB_BYTE = 6;
        private const int OBJECT_EVENT_LSB_BITS = 0xFF;

        private const int OBJECT_EVENT_MSB_BYTE = 7;
        private const int OBJECT_EVENT_MSB_BITS = 0x07;

        // byte 7 bits 0xF8: unknown

        private const int OBJECT_UNKNOWN_B7_BYTE = 7;
        private const int OBJECT_UNKNOWN_B7_BITS = 0xF8;

        public byte getEventVisFlag()
        {
            return get(EVENT_VIS_FLAG_BYTE, EVENT_VIS_FLAG_BITS);
        }

        public void setEventVisFlag(byte eventVisFlag)
        {
            set(EVENT_VIS_FLAG_BYTE, EVENT_VIS_FLAG_BITS, eventVisFlag);
        }

        public byte getEventVisMinimum()
        {
            return get(EVENT_VIS_MIN_BYTE, EVENT_VIS_MIN_BITS);
        }

        public void setEventVisMinimum(byte eventVisFlag)
        {
            set(EVENT_VIS_MIN_BYTE, EVENT_VIS_MIN_BITS, eventVisFlag);
        }

        public byte getEventVisMaximum()
        {
            return get(EVENT_VIS_MAX_BYTE, EVENT_VIS_MAX_BITS);
        }

        public void setEventVisMaximum(byte eventVisFlag)
        {
            set(EVENT_VIS_MAX_BYTE, EVENT_VIS_MAX_BITS, eventVisFlag);
        }

        public void setAlwaysVisible()
        {
            // visible if event flag 00 is any value
            setEventVisFlag(0x00);
            setEventVisMinimum(0x00);
            setEventVisMaximum(0x0F);
        }

        public void setNeverVisible()
        {
            // visible if event flag 00 is F, which it never is
            setEventVisFlag(0x00);
            setEventVisMinimum(0x0F);
            setEventVisMaximum(0x0F);
        }

        public byte getXpos()
        {
            return get(OBJECT_XPOS_BYTE, OBJECT_XPOS_BITS);
        }

        public void setXpos(byte x)
        {
            set(OBJECT_XPOS_BYTE, OBJECT_XPOS_BITS, x);
        }

        public byte getYpos()
        {
            return get(OBJECT_YPOS_BYTE, OBJECT_YPOS_BITS);
        }

        public void setYpos(byte y)
        {
            set(OBJECT_YPOS_BYTE, OBJECT_YPOS_BITS, y);
        }

        public bool getSpawnPriority()
        {
            return get(OBJECT_SPAWN_PRIORITY_BYTE, OBJECT_SPAWN_PRIORITY_BITS) > 0;
        }

        public void setSpawnPriority(bool spawnPriority)
        {
            set(OBJECT_SPAWN_PRIORITY_BYTE, OBJECT_SPAWN_PRIORITY_BITS, (byte)(spawnPriority ? 1 : 0));
        }

        public byte getDirection()
        {
            return get(OBJECT_DIR_BYTE, OBJECT_DIR_BITS);
        }

        public void setDirection(byte dir)
        {
            set(OBJECT_DIR_BYTE, OBJECT_DIR_BITS, dir);
        }

        public bool getUnknown4A()
        {
            return get(OBJECT_UNKNOWN_4A_BYTE, OBJECT_UNKNOWN_4A_BITS) > 0;
        }

        public void setUnknown4A(bool unknown4A)
        {
            set(OBJECT_UNKNOWN_4A_BYTE, OBJECT_UNKNOWN_4A_BITS, (byte)(unknown4A ? 1 : 0));
        }

        public bool getLayer2Collision()
        {
            return get(OBJECT_LAYER2_COLLISION_BYTE, OBJECT_LAYER2_COLLISION_BITS) > 0;
        }

        public void setLayer2Collision(bool layer2Collision)
        {
            set(OBJECT_LAYER2_COLLISION_BYTE, OBJECT_LAYER2_COLLISION_BITS, (byte)(layer2Collision ? 1 : 0));
        }

        public bool getFrozen()
        {
            return get(OBJECT_FROZEN_BYTE, OBJECT_FROZEN_BITS) > 0;
        }

        public void setFrozen(bool frozen)
        {
            set(OBJECT_FROZEN_BYTE, OBJECT_FROZEN_BITS, (byte)(frozen ? 1 : 0));
        }

        public byte getSpecies()
        {
            return get(OBJECT_SPECIES_BYTE, OBJECT_SPECIES_BITS);
        }

        public void setSpecies(byte species)
        {
            set(OBJECT_SPECIES_BYTE, OBJECT_SPECIES_BITS, species);
        }

        public ushort getEvent()
        {
            return getUshort(OBJECT_EVENT_LSB_BYTE, OBJECT_EVENT_LSB_BITS, OBJECT_EVENT_MSB_BYTE, OBJECT_EVENT_MSB_BITS);
        }

        public void setEvent(ushort eventNum)
        {
            setUshort(OBJECT_EVENT_LSB_BYTE, OBJECT_EVENT_LSB_BITS, OBJECT_EVENT_MSB_BYTE, OBJECT_EVENT_MSB_BITS, eventNum);
        }

        public byte getUnknownB7()
        {
            return get(OBJECT_UNKNOWN_B7_BYTE, OBJECT_UNKNOWN_B7_BITS);
        }

        public void setUnknownB7(byte unknown)
        {
            set(OBJECT_UNKNOWN_B7_BYTE, OBJECT_UNKNOWN_B7_BITS, unknown);
        }

        public override string ToString()
        {
            return "[Map Object: " + 
                "x=" + getXpos().ToString("X2") + "; " +
                "y=" + getYpos().ToString("X2") + "; " +
                "type=" + getSpecies().ToString("X2") + "; " +
                "event=" + getEvent().ToString("X4") + "; " +
                "<" + DataUtil.byteArrayToHexString(getRomValue()) + ">" +
                "]";
        }
    }
}
