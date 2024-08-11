using System;

namespace SoMRandomizer.util
{
    /// <summary>
    /// Utilities for color conversions.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ColorUtil
    {
        // RGB to hue from https://stackoverflow.com/questions/23090019/fastest-formula-to-get-hue-from-rgb
        public static double rgbToHue(int r, int g, int b)
        {
            if (r == g && g == b)
            {
                // gray
                return 0;
            }

            int max = Math.Max(r, Math.Max(g, b));
            int min = Math.Min(r, Math.Min(g, b));
            double diff = max - min;
            double h;
            if (r == max)
            {
                h = (g - b) / diff;
            }
            else if (g == max)
            {
                h = 2 + (b - r) / diff;
            }
            else
            {
                h = 4 + (r - g) / diff;
            }
            h *= 60;
            if (h < 0)
            {
                h += 360;
            }
            return h;
        }

        // rgb 0-255, hue in degrees 0>360, sat/val 0->1
        public static void rgbToHsv(int r, int g, int b, out double hue, out double saturation, out double value)
        {
            int max = Math.Max(r, Math.Max(g, b));
            int min = Math.Min(r, Math.Min(g, b));
            hue = rgbToHue(r, g, b);
            saturation = (max == 0) ? 0 : 1d - (1d * min / max);
            value = max / 255d;
        }

        public static void hsvToRgb(double hue, double saturation, double value, out int r, out int g, out int b)
        {
            int hueIndex = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
            double f = hue / 60 - Math.Floor(hue / 60);

            if (saturation < 0)
            {
                saturation = 0;
            }
            if (saturation > 1)
            {
                saturation = 1;
            }
            value = value * 255;
            byte v = (byte)DataUtil.clampToEndpoints(Convert.ToInt32(value), 0, 255);
            byte p = (byte)DataUtil.clampToEndpoints(Convert.ToInt32(value * (1 - saturation)), 0, 255);
            byte q = (byte)DataUtil.clampToEndpoints(Convert.ToInt32(value * (1 - f * saturation)), 0, 255);
            byte t = (byte)DataUtil.clampToEndpoints(Convert.ToInt32(value * (1 - (1 - f) * saturation)), 0, 255);
            switch (hueIndex)
            {
                case 0:
                    r = v;
                    g = t;
                    b = p;
                    break;
                case 1:
                    r = q;
                    g = v;
                    b = p;
                    break;
                case 2:
                    r = p;
                    g = v;
                    b = t;
                    break;
                case 3:
                    r = p;
                    g = q;
                    b = v;
                    break;
                case 4:
                    r = t;
                    g = p;
                    b = v;
                    break;
                default:
                    r = v;
                    g = p;
                    b = q;
                    break;
            }
        }
    }
}
