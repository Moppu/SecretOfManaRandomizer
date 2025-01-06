using System;

namespace SoMRandomizer.util.img
{
    /// <summary>
    /// 24- or 32-bit Color struct, replacing System.Drawing Color class to avoid needing that dependency.
    /// </summary>
    public readonly struct Color : IEquatable<Color>
    {
        private Color(byte r, byte g, byte b, byte a)
        {
            R = r;
            G = g;
            B = b;
            A = a;
        }

        public byte R { get; }
        public byte G { get; }
        public byte B { get; }
        public byte A { get; }

        public bool Equals(Color other)
        {
            return R == other.R && G == other.G && B == other.B && A == other.A;
        }

        // ReSharper disable once InconsistentNaming
        public static Color FromArgb(byte r, byte g, byte b)
        {
            return new Color(r, g, b, 255);
        }

        // ReSharper disable once InconsistentNaming
        public static Color FromArgb(byte a, byte r, byte g, byte b)
        {
            return new Color(r, g, b, a);
        }
    }
}