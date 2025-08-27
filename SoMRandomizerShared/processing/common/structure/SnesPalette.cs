
namespace SoMRandomizer.processing.common.structure
{
    /// <summary>
    /// VanillaStructure representing a 16-color palette pulled from, or added to, rom.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class SnesPalette : VanillaStructure
    {
        public SnesPalette() : base(new byte[32]) { }
        public SnesPalette(byte[] romValue) : base(romValue) { }
        public SnesPalette(SnesPalette other) : this()
        {
            copyFrom(other);
        }

        public SnesColor getColor(int colorNum)
        {
            return new SnesColor(getDataSegment(colorNum * 2, 2));
        }

        public void setColor(int index, SnesColor color)
        {
            color.put(this, index * 2);
        }

        public void copyFrom(SnesPalette other)
        {
            other.put(this, 0);
        }
    }
}
