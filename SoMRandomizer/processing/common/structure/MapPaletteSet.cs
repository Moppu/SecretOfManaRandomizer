
namespace SoMRandomizer.processing.common.structure
{
    /// <summary>
    /// A set of 7 15-color palettes used for a map.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class MapPaletteSet : VanillaStructure
    {
        public MapPaletteSet() : base(new byte[30 * 7]) { }
        public MapPaletteSet(byte[] romValue) : base(romValue) { }

        public SnesColor getColor(int paletteNum, int colorNum)
        {
            // paletteNum 0->6, colorNum 0->14
            return new SnesColor(getDataSegment(paletteNum * 30 + colorNum * 2, 2));
        }

        public void setColor(int paletteNum, int colorNum, SnesColor color)
        {
            color.put(this, paletteNum * 30 + colorNum * 2);
        }
    }
}
