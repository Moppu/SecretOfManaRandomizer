using System;
using System.IO;
using U8Xml;

namespace SoMRandomizer.util
{
    /// <summary>
    /// Utility to more easily extract shit from an U8Xml node.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class XmlWrapperUtil
    {
        private XmlNode node;
        public XmlWrapperUtil(XmlNode node)
        {
            this.node = node;
        }

        // add other helper methods as needed

        // /////
        // Int32
        // /////
        public int loadRequiredIntProperty(string propName, bool hex)
        {
            if (!node.TryFindAttribute(propName, out XmlAttribute attr))
            {
                throw new InvalidDataException("Could not load property " + propName);
            }

            string strAtt = attr.Value.ToString();
            return hex ? Int32.Parse(strAtt, System.Globalization.NumberStyles.AllowHexSpecifier) : Int32.Parse(strAtt);
        }

        public int? loadOptionalIntProperty(string propName, bool hex)
        {
            if (!node.TryFindAttribute(propName, out XmlAttribute attr))
            {
                return null;
            }

            string strAtt = attr.Value.ToString();
            return hex ? Int32.Parse(strAtt, System.Globalization.NumberStyles.AllowHexSpecifier) : Int32.Parse(strAtt);
        }

        // //////
        // UInt16
        // //////
        public ushort loadRequiredUshortProperty(string propName, bool hex)
        {
            if (!node.TryFindAttribute(propName, out XmlAttribute attr))
            {
                throw new InvalidDataException("Could not load property " + propName);
            }

            string strAtt = attr.Value.ToString();
            return hex ? UInt16.Parse(strAtt, System.Globalization.NumberStyles.AllowHexSpecifier) : UInt16.Parse(strAtt);
        }

        public ushort? loadOptionalUshortProperty(string propName, bool hex)
        {
            if (!node.TryFindAttribute(propName, out XmlAttribute attr))
            {
                return null;
            }

            string strAtt = attr.Value.ToString();
            return hex ? UInt16.Parse(strAtt, System.Globalization.NumberStyles.AllowHexSpecifier) : UInt16.Parse(strAtt);
        }

        // /////
        // Int8
        // /////
        public byte loadRequiredByteProperty(string propName, bool hex)
        {
            if (!node.TryFindAttribute(propName, out XmlAttribute attr))
            {
                throw new InvalidDataException("Could not load property " + propName);
            }

            string strAtt = attr.Value.ToString();
            return hex ? Byte.Parse(strAtt, System.Globalization.NumberStyles.AllowHexSpecifier) : Byte.Parse(strAtt);
        }

        public byte? loadOptionalByteProperty(string propName, bool hex)
        {
            if (!node.TryFindAttribute(propName, out XmlAttribute attr))
            {
                return null;
            }

            string strAtt = attr.Value.ToString();
            return hex ? Byte.Parse(strAtt, System.Globalization.NumberStyles.AllowHexSpecifier) : Byte.Parse(strAtt);
        }
    }
}
