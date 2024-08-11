
namespace SoMRandomizer.processing.ancientcave.mapgen
{
    // MOPPLE: this is all similar to something i use in BLPG to generated sides of tiles etc.
    // the two could probably be consolidated into a common thing

    /// <summary>
    /// Utility to post-process a map and change tiles based on known patterns. Used for stuff like adding tile edges and corners.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class TileReplacer
    {
        public static void applyPattern(ReplacementPattern pattern, byte[,] layerOldData, byte[,] layerNewData)
        {
            applyPattern(pattern, layerOldData, layerNewData, null, null);
        }

        public static void applyPattern(ReplacementPattern pattern, byte[,] layerOldData, byte[,] layerNewData, byte[,] altLayerData)
        {
            applyPattern(pattern, layerOldData, layerNewData, altLayerData, null);
        }

        public static void applyPattern(ReplacementPattern pattern, byte[,] layerOldData, byte[,] layerNewData, byte[,] altLayerData, BoundsRectangle rect)
        {
            int width = layerOldData.GetLength(0);
            int height = layerOldData.GetLength(1);
            int xStart = 0;
            int yStart = 0;

            if(rect != null)
            {
                xStart = rect.x1;
                yStart = rect.y1;
                width = rect.x2;
                height = rect.y2;
            }
            string[] searchPattern = pattern.searchPattern;
            int blockWidth = pattern.width;
            int blockHeight = searchPattern.Length / blockWidth;

            // loop over the whole layer
            for (int y = yStart; y < height; y++)
            {
                for (int x = xStart; x < width; x++)
                {
                    bool match = true;
                    // loop over the block
                    int i = 0;
                    for (int py = 0; match && py < blockHeight; py++)
                    {
                        for (int px = 0; match && px < blockWidth; px++)
                        {
                            int xp = px + x;
                            int yp = py + y;

                            if (xp >= 0 && yp >= 0 && xp < width && yp < height)
                            {
                                string search = searchPattern[i];
                                int tileId;
                                bool isNumeric = int.TryParse(search, out tileId);
                                if (isNumeric)
                                {
                                    if (tileId != layerOldData[xp, yp])
                                    {
                                        match = false;
                                    }
                                }
                                else
                                {
                                    if (search.StartsWith("!"))
                                    {
                                        string rest = search.Substring(1);
                                        bool isRestNumeric = int.TryParse(rest, out tileId);
                                        if (isRestNumeric)
                                        {
                                            if (tileId == layerOldData[xp, yp])
                                            {
                                                match = false;
                                            }
                                        }
                                        else if (rest.StartsWith("{") && rest.EndsWith("}"))
                                        {
                                            string r = rest.Substring(1, rest.Length - 2);
                                            string[] numbers = r.Split(',');
                                            bool anyMatch = true;
                                            foreach (string num in numbers)
                                            {
                                                int thisNum = int.Parse(num);
                                                if (layerOldData[xp, yp] == thisNum)
                                                {
                                                    anyMatch = false;
                                                }
                                            }
                                            match &= anyMatch;
                                        }
                                    }
                                    else if(search.StartsWith("{") && search.EndsWith("}"))
                                    {
                                        string rest = search.Substring(1, search.Length - 2);
                                        string[] numbers = rest.Split(',');
                                        bool anyMatch = false;
                                        foreach(string num in numbers)
                                        {
                                            int thisNum = int.Parse(num);
                                            if(layerOldData[xp, yp] == thisNum)
                                            {
                                                anyMatch = true;
                                            }
                                        }
                                        match &= anyMatch;
                                    }
                                }

                            }
                            else
                            {
                                match = false;
                            }
                            i++;
                        }
                    }

                    if (match)
                    {
                        string[] injectionData = pattern.injectionData;
                        string[] altInjectionData = pattern.altInjectionData;
                        i = 0;
                        // inject the new data if the old data matched
                        for (int py = 0; py < blockHeight; py++)
                        {
                            for (int px = 0; px < blockWidth; px++)
                            {
                                int xp = px + x;
                                int yp = py + y;

                                if (xp >= 0 && yp >= 0 && xp < width && yp < height)
                                {
                                    string inject = injectionData[i];
                                    if(inject != "-")
                                    {
                                        int newTile = int.Parse(inject);
                                        layerNewData[xp, yp] = (byte)newTile;
                                    }
                                    if (altInjectionData != null && altLayerData != null)
                                    {
                                        string altInject = altInjectionData[i];
                                        if (altInject != "-")
                                        {
                                            int newTile = int.Parse(altInject);
                                            altLayerData[xp, yp] = (byte)newTile;
                                        }
                                    }
                                }
                                i++;
                            }
                        }
                        //return true;
                    }
                }
            }

            //return false;
        }

        public class BoundsRectangle
        {
            public int x1;
            public int x2;
            public int y1;
            public int y2;

            public BoundsRectangle(int x1, int x2, int y1, int y2)
            {
                this.x1 = x1;
                this.x2 = x2;
                this.y1 = y1;
                this.y2 = y2;
            }
        }

        public class ReplacementPattern
        {
            public string[] searchPattern;
            public string[] injectionData;
            public string[] altInjectionData;
            public int width;
            public ReplacementPattern(string[] searchPattern, string[] injectionData, int width) : this(searchPattern, injectionData, null, width)
            {
            }

            public ReplacementPattern(string[] searchPattern, string[] injectionData, string[] altInjectionData, int width)
            {
                // numeric, "*", or "!numeric"
                this.searchPattern = searchPattern;
                // numeric, or "-" for no change
                this.injectionData = injectionData;
                this.altInjectionData = altInjectionData;
                this.width = width;
            }
        }
    }
}
