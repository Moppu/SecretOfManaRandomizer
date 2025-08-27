using SoMRandomizer.util;

namespace SoMRandomizer.processing.common.structure
{
    /// <summary>
    /// 4-byte mana structure for doors between maps. 
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class Door : VanillaStructure
    {
        public Door() : base(new byte[4]) { }
        public Door(byte[] doorData) : base(doorData) { }

        private const int TARGET_MAP_LSB_BYTE = 0;
        private const int TARGET_MAP_LSB_BITS = 0xFF;

        private const int TARGET_MAP_MSB_BYTE = 1;
        private const int TARGET_MAP_MSB_BITS = 0x01;

        private const int TARGET_X_BYTE = 1;
        private const int TARGET_X_BITS = 0xFE;

        private const int TARGET_MAP_LAYER2_COLLISION_BYTE = 2;
        private const int TARGET_MAP_LAYER2_COLLISION_BITS = 0x01;

        private const int TARGET_Y_BYTE = 2;
        private const int TARGET_Y_BITS = 0xFE;

        private const int TRANSITION_TYPE_BYTE = 3;
        private const int TRANSITION_TYPE_BITS = 0xFF;

        public ushort getTargetMap()
        {
            return getUshort(TARGET_MAP_LSB_BYTE, TARGET_MAP_LSB_BITS, TARGET_MAP_MSB_BYTE, TARGET_MAP_MSB_BITS);
        }

        public void setTargetMap(ushort mapNum)
        {
            setUshort(TARGET_MAP_LSB_BYTE, TARGET_MAP_LSB_BITS, TARGET_MAP_MSB_BYTE, TARGET_MAP_MSB_BITS, mapNum);
        }

        public byte getXpos()
        {
            return get(TARGET_X_BYTE, TARGET_X_BITS);
        }

        public void setXpos(byte x)
        {
            set(TARGET_X_BYTE, TARGET_X_BITS, x);
        }

        public bool getLayer2Collision()
        {
            return get(TARGET_MAP_LAYER2_COLLISION_BYTE, TARGET_MAP_LAYER2_COLLISION_BITS) > 0;
        }

        public void setLayer2Collision(bool layer2Collision)
        {
            set(TARGET_MAP_LAYER2_COLLISION_BYTE, TARGET_MAP_LAYER2_COLLISION_BITS, (byte)(layer2Collision ? 1 : 0));
        }

        public byte getYpos()
        {
            return get(TARGET_Y_BYTE, TARGET_Y_BITS);
        }

        public void setYpos(byte y)
        {
            set(TARGET_Y_BYTE, TARGET_Y_BITS, y);
        }

        public byte getTransitionType()
        {
            return get(TRANSITION_TYPE_BYTE, TRANSITION_TYPE_BITS);
        }

        public void setTransitionType(byte transitionType)
        {
            // eeexxxxx
            // entry style / exit style
            set(TRANSITION_TYPE_BYTE, TRANSITION_TYPE_BITS, transitionType);
        }

        public override string ToString()
        {
            return "[Door: " +
                "map=" + getTargetMap().ToString("X4") + "; " +
                "x=" + getXpos().ToString("X2") + "; " +
                "y=" + getYpos().ToString("X2") + "; " +
                "L2=" + getLayer2Collision() + "; " +
                "xition=" + getTransitionType().ToString("X2") + "; " +
                "<" + DataUtil.byteArrayToHexString(getRomValue()) + ">" +
                "]";
        }
    }
}
