using SoMRandomizer.logging;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; // string[].Contains
using U8Xml;

namespace SoMRandomizer.processing.hacks.common.music
{
    /// <summary>
    /// Reconfigure which vanilla songs come up under which random music categories, via an XML file.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class VanillaMusicTypesLoader
    {
        private static string[] VALID_CATEGORIES = new string[] {
            CustomMusic.SONGTYPELABEL_BATTLE,
            CustomMusic.SONGTYPELABEL_DUNGEON,
            CustomMusic.SONGTYPELABEL_FLIGHT,
            CustomMusic.SONGTYPELABEL_OVERWORLD,
            CustomMusic.SONGTYPELABEL_PALACE,
            CustomMusic.SONGTYPELABEL_TOWN,
            CustomMusic.SONGTYPELABEL_VICTORY,
        };

        public static Dictionary<byte, string> loadCustomVanillaMusicTypes()
        {
            string baseDir = ".";
            string xmlLoadDir = baseDir + "/music";
            string xmlLoadPath = xmlLoadDir + "/vanillamusictypes.xml";
            Dictionary<byte, string> customVanillaMusicTypes = new Dictionary<byte, string>();
            try
            {
                using (var xml = XmlParser.ParseFile(xmlLoadPath))
                {
                    // NOTE: we only look at top level categories and their songs
                    foreach (var node in xml.Root.Children)
                    {
                        if (node.Name == "category")
                        {
                            string currentCategory = node.FindAttribute("name").Value.ToString();
                            if (!VALID_CATEGORIES.Contains(currentCategory))
                            {
                                throw new InvalidDataException("Unsupported category: " + currentCategory
                                    + "; supported categories: " + DataUtil.ListToString(VALID_CATEGORIES.ToList()));
                            }

                            foreach (var child in node.Children)
                            {
                                if (child.Name.ToString() == "song")
                                {
                                        // sample table entry
                                        XmlWrapperUtil wrap = new XmlWrapperUtil(child);
                                        int songId = wrap.loadRequiredIntProperty("id", false);
                                        if(songId < 0 || songId > 58)
                                        {
                                            throw new InvalidDataException("Unsupported vanilla song ID: " + songId + "; must be between 0 and 58");
                                        }
                                        customVanillaMusicTypes[(byte)songId] = currentCategory;
                                }
                                else
                                {
                                    throw new InvalidDataException("Unknown category child node: " + child.Name);
                                }
                            }
                        }
                        else
                        {
                            throw new InvalidDataException("Unknown top level node: " + node.Name);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Logging.log("Couldn't load vanilla music types: " + e);
                return null;
            }
            return customVanillaMusicTypes;
        }
    }
}

