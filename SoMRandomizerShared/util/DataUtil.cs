using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace SoMRandomizer.util
{
    /// <summary>
    /// Utilities for manipulation of numbers and other data.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class DataUtil
    {
        public static int clampToPositiveRange(int value, int maxValueExclusive)
        {
            // limit to range with looping
            if (value >= maxValueExclusive)
            {
                return (value % maxValueExclusive);
            }
            if (value < 0)
            {
                return (value % maxValueExclusive) + maxValueExclusive;
            }
            return value;
        }

        public static int clampToEndpoints(int value, int minInclusive, int maxInclusive)
        {
            // limit to range without looping
            if (value > maxInclusive)
            {
                return maxInclusive;
            }
            if (value < minInclusive)
            {
                return minInclusive;
            }
            return value;
        }

        public static byte[] readResource(string resourcePath)
        {
            // resource path should be SoMRandomizer.Resources.(...)
            Assembly assemb = Assembly.GetExecutingAssembly();
            using (Stream stream = assemb.GetManifestResourceStream(resourcePath))
            {
                byte[] resourceData = new byte[stream.Length];
                stream.Read(resourceData, 0, (int)stream.Length);
                return resourceData;
            }
        }

        // where bit==0 is the msb
        public static bool bitTest(byte src, int bit)
        {
            if ((src >> (7 - bit)) % 2 == 1)
                return true;
            return false;
        }

        public static ushort ushortFromBytes(byte[] byteArray, int index)
        {
            // always little-endian (lsb first)
            // note bounds are not checked
            return (ushort)(byteArray[index] + (byteArray[index + 1] << 8));
        }

        public static void ushortToBytes(byte[] byteArray, int index, ushort value)
        {
            byteArray[index] = (byte)value;
            byteArray[index + 1] = (byte)(value >> 8);
        }

        public static int int24FromBytes(byte[] byteArray, int index)
        {
            // always little-endian (lsb first)
            // note bounds are not checked
            return byteArray[index] + (byteArray[index + 1] << 8) + (byteArray[index + 2] << 16);
        }

        public static void int24ToBytes(byte[] byteArray, int index, int value)
        {
            byteArray[index] = (byte)value;
            byteArray[index + 1] = (byte)(value >> 8);
            byteArray[index + 2] = (byte)(value >> 16);
        }

        public static string byteArrayToHexString(byte[] data)
        {
            string result = "";
            for (int i = 0; i < data.Length; i++)
            {
                result += data[i].ToString("X2");
                if (i != data.Length - 1)
                {
                    result += " ";
                }
            }
            return result;
        }

        public static string ListToString<T>(List<T> list)
        {
            string result = "[List:";
            for(int i=0; i < list.Count; i++)
            {
                T item = list[i];
                result += item;
                if(i != list.Count - 1)
                {
                    result += "; ";
                }
            }
            result += "]";
            return result;
        }

        // encode text string as base64
        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        // decode base64 back to regular text string
        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        // for easy inline construction of pairs of things - see VanillaMapUtil
        public class TupleList<T1, T2> : List<Tuple<T1, T2>>
        {
            public void Add(T1 item, T2 item2)
            {
                Add(new Tuple<T1, T2>(item, item2));
            }
        }
    }
}
