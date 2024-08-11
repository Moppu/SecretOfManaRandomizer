using System;
using System.Xml;

namespace SoMRandomizer.util
{
    /// <summary>
    /// Utility to more easily extract shit from an XmlReader.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class XmlWrapperUtil
    {
        private XmlReader reader;
        public XmlWrapperUtil(XmlReader reader)
        {
            this.reader = reader;
        }

        // add other helper methods as needed

        // /////
        // Int32
        // /////
        public int loadRequiredIntProperty(string propName, bool hex)
        {
            string strAtt = reader[propName];
            if (strAtt != null)
            {
                return hex ? Int32.Parse(strAtt, System.Globalization.NumberStyles.AllowHexSpecifier) : Int32.Parse(strAtt);
            }
            else
            {
                throw new XmlException("Could not load property " + propName);
            }
        }

        public int? loadOptionalIntProperty(string propName, bool hex)
        {
            string strAtt = reader[propName];
            if (strAtt != null)
            {
                return hex ? Int32.Parse(strAtt, System.Globalization.NumberStyles.AllowHexSpecifier) : Int32.Parse(strAtt);
            }
            return null;
        }

        // //////
        // UInt16
        // //////
        public ushort loadRequiredUshortProperty(string propName, bool hex)
        {
            string strAtt = reader[propName];
            if (strAtt != null)
            {
                return hex ? UInt16.Parse(strAtt, System.Globalization.NumberStyles.AllowHexSpecifier) : UInt16.Parse(strAtt);
            }
            else
            {
                throw new XmlException("Could not load property " + propName);
            }
        }

        public ushort? loadOptionalUshortProperty(string propName, bool hex)
        {
            string strAtt = reader[propName];
            if (strAtt != null)
            {
                return hex ? UInt16.Parse(strAtt, System.Globalization.NumberStyles.AllowHexSpecifier) : UInt16.Parse(strAtt);
            }
            return null;
        }

        // /////
        // Int8
        // /////
        public byte loadRequiredByteProperty(string propName, bool hex)
        {
            string strAtt = reader[propName];
            if (strAtt != null)
            {
                return hex ? Byte.Parse(strAtt, System.Globalization.NumberStyles.AllowHexSpecifier) : Byte.Parse(strAtt);
            }
            else
            {
                throw new XmlException("Could not load property " + propName);
            }
        }

        public byte? loadOptionalByteProperty(string propName, bool hex)
        {
            string strAtt = reader[propName];
            if (strAtt != null)
            {
                return hex ? Byte.Parse(strAtt, System.Globalization.NumberStyles.AllowHexSpecifier) : Byte.Parse(strAtt);
            }
            return null;
        }
    }
}
