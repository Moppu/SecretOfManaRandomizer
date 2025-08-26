using SoMRandomizer.util;

namespace SoMRandomizer.processing.common.structure
{
    /// <summary>
    /// 16-bit snes color in format [1] = xBBBBBGG [0] = GGGRRRRR.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class SnesColor : VanillaStructure
    {
        private const int LSB_BYTE = 0;
        private const int MSB_BYTE = 1;

        // construct with black
        public SnesColor() : base(new byte[2]) { }
        // construct with existing 2-byte color from rom
        public SnesColor(byte[] romValue) : base(romValue) { }
        // construct from rom and addr
        public SnesColor(byte[] rom, int addr) : base(rom, addr, 2) { }
        // construct with specified color
        public SnesColor(int r, int g, int b) : this()
        {
            setRed((byte)DataUtil.clampToEndpoints(r, 0, 255));
            setGreen((byte)DataUtil.clampToEndpoints(g, 0, 255));
            setBlue((byte)DataUtil.clampToEndpoints(b, 0, 255));
        }
        public SnesColor(int r, int g, int b, bool nonColorBit) : this(r, g, b)
        {
            setNonColorBit(nonColorBit);
        }

        public byte getRed()
        {
            // ........ ...xxxxx
            // range 0 -> 248
            return (byte)(get(LSB_BYTE, 0x1F) * 8);
        }

        public byte getGreen()
        {
            // ......xx xxx.....
            // range 0 -> 248
            // green spans both bytes
            byte g1 = get(LSB_BYTE, 0xE0);
            byte g2 = get(MSB_BYTE, 0x03);
            return (byte)((g1 + (g2 << 3)) * 8);
        }

        public byte getBlue()
        {
            // .xxxxx.. ........
            // range 0 -> 248
            return (byte)(get(MSB_BYTE, 0x7C) * 8);
        }

        public bool getNonColorBit()
        {
            // return the MSB of byte 1 which isn't part of R,G, or B and can have various uses
            return get(MSB_BYTE, 0x80) > 0;
        }

        public void setRed(byte red)
        {
            // range 0-248 down to 0-31
            byte snesRed = (byte)(red / 8);
            set(LSB_BYTE, 0x1F, snesRed);
        }

        public void setGreen(byte green)
        {
            // range 0-248 down to 0-31
            byte snesGreen = (byte)(green / 8);
            // green spans both bytes
            //setUshort(LSB_BYTE, 0xE0, MSB_BYTE, 0x03, snesGreen);
            set(LSB_BYTE, 0xE0, (byte)(snesGreen & 0x07));
            set(MSB_BYTE, 0x03, (byte)(snesGreen >> 3));
        }

        public void setBlue(byte blue)
        {
            // range 0-248 down to 0-31
            byte snesBlue = (byte)(blue / 8);
            set(MSB_BYTE, 0x7C, snesBlue);
        }

        public void setNonColorBit(bool nonColorBit)
        {
            set(MSB_BYTE, 0x80, (byte)(nonColorBit ? 0x80 : 0));
        }

        // add rgb values in range 0-255
        public void add(int red, int green, int blue)
        {
            int redNew = DataUtil.clampToEndpoints(getRed() + red, 0, 255);
            int greenNew = DataUtil.clampToEndpoints(getGreen() + green, 0, 255);
            int blueNew = DataUtil.clampToEndpoints(getBlue() + blue, 0, 255);
            setRed((byte)redNew);
            setGreen((byte)greenNew);
            setBlue((byte)blueNew);
        }

        // multiply existing rgb values
        public void scale(double red, double green, double blue)
        {
            int redNew = DataUtil.clampToEndpoints((int)(getRed() * red), 0, 255);
            int greenNew = DataUtil.clampToEndpoints((int)(getGreen() * green), 0, 255);
            int blueNew = DataUtil.clampToEndpoints((int)(getBlue() * blue), 0, 255);
            setRed((byte)redNew);
            setGreen((byte)greenNew);
            setBlue((byte)blueNew);
        }
    }
}
