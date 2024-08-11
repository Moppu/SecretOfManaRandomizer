using SoMRandomizer.logging;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

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
                using (XmlReader reader = XmlReader.Create(xmlLoadPath))
                {
                    string currentCategory = null;
                    XmlWrapperUtil xml = new XmlWrapperUtil(reader);
                    while (reader.Read())
                    {
                        // Only detect start elements.
                        if (reader.IsStartElement())
                        {
                            // Get element name and switch on it.
                            switch (reader.Name)
                            {
                                case "category":
                                    {
                                        currentCategory = reader["name"];
                                        if(!VALID_CATEGORIES.Contains(currentCategory))
                                        {
                                            throw new XmlException("Unsupported category: " + currentCategory + "; supported categories: " + DataUtil.ListToString(VALID_CATEGORIES.ToList()));
                                        }
                                    }
                                    break;
                                case "song":
                                    {
                                        if (currentCategory != null)
                                        {
                                            // sample table entry
                                            int songId = xml.loadRequiredIntProperty("id", false);
                                            if(songId < 0 || songId > 58)
                                            {
                                                throw new XmlException("Unsupported vanilla song ID: " + songId + "; must be between 0 and 58");
                                            }
                                            customVanillaMusicTypes[(byte)songId] = currentCategory;
                                        }
                                        else
                                        {
                                            throw new XmlException("Stray sample block");
                                        }
                                    }
                                    break;
                            }
                        }
                        else
                        {
                            // handle end block
                            switch (reader.Name)
                            {
                                case "category":
                                    {
                                        currentCategory = null;
                                    }
                                    break;
                            }
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

