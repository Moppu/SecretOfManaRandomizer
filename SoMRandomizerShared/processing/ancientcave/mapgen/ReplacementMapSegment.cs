
namespace SoMRandomizer.processing.ancientcave.mapgen
{
    /// <summary>
    /// Used to inject decorations onto ruins tileset maps.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ReplacementMapSegment
    {
        public int x;
        public int y;
        public int w;
        public int h;
        public byte[] data;

        public static void applyReplacement(byte[,] layerData, int baseX, int baseY, ReplacementMapSegment replacement)
        {
            for (int y = 0; y < replacement.h; y++)
            {
                for (int x = 0; x < replacement.w; x++)
                {
                    layerData[baseX + replacement.x + x, baseY + replacement.y + y] = replacement.data[y * replacement.w + x];
                }
            }
        }
    }
}
