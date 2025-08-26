using SoMRandomizer.logging;
using SoMRandomizer.util;
using System;
using System.Collections.Generic;
using System.IO;
using U8Xml;

namespace SoMRandomizer.processing.hacks.common.music
{
    /// <summary>
    /// XML reader for externally configured custom music.
    /// </summary>
    /// 
    /// <remarks>Author: Moppleton</remarks>
    public class ExternalMusic
    {
        public static List<ImportedSong> getAllCustomSongs(Dictionary<string, ushort> sampleNamesToIds, ref ushort startId)
        {
            string baseDir = ".";
            List<ImportedSong> importedSongs = new List<ImportedSong>();
            string xmlLoadDir = baseDir + "/music";
            string xmlLoadPath = xmlLoadDir + "/music.xml";
            try
            {
                using (var xml = XmlParser.ParseFile(xmlLoadPath))
                {
                    // NOTE: we only look at top level songs and their children here
                    foreach (var node in xml.Root.Children)
                    {
                        if (node.Name.ToString() == "song")
                        {
                            ImportedSong song = new ImportedSong();
                            song.name = node.FindAttribute("name").Value.ToString();
                            string filePath = node.FindAttribute("sourceFile").Value.ToString();
                            string songFullPath = xmlLoadDir + "/" + filePath;
                            song.data = File.ReadAllBytes(songFullPath);
                            string categoriesString = node.FindAttribute("categories").Value.ToString();
                            song.echoVal = 0x13;
                            XmlWrapperUtil wrap = new XmlWrapperUtil(node);
                            byte? xmlEchoValue = wrap.loadOptionalByteProperty("echo", true);
                            if(xmlEchoValue != null)
                            {
                                song.echoVal = (byte)xmlEchoValue;
                            }
                            string[] categories = categoriesString.Split(new char[] { ',' });
                            song.categories = categories;
                            song.newId = startId++;

                            foreach (var child in node.Children)
                            {
                                if (child.Name.ToString() == "sample")
                                {
                                    // sample table entry
                                    wrap = new XmlWrapperUtil(child);
                                    int sampleId = wrap.loadRequiredIntProperty("id", false);
                                    string sourceString = child.FindAttribute("source").Value.ToString();
                                    string[] sourceParts = sourceString.Split(new char[] { ' ' });
                                    if(sourceParts.Length == 2 && sourceParts[0] == "vanilla")
                                    {
                                        song.sampleTable[sampleId] = UInt16.Parse(sourceParts[1]);
                                    }
                                    else if (sourceParts.Length == 2 && sourceParts[0] == "custom")
                                    {
                                        song.sampleTable[sampleId] = sampleNamesToIds[sourceParts[1]];
                                    }
                                    else
                                    {
                                        throw new InvalidDataException("unsupported sample source: " + sourceString);
                                    }
                                }
                                else
                                {
                                    throw new InvalidDataException("Unknown song child node: " + child.Name);
                                }
                            }

                            importedSongs.Add(song);
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
                Logging.log("Couldn't load external music: " + e);
                return null;
            }

            return importedSongs;
        }

        public class ImportedSong
        {
            public string name;
            public byte[] data;
            public string[] categories;
            public byte echoVal;
            public Dictionary<int, ushort> sampleTable = new Dictionary<int, ushort>();
            public ushort newId;
        }
    }
}
