using System.Collections.Generic;

namespace SoMRandomizer.processing.common
{
    /// <summary>
    /// Map compression and decompression for SoM map pieces.
    /// Not all compression codes are implemented and it could probably be improved a bit.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class SomMapCompressor
    {
        // uncompressed map data for editing -> compressed map data for rom
        public static byte[] EncodeMap(byte[,] mapData)
        {
            byte[] mapData1d = new byte[mapData.GetLength(0) * mapData.GetLength(1)];
            int pos = 0;
            for(int y=0; y < mapData.GetLength(1); y++)
            {
                for (int x = 0; x < mapData.GetLength(0); x++)
                {
                    mapData1d[pos++] = mapData[x, y];
                }
            }
            return EncodeMap(mapData1d, mapData.GetLength(0), mapData.GetLength(1));
        }

        // uncompressed map data for editing -> compressed map data for rom
        public static byte[] EncodeMap(byte[] mapData, int width, int height)
        {
            int decomp_index = 0;

            List<byte> dest = new List<byte>();

            // first two bytes are width and height.  sort of.
            dest.Add((byte)((width - 1) * 2));
            dest.Add((byte)((height - 1) * 2));
            int length = height * width;

            while (decomp_index < height * width)
            {
                int cmd = 0;
                int arg = -1;
                int bestBytesSaved = 0;
                int sourceDataCmdLength = 0;

                // C0-C7: Repeat 1, 2, 3.. 8 times: the previous decoded byte
                if(decomp_index > 0)
                {
                    // must be at least one previous byte.
                    int numMatches = 0;
                    for(int i=0; i < 8; i++)
                    {
                        if(decomp_index + i < mapData.Length && mapData[decomp_index + i] == mapData[decomp_index - 1])
                        {
                            numMatches++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if(numMatches > 1)
                    {
                        // don't use C0 for now
                        // C1=2 matches = 1 saved...
                        int numBytesSaved = numMatches - 1;
                        if(numBytesSaved > bestBytesSaved)
                        {
                            bestBytesSaved = numBytesSaved;
                            cmd = 0xBF + numMatches;
                            arg = -1;
                            sourceDataCmdLength = numMatches;
                        }
                    }
                }

                // C8-CF: Repeat 1, 2, 3.. 8 times: the 2nd previous decoded byte
                if (decomp_index > 1)
                {
                    // must be at least two previous bytes.
                    int numMatches = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if (decomp_index + i < mapData.Length && mapData[decomp_index + i] == mapData[decomp_index - 2])
                        {
                            numMatches++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (numMatches > 1)
                    {
                        // don't use C8 for now
                        // C9=2 matches = 1 saved...
                        int numBytesSaved = numMatches - 1;
                        if (numBytesSaved > bestBytesSaved)
                        {
                            bestBytesSaved = numBytesSaved;
                            cmd = 0xC7 + numMatches;
                            arg = -1;
                            sourceDataCmdLength = numMatches;
                        }
                    }
                }


                // D0-D7: Repeat 1, 2, 3.. 8 times: the 3rd previous decoded byte
                if (decomp_index > 2)
                {
                    // must be at least three previous bytes.
                    int numMatches = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if (decomp_index + i < mapData.Length && mapData[decomp_index + i] == mapData[decomp_index - 3])
                        {
                            numMatches++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (numMatches > 1)
                    {
                        // don't use D0 for now
                        // D1=2 matches = 1 saved...
                        int numBytesSaved = numMatches - 1;
                        if (numBytesSaved > bestBytesSaved)
                        {
                            bestBytesSaved = numBytesSaved;
                            cmd = 0xCF + numMatches;
                            arg = -1;
                            sourceDataCmdLength = numMatches;
                        }
                    }
                }


                // D8-DF: Repeat 1, 2, 3.. 8 times: the 4th previous decoded byte
                if (decomp_index > 3)
                {
                    // must be at least four previous bytes.
                    int numMatches = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if (decomp_index + i < mapData.Length && mapData[decomp_index + i] == mapData[decomp_index - 4])
                        {
                            numMatches++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (numMatches > 1)
                    {
                        // don't use D8 for now
                        // D9=2 matches = 1 saved...
                        int numBytesSaved = numMatches - 1;
                        if (numBytesSaved > bestBytesSaved)
                        {
                            bestBytesSaved = numBytesSaved;
                            cmd = 0xD7 + numMatches;
                            arg = -1;
                            sourceDataCmdLength = numMatches;
                        }
                    }
                }

                // E0-E7: copy the tile "above" (-width)
                // note that unlike C0-DF, which source from a fixed tile, this one is "running" - each tile is copied from the one
                // directly above it, rather than what was above the first tile.
                if (decomp_index >= width)
                {
                    // must be at least (width) previous bytes.
                    int numMatches = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if (decomp_index + i < mapData.Length && mapData[decomp_index + i] == mapData[decomp_index - width + i])
                        {
                            numMatches++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (numMatches > 1)
                    {
                        // don't use E0 for now
                        // D9=2 matches = 1 saved...
                        int numBytesSaved = numMatches - 1;
                        if (numBytesSaved > bestBytesSaved)
                        {
                            bestBytesSaved = numBytesSaved;
                            cmd = 0xDF + numMatches;
                            arg = -1;
                            sourceDataCmdLength = numMatches;
                        }
                    }
                }

                // F0: one parameter; use the next tile (param & 0x80 == 0) or previous tile (param & 0x80 == 0x80) in the tileset, param & 0x7F + 1 times
                if (decomp_index > 0)
                {
                    // must be at least one previous byte.
                    int numMatches = 0;
                    for (int i = 0; i < 0x7f; i++)
                    {
                        if (decomp_index + i < mapData.Length && mapData[decomp_index + i] == mapData[decomp_index - 1] + i + 1)
                        {
                            numMatches++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (numMatches > 1)
                    {
                        // don't use C0 for now
                        // C1=2 matches = 1 saved...
                        int numBytesSaved = numMatches - 2;
                        if (numBytesSaved > bestBytesSaved)
                        {
                            bestBytesSaved = numBytesSaved;
                            cmd = 0xF0;
                            arg = numMatches - 1;
                            sourceDataCmdLength = numMatches;
                        }
                    }
                    numMatches = 0;
                    for (int i = 0; i < 0x7f; i++)
                    {
                        if (decomp_index + i < mapData.Length && mapData[decomp_index + i] == mapData[decomp_index - 1] - i - 1)
                        {
                            numMatches++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (numMatches > 1)
                    {
                        // don't use C0 for now
                        // C1=2 matches = 1 saved...
                        int numBytesSaved = numMatches - 2;
                        if (numBytesSaved > bestBytesSaved)
                        {
                            bestBytesSaved = numBytesSaved;
                            cmd = 0xF0;
                            arg = 0x80 | (numMatches - 1);
                            sourceDataCmdLength = numMatches;
                        }
                    }
                }

                // F1-F7 - F0, but a fixed number of positives
                if (decomp_index > 0)
                {
                    // must be at least one previous byte.
                    int numMatches = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        if (decomp_index + i < mapData.Length && mapData[decomp_index + i] == mapData[decomp_index - 1] + i + 1)
                        {
                            numMatches++;
                        }
                        else
                        {
                            break;
                        }
                    }
                    if (numMatches > 1)
                    {
                        // don't use C0 for now
                        // C1=2 matches = 1 saved...
                        int numBytesSaved = numMatches - 1;
                        if (numBytesSaved > bestBytesSaved)
                        {
                            bestBytesSaved = numBytesSaved;
                            cmd = 0xEF + numMatches;
                            arg = -1;
                            sourceDataCmdLength = numMatches;
                        }
                    }
                }

                // final processing - if we decided a compression command saves us at least one byte, use it, otherwise just put the raw data
                if (cmd == 0)
                {
                    dest.Add(mapData[decomp_index]);
                    decomp_index++;
                }
                else
                {
                    dest.Add((byte)cmd);
                    if (arg != -1)
                    {
                        dest.Add((byte)arg);
                    }
                    decomp_index += sourceDataCmdLength;
                }
            }
            return dest.ToArray();
        }

        /// <summary>
        /// Used in a few decompression codes
        /// </summary>
        class MapStackInfo
        {
            public int stackWidth;
            public int stackHeight;
            public byte[] data;
            public List<int> positions = new List<int>(); // to be filled in on unreset
            public MapStackInfo(int w, int h)
            {
                stackWidth = w;
                stackHeight = h;
                data = new byte[w * h];
            }
        }

        // compressed map data for rom -> uncompressed map data for editing
        public static byte[,] DecodeMap(byte[] data)
        {
            // 00-BF are single tile IDs (tileset is only 192 tiles).  C0-FF are compression codes.  see below.
            if(data.Length < 2)
            {
                return null;
            }
            List<MapStackInfo> mapStack = new List<MapStackInfo>();
            int inputPos = 0;
            byte width = data[inputPos++];
            byte height = data[inputPos++];
            width = (byte)(width / 2 + 1);
            height = (byte)(height / 2 + 1);
            byte[] decompressedDataOneDimensional = new byte[width * height];
            int decompressedDataSizeTotal = width * height;
            int decompressedDataSizeSoFar = 0;
            // decompress loop.
            while (decompressedDataSizeSoFar < decompressedDataSizeTotal)
            {
                byte compressedByte = data[inputPos++];
                switch (compressedByte)
                {
                    // Repeat 1, 2, 3.. 8 times: the previous decoded byte
                    case 0xC0:
                    case 0xC1:
                    case 0xC2:
                    case 0xC3:
                    case 0xC4:
                    case 0xC5:
                    case 0xC6:
                    case 0xC7:
                        {
                            for (int i = 0; i < (compressedByte - 0xBF); i++)
                            {
                                if (decompressedDataSizeSoFar < decompressedDataOneDimensional.Length && decompressedDataSizeSoFar - (1 + i) >= 0)
                                {
                                    decompressedDataOneDimensional[decompressedDataSizeSoFar] = decompressedDataOneDimensional[decompressedDataSizeSoFar - (1 + i)];
                                }
                                decompressedDataSizeSoFar++;
                            }
                        }
                        break;
                    // Repeat 1, 2, 3.. 8 times: the 2nd previous decoded byte
                    case 0xC8:
                    case 0xC9:
                    case 0xCA:
                    case 0xCB:
                    case 0xCC:
                    case 0xCD:
                    case 0xCE:
                    case 0xCF:
                        {
                            for (int i = 0; i < (compressedByte - 0xC7); i++)
                            {
                                if (decompressedDataSizeSoFar < decompressedDataOneDimensional.Length && decompressedDataSizeSoFar - (2 + i) >= 0)
                                {
                                    decompressedDataOneDimensional[decompressedDataSizeSoFar] = decompressedDataOneDimensional[decompressedDataSizeSoFar - (2 + i)];
                                }
                                decompressedDataSizeSoFar++;
                            }
                        }
                        break;
                    // Repeat 1, 2, 3.. 8 times: the 3rd previous decoded byte
                    case 0xD0:
                    case 0xD1:
                    case 0xD2:
                    case 0xD3:
                    case 0xD4:
                    case 0xD5:
                    case 0xD6:
                    case 0xD7:
                        {
                            for (int i = 0; i < (compressedByte - 0xCF); i++)
                            {
                                if (decompressedDataSizeSoFar < decompressedDataOneDimensional.Length && decompressedDataSizeSoFar - (3 + i) >= 0)
                                {
                                    decompressedDataOneDimensional[decompressedDataSizeSoFar] = decompressedDataOneDimensional[decompressedDataSizeSoFar - (3 + i)];
                                }
                                decompressedDataSizeSoFar++;
                            }
                        }
                        break;
                    // Repeat 1, 2, 3.. 8 times: the 4th previous decoded byte
                    case 0xD8:
                    case 0xD9:
                    case 0xDA:
                    case 0xDB:
                    case 0xDC:
                    case 0xDD:
                    case 0xDE:
                    case 0xDF:
                        {
                            for (int i = 0; i < (compressedByte - 0xD7); i++)
                            {
                                if (decompressedDataSizeSoFar < decompressedDataOneDimensional.Length && decompressedDataSizeSoFar - (4 + i) >= 0)
                                {
                                    decompressedDataOneDimensional[decompressedDataSizeSoFar] = decompressedDataOneDimensional[decompressedDataSizeSoFar - (4 + i)];
                                }
                                decompressedDataSizeSoFar++;
                            }
                        }
                        break;
                    // w/ 1 Parameter m - if MSB is 0, repeat m times: copy the tile "above", else copy the tile 2 above.
                    case 0xE0:
                        {
                            byte dataByte = data[inputPos++];
                            if (dataByte < 0x80)
                            {
                                for (int i = 0; i <= dataByte; i++)
                                {
                                    if (decompressedDataSizeSoFar < decompressedDataOneDimensional.Length && decompressedDataSizeSoFar - width >= 0)
                                        decompressedDataOneDimensional[decompressedDataSizeSoFar] = decompressedDataOneDimensional[decompressedDataSizeSoFar - width];
                                    decompressedDataSizeSoFar++;
                                }
                            }
                            else
                            {
                                dataByte %= 0x80;
                                for (int i = 0; i <= dataByte; i++)
                                {
                                    if (decompressedDataSizeSoFar < decompressedDataOneDimensional.Length && decompressedDataSizeSoFar - (2 * width) >= 0)
                                        decompressedDataOneDimensional[decompressedDataSizeSoFar] = decompressedDataOneDimensional[decompressedDataSizeSoFar - (2 * width)];
                                    decompressedDataSizeSoFar++;
                                }
                            }
                        }
                        break;
                    // Repeat 2, 3.. 8 times: the tile "above"
                    case 0xE1:
                    case 0xE2:
                    case 0xE3:
                    case 0xE4:
                    case 0xE5:
                    case 0xE6:
                    case 0xE7:
                        for (int i = 0; i < (compressedByte - 0xDF); i++)
                        {
                            if (decompressedDataSizeSoFar < decompressedDataOneDimensional.Length && decompressedDataSizeSoFar - width >= 0)
                                decompressedDataOneDimensional[decompressedDataSizeSoFar] = decompressedDataOneDimensional[decompressedDataSizeSoFar - width];
                            decompressedDataSizeSoFar++;
                        }
                        break;

                    // w/ 1 Parameter m - 
                    // capture the previous 16 decompressed bytes in reverse order
                    // repeat m%x80 times: 
                    //     for E8 [and m<80]: next byte = 0th of captured
                    //     for others: seek back [2, 4... 14] + [0,1: m >= 80] and use that byte of captured
                    case 0xE8:
                    case 0xE9:
                    case 0xEA:
                    case 0xEB:
                    case 0xEC:
                    case 0xED:
                    case 0xEE:
                    case 0xEF:
                        {
                            byte dataByte = data[inputPos++];
                            byte[] word = new byte[16];
                            for (int i = 0; i < 16; i++)
                            {
                                if (decompressedDataSizeSoFar - i - 1 >= 0)
                                    word[i] = decompressedDataOneDimensional[decompressedDataSizeSoFar - i - 1];
                                else
                                    word[i] = 0;
                            }
                            for (int i = 0; i <= (dataByte % 0x80); i++)
                            {
                                // Separate code for E8 because the remainder of division by one
                                // does not produce desired results.
                                if (compressedByte == 0xE8 && dataByte < 0x80)
                                {
                                    if (decompressedDataSizeSoFar < decompressedDataOneDimensional.Length)
                                        decompressedDataOneDimensional[decompressedDataSizeSoFar] = (byte)word[0];
                                }
                                else
                                {
                                    // Determine the number of spaces to go back initially.
                                    int numofprevs = (2 * (compressedByte - 0xE8)) + (dataByte / 0x80);
                                    int index = numofprevs - (i % (numofprevs + 1));
                                    if (decompressedDataSizeSoFar < decompressedDataOneDimensional.Length)
                                        decompressedDataOneDimensional[decompressedDataSizeSoFar] = (byte)(word[index]);
                                }
                                decompressedDataSizeSoFar++;
                            }
                        }
                        break;
                    // w/ 1 Parameter m:
                    // repeat m times: 
                    //     next byte = next in tileset after previous.
                    case 0xF0:
                        {
                            byte dataByte = data[inputPos++];
                            int numIters = dataByte;
                            bool upward = true;
                            if (numIters >= 128)
                            {
                                numIters -= 128;
                                upward = false;
                            }
                            for (int i = 0; i <= numIters; i++)
                            {
                                if (upward)
                                {
                                    if (decompressedDataSizeSoFar < decompressedDataOneDimensional.Length && decompressedDataSizeSoFar - (1 + i) >= 0)
                                        decompressedDataOneDimensional[decompressedDataSizeSoFar] = (byte)((decompressedDataOneDimensional[decompressedDataSizeSoFar - (1 + i)] + i + 1) % 0x100);
                                }
                                else
                                {
                                    if (decompressedDataSizeSoFar < decompressedDataOneDimensional.Length && decompressedDataSizeSoFar - (1 + i) >= 0)
                                        decompressedDataOneDimensional[decompressedDataSizeSoFar] = (byte)((decompressedDataOneDimensional[decompressedDataSizeSoFar - (1 + i)] - i - 1) % 0x100);
                                }
                                decompressedDataSizeSoFar++;
                            }
                        }
                        break;

                    // Repeat 1, 2... 8 times: the next in tileset after previous
                    case 0xF1:
                    case 0xF2:
                    case 0xF3:
                    case 0xF4:
                    case 0xF5:
                    case 0xF6:
                    case 0xF7:
                        {
                            for (int i = 0; i < (compressedByte - 0xEF); i++)
                            {
                                if (decompressedDataSizeSoFar < decompressedDataOneDimensional.Length && decompressedDataSizeSoFar - (1 + i) >= 0)
                                    decompressedDataOneDimensional[decompressedDataSizeSoFar] = (byte)(decompressedDataOneDimensional[decompressedDataSizeSoFar - (1 + i)] + i + 1);
                                decompressedDataSizeSoFar++;
                            }
                        }
                        break;
                    // F8?  [map 13]
                    case 0xF8: // map 13 uses this
                    case 0xF9:
                    case 0xFA:
                    case 0xFB:
                    case 0xFC:
                    case 0xFD:
                        {
                            // F8 / FE
                            // F9 / FF
                            // F8 / FF
                            int thisIndex = compressedByte - 0xF8;
                            if (thisIndex < mapStack.Count)
                            {
                                mapStack[thisIndex].positions.Add(decompressedDataSizeSoFar);
                                // mark for insertion later - only increment pos by 1 (?)
                                decompressedDataSizeSoFar += 1;
                            }
                        }
                        break;
                    case 0xFE:
                    // i think ff is just the last one; maybe it's supposed to set pos=0
                    case 0xFF: // map 13 uses this 
                        {
                            int _stackIndex = 0xFF - compressedByte;
                            int length = decompressedDataSizeSoFar;
                            byte settings = decompressedDataOneDimensional[0];
                            byte _width = (byte)((settings & 0x0F) + 1);
                            byte _height = (byte)(((settings & 0xF0) >> 4) + 1);
                            MapStackInfo stackInfo = new MapStackInfo(_width, _height);
                            int stackX = 0;
                            int stackY = 0;
                            while (stackY < _height)
                            {
                                if (stackX + stackY * _width + 1 < decompressedDataOneDimensional.Length)
                                {
                                    stackInfo.data[stackY * _width + stackX] = decompressedDataOneDimensional[stackX + stackY * _width + 1];
                                }
                                stackX++;
                                if (stackX >= _width)
                                {
                                    stackX = 0;
                                    stackY++;
                                }
                            }
                            decompressedDataSizeSoFar -= (_width * _height + 1);
                            if (decompressedDataSizeSoFar < 0)
                                decompressedDataSizeSoFar = 0;

                            mapStack.Add(stackInfo);
                        }
                        break;
                    default:
                        // 00-BF are normal tiles
                        decompressedDataOneDimensional[decompressedDataSizeSoFar] = compressedByte;
                        decompressedDataSizeSoFar += 1;
                        break;

                }
            }

            foreach (MapStackInfo msi in mapStack)
            {
                foreach (int pos in msi.positions)
                {
                    for (int y = 0; y < msi.stackHeight; y++)
                    {
                        for (int x = 0; x < msi.stackWidth; x++)
                        {
                            if (pos + x + y * width < decompressedDataOneDimensional.Length)
                            {
                                decompressedDataOneDimensional[pos + x + y * width] = msi.data[y * msi.stackWidth + x];
                            }
                        }
                    }
                }
            }

            byte[,] outputMapData = new byte[width, height];
            for(int y=0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    outputMapData[x, y] = decompressedDataOneDimensional[y * width + x];
                }
            }
            return outputMapData;
        }
    }
}

