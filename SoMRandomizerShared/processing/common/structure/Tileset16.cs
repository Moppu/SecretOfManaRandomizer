using System;

namespace SoMRandomizer.processing.common.structure
{
    /// <summary>
    /// Tileset containing 384 16x16 Tiles - 192 background and 192 foreground.
    /// </summary>
    public class Tileset16
    {
        const int LAYER_SIZE = 192;
        public Tile16[] Tiles { get; set; }
        
        /// <summary>
        /// for directly indexing tiles
        /// </summary>
        /// <param name="index"></param>
        /// <exception cref="InvalidOperationException"></exception>
        public Tile16 this[int index]
        {
            get
            {
                if (index < 0 || index >= 384) throw new InvalidOperationException("index has to be between 0 and 384");
                return Tiles[index];
            }
            set
            {
                if (index < 0 || index >= 384) throw new InvalidOperationException("index has to be between 0 and 384");
                Tiles[index] = value;
            }
        }
        
        /// <summary>
        /// for indexing into foreground/background layer directly
        /// </summary>
        /// <param name="foreground">if true, 192 will be added to <paramref name="index"/> for the actual index into <see cref="Tiles"/></param>
        /// <param name="index">index into the layer</param>
        public Tile16 this[bool foreground, int index]
        {
            get
            {
                if (index < 0 || index >= LAYER_SIZE) throw new InvalidOperationException("index has to be between 0 and 192");
                return Tiles[foreground ? LAYER_SIZE : 0 + index];
            }
            set
            {
                if (index < 0 || index >= LAYER_SIZE) throw new InvalidOperationException("index has to be between 0 and 192");
                Tiles[foreground ? LAYER_SIZE : 0 + index] = value;
            }
        }

        public Tileset16(Tile16[] tiles)
        {
            if (tiles.Length != 384)
                throw new ArgumentException("Tileset16 has to have 384 tiles", nameof(tiles));
            Tiles = tiles;
        }
    }
    
    /// <summary>
    /// 16x16 Tile constructed by 4 8x8 Tiles
    /// </summary>
    /// <seealso cref="Tile8"/>
    public class Tile16
    {
        public Tile16(Tile8[] tiles)
        {
            if (tiles.Length != 4)
                throw new ArgumentException("Tile16 has to have 4 Tile8s", nameof(tiles));
            Tiles = tiles;
        }
        
        public Tile8 this[int index]
        {
            get => Tiles[index];
            set => Tiles[index] = value;
        }
        
        public Tile8[] Tiles { get; set; }
    }
    
    /// <summary>
    /// 8x8 Tile
    /// </summary>
    public class Tile8
    {
        /// <summary>
        /// Vertical Flip Flag
        /// </summary>
        public bool VerticalFlip { get; set; }
        /// <summary>
        /// Horizontal Flip Flag
        /// </summary>
        public bool HorizontalFlip { get; set; }
        /// <summary>
        /// Alternate Render Layer Flag
        /// </summary>
        public bool AlternateRenderLayer { get; set; }
        private byte _palette { get; set; }
        /// <summary>
        /// 3-bit Palette ID
        /// </summary>
        public byte Palette
        {
            get => _palette;
            //limit to 3-bit
            set => _palette = (byte)(value & 0x7);
        }

        private ushort _tileNum;
        /// <summary>
        /// 10-bit Tileset8 Index
        /// </summary>
        public ushort TileNum
        {
            get => _tileNum;
            //limit to 10-bit
            set => _tileNum = (ushort)(value & 0x3FF);
        }

        public Tile8(bool verticalFlip = false, bool horizontalFlip = false, bool alternateRenderLayer = false, byte palette = 0, ushort tileNum = 0)
        {
            VerticalFlip = verticalFlip;
            HorizontalFlip = horizontalFlip;
            AlternateRenderLayer = alternateRenderLayer;
            Palette = palette;
            TileNum = tileNum;
        }

        /// <summary>
        /// convert to raw 16-bit value of the 8x8 Tile
        /// in the format of
        /// [VHAPPPTTTTTTTTT]
        /// V is a vertical flip flag
        /// H is a horizontal flip flag
        /// A is an alternate render layer flag (causes some tiles to show in front of or behind sprites)
        /// PPP is a 3-bit palette id
        /// TTTTTTTTTT is a 10-bit tile index into the associated tileset8
        /// </summary>
        public ushort ToBinary()
        {
            return (ushort)(
                (VerticalFlip ? 1<<15 : 0) |
                (HorizontalFlip ? 1<<14 : 0) |
                (AlternateRenderLayer ? 1<<13 : 0) |
                (Palette << 10) |
                TileNum);
        }

        /// <summary>
        /// Decode into <see cref="Tile8"/> structure from the raw binary value
        /// </summary>
        /// <param name="data">binary value</param>
        /// <returns>decoded <see cref="Tile8"/></returns>
        /// <seealso cref="ToBinary"/>
        public static Tile8 FromBinary(ushort data)
        {
            return new Tile8(
                (data & 0x8000) != 0,
                (data & 0x4000) != 0,
                (data & 0x2000) != 0,
                (byte)((data >> 10) & 0x7),
                (ushort)(data & 0x3FF));
        }
    }
}