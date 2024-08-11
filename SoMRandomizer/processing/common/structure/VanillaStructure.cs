using System;

namespace SoMRandomizer.processing.common.structure
{
    /// <summary>
    /// A byte-array-backed mana structure.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public abstract class VanillaStructure
    {
        private byte[] romvalue;

        protected VanillaStructure(byte[] romvalue)
        {
            this.romvalue = romvalue;
        }

        protected VanillaStructure(byte[] rom, int addr, int length)
        {
            romvalue = new byte[length];
            for(int i=0; i < length; i++)
            {
                romvalue[i] = rom[addr + i];
            }
        }

        protected byte[] getRomValue()
        {
            return romvalue;
        }

        private static int getShift(int bits)
        {
            for(int i=0; i < 32; i++)
            {
                if((bits & (1 << i)) > 0)
                {
                    return i;
                }
            }
            return 32;
        }

        protected byte get(int byteIndex, int bits)
        {
            return (byte)((romvalue[byteIndex] & bits) >> getShift(bits));
        }

        protected void set(int byteIndex, int bits, byte value)
        {
            byte otherBits = (byte)(romvalue[byteIndex] & (~bits));
            romvalue[byteIndex] = (byte)(value << getShift(bits));
            romvalue[byteIndex] |= otherBits;
        }

        protected byte[] getRaw()
        {
            return romvalue;
        }

        protected ushort getUshort(int lsbByte, int lsbBits, int msbByte, int msbBits)
        {
            return (ushort)(get(lsbByte, lsbBits) + (get(msbByte, msbBits) << 8));
        }

        protected void setUshort(int lsbByte, int lsbBits, int msbByte, int msbBits, ushort value)
        {
            set(lsbByte, lsbBits, (byte)value);
            set(msbByte, msbBits, (byte)(value >> 8));
        }

        protected byte[] getDataSegment(int index, int length)
        {
            byte[] seq = new byte[length];
            Array.Copy(romvalue, index, seq, 0, length);
            return seq;
        }

        public void put(byte[] rom, int offset)
        {
            for (int i = 0; i < romvalue.Length; i++)
            {
                rom[offset++] = romvalue[i];
            }
        }

        public void put(VanillaStructure structure, int offset)
        {
            put(structure.romvalue, offset);
        }
    }
}
